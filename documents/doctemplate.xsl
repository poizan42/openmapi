<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" omit-xml-declaration="yes" />
  <xsl:template match="Page">
    <html>
      <head>
        <title>
          openmapi.org - <xsl:value-of select="Title" />
        </title>
	<meta name="ROBOTS" content="NOARCHIVE" />
        <style>
					a { text-decoration: none }
				
					.CollectionTitle { font-weight: bold }
					.PageTitle { font-size: 150%; font-weight: bold }

					.Summary { }
					.Signature { }
					.Remarks { }
					.Members { }
					.Copyright { }
					
					.Section { font-size: 125%; font-weight: bold }
					.SectionBox { margin-left: 2em }
					.NamespaceName { font-size: 105%; font-weight: bold }
					.NamespaceSumary { }
					.MemberName { font-size: 115%; font-weight: bold; margin-top: 1em }
					.MemberSignature { font-family: monospace; margin-top: 1em; }
					.MemberBox { }
					.Subsection { font-size: 105%; font-weight: bold }
					.SubsectionBox { margin-left: 2em; margin-bottom: 1em }

					.SignatureTable { background-color: #c0c0c0; }
					.EnumerationsTable th { background-color: #f2f2f2; }
					.CodeExampleTable { background-color: #f5f5dd; border: thin solid black; padding: .25em; }
					
					.MembersListing td { margin: 0px; background-color:#efefef; border: 1px solid #cccccc; padding: 3px; }
					
					.TypesListing td { margin: 0px;  padding: .25em }
					.InnerSignatureTable tr { background-color: #f2f2f2; }
					.TypePermissionsTable tr { background-color: #f2f2f2; }
					
					body, tr, td {
						font-size:8pt;	
					}
					
					body {
						font-family:sans,Verdana,Arial,Trebuchet MS;
						line-height:1.3;
						text-align:center;
						padding:0px;
						margin:0px;
						background-color:#f0f8fc;
					}

					a { color:#555555; }
					a:active { color:#555555; }
					a:visited { color:#555555; }

					h1 {
						color:#547ba7;
						border-bottom:2px #547ba7 solid;
						padding-bottom:10px;
					}

					h2, h3, h4 {
						color:#6c9446;
					}	

					h2 {	
						border-bottom:1px #c0c0c0 solid;
					}

					#content {
						background-color:#ffffff;
						border-left: 3px #cccccc solid;
						border-right: 3px #cccccc solid;
						max-width:700px;
						padding:20px;
						padding-left:40px;
						padding-right:40px;
						margin:0px;
						text-align:left;
						margin-left: auto;
						margin-right: auto;
						text-align:justify;
					}

					#footer, .simplebox {
						background-color: #efefef;
						border:1px #c0c0c0 solid;
						padding:5px;
					}

					#head {
						color:#777777;
						text-align:right;
						font-size:10pt;
						padding:10px;
					}
					#head .heading {
						color:#2664c6;
						text-align:right;
						font-size:15pt;
					}

					#footer {
						margin-top:50px;
						margin-bottom:20px;
						font-size:8.5pt;
					}

					.title {
						color:#547ba7;
						font-weight:bold;
						font-variant:small-caps;
					}
					
					
				</style>
      </head>
      <body>
		<div id="content">

			<div id="head">
				<span class="heading"><a href="http://www.openmapi.org">openmapi.org</a> - NMapi API Documentation</span><br />
				<big>open, multi-language, cross-platform MAPI implementation</big>

			</div>

        <!-- HEADER -->
        <div class="CollectionTitle">
          <xsl:apply-templates select="CollectionTitle/node()" />
        </div>
        <h1 class="PageTitle">
          <xsl:apply-templates select="PageTitle/node()" />
        </h1>
        <p class="Summary">
          <xsl:apply-templates select="Summary/node()" />
        </p>
        <div class="Signature">
          <xsl:apply-templates select="Signature/node()" />
        </div>
        <div class="Remarks">
          <xsl:apply-templates select="Remarks/node()" />
        </div>
        <div class="Members">
          <xsl:apply-templates select="Members/node()" />
        </div>
        <hr size="1" />
        <div class="Copyright">
          <xsl:apply-templates select="Copyright/node()" />
        </div>
		</div>
      </body>
    </html>
  </xsl:template>
  <!-- IDENTITY TRANSFORMATION -->
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
