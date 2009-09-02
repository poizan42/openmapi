//
// openmapi.org - NMapi C# Mapi API - ProxySession.cs
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
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Authenticode;
using MX = Mono.Security.X509;
using MC = Mono.Security.Cryptography;

namespace CompactTeaSharp {

    public class SslStore {
        X509Certificate _cert = null;
        RSA _rsa = null;

		private byte[] Load(string fileName) {
			byte[] data = null;
			using (FileStream fs = File.OpenRead (fileName)) {
				data = new byte [fs.Length];
				fs.Read (data, 0, data.Length);
				fs.Close ();
			}
			return data;
		}

        private MX.X509Certificate ImportPkcs12(byte[] rawData, string password) {
		    MX.X509Certificate _ret = null;
            MX.PKCS12 pfx = (null == password) ? new MX.PKCS12(rawData) : new MX.PKCS12(rawData, password);
            if (0 < pfx.Certificates.Count)
                _ret = pfx.Certificates[0];
            if (0 < pfx.Keys.Count) {
                _ret.RSA = (pfx.Keys [0] as RSA);
                _ret.DSA = (pfx.Keys [0] as DSA);
            }
            return _ret;
        }

        private MX.X509Certificate Import(byte[] rawData, string password) {
		    MX.X509Certificate _ret = null;
            X509ContentType ct = X509ContentType.Unknown;
            try {
                ct = X509Certificate2.GetCertContentType(rawData);
            } catch { }
            if (password == null) {
                try {
                    _ret = new Mono.Security.X509.X509Certificate(rawData);
                } catch (Exception e) {
                    try {
                        _ret = ImportPkcs12(rawData, "x");
                    } catch (Exception e2) {
                        if (X509ContentType.Pkcs12 == ct)
                            throw new CryptographicException("Unable to decode certificate.", e2);
                        else
                            throw new CryptographicException("Unable to decode certificate.", e);
                    }
                }
            } else {
                // try PKCS#12
                try {
                    _ret = ImportPkcs12(rawData, password);
                } catch {
                    // it's possible to supply a (unrequired/unusued) password
                    _ret = new Mono.Security.X509.X509Certificate(rawData);
                }
            }
            return _ret;
        }

        private MX.X509Certificate Import(string certFile, string password) {
            byte[] rawData = Load(certFile);
            return Import(rawData, password);
        }

        private MX.X509Certificate Import(string certFile) {
            return Import(certFile,  (string)null);
        }

        private void ExtractPrivateKey(MX.X509Certificate cert) {
            if (null != cert) {
                try {
                    if (cert.RSA != null) {
                        RSACryptoServiceProvider rcsp = cert.RSA as RSACryptoServiceProvider;
                        if (rcsp != null)
                            _rsa = rcsp.PublicOnly ? null : rcsp;
                        MC.RSAManaged rsam = cert.RSA as MC.RSAManaged;
                        if (rsam != null)
                            _rsa = rsam.PublicOnly ? null : rsam;
                        cert.RSA.ExportParameters(true);
                        _rsa = cert.RSA;
                    }
                } catch { }
            }
        }

		byte[] PEM (string type, byte[] data) {
			string pem = Encoding.ASCII.GetString (data);
			string header = String.Format ("-----BEGIN {0}-----", type);
			string footer = String.Format ("-----END {0}-----", type);
			int start = pem.IndexOf (header) + header.Length;
			int end = pem.IndexOf (footer, start);
			string base64 = pem.Substring (start, (end - start));
			return Convert.FromBase64String (base64);
		}

        private void ReadKeyFile(string keyFile) {
            PrivateKey pvk = null;
            byte[] data = Load(keyFile);
            if ((data != null) && (data.Length > 3)) {
                // Does file start with a PVK magic?
                if ((data[0] == 0x1e) && (data[1] == 0xf1) && (data[2] == 0xb5) && (data[3] == 0xb0)) {
                    pvk = new PrivateKey(data, null);
                    _rsa = pvk.RSA;
                    return;
                } else if (data[0] == 0x2d) {
                    data = PEM("RSA PRIVATE KEY", data);
                    _rsa = MC.PKCS8.PrivateKeyInfo.DecodeRSA(data);
                    return;
                }
            }
            throw new Exception("Invalid private key file (only PVK keys are supported for now)");
        }

        private void ReadFiles(string certFile, string keyFile) {
            MX.X509Certificate cert = Import(certFile);
            ExtractPrivateKey(cert);
            _cert = new X509Certificate(cert.RawData);
            if (_rsa == null) {
                if (null == keyFile)
                    throw new Exception("A private key file is required");
                if (!File.Exists (keyFile))
                    throw new Exception("The specified private key file does not exist");
                ReadKeyFile(keyFile);
            }
            if (null != _rsa)
                cert.VerifySignature(_rsa);
        }

        public X509Certificate GetCertificate() {
            return _cert;
        }

        public RSA GetPrivateKey() {
            return _rsa;
        }

        public static SslStore CreateFromFiles(string certFile, string keyFile) {
            if (null == certFile)
                throw new Exception("At least a certificate file is required");
            if (!File.Exists (certFile))
                throw new Exception("The specified cert file does not exist");
            SslStore h = new SslStore();
            h.ReadFiles(certFile, keyFile);
            return h;
        }
    }
}
