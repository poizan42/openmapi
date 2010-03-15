//
// openmapi.org - NMapi C# Mapi API - backends_conf.aspx.cs
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NMapi.Admin;
using NMapi.Server;
using NMapi.Server.ICalls;

namespace NMapi.Server.UI.Pages {

	public partial class BackendsConfPage : Page
	{
		IMapiAdmin mAdmin;

		protected void Page_Init (object sender, EventArgs ea)
		{
			// TODO: BAD! (Session!)

			if (!IsPostBack) { // copy form data to page state
				Session ["confBackendId"] = Request.Form ["confBackendId"];
				Session ["confBackendTitle"] = Request.Form ["confBackendTitle"];
				Session ["confPassword"] = Request.Form ["confPassword"];
			}

			bool success = false;
			backendTitle.Text = (string) Session ["confBackendTitle"];

			int backendId = 0; // TODO!
			string backendIdStr = (string) Session ["confBackendId"];
			bool worked = Int32.TryParse (backendIdStr, out backendId);
			
			if (!worked)
				throw new ArgumentException ("Backend id must be an integer!");

			string password = (string) Session ["confPassword"];

			IInternalCalls calls = InternalCallsClient.GetNewClient ();
			mAdmin = calls.GetMapiAdmin (backendId);

			if (password != null) {
				success = mAdmin.Logon (password);
				if (success && !IsPostBack)
					FillUserDopDown (mAdmin);
			}
			if (!success)
				Response.Redirect ("/backends.aspx?wrongPassword=yes");
		}

		protected void Page_PreRender (object sender, EventArgs ea)
		{
			// mAdmin.Close (); TODO
		}

		private void FillUserDopDown (IMapiAdmin mAdmin)
		{
			AdminUser [] users = mAdmin.GetUsers ();
			foreach (AdminUser user in users) {
				string id = user.Id;
				string label = user.Id + " (" + user.Comment + ")";
				activeUser.Items.Add (new ListItem (label, id));
			}
			if (activeUser.Items.Count > 0)
				ShowUser (activeUser.Items [0].Value);
		}

		protected void Page_Load (object sender, EventArgs ea)
		{
			Master.ActiveCategory = "backends";
		}

		private void ShowUser (string id)
		{

			// TODO: Get user!

			userId.Text = id;

			userFirstName.Text = id; // TODO
			userLastName.Text = id; // TODO
			userComment.Text = id; // TODO
		}


		protected void UserChanged (object sender, EventArgs ae)
		{
			string userId = activeUser.SelectedItem.Value;
			ShowUser (userId);
		}

		protected void AddUser (object sender, EventArgs ae)
		{
			string uid = addUserName.Text.Trim ();
			if (uid == String.Empty)
				return;
			bool worked = mAdmin.AddUser (uid);
			if (worked) {
				activeUser.Items.Add (new ListItem (uid + " ()", uid));
				activeUser.SelectedIndex = activeUser.Items.Count-1;
				ShowUser (uid);
			}
			addUserName.Text = "";
		}

		// TODO: check!

		protected void DeleteCurrentUser (object sender, EventArgs ae)
		{
			bool worked = mAdmin.DeleteUser (userId.Text);
			if (worked) {
				activeUser.Items.Remove (activeUser.Items.FindByValue (userId.Text));
				activeUser.SelectedIndex = 0;
				ShowUser (activeUser.SelectedItem.Value);
			}
		}

		protected void TreeNodeSelected (object sender, EventArgs ea)
		{
			xxx.Text = foldersTreeView.SelectedNode.ValuePath;
		}

		protected void TreeNodePopulate (object sender, TreeNodeEventArgs ea)
		{
			Console.WriteLine (ea.Node.ValuePath);


			TreeNode newNode;

			newNode = new TreeNode ();
			newNode.Text = "test3";
			newNode.SelectAction = TreeNodeSelectAction.Select;
			ea.Node.ChildNodes.Add (newNode);

			// TODO: postback only!
			newNode = new TreeNode ();
			newNode.Text = "test4";
//			newNode.SelectAction = TreeNodeSelectAction.Expand;
//			newNode.PopulateOnDemand = true;
			ea.Node.ChildNodes.Add (newNode);
		}
	}

}
