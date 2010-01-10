//
// openmapi.org - NMapi C# Mapi API - Hello.cs
//
// Copyright 2008 Topalis AG
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
using System.Linq;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace Test {

	public partial class MyTask {

		public static void Main (string[] args)
		{
			var factory = new AutoConfigurationFactory ();
			using (IMapiSession session = factory.CreateMapiSession ())
			{
				session.Logon  ("localhost", "jroith", "");
				var priv = session.PrivateStore;

				priv.Events [null].TableModified += (sender, ea) =>
					Console.WriteLine ("Table was modified!");

				using (IMapiFolder taskFolder = priv.OpenTaskFolder ())
				{

					using (MapiContext context = new MapiContext (session)) 	
					{
						MapiQuery<MyTask> tasks = context.GetQuery<MyTask> (taskFolder);
						var query = tasks
								.Where (t => t.MessageClass == "IPM.Task" 
									&& t.Subject != ((wurzelbrumpft ("1"))))
								.OrderBy (t => t.Subject);

						TableBookmark cursor = query.GetBookmarkCurrent ();
						foreach (var item in query.SharedRangeFromBookmark (cursor, 20)) {
							item.Modified += ObjectModifiedHandler;
							//item.Subject += " (suffix)";
						}

						// Console.WriteLine ("Waiting for events ...");
						// while (true);

						context.SubmitChanges ();
					}
				}
			}
		}

		private static void ObjectModifiedHandler (object sender, MapiEntityEventArgs ea)
		{
			Console.WriteLine ("Hey, I was notified through context and Context updated the values:");
			ea.Entity.Dump ();
		}

		public static string wurzelbrumpft (string blah)
		{
			return "task" + blah;
		}

	}
}
