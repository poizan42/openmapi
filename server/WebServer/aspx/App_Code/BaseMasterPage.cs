//
// openmapi.org - NMapi C# Mapi API - BaseMasterPage.cs
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

namespace NMapi.Server {

	public class BaseMasterPage : MasterPage
	{
		protected Panel sidebar;
		protected ContentPlaceHolder SidebarContent;

		protected string activeMenu;
		protected string activeCategory;
		protected bool showSideBar;
		protected ServerSideInfo ssi;

		public bool ShowSideBar {
			get { return showSideBar; }
			set { showSideBar = value; }
		}

		public string ActiveMenu {
			get { return activeMenu; }
			set { activeMenu = value; }
		}

		public string ActiveCategory {
			get { return activeCategory; }
			set { activeCategory = value; }
		}

		public ServerSideInfo ServerSideInfo {
			get { return ssi; }
			set { ssi = value; }
		}

		protected override void OnInit (EventArgs ea)
		{
			ssi = new ServerSideInfo ();
			showSideBar = false;
			ActiveCategory = "overview";
		}

		protected override void OnLoad (EventArgs ea)
		{
			if (Session ["loggedIn"] != "yes")
				Response.Redirect ("/login.aspx");
		}

		protected override void OnPreRender (EventArgs ea)
		{
			if (!showSideBar && SidebarContent.Controls.Count > 0)
				showSideBar = true;
			sidebar.Visible = showSideBar;
		}

	}

}
