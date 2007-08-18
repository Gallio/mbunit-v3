<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!--
	    format a number in to display its value in percent
	    @param value the number to format
	-->
	<xsl:template name="display-time-second">
		<xsl:param name="value"/>
		<span class="time"><xsl:value-of select="format-number($value,'0.00')"/>s</span>
	</xsl:template>

    <!--
	    format a number in to display its value in percent
	    @param value the number to format
	-->
	<xsl:template name="display-time">
		<xsl:param name="value"/>
		<span class="time"><xsl:value-of select="format-number($value*1000,'0.000')"/>ms</span>
	</xsl:template>
	<!--
	    format a number in to display its value in percent
	    @param value the number to format
	-->
	<xsl:template name="display-percent">
		<xsl:param name="value"/>
		<xsl:value-of select="format-number($value,'0.00 %')"/>
	</xsl:template>
	<!--
	    format a number in to display its value in Kb
	    @param value the number to format
	-->
	<xsl:template name="display-memory">
		<xsl:param name="value"/>
		<xsl:variable name="kbvalue">
			<xsl:value-of select="$value * 0.0001"/>
		</xsl:variable>
		<xsl:value-of select="format-number($kbvalue,'0.00 Kb')"/>
	</xsl:template>
	<!--
	template that will convert a carriage return into a br tag
	@param word the text from which to convert CR to BR tag
	-->
	<xsl:template name="br-replace">
		<xsl:param name="word"/>
		<xsl:choose>
			<xsl:when test="contains($word,'&#xA;')">
				<xsl:value-of select="substring-before($word,'&#xA;')"/>
				<br/>
				<xsl:call-template name="br-replace">
					<xsl:with-param name="word" select="substring-after($word,'&#xA;')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$word"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!--
	Scales value
	-->
	<xsl:template name="scale">
		<xsl:param name="origLength"/>
		<xsl:param name="targetLength"/>
		<xsl:param name="value"/>
		<xsl:value-of select="($value div $origLength) * $targetLength"/>
	</xsl:template>

	<!--
	Graphical result bar
	-->
	<xsl:template name="counter-progressbar">
		<xsl:param name="width"/>
		<xsl:param name="height"/>
		<div>
			<xsl:attribute name="style">position:relative;background-color:#DDDDDD;border-color:black;width:<xsl:value-of select="$width"/>px;height:<xsl:value-of select="$height"/>px;</xsl:attribute>
			<!-- success -->
			<xsl:if test="@success-count > 0">
			<div>
				<xsl:attribute name="style">position:absolute;top:0px;left:0px;height:<xsl:value-of select="$height"/>px;background-color:lightgreen;font-size:1px;width:<xsl:call-template name="scale">
						<xsl:with-param name="origLength" select="@run-count"/>
						<xsl:with-param name="targetLength" select="$width"/>
						<xsl:with-param name="value" select="@success-count"/>
					</xsl:call-template>px;</xsl:attribute>
			</div>
			</xsl:if>
			<!-- failure -->
			<xsl:if test="@failure-count > 0">
			<div>
				<xsl:attribute name="style">position:absolute;top:0px;left:<xsl:call-template name="scale">
						<xsl:with-param name="origLength" select="@run-count"/>
						<xsl:with-param name="targetLength" select="$width"/>
						<xsl:with-param name="value" select="@success-count"/>
					</xsl:call-template>px;height:<xsl:value-of select="$height"/>px;background-color:red;font-size:1px;width:<xsl:call-template name="scale">
						<xsl:with-param name="origLength" select="@run-count"/>
						<xsl:with-param name="targetLength" select="$width"/>
						<xsl:with-param name="value" select="@failure-count"/>
					</xsl:call-template>px;</xsl:attribute>
			</div>
			</xsl:if>
			<!-- skip -->
			<xsl:if test="@skip-count > 0">
			<div>
				<xsl:attribute name="style">position:absolute;top:0px;left:<xsl:call-template name="scale">
						<xsl:with-param name="origLength" select="@run-count"/>
						<xsl:with-param name="targetLength" select="$width"/>
						<xsl:with-param name="value" select="@success-count+@failure-count"/>
					</xsl:call-template>px;height:<xsl:value-of select="$height"/>px;background-color:blueviolet;font-size:1px;width:<xsl:call-template name="scale">
						<xsl:with-param name="origLength" select="@run-count"/>
						<xsl:with-param name="targetLength" select="$width"/>
						<xsl:with-param name="value" select="@skip-count"/>
					</xsl:call-template>px;</xsl:attribute>
			</div>
			</xsl:if>
			<!-- ignore -->
			<xsl:if test="@ignore-count > 0">
			<div>
				<xsl:attribute name="style">position:absolute;top:0px;left:<xsl:call-template name="scale">
						<xsl:with-param name="origLength" select="@run-count"/>
						<xsl:with-param name="targetLength" select="$width"/>
						<xsl:with-param name="value" select="@success-count+@failure-count+@skip-count"/>
					</xsl:call-template>px;height:<xsl:value-of select="$height"/>px;background-color:orange;font-size:1px;width:<xsl:call-template name="scale">
						<xsl:with-param name="origLength" select="@run-count"/>
						<xsl:with-param name="targetLength" select="$width"/>
						<xsl:with-param name="value" select="@ignore-count"/>
					</xsl:call-template>px;</xsl:attribute>
			</div>
			</xsl:if>
			<div>
				<xsl:attribute name="style">position:absolute;top:0px;left:<xsl:value-of select="$width +2"/>px;height:<xsl:value-of select="$height"/>px;font-size:<xsl:value-of select="$height - 2"/>px;font-family:Verdana;</xsl:attribute><xsl:value-of select="@run-count"/>/<xsl:value-of select="@success-count"/>/<xsl:value-of select="@failure-count"/>/<xsl:value-of select="@skip-count"/>/<xsl:value-of select="@ignore-count"/>/<xsl:value-of select="@assert-count"/></div>
		</div>
	</xsl:template>

	<xsl:template name="counter-literal">
		<xsl:value-of select="@run-count"/> test,
		<xsl:value-of select="@success-count"/> success,
		<xsl:value-of select="@failure-count"/> failures,
        <xsl:value-of select="@skip-count" /> skipped,
		<xsl:value-of select="@ignore-count"/> ignored,
		<xsl:value-of select="@assert-count"/> asserts
	</xsl:template>


	<xsl:template name="icon">
		<xsl:param name="src"/>
		<img width="16" height="16" border="0">
			<xsl:attribute name="src"><xsl:value-of select="$src"/></xsl:attribute>
		</img>
	</xsl:template>

	<xsl:template name="assembly-icon">
		<xsl:call-template name="icon">
			<xsl:with-param name="src">Populator.png</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="fixture-icon">
		<xsl:call-template name="icon">
			<xsl:with-param name="src">Fixture.png</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="namespace-icon">
		<xsl:call-template name="icon">
			<xsl:with-param name="src">Category.png</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="run-icon">
		<xsl:call-template name="icon">
			<xsl:with-param name="src">Test.png</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="assemblyid">
		<xsl:param name="name"/>
		as<xsl:value-of select="$name"/>
	</xsl:template>

	<xsl:template name="exceptionid">
		<xsl:param name="name"/>
		ex<xsl:value-of select="$name"/>
	</xsl:template>
    <xsl:template name="fixture-replay">
    MbUnit.Cons.exe -filter-type:<xsl:value-of select="@type" disable-output-escaping="yes" /> "<xsl:value-of select="ancestor::assembly/@location" disable-output-escaping="yes"/>"
    </xsl:template>
	<xsl:template match="report-result">
    <xsl:comment> saved from url=(0022)http://www.mbunit.org/ </xsl:comment>
		<html>
			<head>
				<title>MbUnit Test Report</title>
				<style>
          h1 				{ font-family:Verdana; font-weight:Bold; font-size:14pt; margin-bottom:0pt; }
          h2 				{ font-family:Verdana; font-weight:Bold; font-size:12pt; margin-bottom:0pt; }
          p,li			{ font-family:Verdana; font-size:9pt;}
          table 			{ font-family:Verdana; font-weight:normal; font-size:8pt; color:black; }
          tr.heading    	{ background-color:#D0D0FF; font-weight:Bold; font-size:12pt; }
          tr.titles     	{ background-color:#B0B0FF; font-weight:Bold; font-size:9pt; }
          tr.assemblysetupteardown     	{ background-color:#DDDDFF; font-weight:Bold; font-size:9pt; }
          tr.fixture     	{ background-color:#B0B0FF; font-weight:Bold; font-size:9pt; }
          tr.fixturesetupteardown     	{ background-color:#DDDDFF; font-weight:Bold; font-size:9pt; }
          tr.assemblySummary { font-weight:Bold; }
          tr.fixtureSummary  {}
          tr.fixtureReplay { font-size:7pt; background-color:#D0D0FF;}
          tr.successEven  { background-color:#E0FFE0; }
          tr.successOdd   { background-color:#C0FFC0; }
          tr.failureEven  { background-color:#FFEEDE; font-weight:Bold; }
          tr.failureOdd   { background-color:#FFD0C0; font-weight:bold; }
          tr.skippedEven 	{ background-color:#FFEEEE; }
          tr.skippedOdd  	{ background-color:#FFD0D0; }
          tr.exceptionType  { background-color:#DDDDDD; font-weight:Bold; }
          tr.notRunEven 	{ background-color:#F0F0FF; }
          tr.notRunOdd  	{ background-color:#D8D8FF; }
          tr.ignoreEven 	{ background-color:#FFFFE0; }
          tr.ignoreOdd  	{ background-color:#FFFFC0; }
          td.smallLeft  	{ font-size:7pt; font-weight: normal; text-align:left; vertical-align:top; }
          td.smallRight 	{ font-size:7pt; font-weight: normal; text-align:right; vertical-align:top; }
          pre.console     { }
          pre.stackTrace  { }
          counter         { font-size:7pt;}
          time            { font-size:8pt;}
          button.expandCollapse { height:20px; width:20px; line-height:80%; }
        </style>
				<script language="JavaScript" type="text/javascript">
          <!-- put the JavaScript in a comment so it can include greater-than and less-than characters,
            and the result is still valid XHTML -->
          <xsl:comment>
            <![CDATA[

        function expandCollapse(button, id) 
        {
          area = document.getElementById(id);
          if (area.style.display == "none") {
            area.style.display = "block";
            button.innerHTML = "-";
          }
          else 
          {
            area.style.display = "none";
            button.innerHTML = "+";
          }
        }
        
        function expandAll() 
        {
          expandCollapseAll("-", "block");
        }
        
        function collapseAll() 
        {
          expandCollapseAll("+", "none");
        }

        function expandCollapseAll(buttonHTML, disp) 
        {
          items = document.getElementsByTagName("table");
          for (i = 0; i != items.length; i++) 
          {
            if (items[i].className == "expandCollapse")
            {
              items[i].style.display = disp;
            }
          }

          items = document.getElementsByTagName("button");
          for (i = 0; i != items.length; i++) 
          {
            if (items[i].className == "expandCollapse")
            {
              items[i].innerHTML = buttonHTML;
            }
          }
        }

        if (document.images)
				{
					populator = new Image();
					fixture = new Image();
					category = new Image();
					test = new Image();
					fixture.src = "Fixture.png";
					test.src = "Test.png"
					category.src = "Category.png"
					populator.src = "Populator.png"
				}
				]]>
          </xsl:comment>
        </script>
			</head>
			<body>
				<img src="mbunitlogo.png" />
        <br/>
				<h1>Test Summary</h1>
				<ul>
				<li><strong>Date:</strong>
					<xsl:value-of select="@date"/>
				</li>
                <li><strong>Duration:</strong>
				    <xsl:call-template name="display-time-second">
					    <xsl:with-param name="value" select="counter/@duration"/>
				    </xsl:call-template>
                </li>
				</ul>
				<xsl:call-template name="report-summary" />
                <xsl:if test="count(//warnings)>0">
                <h1>Warnings</h1>
				<xsl:call-template name="warnings" />
                </xsl:if>
				<h1>Test result details</h1>
        <div style="float: right">
          <button onclick="expandAll()" >Expand All</button>
          <button onclick="collapseAll()">Collapse All</button>
        </div>
        <xsl:call-template name="assemblies" />
				<br/>
				<p><img src="mbuniticon.gif"/>
					This report was generated using <a href="http://www.mbunit.org">MbUnit</a>.
				</p>
			</body>
		</html>
	</xsl:template>

	<xsl:template name="report-summary">
		<table border="0" cellpadding="1" cellspacing="1">
		<tr class="titles">
		<td>Total</td>
		<td>
					<xsl:for-each select="counter">
					<xsl:call-template name="counter-progressbar">
						<xsl:with-param name="width">100</xsl:with-param>
						<xsl:with-param name="height">14</xsl:with-param>
					</xsl:call-template>
					</xsl:for-each>
		</td>
		</tr>
			<xsl:call-template name="assembly-summary"/>
		</table>
	</xsl:template>

	<xsl:template name="assembly-summary">
			<xsl:for-each select="assemblies/assembly">
				<tr class="titles">
				<td>
					<a>
					<xsl:attribute name="href">#<xsl:value-of select="@name"/>Assembly</xsl:attribute>
					<xsl:call-template name="assembly-icon"/>
					<xsl:value-of select="@name"/>
					</a>
				</td>
				<td>
					<xsl:for-each select="counter">
					<xsl:call-template name="counter-progressbar">
						<xsl:with-param name="width">100</xsl:with-param>
						<xsl:with-param name="height">12</xsl:with-param>
					</xsl:call-template>
					</xsl:for-each>
				</td>
				</tr>
				<xsl:call-template name="fixture-summary" />
			</xsl:for-each>
	</xsl:template>

<xsl:template name="fixture-id"><xsl:param name="name" /><xsl:value-of select="$name" /></xsl:template>

	<xsl:template name="fixture-summary">
			<xsl:for-each select="//fixture">
			<tr class="fixtureSummary">
			<td>
			<a>
				<xsl:attribute name="href">#<xsl:call-template name="fixture-id"><xsl:with-param name="name">
							<xsl:value-of select="@type" />
						</xsl:with-param></xsl:call-template>
				</xsl:attribute>
				<xsl:call-template name="fixture-icon"/>
				<xsl:value-of select="@type" />.<xsl:value-of select="@name" />
			</a>
			</td>
			<td>
				<xsl:for-each select="counter">
				<xsl:call-template name="counter-progressbar">
					<xsl:with-param name="width">100</xsl:with-param>
					<xsl:with-param name="height">10</xsl:with-param>
				</xsl:call-template>
				</xsl:for-each>
			</td>
			</tr>
			</xsl:for-each>
	</xsl:template>

    <xsl:template name="warnings">
<table>  
<xsl:for-each select="//warning">
<tr>
    <xsl:attribute name="class">
		<xsl:choose>
	        <xsl:when test="position() mod 2 = 1">failureOdd</xsl:when>
		    <xsl:otherwise>failureEven</xsl:otherwise>
		</xsl:choose>
    </xsl:attribute>
    <td><xsl:call-template name="run-icon"/>
        <xsl:value-of select="ancestor::run/@name" /></td>
    <td><xsl:value-of select="text()"/></td>
</tr>
</xsl:for-each>
</table>
    </xsl:template>
    
	<xsl:template name="assemblies">
		<xsl:for-each select="assemblies/assembly">
			<xsl:call-template name="assembly-detail" />
		</xsl:for-each>
	</xsl:template>

	<xsl:template name="assembly-detail">
		<h2><xsl:attribute name="id"><xsl:value-of select="@name"/>Assembly</xsl:attribute>
			<xsl:call-template name="assembly-icon"/>
			<xsl:value-of select="@name"/>
        </h2>
		<ul>
		<li><strong>Full Name:</strong> <xsl:value-of select="@full-name"/></li>
		<li><strong>Results:</strong>
			<xsl:for-each select="counter">
				<xsl:call-template name="counter-literal"/>
			</xsl:for-each>
		</li>
        <li><strong>Duration:</strong>
				<xsl:call-template name="display-time-second">
					<xsl:with-param name="value" select="counter/@duration"/>
				</xsl:call-template>
        </li>
		</ul>
		<xsl:if test="set-up|tear-down">
	<table>
		<tr class="assemblysetupteardown" ><td>Assembly SetUp and TearDown</td></tr>
		<xsl:apply-templates select="set-up"/>
		<xsl:apply-templates select="tear-down"/>
	</table>
		</xsl:if>
		<xsl:call-template name="namespaces-table" />
	</xsl:template>
	
	<xsl:template name="namespaces-table">
		<xsl:apply-templates select="namespaces" />
		<xsl:apply-templates select="namespace" />
	</xsl:template>
	
	<xsl:template match="namespaces">
		<xsl:apply-templates select="namespace" />
	</xsl:template>
	
	<xsl:template match="namespace">
		<xsl:apply-templates select="namespaces" />
		<xsl:apply-templates select="fixtures" />
	</xsl:template>
	
	<xsl:template match="fixtures">
		<table width="800" border="0" cellpadding="1" cellspacing="1">
				<col width="700"/>
				<col width="50"/>
				<col width="50"/>
			<xsl:apply-templates select="fixture" />
		</table>
	</xsl:template>
	
	<xsl:template match="fixture">
		<tr>
			<xsl:attribute name="id"><xsl:call-template name="fixture-id"><xsl:with-param name="name">
						<xsl:value-of select="@type" />
					</xsl:with-param></xsl:call-template>
			</xsl:attribute>
			<td>
				<xsl:for-each select="counter">
					<xsl:call-template name="counter-progressbar">
						<xsl:with-param name="width" select="100" />
						<xsl:with-param name="height" select="10" />
					</xsl:call-template>
				</xsl:for-each>
			</td>
		</tr>
		<tr class="fixture" >
			<td>
				<xsl:call-template name="fixture-icon"/>
				<xsl:value-of select="@type" />.<xsl:value-of select="@name"/>

		    	(<xsl:call-template name="display-time-second">
	    		    <xsl:with-param name="value" select="counter/@duration"/>
    			</xsl:call-template>)
			</td>
		</tr>
        <tr class="fixtureReplay">
            <td>
                <xsl:call-template name="fixture-replay" />
            </td>
        </tr>
		<xsl:if test="set-up|tear-down">
			<tr class="fixturesetupteardown"><td>Fixture SetUp and TearDown</td></tr>
			<xsl:apply-templates select="set-up"/>
			<xsl:apply-templates select="tear-down"/>
		</xsl:if>
		<tr>
			<td>
				<xsl:apply-templates select="runs" />
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="runs">
		<!-- table width="100%" border="0" cellpadding="1" cellspacing="1" -->
    <tr>
      <xsl:apply-templates select="*" />
    </tr>
		<!-- /table -->
	</xsl:template>

<xsl:template match="set-up">
    <xsl:call-template name="set-up-or-tear-down" />
</xsl:template>
<xsl:template match="tear-down">
    <xsl:call-template name="set-up-or-tear-down" />
</xsl:template>
<xsl:template name="set-up-or-tear-down">
		<tr>
			<!-- Choosing style and alternate -->
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="@result = 'success'">successEven</xsl:when>
					<xsl:when test="@result = 'failure'">failureEven</xsl:when>
				</xsl:choose>
			</xsl:attribute>
      <td>
        <button class="expandCollapse" onclick="expandCollapse(this, '{generate-id(.)}')">-</button>
        <xsl:value-of select="@name"/>
			</td>
			<td>
				<xsl:call-template name="display-time">
					<xsl:with-param name="value" select="@duration"/>
				</xsl:call-template>
			</td>
			<td>
				<xsl:call-template name="display-memory">
					<xsl:with-param name="value" select="@memory"/>
				</xsl:call-template>
			</td>
		</tr>
		<!-- Adding exception log -->
  <tr>
    <td>
      <!-- This nested table is theoretically not needed, but lets me avoid rewriting exception-log and console-output templates -MLS -->
      <table id="{generate-id(.)}" class="expandCollapse">

        <xsl:if test="@result = 'failure'">
          <xsl:call-template name="exception-log"/>
        </xsl:if>
        <!-- Adding console out, error -->
        <xsl:call-template name="console-output" />
      </table>
    </td>
  </tr>
</xsl:template>

	<!--

	Run template

	-->
	<xsl:template match="run">
		<tr>
			<!-- Choosing style and alternate -->
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="@result = 'success'">
						<xsl:choose>
							<xsl:when test="position() mod 2 = 1">successOdd</xsl:when>
							<xsl:otherwise>successEven</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="@result = 'failure'">
						<xsl:choose>
							<xsl:when test="position() mod 2 = 1">failureOdd</xsl:when>
							<xsl:otherwise>failureEven</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="@result = 'skip'">
						<xsl:choose>
							<xsl:when test="position() mod 2 = 1">skippedOdd</xsl:when>
							<xsl:otherwise>skippedEven</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="@result = 'ignore'">
						<xsl:choose>
							<xsl:when test="position() mod 2 = 1">ignoreOdd</xsl:when>
							<xsl:otherwise>ignoreEven</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
				</xsl:choose>
			</xsl:attribute>
      <td>
        <table>
          <tr>
            <td>
              <button class="expandCollapse" onclick="expandCollapse(this, '{generate-id(.)}')">-</button>
                <xsl:call-template name="run-icon"/>
            </td>
            <td>
              <xsl:value-of select="@name"/>
            </td>
          </tr>
        </table>
      </td>
			<td>
				<xsl:call-template name="display-time">
					<xsl:with-param name="value" select="@duration"/>
				</xsl:call-template>
			</td>
			<td>
				<xsl:call-template name="display-memory">
					<xsl:with-param name="value" select="@memory"/>
				</xsl:call-template>, <xsl:value-of select="@assert-count" />
			</td>
		</tr>
    <tr>
      <td>
        <!-- This nested table is theoretically not needed, but lets me avoid rewriting exception-log and console-output templates -MLS 6/27/06 -->
        <table id="{generate-id(.)}" class="expandCollapse">
          <!-- Adding exception log -->
          <xsl:if test="@result = 'failure'">
            <xsl:call-template name="exception-log"/>
          </xsl:if>
          <!-- Adding console out, error -->
          <xsl:call-template name="console-output" />
        </table>
      </td>
    </tr>
	</xsl:template>
	<xsl:template name="exception-log">
		<tr class="failure-exception">
			<td>

			<div style="display;">
				<table border="0" cellpadding="1" cellspacing="1" width="600">
					<xsl:apply-templates select="exception" />
				</table>
			</div>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="exception">
		<tr class="exceptionType">
			<td>
				<strong>Type:</strong>
				<xsl:value-of select="@type"/>
			</td>
		</tr>
		<tr>
			<td>
				<strong>Message:</strong>
				<xsl:value-of select="message"/>
			</td>
		</tr>
		<tr>
			<td>
				<strong>Source:</strong>
				<xsl:value-of select="source"/>
			</td>
		</tr>
        <xsl:for-each select="properties/property">
		<tr>
			<td>
				<bold><xsl:value-of select="@name"/>:</bold>
				<xsl:value-of select="@value"/>
			</td>
		</tr>
        </xsl:for-each>            
		<tr>
			<td  width="80">
				<strong>StackTrace:</strong>
				<br/>
				<pre class="stackTrace"  width="80">
					<xsl:value-of select="stack-trace"/>
				</pre>
			</td>
		</tr>
		<xsl:apply-templates select="exception" />
	</xsl:template>
	<xsl:template name="console-output">
		<xsl:apply-templates select="console-out"/>
		<xsl:apply-templates select="console-error"/>
	</xsl:template>
	<xsl:template match="console-out">
		<xsl:call-template name="console">
			<xsl:with-param name="name">Console Output</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="console-error">
		<xsl:call-template name="console">
			<xsl:with-param name="name">Console Error</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template name="console">
		<xsl:param name="name" />
		<xsl:if test="string-length( text() ) != 0">
			<tr>
				<td>
					<table>
						<tr><td><strong><xsl:value-of select="$name"/></strong></td></tr>
						<tr>
							<td>
								<pre class="console"><xsl:value-of select="text()"/></pre>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
