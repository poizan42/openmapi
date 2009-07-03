//
// openmapi.org - NMapi C# Mime API - TestMime.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
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

// controls, output of WriteTo-Tests into File with MethodName of calling funktion
// uncomment the undef to make it work
#define WRITETO_FILE
//#undef WRITETO_FILE

namespace NMapi.Format.Mime
{

	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NUnit.Framework;
	using System.Diagnostics;
	using System.Reflection;
 


	[TestFixture]
	public class TestInternetAddress
	{

		[Test]
		public void ConstructInternetAddress ()
		{

			InternetAddress ia;

			ia = new InternetAddress ();
			ia.Personal = "Andreas Hügel";
			Assert.AreEqual ("Andreas Hügel<>", ia.Address);
			Assert.AreEqual ("=?utf-8?Q?Andreas_H=C3=BCgel?= <>", ia.ToString ());
			ia.Email = null;
			Assert.AreEqual ("Andreas Hügel<>", ia.Address);
			Assert.AreEqual ("=?utf-8?Q?Andreas_H=C3=BCgel?= <>", ia.ToString ());
			ia.Email = "";
			Assert.AreEqual ("Andreas Hügel<>", ia.Address);
			Assert.AreEqual ("=?utf-8?Q?Andreas_H=C3=BCgel?= <>", ia.ToString ());


			ia = new InternetAddress ();
			ia.Email = "andreas.huegel@topalis.com";
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.Address);
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.ToString ());
			ia.Personal = null;
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.Address);
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.ToString ());
			ia.Personal = "";
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.Address);
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.ToString ());
		
			ia = new InternetAddress ();
			ia.Personal = "Andreas Hügel";
			ia.Email = "andreas.huegel@topalis.com";
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", ia.Address);
			Assert.AreEqual ("=?utf-8?Q?Andreas_H=C3=BCgel?= <andreas.huegel@topalis.com>", ia.ToString ());
		
			// same thing made the other way around
			ia = new InternetAddress ();
			ia.Email = "andreas.huegel@topalis.com";
			ia.Personal = "Andreas Hügel";
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", ia.Address);
			Assert.AreEqual ("=?utf-8?Q?Andreas_H=C3=BCgel?= <andreas.huegel@topalis.com>", ia.ToString ());		
		}

		[Test]
		public void AnalyseInternetAddress ()
		{

			InternetAddress ia;

			ia = new InternetAddress ("Andreas Hügel");
			Assert.AreEqual ("Andreas Hügel", ia.Email);
			Assert.AreEqual ("", ia.Personal);

			ia = new InternetAddress ("andreas.huegel@topalis.com");
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.Email);
			Assert.AreEqual ("", ia.Personal);

			ia = new InternetAddress ("<andreas.huegel@topalis.com>");
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.Email);
			Assert.AreEqual ("", ia.Personal);

			ia = new InternetAddress ("Andreas Hügel <andreas.huegel@topalis.com>");
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", ia.Address);
			Assert.AreEqual ("Andreas Hügel <andreas.huegel@topalis.com>", ia.ToString ());  // extra space
			Assert.AreEqual ("Andreas Hügel", ia.Personal);
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.Email);
			ia.Personal = "Andreas Hügel";
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", ia.Address);
			Assert.AreEqual ("=?utf-8?Q?Andreas_H=C3=BCgel?= <andreas.huegel@topalis.com>", ia.ToString ()); // extra space is now missing and encoding is done

		
			ia = new InternetAddress ("Andreas Hügel         <andreas.huegel@topalis.com>");
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", ia.Address);
			Assert.AreEqual ("Andreas Hügel         <andreas.huegel@topalis.com>", ia.ToString ());  // extra spaces
			Assert.AreEqual ("Andreas Hügel", ia.Personal);
			Assert.AreEqual ("andreas.huegel@topalis.com", ia.Email);
			ia.Personal = "Andreas Hügel";
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", ia.Address);
			Assert.AreEqual ("=?utf-8?Q?Andreas_H=C3=BCgel?= <andreas.huegel@topalis.com>", ia.ToString ()); // extra spaces is now missing and encoding is done
		
		
		}


	}
}
