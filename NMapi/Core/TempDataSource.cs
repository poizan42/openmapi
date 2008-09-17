//
// openmapi.org - NMapi C# Mapi API - TempDataSource.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
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
using System.IO;

namespace NMapi {

	public class TempDataSource : JavaBeanDataSource, IDisposable
	{
		private bool disposed;
		private string fileName;
		private string contentType;

		// throws IOException
		public TempDataSource () : this ("application/temporary")
		{
		}
	

		// throws IOException
		public TempDataSource (string contentType)
		{
			this.contentType = contentType;
			this.fileName = Path.GetTempPath() + "TDS" + 
					Guid.NewGuid().ToString() + ".tmp";

			// this.file.DeleteOnExit (); // TODO
		}
	
		public void Dispose ()
		{
			if (disposed)
				return;
			disposed = true;
			if (File.Exists (fileName))
				File.Delete (fileName);
		}

		public void Close ()
		{
			Dispose ();
		}

		public string ContentType {
			get {
				return contentType;
			}
		}

		public string Name {
			get {
				return fileName;
			}
		}

		// throws IOException
		public StreamReader InputStream {
			get {
				return new StreamReader (File.OpenRead (fileName));
			}
		}

		// throws IOException
		public StreamWriter OutputStream {
			get {
				return new StreamWriter (File.OpenWrite (fileName));
			}
		}

	}
}
