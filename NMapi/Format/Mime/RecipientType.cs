//
// openmapi.org - NMapi C# Mime API - RecipientType.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMapi.Format.Mime
{
	public class RecipientType
	{
		protected String val;

		protected RecipientType (String val) { this.val = val; }

		private static RecipientType rt_TO;
		private static RecipientType rt_CC;
		private static RecipientType rt_BCC;

		public static RecipientType TO
		{
			get { return rt_TO; }
		}
		public static RecipientType CC
		{
			get { return rt_CC; }
		}
		public static RecipientType BCC
		{
			get { return rt_BCC; }
		}

		static RecipientType ()
		{
			rt_TO = new RecipientType ("To");
			rt_CC = new RecipientType ("Cc");
			rt_BCC = new RecipientType ("Bcc");
		}


		public override String ToString () { return val; }
	}
}
