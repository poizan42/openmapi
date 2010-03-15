//
// openmapi.org - OpenMapi Proxy Server - CertificateManager.cs
//
// Copyright 2010 Topalis AG
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
using System.IO;
using System.Security.Cryptography;
using Mono.Security.Authenticode;
using Mono.Security.X509;

namespace NMapi.Server {

	/// <summary>Helper to create default SSL certificates.</summary>
	public static class CertificateManager
	{
		public static bool CheckValidCertificates (string crtFile, string keyFile)
		{
			try {
				if (!File.Exists (crtFile) || !File.Exists (keyFile))
					return false;
					
				if (new FileInfo (crtFile).Length == 0 || new FileInfo (keyFile).Length == 0)
					return false;
				// TODO: add checks to detect invalid file content.
				
				return true;
			} catch (Exception) {
				return false;
			}
		}
		
		//
		// This code has been adapted from Mono.Security
		//

		private static string MonoTestRootAgency = "<RSAKeyValue><Modulus>v/4nALBxCE+9JgEC0LnDUvKh6e96PwTpN4Rj+vWnqKT7IAp1iK/JjuqvAg6DQ2vTfv0dTlqffmHH51OyioprcT5nzxcSTsZb/9jcHScG0s3/FRIWnXeLk/fgm7mSYhjUaHNI0m1/NTTktipicjKxo71hGIg9qucCWnDum+Krh/k=</Modulus><Exponent>AQAB</Exponent><P>9jbKxMXEruW2CfZrzhxtull4O8P47+mNsEL+9gf9QsRO1jJ77C+jmzfU6zbzjf8+ViK+q62tCMdC1ZzulwdpXQ==</P><Q>x5+p198l1PkK0Ga2mRh0SIYSykENpY2aLXoyZD/iUpKYAvATm0/wvKNrE4dKJyPCA+y3hfTdgVag+SP9avvDTQ==</Q><DP>ISSjCvXsUfbOGG05eddN1gXxL2pj+jegQRfjpk7RAsnWKvNExzhqd5x+ZuNQyc6QH5wxun54inP4RTUI0P/IaQ==</DP><DQ>R815VQmR3RIbPqzDXzv5j6CSH6fYlcTiQRtkBsUnzhWmkd/y3XmamO+a8zJFjOCCx9CcjpVuGziivBqi65lVPQ==</DQ><InverseQ>iYiu0KwMWI/dyqN3RJYUzuuLj02/oTD1pYpwo2rvNCXU1Q5VscOeu2DpNg1gWqI+1RrRCsEoaTNzXB1xtKNlSw==</InverseQ><D>nIfh1LYF8fjRBgMdAH/zt9UKHWiaCnc+jXzq5tkR8HVSKTVdzitD8bl1JgAfFQD8VjSXiCJqluexy/B5SGrCXQ49c78NIQj0hD+J13Y8/E0fUbW1QYbhj6Ff7oHyhaYe1WOQfkp2t/h+llHOdt1HRf7bt7dUknYp7m8bQKGxoYE=</D></RSAKeyValue>";
		private static string defaultIssuer = "CN=Mono Test Root Agency";
		private static string defaultSubject = "CN=Poupou's-Software-Factory";

		public static void Generate (string crtFile, string pvkFile)
		{
			byte[] sn = Guid.NewGuid ().ToByteArray ();
			RSA issuerKey = (RSA) RSA.Create ();
			issuerKey.FromXmlString (MonoTestRootAgency);
			RSA subjectKey = (RSA) RSA.Create ();

			PrivateKey key = new PrivateKey ();
			key.RSA = subjectKey;
			key.Save (pvkFile);

			try {
				// serial number MUST be positive
				if ((sn [0] & 0x80) == 0x80)
					sn [0] -= 0x80;

				X509CertificateBuilder cb = new X509CertificateBuilder (3);
				cb.SerialNumber = sn;
				cb.IssuerName = defaultIssuer;
				cb.NotBefore = DateTime.Now;
				cb.NotAfter = new DateTime (643445675990000000); // 12/31/2039 23:59:59Z
				cb.SubjectName = defaultSubject;
				cb.SubjectPublicKey = subjectKey;
				cb.Hash = "SHA1";
				
				// TODO: handle exceptions!

				FileStream fs = File.Open (crtFile, FileMode.Create, FileAccess.Write);
				byte[] rawcert = cb.Sign (issuerKey);
				fs.Write (rawcert, 0, rawcert.Length);
				fs.Close ();
				
			} catch (Exception e) {
				Console.WriteLine ("ERROR: " + e.ToString ());
			}
		}
	}

}

