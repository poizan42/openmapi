<%@ language="C#" %>
<%@ Import Namespace="NMapi.Server " %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">

protected void Application_Start (object sender, EventArgs e)
{
	RegisterRoutes (RouteTable.Routes);
}

public static void RegisterRoutes (RouteCollection routes)
{
	// GET/SET
	routes.Add (new Route ("rest/{storetype}/{eid_or_special}/property/{propname}", new RestHttpRouteHandler ())); // property router
//	routes.Add (new Route ("rest/{storetype}/{eid_or_special}/get?props=propname,propname2,propname3", new RestHttpRouteHandler ())); // property router
//	routes.Add (new Route ("rest/{storetype}/{eid_or_special}/set", new RestHttpRouteHandler ())); // set props router
	
	// TABLES ...	
	// routes.Add (new Route ("rest/{storetype}/{eid_or_root_or_special}/contents?restriction=filter&sortby=blub", new RestHttpRouteHandler ())); // table router
	// routes.Add (new Route ("rest/{storetype}/{eid_or_root_or_special}/associated?restriction=filter&sortby=blub", new RestHttpRouteHandler ())); // table router
	// routes.Add (new Route ("rest/{storetype}/{eid_or_root_or_special}/hierarchy", new RestHttpRouteHandler ())); // table router

	// routes.Add (new Route ("rest/{storetype}/namedprops/map/guid/namedprop", new RestHttpRouteHandler ())); // named prop resolver
	// routes.Add (new Route ("rest/{storetype}/namedprops/unmap?ids=a,b,c", new RestHttpRouteHandler ())); // named prop resolver
	// routes.Add (new Route ("rest/{storetype}/", new RestHttpRouteHandler ())); // named prop resolver
	
}

</script>