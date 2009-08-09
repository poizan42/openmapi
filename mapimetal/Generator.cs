//
// openmapi.org - NMapi C# Mapi API - Generator.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Data;
using System.IO;

using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.Linq;

using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Microsoft.JScript;

using Mono.Cecil;
using Mono.Cecil.Cil;

#if WITH_BOO
using Boo.CodeDom;
#endif

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	/// <summary>
	///  A Mapi-Model generator, similiar in spirit to sqlmetal.
	/// </summary>
	public sealed class Generator
	{
		private const string PRIVATE_PROPERTY_PREFIX = "_metal_";

		private string extendedClass;
		private string nameSpace;
		private string className;
		private List<PropertyDescription> props;

		private TypeResolver resolver;

		public TypeResolver Resolver {
			get { return resolver; }
		}

		public ICodeGenerator CodeGenerator {
			get; set;
		}

		public Generator ()
		{
			resolver = new TypeResolver ();
			resolver.AddAssembly ("mscorlib", new Version (2, 0));
			resolver.AddAssembly ("System", new Version (2, 0));
			resolver.AddAssembly ("NMapi", new Version (0, 1));

			extendedClass = "MapiEntityBase";
			nameSpace = String.Empty;
			className = String.Empty;
			props = new  List<PropertyDescription> ();
		}

		public void Process (string fileName, string outputFileName)
		{
			XmlReaderSettings metalXml = new XmlReaderSettings ();

			Assembly asm = this.GetType().Assembly;
			Stream xsdStream = asm.GetManifestResourceStream ("MapiMetal.xsd");
			metalXml.Schemas.Add (null, XmlReader.Create (xsdStream));

			metalXml.ValidationType = ValidationType.Schema;
			metalXml.ValidationEventHandler += XmlValEventHandler;

			XmlValidatingReader xml = new XmlValidatingReader (
					XmlReader.Create (fileName, metalXml));

			while (xml.Read ()) {
				if (xml.NodeType !=  XmlNodeType.Element)
					continue;
				if (xml.Name == "mapimetal") {
					nameSpace = xml.GetAttribute ("namespace");
					className = xml.GetAttribute ("class");
				}
				else if (xml.Name == "property") {
					var pdesc = new PropertyDescription (resolver);
					pdesc.LineNumber = xml.LineNumber;
					string str = xml.GetAttribute ("type");
					if (str == null)
						str = String.Empty;
					pdesc.PropertyTypeString = str.Trim ();

					string namedprop = xml.GetAttribute ("namedprop");
					if (namedprop != null)
						namedprop = namedprop.Trim ().ToLower ();
					if (namedprop == "yes")
						pdesc.NamedProperty = true;

					string lmode = xml.GetAttribute ("load");
					if (lmode != null)
						lmode = lmode.Trim ().ToLower ();
					if (lmode == null || lmode == String.Empty)
						pdesc.LoadMode = LoadMode.PreFetch;
					else {
						switch (lmode) {
							case "prefetch":
								pdesc.LoadMode = LoadMode.PreFetch;
							break;
							case "lazy":
								pdesc.LoadMode = LoadMode.Lazy;
							break;
							case "stream":
								pdesc.LoadMode = LoadMode.Stream;
							break;
							default:
								Driver.WriteError (xml.LineNumber, xml.LinePosition, "Unknown LoadMode!");
							break;
						}
					}

					string databind = xml.GetAttribute ("databind");
					if (databind != null)
						databind = databind.Trim ().ToLower ();
					if (databind == null || databind == String.Empty)
						pdesc.DataBind = true;
					else if (databind == "false")
						pdesc.DataBind = false;
					else
						Driver.WriteError (xml.LineNumber, xml.LinePosition, "Unknown DataBind value!");

					string guid = xml.GetAttribute ("guid");
					if (guid == null)
						guid = String.Empty;
					pdesc.Guid = guid.Trim ();

					pdesc.Name = xml.ReadString ().Trim (); // go to inner node!
					foreach (PropertyDescription pd in props)
						if (pd.Name == pdesc.Name)
							Driver.WriteError (xml.LineNumber, xml.LinePosition, 
								"Property with name '"+ pdesc.Name + "' already defined!");
					props.Add (pdesc);
				}
				if (nameSpace != null)
					nameSpace = nameSpace.Trim ();
				if (nameSpace == String.Empty)
					nameSpace = null;
				if (className == null || className == String.Empty)
					Driver.WriteError (0, 0, "Class-Name can't be empty!");
			}

			// TODO: Namespace is optional!

			CodeNamespace ns = new CodeNamespace (nameSpace);
		
			ns.Imports.Add (new CodeNamespaceImport ("System"));
			ns.Imports.Add (new CodeNamespaceImport ("NMapi"));
			ns.Imports.Add (new CodeNamespaceImport ("NMapi.Flags"));
			ns.Imports.Add (new CodeNamespaceImport ("NMapi.Table"));
			ns.Imports.Add (new CodeNamespaceImport (Driver.MAPI_LINQ_NS));

			CodeTypeDeclaration generatedClass = new CodeTypeDeclaration (className);
			generatedClass.IsPartial = true;
			generatedClass.Attributes = MemberAttributes.Public;
			generatedClass.BaseTypes.Add (extendedClass);

			ns.Types.Add (generatedClass);

			if (Driver.Errors > 0)
				Driver.ExitWithStats ();

			string tmpFile = outputFileName + ".tmp";
			using (TextWriter streamWriter = new StreamWriter (tmpFile))
			{
				foreach (PropertyDescription pdesc in props)
					AttachProperty (generatedClass, pdesc);

				streamWriter.WriteLine ("//");
				streamWriter.WriteLine ("// This file was AUTOGENERATED by MapiMetal.");
				streamWriter.WriteLine ("// Do NOT edit. Please edit '" + fileName + "' instead.");
				streamWriter.WriteLine ("//\n");

				CodeGeneratorOptions options = new CodeGeneratorOptions ();
				options.BracingStyle = "C";
				options.IndentString = "\t";

				StringBuilder sbCode = new StringBuilder ();
				StringWriter sw  = new StringWriter (sbCode);
				CodeGenerator.GenerateCodeFromNamespace (ns, sw, options);
				streamWriter.WriteLine (sbCode.ToString ());
			}

			if (Driver.Errors > 0) {
				if (File.Exists (tmpFile))
					File.Delete (tmpFile);
				Driver.ExitWithStats ();
			} else {
				if (File.Exists (outputFileName))
					File.Delete (outputFileName);
				File.Move (tmpFile, outputFileName);
			}
		}

		private void XmlValEventHandler (object sender, ValidationEventArgs e)
		{
			int lnum = e.Exception.LineNumber;
			int lpos = e.Exception.LinePosition;
			if (e.Severity == XmlSeverityType.Warning)
				Driver.WriteWarning (lnum, lpos, e.Message);
			else if (e.Severity == XmlSeverityType.Error)
				Driver.WriteError (lnum, lpos, e.Message);
		}

		private void AttachProperty (CodeTypeDeclaration generatedClass, 
			PropertyDescription pdesc)
		{
			var privateStorageField = new CodeMemberField (
				pdesc.CSharpType, PRIVATE_PROPERTY_PREFIX  + pdesc.Name);				
			privateStorageField.Attributes = MemberAttributes.Private;
			// primitives
			switch (pdesc.CSharpType) {
				case "System.Int32":
				case "System.Short":
				case "System.Float":
				case "System.Double":
				case "System.Long":
				case "System.Int64":
					privateStorageField.InitExpression = new CodePrimitiveExpression (0);
				break;
				// Not really possible, but check just in case ...
				case "System.Boolean":
				case "System.Byte":
				case "System.Int16":
				case "System.UInt16":
				case "System.UInt32":
				case "System.UInt64":
				case "System.IntPtr":
				case "System.UIntPtr":
				case "System.Char":
				case "System.Single":
					throw new Exception ("Type '" + 
						pdesc.CSharpType + 
						"' not supported by generator!");
				default:
					privateStorageField.InitExpression = new CodePrimitiveExpression (null);
				break;
			}

			generatedClass.Members.Add (privateStorageField);

			var property = new CodeMemberProperty ();
			property.Name = pdesc.Name;
			property.Type = new CodeTypeReference (pdesc.CSharpType);
			property.Attributes = MemberAttributes.Public;

			byte[] guidBytes = null;
			string guidStr = pdesc.Guid;
			if (guidStr == String.Empty)
				guidStr = null;

			if (guidStr != null && guidStr.StartsWith ("{")) {
				if (guidStr.EndsWith ("}")) {
					string[] hexBytes = guidStr.Substring (1, guidStr.Length-2).Split ('-');
					if (hexBytes.Length != 16)
						throw  new Exception ("The guid must consist of exactly 16 bytes.");
					guidBytes = new byte [hexBytes.Length];
					for (int i=0;i<hexBytes.Length;i++)
						guidBytes [i] = Byte.Parse (hexBytes [i], NumberStyles.HexNumber);

				} else {
					throw new Exception ("Missing '}' at end of " + 
						"guid-attribute for property '" + pdesc.Name + "'.");
				}
			} else {
				if (guidStr != null) {
					string[] tmp = TypeResolver.SplitTypeNameAndProperty (guidStr);
					string typeName = tmp [0];
					string fieldName = tmp [1];

					Assembly asm = Assembly.Load ("NMapi"); // TODO! => all/correct assemblies
					Type [] cls = asm.GetTypes ();
					foreach (Type cl in cls) {
						if (cl.FullName == typeName) {
							System.Reflection.FieldInfo fi = cl.GetField (fieldName);
							NMapiGuid guid = (NMapiGuid) fi.GetValue (null);
							guidBytes = guid.ToByteArray ();
						}
					}

					if (guidBytes != null)
						Console.WriteLine ("resolved guid: " + guidBytes.Length);

				}

				// SPropTagArray propArray = msg.GetIDsFromNames (names, 0);

			}

			var name = Driver.MAPI_LINQ_NS + ".MapiProperty";
			string namedProp = "NamedProperty.No";
			if (pdesc.NamedProperty)
				namedProp = "NamedProperty.Yes";

			var namedPropertyArgument = new CodeAttributeArgument (
					new CodeTypeReferenceExpression (namedProp));

			CodeTypeReference tr = new CodeTypeReference (typeof (byte));

			CodeAttributeArgument guidAttribute;
			if (guidBytes == null)
				guidAttribute = new CodeAttributeArgument (
						new CodePrimitiveExpression (null));
			else {
				var primitive = new CodePrimitiveExpression [guidBytes.Length];
				for (int i = 0; i < guidBytes.Length; i++)
					primitive [i] = new CodePrimitiveExpression (guidBytes [i]);
				guidAttribute = new CodeAttributeArgument (
						new CodeArrayCreateExpression (tr, primitive));
			}

			var propertyTypeArgument = new CodeAttributeArgument (
				new CodeTypeReferenceExpression (pdesc.PropertyTypeString));

			var mapiTypeArgument = new CodeAttributeArgument (
					new CodeTypeReferenceExpression (pdesc.MapiType));

			var loadModeArgument = new CodeAttributeArgument (
				new CodeTypeReferenceExpression (
					Driver.MAPI_LINQ_NS + ".LoadMode." + pdesc.LoadMode));

			var allowDataBinding = new CodeAttributeArgument (
				new CodePrimitiveExpression (pdesc.DataBind));

			var mapiAttribute = new CodeAttributeDeclaration (name, 
				namedPropertyArgument, guidAttribute, propertyTypeArgument, 
				mapiTypeArgument, loadModeArgument, allowDataBinding);

			if (property.CustomAttributes == null)
				property.CustomAttributes = new CodeAttributeDeclarationCollection ();
			property.CustomAttributes.Add  (mapiAttribute);

			var privateFieldRef = new CodeFieldReferenceExpression (
				new CodeThisReferenceExpression (), 
				privateStorageField.Name);

			AddGetStatements (property, pdesc, privateFieldRef);
			AddSetStatements (property, pdesc, privateFieldRef);
			generatedClass.Members.Add (property);
		}

		private void AddGetStatements (CodeMemberProperty property, 
			PropertyDescription pdesc, CodeFieldReferenceExpression privateFieldRef)
		{
			var retStmt = new CodeMethodReturnStatement ();
			retStmt.Expression = privateFieldRef;
			if (pdesc.LoadMode == LoadMode.Lazy) {
				var checkLazyIsLoadedCall = new CodeMethodInvokeExpression (
					new CodeThisReferenceExpression (),
					"CheckLazyIsLoaded", new CodeExpression [] { 
						new CodePrimitiveExpression (pdesc.Name) });

				var parameters = new CodeExpression [] {
					new CodePrimitiveExpression (pdesc.Name)
				};

				var lazyCall = new CodeMethodInvokeExpression (
					new CodeThisReferenceExpression (),
					"LazyLoad", parameters);
				var cast = new CodeCastExpression (pdesc.CSharpType, lazyCall);
				var assignStmt = new CodeAssignStatement (
						privateFieldRef, cast);
				var checkTrueStatements = new CodeStatement [] { 
					assignStmt
				};
				var ifLoadedStmt = new CodeConditionStatement (
					new CodeBinaryOperatorExpression(
						checkLazyIsLoadedCall,
						CodeBinaryOperatorType.ValueEquality,
						new CodePrimitiveExpression (false)), 
					checkTrueStatements, new CodeStatement [] {});
				property.GetStatements.Add (ifLoadedStmt);

			}
			property.GetStatements.Add (retStmt);
		}

		private void AddSetStatements (CodeMemberProperty property, 
			PropertyDescription pdesc, CodeFieldReferenceExpression privateFieldRef)
		{
			var propChangingCall = new CodeMethodInvokeExpression (
				new CodeThisReferenceExpression (),
				"OnPropertyChanging", new CodeExpression [] { 
					new CodePrimitiveExpression (pdesc.Name) });

			var propChangedCall = new CodeMethodInvokeExpression (
				new CodeThisReferenceExpression (),
				"OnPropertyChanged", new CodeExpression [] { 
					new CodePrimitiveExpression (pdesc.Name) });

			var setStatements = new CodeStatement [3];
			setStatements [0] = new CodeExpressionStatement (propChangingCall);
			setStatements [1] = new CodeAssignStatement (
						privateFieldRef, 
						new CodePropertySetValueReferenceExpression ());
			setStatements [2] = new CodeExpressionStatement (propChangedCall);

			var comparisonExpression = new CodeBinaryOperatorExpression (
						privateFieldRef, 
						CodeBinaryOperatorType.IdentityInequality,
						new CodePropertySetValueReferenceExpression ());

			var ifStmt = new CodeConditionStatement (
				comparisonExpression, setStatements, 
				new CodeExpressionStatement [] {});
			property.SetStatements.Add (ifStmt);
		}
	}
}

