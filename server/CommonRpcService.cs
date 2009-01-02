//
// openmapi.org - OpenMapi Proxy Server - CommonRpcService.cs
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
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

using System.Reflection;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Server {

	/// <summary>
	///  
	/// </summary>
	public partial class CommonRpcService
	{
		private Dictionary<string, List<IServerModule>> preModules;
		private Dictionary<string, List<IServerModule>> postModules;

		private bool sessionOpen;
		protected List<IServerModule> modules; // TODO: bad...
		
		public string Name {
			get {
				StringBuilder moduleList = new StringBuilder ();
				int i = 0;
				foreach (IServerModule module in modules) {
					moduleList.Append (module.ShortName);
					if (i < modules.Count-1)
						moduleList.Append (", ");
					i++;
				}
				return "NMapi Server "  + Version.ToString () + " (" + moduleList.ToString () + ")";
			}
		}

		public Version Version {
			get {
				return Assembly.GetExecutingAssembly ().GetName ().Version;
			}
		}
			
		private void DebugStartCall ()
		{
			Trace.WriteLine ("\n--> Backend:");
			Trace.Indent ();
		}
		
		private void DebugCleanUp (Exception e)
		{
			Trace.WriteLine ("<-- EXCEPTION:" + e + " \n");
			Trace.Unindent ();	
		}
		
		private void DebugEndCall ()
		{
			Trace.Unindent ();
			Trace.WriteLine ("<--\n");
		}
		
		public CommonRpcService (List<IServerModule> modules)
		{
			this.preModules = new Dictionary<string, List<IServerModule>> ();
			this.postModules = new Dictionary<string, List<IServerModule>> ();
				
			this.modules = modules;
			Console.WriteLine (Name);
		}

	// TODO
	//	[MapiModableCall (RemoteCall.PSEUDO_OpenSession)]
		public virtual CommonRpcObjRef OpenSession (Request request, string connectionString)
		{
			DebugStartCall (); try {
			CommonRpcObjRef result;
			try {
				var connStr = new ServerConnectionString (connectionString);
				string assemblyName = null;
				string typeName = null;
				string providerStr = connStr.TargetProvider;

				if (providerStr == null || providerStr == String.Empty)
					providerStr = "org.openmapi.txc";

				IMapiFactory factory = null;
				var providers = ProviderManager.FindProviders ();
				if (providers.ContainsKey (providerStr)) {
					string[] asmAndType = providers [providerStr];
					factory = ProviderManager.GetFactory (asmAndType);
				}
				try {
					factory = Activator.CreateInstance (assemblyName, 
							typeName).Unwrap () as IMapiFactory;
				} catch (Exception) {
					// ignore
				}
				
				if (factory == null)
					throw new MapiException ("Couldn't create provider from " + 
						"specified backend '"  + typeName + 
						" (Assembly: " + assemblyName + ")' !");
				result = request.ProxySession.ObjectStore.MapObject (factory.CreateMapiSession ());
			} catch (MapiException)  {
				throw;
			} catch (Exception e)  {
				throw new MapiException (e);
			}
			sessionOpen = true;
			DebugEndCall ();return result; } catch (Exception e) { DebugCleanUp (e); throw; }
		}


		// TODO
		//		[MapiModableCall (RemoteCall.PSEUDO_CloseSession)]
		public virtual void CloseSession ()
		{
			sessionOpen = false;
		}




		# region hack
		
		public virtual int IMsgStore_Advise (Request request, IMsgStore obj, byte [] entryID, NotificationEventType eventMask, int txcOutlookHackConnection)
		{
			DebugStartCall (); try {
			int result;
			try {
				result = request.ProxySession.EventDispatcher.Register (obj, entryID, eventMask, txcOutlookHackConnection);
				// TODO: sink will be identified by returned int!
			} catch (MapiException) {
				throw;
			} catch (Exception e) {
				throw new MapiException (e);
			}
			DebugEndCall ();return result; } catch (Exception e) { DebugCleanUp (e); throw; }
		}
		
		public virtual int IMapiTable_Advise_2 (Request request, IMapiTable obj, byte[] ignored, NotificationEventType eventMask, int txcOutlookHackConnection)
		{
			DebugStartCall (); try {
			try {
				return request.ProxySession.EventDispatcher.Register (obj, ignored, eventMask, txcOutlookHackConnection);
			} catch (MapiException) {
				throw;
			} catch (Exception e) {
				throw new MapiException (e);
			}
			DebugEndCall (); } catch (Exception e) { DebugCleanUp (e); throw; }
		}

		public virtual int IMapiTable_Advise (Request request, IMapiTable obj, NotificationEventType eventMask, int txcOutlookHackConnection)
		{
			DebugStartCall (); try {
			try {
				return request.ProxySession.EventDispatcher.Register (obj, null, eventMask, txcOutlookHackConnection);
			} catch (MapiException) {
				throw;
			} catch (Exception e) {
				throw new MapiException (e);
			}
			DebugEndCall (); } catch (Exception e) { DebugCleanUp (e); throw; }
		}

		#endregion

	}

}
