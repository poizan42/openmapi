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
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Authenticode;
using MX = Mono.Security.X509;
using MC = Mono.Security.Cryptography;

namespace CompactTeaSharp {

    public class SslStore {
        private X509Certificate _cert = null;
        private RSA _rsa = null;

        /// <summary>
        /// Prompt for a password on the console
        /// </summary>
        /// <returns>Password stored in a secure string</returns>
        private static SecureString enterPasswd(string prompt) {
            SecureString passwd = new SecureString();
            Console.Write(prompt);
            ConsoleKeyInfo nextKey = Console.ReadKey(true);
            while(nextKey.Key != ConsoleKey.Enter) {
                if (nextKey.Key == ConsoleKey.Backspace) {
                    if (0 < passwd.Length) {
                        passwd.RemoveAt(passwd.Length - 1);
                        // erase the last * as well
                        Console.Write(nextKey.KeyChar);
                        Console.Write(" ");
                        Console.Write(nextKey.KeyChar);
                    }
                } else {
                    passwd.AppendChar(nextKey.KeyChar);
                    Console.Write("*");
                }
                nextKey = Console.ReadKey(true);
            }    
            Console.WriteLine();
            passwd.MakeReadOnly();
            return passwd;
        }

        private byte[] Load(string fileName) {
            byte[] data = null;
            using (FileStream fs = File.OpenRead (fileName)) {
                data = new byte [fs.Length];
                fs.Read (data, 0, data.Length);
                fs.Close ();
            }
            return data;
        }

        private MX.X509Certificate ImportPkcs12(byte[] rawData, SecureString passwd) {
            MX.X509Certificate _ret = null;
            MX.PKCS12 pfx = null;
            if (null == passwd) {
                pfx = new MX.PKCS12(rawData);
            } else {
                IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(passwd);
                try {
                    pfx = new MX.PKCS12(rawData, System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr));
                } finally {
                    System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
                }
            }
            if (0 < pfx.Certificates.Count)
                _ret = pfx.Certificates[0];
            if (0 < pfx.Keys.Count) {
                _ret.RSA = (pfx.Keys [0] as RSA);
                _ret.DSA = (pfx.Keys [0] as DSA);
            }
            return _ret;
        }

        private MX.X509Certificate Import(byte[] rawData, SecureString passwd) {
            MX.X509Certificate _ret = null;
            X509ContentType ct = X509ContentType.Unknown;
            try {
                ct = X509Certificate2.GetCertContentType(rawData);
            } catch { }
            if (null == passwd) {
                try {
                    _ret = new Mono.Security.X509.X509Certificate(rawData);
                } catch (Exception e) {
                    try {
                        _ret = ImportPkcs12(rawData, new SecureString());
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
                    _ret = ImportPkcs12(rawData, passwd);
                } catch {
                    // it's possible to supply a (unrequired/unusued) password
                    _ret = new Mono.Security.X509.X509Certificate(rawData);
                }
            }
            return _ret;
        }

        private MX.X509Certificate Import(string certFile, SecureString passwd) {
            byte[] rawData = Load(certFile);
            return Import(rawData, passwd);
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

        private byte[] PEM (string type, byte[] data) {
            string pem = Encoding.ASCII.GetString (data);
            string header = String.Format ("-----BEGIN {0}-----", type);
            string footer = String.Format ("-----END {0}-----", type);
            int start = pem.IndexOf (header) + header.Length;
            int end = pem.IndexOf (footer, start);
            string base64 = pem.Substring (start, (end - start));
            return Convert.FromBase64String (base64);
        }

        private void ReadKeyFile(string keyFile, SecureString passwd) {
            PrivateKey pvk = null;
            byte[] data = Load(keyFile);
            if ((data != null) && (data.Length > 3)) {
                if ((data[0] == 0x1e) && (data[1] == 0xf1) && (data[2] == 0xb5) && (data[3] == 0xb0)) {
                    // File starts with a PVK magic: read it as is.
                    if (null == passwd) {
                        pvk = new PrivateKey(data, null);
                    } else {
                        IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(passwd);
                        try {
                            pvk = new PrivateKey(data, System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr));
                        } finally {
                            System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
                        }
                    }
                    _rsa = pvk.RSA;
                    return;
                } else if ((data[0] == 0x2d) && (data[1] == 0x2d) && (data[2] == 0x2d) && (data[3] == 0x2d)) {
                    // File starts with "----": read PEM-encoded pkcs8 key
                    data = PEM("RSA PRIVATE KEY", data);
                    _rsa = MC.PKCS8.PrivateKeyInfo.DecodeRSA(data);
                    return;
                }
            }
            throw new Exception("Invalid private key file");
        }

        private void ReadFiles(string certFile, string keyFile, SecureString passwd) {
            MX.X509Certificate cert = null;
            if (null == passwd) {
                try {
                    cert = Import(certFile, passwd);
                } catch (CryptographicException ce) {
                    if ("Unable to decode certificate." == ce.Message) {
                        string prompt = String.Format ("\nEnter password for {0}: ", certFile);
                        passwd = enterPasswd(prompt);
                        cert = Import(certFile, passwd);
                    } else
                        throw ce;
                }
            } else
                cert = Import(certFile, passwd);
            ExtractPrivateKey(cert);
            _cert = new X509Certificate(cert.RawData);
            if (_rsa == null) {
                if ((null == keyFile) || (0 == keyFile.Length))
                    throw new Exception("A private key file is required");
                if (!File.Exists (keyFile))
                    throw new Exception("The specified private key file does not exist");
                if (null == passwd) {
                    try {
                        ReadKeyFile(keyFile, null);
                    } catch (CryptographicException ce) {
                        if ("Invalid data and/or password" == ce.Message) {
                            string prompt = String.Format ("\nEnter password for {0}: ", keyFile);
                            passwd = enterPasswd(prompt);
                            ReadKeyFile(keyFile, passwd);
                        } else
                            throw ce;
                    }
                } else
                    ReadKeyFile(keyFile, passwd);
            }
            if (null != _rsa)
                cert.VerifySignature(_rsa);
        }

        public X509Certificate Cert {
            get { return _cert; }
        }

        public RSA PrivateKey {
            get { return _rsa; }
        }

        public SslStore(string certFile, string keyFile, SecureString passwd) {
            if ((null == certFile) || (0 == certFile.Length))
                throw new Exception("At least a certificate file is required");
            if (!File.Exists (certFile))
                throw new Exception("The specified cert file does not exist");
            ReadFiles(certFile, keyFile, passwd);
        }

        public SslStore(string certFile, string keyFile) {
            if ((null == certFile) || (0 == certFile.Length))
                throw new Exception("At least a certificate file is required");
            if (!File.Exists (certFile))
                throw new Exception("The specified cert file does not exist");
            ReadFiles(certFile, keyFile, (SecureString)null);
        }
    }
}
