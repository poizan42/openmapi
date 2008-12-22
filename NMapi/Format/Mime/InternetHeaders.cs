//
// openmapi.org - NMapi C# Mime API - InternetHeaders.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
// Author Java: <a href="mailto:dog@gnu.org">Chris Burdess</a> version 1.4
// Author C#: Andreas Huegel, Topalis AG
//
// GNU JavaMail is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// GNU JavaMail is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//


using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMapi.Format.Mime;

namespace NMapi.Format.Mime
{
	public class InternetHeaders : IEnumerable
	{
		protected List<InternetHeader> headers = new List<InternetHeader> ();
		/// <summary>
		/// Create an empty InternetHeaders object.
		/// </summary>
		public InternetHeaders ()
		{
			headers.Add (new InternetHeader ("Return-Path", null));
			headers.Add (new InternetHeader ("Received", null));
			headers.Add (new InternetHeader ("Message-Id", null));
			headers.Add (new InternetHeader ("Resent-Date", null));
			headers.Add (new InternetHeader ("Date", null));
			headers.Add (new InternetHeader ("Resent-From", null));
			headers.Add (new InternetHeader ("From", null));
			headers.Add (new InternetHeader ("Reply-To", null));
			headers.Add (new InternetHeader ("To", null));
			headers.Add (new InternetHeader ("Subject", null));
			headers.Add (new InternetHeader ("Cc", null));
			headers.Add (new InternetHeader ("In-Reply-To", null));
			headers.Add (new InternetHeader ("Resent-Message-Id", null));
			headers.Add (new InternetHeader ("Errors-To", null));
			headers.Add (new InternetHeader ("Mime-Version", null));
			headers.Add (new InternetHeader ("Content-Type", null));
			headers.Add (new InternetHeader ("Content-Transfer-Encoding", null));
			headers.Add (new InternetHeader ("Content-MD5", null));
			headers.Add (new InternetHeader ("Content-Length", null));
			headers.Add (new InternetHeader ("Status", null));

		}

		/// <summary>
		/// Read and parse the given RFC822 message stream till the blank line separating the header from the body.
		/// </summary>
		///     throws MessagingException
		public InternetHeaders (Stream inS)
		{
			Load (inS);
		}

		/// <summary>
		/// Parses the specified RFC 822 message stream, storing the headers in
		/// this InternetHeaders.
		/// The stream is parsed up to the blank line separating the headers from
		/// the body, and is left positioned at the start of the body.
		/// Note that the headers are added into this InternetHeaders object:
		/// any existing headers in this object are not affected.
		/// @param is an RFC 822 input stream
		/// </summary>
		///     throws MessagingException
		public void Load (Stream inS)
		{
			try {
				int[] buf = new int[2];
				StringBuilder sb = new StringBuilder ();
				// cant use StreamReader here, as it will buffer uncontrollably and
				// the content part of the stream in inS will be truncated at the beginning
				for (buf[0] = inS.ReadByte (); buf[0] != -1; ) {
					if (buf[1] == '\r' && buf[0] == '\n') {
						String line = trim (sb.ToString ());
						if (line.Length == 0) {
							break;

						}
						AddHeaderLine (line);
						sb = new StringBuilder ();
						buf[1] = 0;
						buf[0] = inS.ReadByte ();
					} else {
						if (buf[1] != 0) sb.Append ((char)buf[1]);
						buf[1] = buf[0];
						buf[0] = inS.ReadByte ();
					}

				}
			} catch (IOException e) {
				throw new IOException ("I/O error", e);
			}
		}


		/// <summary>
		/// Adds an RFC 822 header-line to this InternetHeaders.
		/// If the line starts with a space or tab (a continuation line for a
		/// folded header), adds it to the last header line in the list.
		/// @param line the raw RFC 822 header-line
		/// </summary>
		public void AddHeaderLine (String line)
		{
			try {
				char c = line.ToCharArray (0, 1)[0];
				if (c == ' ' || c == '\t') // continuation character
				{
					int len = headers.Count;
					InternetHeader header = (InternetHeader)headers[len - 1];
					StringBuilder buffer = new StringBuilder ();
					buffer.Append (header.Value);
					buffer.Append ("\r\n");
					buffer.Append (line);
					header.Value = buffer.ToString ();
				} else {
					lock (headers) {
						headers.Add (new InternetHeader (line));
					}
				}
			} catch (ArgumentOutOfRangeException) {
			}
		}



		/// <summary>
		/// Add a header with the specified name and value to the header list.
		/// </summary>
		public void AddHeader (String name, String value)
		{
			lock (headers) {
				int len = headers.Count;
				for (int i = len - 1; i >= 0; i--) {
					InternetHeader header = (InternetHeader)headers[i];
					if (header.Equals (name)) {
						headers.Insert (i + 1, new InternetHeader (name, value));
						return;
					}
					if (header.Equals ("")) {
						len = i;
					}
				}
				headers.Insert (len, new InternetHeader (name, value));
			}

		}

		public void AddHeader (InternetHeader ih)
		{
			AddHeader (ih.Name, ih.Value);
		}

		/// <summary>
		/// Remove all header entries that match the given name
		/// </summary>
		public void RemoveHeader (String name)
		{
			lock (headers) {
				for (int i = 0; i < headers.Count; i++) {
					InternetHeader header = (InternetHeader)headers[i];
					if (header.Equals (name)) {
						headers.RemoveAt (i);
						i--;
					}
				}
			}

		}

		/// <summary>
		/// Change the first header line that matches name to have value, adding a new header if no existing header matches.
		/// </summary>
		public void SetHeader (String name, String value)
		{
			bool first = true;
			for (int i = 0; i < headers.Count; i++) {
				InternetHeader header = (InternetHeader)headers[i];
				if (header.Equals (name)) {
					if (first) {
						header.Value = value;
						first = false;
					} else {
						headers.RemoveAt (i);
						i--;
					}
				}
			}
			if (first) {
				AddHeader (name, value);
			}
		}

		public void SetHeader (InternetHeader ih)
		{
			SetHeader (ih.Name, ih.Value);
		}

		/// <summary>
		/// Get all the headers for this header name, returned as a single String, with headers separated by the delimiter.
		/// </summary>
		public String GetHeader (String name, String delimiter)
		{
			IEnumerable<Object> myHeaders = from h in headers
											where h.Name.ToLower () == name.ToLower () &&
													h.Value != null && h.Value.Trim () != string.Empty
											select (Object)h.Value;
			return Field.AppendItemsFormat (myHeaders, delimiter, name.Length);
		}

		public String GetHeader (String name)
		{
			return GetHeader (name, InternetHeader.GetDelimiter (name));
		}

		/// <summary>
		/// Get all the headers for this header name, returned as a InternetHeader Object, with headers separated by the delimiter.
		/// </summary>
		public InternetHeader GetInternetHeaders (String name)
		{
			return GetInternetHeaders (name, InternetHeader.GetDelimiter (name));
		}

		/// <summary>
		/// Get all the headers for this header name, returned as a InternetHeader Object, with headers separated by the delimiter.
		/// </summary>
		public InternetHeader GetInternetHeaders (String name, String delimiter)
		{
			String dels = String.IsNullOrEmpty (delimiter) ? InternetHeader.GetDelimiter (name) : delimiter;
			return new InternetHeader (name, GetHeader(name, dels));
		}

		/// <summary>
		/// Get all the headers for this header name, returned as an array of Strings, with header items split up at the delimiter.
		/// </summary>
		public String[] GetHeaderParts (String name, String delimiter)
		{

			IEnumerable<InternetHeader> myHeaders = from h in headers
												    where h.Name.ToLower () == name.ToLower ()
												    select h;
			List<String> ihs = new List<String> ();
			foreach (InternetHeader h in myHeaders) {
				String[] parts = h.GetParts (HeaderTokenizer.MIME, delimiter);
				ihs.AddRange (parts);
			}

			return ihs.ToArray ();
		}

		/// <summary>
		/// Return all the header lines as an Enumeration of Strings.
		/// </summary>
		public IEnumerator<String> GetAllHeaderLines ()
		{
			IEnumerable<String> myHeaders = from h in headers
											select h.ToString ();
			return myHeaders.GetEnumerator ();
		}

		private static String trim (String line)
		{
			int len = line.Length;
			if (len > 0 && line.ToCharArray (len - 1, 1)[0] == '\r') {
				line = line.Substring (0, len - 1);
			}
			return line;
		}

		public IEnumerator GetEnumerator ()
		{
			return headers.GetEnumerator ();
		}


		public void WriteTo (Stream os)
		{

			foreach (InternetHeader h in headers) {
				if (trim (h.Value) != "") {
					byte[] hBytes = Encoding.ASCII.GetBytes (h.ToString () + "\r\n");
					os.Write (hBytes, 0, hBytes.Length);
				}
			}
			os.Write (Encoding.ASCII.GetBytes ("\r\n"), 0, 2);
		}
	}

}
