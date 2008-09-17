/*
 * NRpcGen - NRpcTypeMap.cs
 *
 * Copyright (c) 2008 Johannes Roith
 *
 * This library is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Library General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Library General Public License for more details.
 *
 * You should have received a copy of the GNU Library General Public
 * License along with this program (see the file COPYING.LIB for more
 * details); if not, write to the Free Software Foundation, Inc.,
 * 675 Mass Ave, Cambridge, MA 02139, USA.
 */

package remotetea.nrpcgen;

import java.util.HashMap;
import java.io.*;

import org.w3c.dom.*;
import org.xml.sax.SAXException;
import javax.xml.parsers.*;
import javax.xml.xpath.*;

public class NRpcTypeMap {

	private HashMap<String, String> typeMap = null;

	public NRpcTypeMap  (String fileName) 
		throws ParserConfigurationException, SAXException, 
			IOException, XPathExpressionException
	{
		if (fileName != null) {
			typeMap = new HashMap <String, String> ();
			DocumentBuilderFactory domFactory = DocumentBuilderFactory.newInstance ();
			domFactory.setNamespaceAware(true); // never forget this!
			DocumentBuilder builder = domFactory.newDocumentBuilder ();
			Document doc = builder.parse (fileName);
	
			XPathFactory factory = XPathFactory.newInstance ();
			XPath xpath = factory.newXPath ();
			XPathExpression expr = xpath.compile ("/nrpcgen/map/type");
			Object result = expr.evaluate (doc, XPathConstants.NODESET);
			NodeList nodes = (NodeList) result;
			for (int i = 0; i < nodes.getLength(); i++) {
				NamedNodeMap attribs = nodes.item (i).getAttributes ();
				String from = attribs.getNamedItem ("from").getNodeValue ();
				String to = attribs.getNamedItem ("to").getNodeValue ();
				typeMap.put (from, to);
			}
		}
	}

	public String map (String key) {
		if (typeMap == null)
			return key;
		String result = null;
		if (typeMap.containsKey (key))
			result = typeMap.get (key);
		if (result == null)
			result = key;
		return result;
	}

  }


