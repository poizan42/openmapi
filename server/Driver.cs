//
// openmapi.org - OpenMapi Proxy Server - Driver.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

using System.IO;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using NMapi;
//using NMapi.WCF.Xmpp;

using NMapi.Server.ICalls;

namespace NMapi.Server {

	public class Driver
	{
		public static void Main (string[] args)
		{
			if (args.Length > 0 && args [0] == "client") {
				MainClient ();
				return;
			}
			new Driver ().Run ();
		}

		public static void MainClient ()
		{
			EndpointAddress ep = new EndpointAddress ("http://localhost:9000/IMapiOverIndigo/MapiOverIndigoService");
			IMapiOverIndigo proxy = ChannelFactory<IMapiOverIndigo>.CreateChannel (new WSDualHttpBinding(), ep);

			Console.WriteLine (proxy.Handshake ());
		}


		class ServerModuleCall
		{
			public IServerModule Module { get; set; }
			public string MethodName { get; set; }
			public bool PassValuesByRef { get; set; }
			public bool IsUniversal { get; set; }

			public ServerModuleCall (IServerModule module, string methodName)
			{
				this.Module = module;
				this.MethodName = methodName;
				this.PassValuesByRef = true;
			}
		}

		private Configuration cfg = new Configuration ();
		private SessionManager sessionMan;
		private ProxyInformation pinfo;

		private List<IServerModule> modules; // TODO: This needs to be the same as the module list in the server-class!
		private Dictionary<RemoteCall, List<ServerModuleCall>> preCalls;
		private Dictionary<RemoteCall, List<ServerModuleCall>> postCalls;

		private void DetectModules ()
		{
			modules.Clear ();
			Type[] types = Assembly.GetExecutingAssembly ().GetTypes ();
			foreach (Type type in types) {
				TypeFilter filter = (current, comp) => current.ToString () == comp.ToString ();

				var foundInterfaces = type.FindInterfaces (filter, "NMapi.Server.IServerModule");
				if (foundInterfaces.Length > 0) {
					IServerModule module = null;
					try {
						module = (IServerModule) Activator.CreateInstance (type);
					} catch (MissingMethodException) {
						throw new Exception ("The module '" + type + "' does not contain an empty constructor.");
					}
					this.modules.Add (module);

					RemoteCall[] allCalls = (RemoteCall[]) Enum.GetValues (typeof (RemoteCall));

					MethodInfo[] methods = module.GetType ().GetMethods ();
					foreach (var method in methods) {
						var preAttribs = (PreCallAttribute[]) method.GetCustomAttributes (typeof (PreCallAttribute), true);
						var postAttribs =  (PostCallAttribute[]) method.GetCustomAttributes (typeof (PostCallAttribute), true);

						RegisterCalls (preCalls, allCalls, preAttribs, new ServerModuleCall (module, method.Name));
						RegisterCalls (postCalls, allCalls, postAttribs, new ServerModuleCall (module, method.Name));
					}
				}
			}
		}

		private void RegisterCalls (Dictionary<RemoteCall, List<ServerModuleCall>> callDictionary, 
			RemoteCall[] allCalls, CallAttribute[] attributes, ServerModuleCall moduleCall)
		{
			if (attributes  == null || attributes.Length < 1)
				return;
			CallAttribute attrib = attributes [0];

			RemoteCall[] targetCalls = new RemoteCall [1];
			targetCalls [0] = attrib.RemoteCall;
			if (attrib.Any) {
				targetCalls = allCalls;
				moduleCall.IsUniversal = true;
			}

			foreach (RemoteCall currentCall in targetCalls) {
				if (!callDictionary.ContainsKey (currentCall))
					callDictionary [currentCall] = new List<ServerModuleCall> ();
				if (!callDictionary [currentCall].Contains (moduleCall))
					callDictionary [currentCall].Add (moduleCall);
			}
		}

		public Driver ()
		{
			this.sessionMan = new SessionManager ();
			this.pinfo = new ProxyInformation ();
			this.modules = new List<IServerModule> ();
			this.preCalls = new Dictionary<RemoteCall, List<ServerModuleCall>> ();
			this.postCalls = new Dictionary<RemoteCall, List<ServerModuleCall>> ();
		}


		#region public interface

		public ProxyInformation ProxyInformation {
			get { return pinfo; }
		}

		public Configuration Configuration {
			get { return cfg; }
		}
		public List<IServerModule> Modules {
			get { return modules; }
		}

		public SessionManager SessionManager {
			get { return sessionMan; }
		}



		#endregion

		private static string GetTempDir ()
		{
			return Environment.GetEnvironmentVariable ("temp");
		}

		public void Run ()
		{
			try {	

				//// Dump configuration to STDOUT
				Console.Write(cfg.GetConfigurationString());

				Console.Write ("Loading 'web <-> proxy remoting' ... ");


				InternalCallServer.Driver = this;

				InternalCallServer icall = new InternalCallServer ();
				icall.Run ();
				Console.WriteLine ("done.");

				Console.Write ("Loading 'http server' ... ");
				WebServer svr = new WebServer ();
				svr.Run ();
				Console.WriteLine ("done.");

				DetectModules ();
				CommonRpcService decoratedRpc = GenerateAssembly ();

				Console.Write ("Loading 'onc server' ... ");

				CompactTeaSharp.SslStore sslParams = null;
				if (cfg.X509CertificateCertFile != null){
					sslParams = 
						new CompactTeaSharp.SslStore(cfg.X509CertificateCertFile, cfg.X509CertificateKeyFile);
				}else{
					Console.Write ("No ssl credentials configured, starting server without encryption... \n");
				}

				OncRpcService oncService = new OncRpcService (decoratedRpc, sessionMan, 
											cfg.ListenAddress, cfg.ListenPort, sslParams);
				Thread oncThread = new Thread (new ThreadStart (oncService.Run));
				oncThread.Start ();

				Console.WriteLine ("done.");


/*				Shutdown.RegisterHandler ((ignored) => {
					// TODO: close stuff properly ...
					
					// oncService.Close ();
					
					Console.WriteLine ("Server stopped!");
					
				});

				Shutdown.BlockUntilShutdown ();				

*/
				
				Console.WriteLine ("Press ENTER to stop server.");
				Console.ReadLine ();
				
				oncService.Dispose ();
				
			} finally {
				
				// cleanup if required ....
			
				
			}
			Environment.Exit (1);
		}

		/*******************************************************************************/

		private CommonRpcService GenerateAssembly ()
		{
			object[] args = new object [1];
			args [0] = this;

			Type baseType = typeof (CommonRpcService);

			AssemblyName asmName = new AssemblyName ();
			asmName.Name = "DecoratedCommonRpcServiceAssembly";
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (
						asmName, AssemblyBuilderAccess.RunAndSave);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule ("MainServerModule");


			TypeAttributes attributes = TypeAttributes.Public;
			TypeBuilder typeBuilder = moduleBuilder.DefineType (
						"DecoratedCommonRpcService", 
						attributes, baseType);

			AddSingleParamConstructor (baseType, typeBuilder, typeof (Driver) );

			// inject calls ...

			MethodInfo [] baseMethods = baseType.GetMethods (BindingFlags.Public | BindingFlags.Instance);
			foreach (var methodInfo in baseMethods) {
				// check attribute ...
				var methodAttribs = methodInfo.GetCustomAttributes (typeof (MapiModableCallAttribute), false);
				if (methodAttribs.Length < 1)
					continue;
				MapiModableCallAttribute attribute = (MapiModableCallAttribute) methodAttribs [0];

				if (preCalls.ContainsKey (attribute.RemoteCall) || 
					postCalls.ContainsKey (attribute.RemoteCall))
				{
					var preMethods = new List<ServerModuleCall> ();
					var postMethods = new List<ServerModuleCall> ();

					if (preCalls.ContainsKey (attribute.RemoteCall))
						preMethods = preCalls [attribute.RemoteCall];
					if (postCalls.ContainsKey (attribute.RemoteCall))
						postMethods = postCalls [attribute.RemoteCall];

					ParameterInfo[] parameters = methodInfo.GetParameters ();
					Type[] methodArgs = new Type [parameters.Length];
					for (int i=0;i<parameters.Length;i++)
						methodArgs [i] = parameters [i].ParameterType;

					CreateMethod (baseType, typeBuilder, methodInfo.Name, 
						methodInfo.ReturnType, methodArgs, attribute.RemoteCall, 
						preMethods, postMethods);
				}
			}

			Type generatedClass = typeBuilder.CreateType ();

			return (CommonRpcService) Activator.CreateInstance (generatedClass, args);
		}

		private void CreateMethod (Type baseType, TypeBuilder typeBuilder, 
			string methodName, Type returnType, Type[] args, 
			RemoteCall remoteCall, List<ServerModuleCall> preMethods, 
			List<ServerModuleCall> postMethods)
		{
			MethodBuilder methodBuilder = typeBuilder.DefineMethod (methodName,
					MethodAttributes.Public | MethodAttributes.ReuseSlot |
					MethodAttributes.HideBySig | MethodAttributes.Virtual,
					CallingConventions.Standard, returnType, args);
			methodBuilder.InitLocals = true;

			ILGenerator methodCode = methodBuilder.GetILGenerator ();

			// local storage ...

			methodCode.DeclareLocal (typeof (Exception));
			if (returnType != typeof (void)) {
				LocalBuilder localBuilder = methodCode.DeclareLocal (returnType);
	//			methodCode.Emit (OpCodes.Ldnull);
	//			methodCode.Emit (OpCodes.Stloc_1); // init return value with null...
			}

			FieldInfo moduleField = baseType.GetField ("modules", BindingFlags.Instance | BindingFlags.NonPublic);
			if (moduleField == null)
				throw new Exception (baseType + " must contains a field 'modules'!");


			InjectMethods (methodCode, moduleField, methodName, args, remoteCall, preMethods);

			// TODO: Return value!

			// Call base ...
			methodCode.Emit (OpCodes.Ldarg_0); // Push object!
			for (int i=0;i<args.Length;i++)
				EmitAddress (methodCode, args [i], i);
			methodCode.Emit (OpCodes.Call, baseType.GetMethod (methodName, args));
			if (returnType != typeof (void))
				methodCode.Emit (OpCodes.Stloc_1); // store result locally ...

			InjectMethods (methodCode, moduleField, methodName, args, remoteCall, postMethods);

			// return ...
			if (returnType != typeof (void))
				methodCode.Emit (OpCodes.Ldloc_1);
			methodCode.Emit (OpCodes.Ret);
		}

		private void InjectMethods (ILGenerator methodCode, FieldInfo moduleField, 
			string methodName, Type[] args, RemoteCall remoteCall, 
			List<ServerModuleCall> methods)
		{
			// Catch exceptions ...
			methodCode.BeginExceptionBlock ();

			for (int i=0; i< methods.Count;i++) {
				ServerModuleCall call = methods [i];

				int moduleIndex = FindModuleIndex (call);
				PushItemFromListFieldOnStack<IServerModule> (methodCode, moduleField, moduleIndex);

				// Push parameters that match the call...
				if (call.IsUniversal) {
					if (!VerifyUniversalSignature (call))
						throw new Exception ("The module method '" + call.MethodName + 
							"' of module '" + call.Module.Name + "' does not " + 
							"match the required signature " + 
							"(RemoteCall call, object o) !");
					methodCode.Emit (OpCodes.Ldc_I4, (int) remoteCall); // Push Argument 1
					methodCode.Emit (OpCodes.Ldnull); // TODO: Push Argument 2

				} else {
					if (!VerifySignature (call, args, call.PassValuesByRef))
						throw new Exception ("The module method '" + call.MethodName + 
							"' of module '" + call.Module.Name + "' must match " + 
							"the parameter list of the remote method!");
					for (int l=0;l<args.Length;l++)
						if (call.PassValuesByRef)
							EmitRefAddress (methodCode, args [l], l);
						else
							EmitAddress (methodCode, args [l], l);
				}

				// Call the method on the module object (that is currently on the stack)
				var moduleType = call.Module.GetType ();
				var method = moduleType.GetMethod (call.MethodName);
				methodCode.Emit (OpCodes.Callvirt, method);
			}

			// Rethrow Mapi exception ...
			methodCode.BeginCatchBlock (typeof (MapiException));
			methodCode.Emit (OpCodes.Rethrow);

			// ... Or otherwise wrap exception in MapiException
			methodCode.BeginCatchBlock (typeof (Exception));
			CreateThrowMapiException (methodCode, "A Server module caused an exception!");
			methodCode.EndExceptionBlock ();
		}

		private void CreateThrowMapiException (ILGenerator methodCode, string message)
		{
			Type mapiException = typeof (MapiCallFailedException);
			var exceptionConstructor = mapiException.GetConstructor (
						new Type[] { typeof (string), typeof (Exception) });

			methodCode.Emit (OpCodes.Stloc_0); // store exception locally ...			
			methodCode.Emit (OpCodes.Ldstr, message); // push message ...
			methodCode.Emit (OpCodes.Ldloc_0); // push exception
			methodCode.Emit (OpCodes.Newobj, exceptionConstructor); // create MapiException	
			methodCode.Emit (OpCodes.Throw);
		}

		private void AddSingleParamConstructor (Type baseType, 
			TypeBuilder typeBuilder, Type paramType)
		{
			Type[] types = new Type[] { paramType } ;
			var constructor = typeBuilder.DefineConstructor (
					MethodAttributes.Public, 
					CallingConventions.Standard, 
					types);

			ILGenerator constructorCode = constructor.GetILGenerator ();
			var baseConstructor = baseType.GetConstructor (types);
			constructorCode.Emit (OpCodes.Ldarg_0);
			constructorCode.Emit (OpCodes.Ldarg_1); // parameter
			constructorCode.Emit (OpCodes.Call, baseConstructor);
			constructorCode.Emit (OpCodes.Ret);
		}


		private void PushItemFromListFieldOnStack<T> (ILGenerator methodCode, 
			FieldInfo moduleField, int index)
		{
			methodCode.Emit (OpCodes.Ldarg_0); // Push object!
			methodCode.Emit (OpCodes.Ldfld, moduleField); // Load module object from list ...
			methodCode.Emit (OpCodes.Ldc_I4, index);
			var listIndexer = typeof (List<T>).GetMethod ("get_Item");
			methodCode.Emit (OpCodes.Callvirt, listIndexer);
		}

		private ParameterInfo[] GetParameters (ServerModuleCall call)
		{
			Type moduleType = call.Module.GetType ();
			MethodInfo methodInfo = moduleType.GetMethod (call.MethodName);
			if (methodInfo == null)
				return null;
			return methodInfo.GetParameters ();
		}

		private bool VerifyUniversalSignature (ServerModuleCall call)
		{
			ParameterInfo[] paramInfos = GetParameters (call);
			if (paramInfos == null || paramInfos.Length != 2)
				return false;
			if (paramInfos [0].ParameterType != typeof (RemoteCall))
				return false;
			if (paramInfos [1].ParameterType != typeof (object))
				return false;
			return true;
		}

		private bool VerifySignature (ServerModuleCall call, Type[] args, bool passValuesByRef)
		{
			ParameterInfo[] paramInfos = GetParameters (call);
			if (paramInfos == null || paramInfos.Length != args.Length)
				return false;
			for (int i=0;i<args.Length;i++) {
				if (passValuesByRef) {
					var type = paramInfos [i].ParameterType;
					if (type.GetElementType () != args [i])
						return false;
				} else {
					if (paramInfos [i].ParameterType != args [i])
						return false;
				}
	

			}
			return true;
		}

		private int FindModuleIndex (ServerModuleCall call)
		{
			int moduleIndex = -1;
			for (int k=0; k<this.modules.Count; k++)
				if (modules [k] == call.Module)
					moduleIndex = k;
			if (moduleIndex == -1)
				throw new Exception ("Can't determine module!");
			return moduleIndex;
		}

		private void EmitAddress (ILGenerator methodCode, Type arg, int index)
		{
			methodCode.Emit (OpCodes.Ldarg, index+1);
		}

		private void EmitRefAddress (ILGenerator methodCode, Type arg, int index)
		{
			if (index < 128)
				methodCode.Emit (OpCodes.Ldarga_S, index+1);
			else
				methodCode.Emit (OpCodes.Ldarga, index+1);
		}

	}
}

