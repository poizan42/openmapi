//
// openmapi.org - NMapi C# Mime API - MimeMultipart.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
// Author Java: <a href="mailto:dog@gnu.org">Chris Burdess</a>
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
	public class MimeMultipart : IEnumerable
	{

		MimeMessage parent;
		List<MimeBodyPart> parts = new List<MimeBodyPart> ();
		
		String preamble = "";


		/// <summary>
		/// Constructs a MimeMultipart object and its bodyparts from the given DataSource.
		/// </summary>
		public MimeMultipart (MimeMessage mm)
		{
			Stream inS = mm.ContentStream;
			Connect(mm);
			
			try {
				Parse (inS);
			} catch (MessagingException) { }
		}
		
		public void Connect(MimeMessage mm)
		{
			if (parent == null)
			{
				parent = mm;
				mm.Content = this;

				try {
					if (mm.Boundary == null)
						mm.Boundary = "";     // implicitly set Boundary to newly generated value
				} catch (Exception) {
					mm.Boundary = ""; // implicitly set Boundary to newly generated value
				}
			}
		}
		
		public void Disconnect()
		{
			if (parent != null) {
				MimeMessage mm = parent;
				parent = null;
				mm.Content = null;
			}
		}
		
		public String Preamble  {
			get { return preamble; }
			set { preamble = value; }
		}
		
		protected void Parse (Stream inS)
		{
			if (inS != null) {
				String boundary = "\r\n--" + parent.Boundary;
				byte[] boundaryBytes = Encoding.ASCII.GetBytes (boundary);
				BinarySplitter bs = new BinarySplitter (inS);
				Stream x;
				bool preambleCheck = false;
				while ((x = bs.ReadToDelimiter (boundaryBytes)) != null) {

					if (!preambleCheck) {
						preamble = new StreamReader(x,Encoding.ASCII).ReadToEnd();
						preambleCheck = true;
					} else {
						MimeBodyPart mp = new MimeBodyPart (x);
						if (mp.ContentType != "" && mp.ContentType != null) {
							parts.Add (mp);
						}
					}
					int b;
					// check if Message-end Sign appears (-- after boundary)
					if ((b = bs.ReadByte ()) == '-')
						if ((b = bs.ReadByte ()) == '-') break;
					// otherwise eliminate LineBreak after Boundary sign
					if (b != '\r')
						throw new MessagingException ("No correct line break after boundary sign");
					if ((b = bs.ReadByte ()) != '\n')
						throw new MessagingException ("No correct line break after boundary sign");
				}

				bs.Close ();
			}
		}


		/// <summary>
		/// Adds a Part to the multipart.
		/// </summary>
		public void AddBodyPart (MimeBodyPart part)
		{
			parts.Add (part);
		}

		/// <summary>
		/// Adds a BodyPart at position index.
		/// </summary>
		public void AddBodyPart (MimeBodyPart part, int index)
		{
			parts.Insert (index, part);

		}

		/// <summary>
		/// Get the specified BodyPart.
		/// </summary>
		[System.Runtime.CompilerServices.IndexerName ("BodyPart")]
		public MimeBodyPart this[int i] {
      get { return parts[i]; }
			set { AddBodyPart (value, i); }
		}
		
		public int Count {
			get { return parts.Count; }
		}
			
		public IEnumerator GetEnumerator()
		{
			return parts.GetEnumerator();
		}

		/// <summary>
		/// Parse the InputStream from our DataSource, constructing the appropriate MimeBodyParts.
		/// </summary>
		protected void Parse ()
		{
		}

		/// <summary>
		/// Remove the specified part from the multipart message.
		/// </summary>
		void RemoveBodyPart (MimeBodyPart part)
		{
			parts.Remove(part); 
		}

		/// <summary>
		/// Remove the part at specified location (starting from 0).
		/// </summary>
		void RemoveBodyPart (int index)
		{
			parts.RemoveAt(index);
		}

		/// <summary>
		/// Set the subtype.
		/// </summary>
		String SubType {
			get 
			{ 
				if (parent != null)	{
					InternetHeader ih = parent.GetInternetHeader(MimePart.CONTENT_TYPE_NAME, "; ");
					return ih.GetSubtype();
				}
				return null;
			}
			set 
			{
				if (parent != null) {
					InternetHeader ih = parent.GetInternetHeader(MimePart.CONTENT_TYPE_NAME, "; ");
					ih.SetSubtype(value);
					parent.SetHeader(ih);
				}
			}
		}

		/// <summary>
		/// Iterates through all the parts and outputs each MIME part separated by a boundary.
		/// </summary>
		public void WriteTo (Stream os)
		{

			String boundary = "\r\n--" + parent.Boundary;
			byte[] boundaryBytes = Encoding.ASCII.GetBytes (boundary);
			if (preamble != null)
			{
				byte[] preambleBytes = Encoding.ASCII.GetBytes (preamble);
				os.Write(preambleBytes, 0, preambleBytes.Length);
			}
			foreach (MimePart p in parts) {
				os.Write (boundaryBytes, 0, boundaryBytes.Length);
				os.Write (Encoding.ASCII.GetBytes ("\r\n"), 0, 2);
				p.WriteTo (os);
			}
			os.Write (boundaryBytes, 0, boundaryBytes.Length);
			os.Write (Encoding.ASCII.GetBytes ("--\r\n\r\n"), 0, 6);
		}

	}
}
