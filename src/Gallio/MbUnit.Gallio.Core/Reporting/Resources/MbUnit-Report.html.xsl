<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:g="http://www.mbunit.com/gallio">
<xsl:output method="xml" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" 
            doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" indent="yes" />
  
  <xsl:template match="g:report">
    <html>
      <head>
        <title>MbUnit Test Report</title>
        <link rel="stylesheet" type="text/css" href="css/MbUnit-Report.css" />
      </head>
      <body>
        <img src="img/Logo.png" alt="MbUnit Logo" />
        <h1>MbUnit Test Report</h1>
        <xsl:apply-templates select="g:package/g:assemblyFiles" />
        <xsl:apply-templates select="g:packageRun/g:statistics" />
        <xsl:apply-templates select="g:testModel/g:test" />
        <xsl:apply-templates select="g:packageRun/g:testRuns" />
      </body>
    </html>
  </xsl:template>

  <xsl:template match="g:package/g:assemblyFiles">
    <div id="Assemblies">
      <h2>Assemblies</h2>
      <ul>
        <xsl:for-each select="g:assemblyFile">
          <li>
            <xsl:value-of select="."/>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>

  <xsl:template match="g:packageRun/g:statistics">
    <div id="Statistics">
      <h2>Statistics</h2>
      <table id="Stats">
        <tr>
          <th>Tests</th>
          <th>Run</th>
          <th>Passed</th>
          <th>Failed</th>
          <th>Inconclusive</th>
          <th>Ignored</th>
          <th>Skipped</th>
          <th>Asserts</th>
          <th>Duration</th>
        </tr>
        <tr>
          <td>
            <xsl:value-of select="@testCount" />
          </td>
          <td>
            <xsl:value-of select="@runCount" />
          </td>
          <td>
            <xsl:value-of select="@passCount" />
          </td>
          <td>
            <xsl:value-of select="@failureCount" />
          </td>
          <td>
            <xsl:value-of select="@inconclusiveCount" />
          </td>
          <td>
            <xsl:value-of select="@ignoreCount" />
          </td>
          <td>
            <xsl:value-of select="@skipCount" />
          </td>
          <td>
            <xsl:value-of select="@assertCount" />
          </td>
          <td>
            <xsl:value-of select="@duration" />
          </td>
        </tr>
      </table>
    </div>
  </xsl:template>

  <xsl:template match="g:testModel/g:test">
    <div id="Summary">
      <h2>Summary</h2>
      <ul>
        <xsl:apply-templates select="g:children/g:test" />
      </ul>
    </div>
  </xsl:template>

  <xsl:template match="g:children/g:test">
    <li>
      <xsl:attribute name="class">
        <xsl:value-of select="g:metadata/g:entry[@key='ComponentKind']/g:value" />
      </xsl:attribute>
      <a>
        <xsl:attribute name="href">
          <xsl:text>#</xsl:text><xsl:value-of select="@id" />
        </xsl:attribute>
        <xsl:choose>
          <xsl:when test="g:metadata/g:entry[@key='ComponentKind']/g:value = 'Fixture'">
            <img src="img/Fixture.png" alt="Fixture icon" />
          </xsl:when>
          <xsl:otherwise>
            <img src="img/Container.png" alt="Container icon" />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:value-of select="@name" />
      </a>
      <xsl:if test="count(g:children/g:test) > 0 and g:metadata/g:entry[@key='ComponentKind']/g:value != 'Fixture'">
        <ul>
          <xsl:apply-templates select="g:children/g:test" />
        </ul>
      </xsl:if>
    </li>
  </xsl:template>

  <xsl:template match="g:packageRun/g:testRuns">
    <div id="Details">
      <h2>Details</h2>
      <ul>
        <xsl:apply-templates select="g:testRun" />
      </ul>
    </div>
  </xsl:template>

  <xsl:template match="g:testRun">
    <xsl:apply-templates select="g:stepRun">
      <xsl:with-param name="testId" select="@id" />
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="g:stepRun">
    <xsl:param name="testId" />
    <xsl:variable name="test" select="//g:testModel//g:test[@id=$testId]" />
    
    <li>
      <span>
        <xsl:attribute name="testId">
          <xsl:value-of select="$testId" />
        </xsl:attribute>
        <xsl:attribute name="stepId">
          <xsl:value-of select="@id" />
        </xsl:attribute>
        <xsl:attribute name="class">
          <xsl:if test="g:result/@state != 'ignored'">
            <xsl:value-of select="g:result/@outcome" />
          </xsl:if>
        </xsl:attribute>
        
        <xsl:choose>
          <xsl:when test="$test/g:metadata/g:entry[@key='ComponentKind']/g:value = 'Assembly'">
            <img src="img/Container.png" alt="Container icon" />
          </xsl:when>
          <xsl:when test="$test/g:metadata/g:entry[@key='ComponentKind']/g:value = 'Fixture'">
            <img src="img/Fixture.png" alt="Fixture icon" />
          </xsl:when>
          <xsl:when test="$test/g:metadata/g:entry[@key='ComponentKind']/g:value = 'Test'">
            <img src="img/Test.png" alt="Test icon" />
          </xsl:when>
          <xsl:otherwise>
            <img src="img/Container.png" alt="Container icon" />
          </xsl:otherwise>
        </xsl:choose>
        
        <xsl:value-of select="@fullName" />
        
        <xsl:if test="$test/@isTestCase">
          <xsl:text> (Duration: </xsl:text>
          <xsl:value-of select="g:result/@duration" />
          <xsl:text>, Assertions: </xsl:text>
          <xsl:value-of select="g:result/@assertCount" />
          <xsl:text>)</xsl:text>
        </xsl:if>
      </span>
    </li>

    <xsl:apply-templates select="g:stepRuns">
      <xsl:with-param name="testId" select="$testId" />
    </xsl:apply-templates>
  </xsl:template>
  
</xsl:stylesheet>