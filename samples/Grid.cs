//
// openmapi.org - NMapi C# Mapi API - Grid.cs
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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace Test {
	class TaskView : Form
	{
		private bool loggedOn;
		private IMapiFactory factory;
		private IMapiSession session;
		private IMapiFolder taskFolder;
		private MapiContext context;

		private TextBox host, user, password, pathBox;
		private Button button;
		private DataGridView grid;

		public TaskView ()
		{
			factory = new AutoConfigurationFactory ();

			Text = "openmapi.org NMapi - Mini Linq/DataBinding Demo";
			Width = Height = 600;

			var layout = new TableLayoutPanel { Width = this.Width, Height = 50,
				ColumnCount = 1, RowCount = 1, Padding = new Padding (1, 1, 1, 1), 
				GrowStyle = TableLayoutPanelGrowStyle.AddColumns, 
				Location = new Point (0, 0), };
			Controls.Add (layout);

			host = new TextBox {Text = "localhost"};
			user = new TextBox {Text = "jroith"};
			password = new TextBox { PasswordChar = '*'};
			pathBox = new TextBox {Text = "Mailbox - jroith/Tasks", Width = 150};
			button =  new Button { Text = "Ok" };
			layout.Controls.AddRange (new Control [] {host, user, 
				password, pathBox, button});

			grid = new DataGridView {Location = new Point (10, 60), 
					Width = this.Width-25, Height = 500 };
			Controls.Add (grid);

			button.Click += (sender, args) => FillGrid ();
		}

		public void Logon ()
		{
			try {
				session = factory.CreateMapiSession ();
			} catch (MapiException e) {
				MessageBox.Show ("Error: Can't open Mapi-Session! " + 
					"Application will now quit.\n\n" + e.Message);
				Application.Exit ();
			}

			try {
				session.Logon  (host.Text, user.Text, password.Text);
			} catch (MapiException e) {
				if (e.HResult == Error.NetworkError)
					MessageBox.Show ("Host nicht gefunden.");
				else if (e.HResult == Error.NoAccess)
					MessageBox.Show ("Kein Zugriff. Passwort " + 
						"und Benutzername korrekt?");
				else
					throw;
				return;
			}
			loggedOn = true;
		}

		public void FillGrid ()
		{
			if (!loggedOn)
				Logon ();
			if (loggedOn) {
				var store = session.PrivateStore;
				taskFolder = store.OpenIpmFolder (pathBox.Text, Mapi.Modify);

				context = new MapiContext (session);
				MapiQuery<MyTask> tasks = context.GetQuery<MyTask> (taskFolder);
	
				var query = from t in tasks
						where t.MessageClass == "IPM.Task" && t.Subject != "task1"
						orderby t.Priority descending, t.Subject
						select t;
	
				grid.DataSource = query;
			}
		}

		protected override void OnClosing (CancelEventArgs ea)
		{
			if (context != null)
				context.Dispose ();
			if (taskFolder != null)
				taskFolder.Dispose ();
			if (session != null)
				session.Dispose ();
			base.OnClosing (ea);
		}

		public static void Main (string[] args)
		{
			Application.Run (new TaskView ());
		}
	}
}
