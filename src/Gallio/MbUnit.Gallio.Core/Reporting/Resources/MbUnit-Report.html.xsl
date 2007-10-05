<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:g="http://www.mbunit.com/gallio">
  <xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
              doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" indent="yes" encoding="utf-8" />
  <xsl:param name="contentRoot" />

  <xsl:key name="outcome" match="/g:report/g:packageRun/g:testRuns/g:testRun" use="g:stepRun/g:result/@outcome" />
  <xsl:key name="status" match="/g:report/g:packageRun/g:testRuns/g:testRun" use="g:stepRun/g:result/@status" />

  <xsl:template match="g:report">
    <html xml:lang="en" lang="en" dir="ltr">
      <head>
        <title>MbUnit Test Report</title>
        <link rel="stylesheet" type="text/css" href="css/MbUnit-Report.css" />
        <script type="text/javascript" src="js/MbUnit-Report.js">
          <xsl:comment> comment inserted for Internet Explorer </xsl:comment>
        </script>
      </head>
      <body>
        <div id="header">
          <h1>MbUnit Test Report</h1>
        </div>
        <xsl:apply-templates select="g:package/g:assemblyFiles" />
        <xsl:apply-templates select="g:packageRun" />
        <xsl:apply-templates select="g:testModel/g:test" mode="summary"/>
        <xsl:apply-templates select="g:testModel/g:test" mode="details"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="g:package/g:assemblyFiles">
    <div id="Assemblies" class="section">
      <h2>Assemblies</h2>
      <div class="section-content">
        <ul>
          <xsl:for-each select="g:assemblyFile">
            <li>
              <xsl:value-of select="."/>
            </li>
          </xsl:for-each>
        </ul>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="g:packageRun">
    <div id="Statistics" class="section">
      <h2>Statistics</h2>
      <div id="statistics-section-content" class="section-content">
        <table class="statistics-table">
          <tr>
            <td class="statistics-label-cell">
              Start time:
            </td>
            <td>
              <xsl:call-template name="format-datetime">
                <xsl:with-param name="datetime" select="@startTime" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="alternate-row">
            <td class="statistics-label-cell">
              End time:
            </td>
            <td>
              <xsl:call-template name="format-datetime">
                <xsl:with-param name="datetime" select="@endTime" />
              </xsl:call-template>
            </td>
          </tr>
          <xsl:apply-templates select="g:statistics" />
        </table>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="g:statistics">
    <tr>
      <td class="statistics-label-cell">
        Tests:
      </td>
      <td>
        <xsl:value-of select="@testCount" /> (<xsl:value-of select="@stepCount" /> steps)
      </td>
    </tr>
    <tr class="alternate-row">
      <td class="statistics-label-cell">
        Results:
      </td>
      <td>
        <xsl:value-of select="@runCount" /> run,
        <xsl:value-of select="@passCount" /> passed,
        <xsl:value-of select="@failureCount" /> failed,
        <xsl:value-of select="@inconclusiveCount" /> inconclusive (<xsl:value-of select="@ignoreCount" /> ignored / <xsl:value-of select="@skipCount" /> skipped)
      </td>
    </tr>
    <tr>
      <td class="statistics-label-cell">
        Duration:
      </td>
      <td>
        <xsl:value-of select="format-number(@duration, '0.00')" />s
      </td>
    </tr>
    <tr class="alternate-row">
      <td class="statistics-label-cell">
        Assertions:
      </td>
      <td>
        <xsl:value-of select="@assertCount" />
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="g:testModel/g:test" mode="summary">
    <div id="Summary" class="section">
      <h2>Summary</h2>
      <div class="section-content">
        <xsl:choose>
          <xsl:when test="g:children/g:test[@id=/g:report/g:packageRun/g:testRuns/g:testRun/@id]">
            <ul>
              <xsl:apply-templates select="g:children/g:test[@id=/g:report/g:packageRun/g:testRuns/g:testRun/@id]" mode="summary" />
            </ul>
          </xsl:when>
          <xsl:otherwise>
            <em>This report does not contain any test runs.</em>
          </xsl:otherwise>
        </xsl:choose>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="g:children/g:test" mode="summary">
    <xsl:variable name="id" select="@id" />
    <xsl:variable name="kind" select="g:metadata/g:entry[@key='ComponentKind']/g:value" />
    <xsl:variable name="tests" select="descendant-or-self::g:test[@isTestCase='true']" />
    <xsl:variable name="run" select="$tests[@id = key('status', 'executed')/@id]" />
    <xsl:variable name="passed" select="$tests[@id = key('outcome', 'passed')/@id]" />
    <xsl:variable name="failed" select="$tests[@id = key('outcome', 'failed')/@id]" />
    <xsl:variable name="inconclusive" select="$tests[@id = key('outcome', 'inconclusive')/@id]" />
    <xsl:variable name="ignored" select="$tests[@id = key('status', 'ignored')/@id]" />
    <xsl:variable name="skipped" select="$tests[@id = key('status', 'skipped')/@id]" />

    <li>
      <span>
        <xsl:choose>
          <xsl:when test="g:children/g:test and $kind != 'Fixture'">
            <img src="img/Minus.gif" class="toggle">
              <xsl:attribute name="id">toggle-testChildrenPanel-<xsl:value-of select="$id" /></xsl:attribute>
              <xsl:attribute name="onclick">toggle('testChildrenPanel-<xsl:value-of select="$id" />');</xsl:attribute>
            </img>
          </xsl:when>
          <xsl:otherwise>
            <img src="img/FullStop.gif" />
          </xsl:otherwise>
        </xsl:choose>

        <xsl:call-template name="icon">
          <xsl:with-param name="kind" select="$kind" />
        </xsl:call-template>

        <a>
          <xsl:attribute name="href">#testRun-<xsl:value-of select="@id" /></xsl:attribute>
          <xsl:value-of select="@name" />
        </a>

        <xsl:call-template name="progressBar">
          <xsl:with-param name="passed">
            <xsl:value-of select="count($passed)" />
          </xsl:with-param>
          <xsl:with-param name="failed">
            <xsl:value-of select="count($failed)" />
          </xsl:with-param>
          <xsl:with-param name="inconclusive">
            <xsl:value-of select="count($inconclusive)" />
          </xsl:with-param>
        </xsl:call-template>
        (<xsl:value-of select="count($passed)" />/<xsl:value-of select="count($failed)" />/<xsl:value-of select="count($inconclusive)" />)
      </span>

      <xsl:if test="g:children/g:test[@id=/g:report/g:packageRun/g:testRuns/g:testRun/@id] and $kind != 'Fixture' ">
        <ul>
          <xsl:attribute name="id">testChildrenPanel-<xsl:value-of select="$id" /></xsl:attribute>
          <xsl:apply-templates select="g:children/g:test[@id=/g:report/g:packageRun/g:testRuns/g:testRun/@id]" mode="summary" />
        </ul>
      </xsl:if>
    </li>
  </xsl:template>

  <xsl:template match="g:testModel/g:test" mode="details">
    <div id="Details" class="section">
      <h2>Details</h2>
      <div class="section-content">
        <xsl:choose>
          <xsl:when test="g:children/g:test[@id=/g:report/g:packageRun/g:testRuns/g:testRun/@id]">
            <ul class="testRunContainer">
              <xsl:apply-templates select="g:children/g:test[@id=/g:report/g:packageRun/g:testRuns/g:testRun/@id]" mode="details" />
            </ul>
          </xsl:when>
          <xsl:otherwise>
            <em>This report does not contain any test runs.</em>
          </xsl:otherwise>
        </xsl:choose>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="g:children/g:test" mode="details">
    <xsl:variable name="testId" select="@id" />
    <xsl:variable name="kind" select="g:metadata/g:entry[@key='ComponentKind']/g:value" />

    <xsl:variable name="tests" select="descendant-or-self::g:test[@isTestCase='true']" />
    <xsl:variable name="run" select="$tests[@id = key('status', 'executed')/@id]" />
    <xsl:variable name="passed" select="$tests[@id = key('outcome', 'passed')/@id]" />
    <xsl:variable name="failed" select="$tests[@id = key('outcome', 'failed')/@id]" />
    <xsl:variable name="inconclusive" select="$tests[@id = key('outcome', 'inconclusive')/@id]" />
    <xsl:variable name="ignored" select="$tests[@id = key('status', 'ignored')/@id]" />
    <xsl:variable name="skipped" select="$tests[@id = key('status', 'skipped')/@id]" />

    <xsl:variable name="testRun" select="/g:report/g:packageRun/g:testRuns/g:testRun[@id=$testId]" />
    <xsl:variable name="rootStepRun" select="$testRun/g:stepRun" />
    <xsl:variable name="rootStepResult" select="$rootStepRun/g:result" />

    <xsl:variable name="allTestAndFixtures" select="descendant-or-self::g:test" />
    <xsl:variable name="resultsForAllDescendants" select="/g:report/g:packageRun/g:testRuns/g:testRun[@id = $allTestAndFixtures/@id]//g:stepRun/g:result" />
    <xsl:variable name="assertions" select="sum($resultsForAllDescendants/@assertCount)" />

    <xsl:variable name="nestingLevel" select="count(ancestor::g:test)" />

    <li name="testRun">
      <xsl:attribute name="id">testRun-<xsl:value-of select="$testId" /></xsl:attribute>

      <span>
        <xsl:attribute name="class">testRunHeading testRunHeading-Level<xsl:value-of select="$nestingLevel"/></xsl:attribute>

        <img src="img/Minus.gif" class="toggleAbs">
          <xsl:attribute name="id">toggle-testRunPanel-<xsl:value-of select="$testId" /></xsl:attribute>
          <xsl:attribute name="onclick">toggle('testRunPanel-<xsl:value-of select="$testId" />');</xsl:attribute>
        </img>
        <xsl:call-template name="icon">
          <xsl:with-param name="kind" select="$kind" />
        </xsl:call-template>

        <xsl:value-of select="@name" />

        <xsl:choose>
          <xsl:when test="g:children/g:test">
          <xsl:call-template name="progressBar">
            <xsl:with-param name="passed">
              <xsl:value-of select="count($passed)" />
            </xsl:with-param>
            <xsl:with-param name="failed">
              <xsl:value-of select="count($failed)" />
            </xsl:with-param>
            <xsl:with-param name="inconclusive">
              <xsl:value-of select="count($inconclusive)" />
            </xsl:with-param>
          </xsl:call-template>
          (<xsl:value-of select="count($passed)" />/<xsl:value-of select="count($failed)" />/<xsl:value-of select="count($inconclusive)" />)
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="outcomeBar">
              <xsl:with-param name="outcome" select="$rootStepResult/@outcome" />
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </span>

      <div class="testRunPanel">
        <xsl:attribute name="id">testRunPanel-<xsl:value-of select="$testId" /></xsl:attribute>

        <xsl:choose>
          <xsl:when test="$kind = 'Assembly' or $kind = 'Framework'">
            <table class="statistics-table">
              <tr class="alternate-row">
                <td>Results:</td>
                <td>
                  <xsl:value-of select="count($run)" /> run,
                  <xsl:value-of select="count($passed)" /> passed,
                  <xsl:value-of select="count($failed)" /> failed,
                  <xsl:value-of select="count($inconclusive)" /> inconclusive (<xsl:value-of select="count($ignored)" /> ignored / <xsl:value-of select="count($skipped)" /> skipped)
                </td>
              </tr>
              <tr>
                <td>Duration:</td>
                <td>
                  <xsl:value-of select="format-number($rootStepResult/@duration, '0.00')" />s
                </td>
              </tr>
              <tr class="alternate-row">
                <td>
                  Assertions:
                </td>
                <td>
                  <xsl:value-of select="$assertions" />
                </td>
              </tr>
            </table>
          </xsl:when>
          <xsl:otherwise>
            Duration: <xsl:value-of select="format-number($rootStepResult/@duration, '0.00')" />s,
            Assertions: <xsl:value-of select="$assertions"/>.
          </xsl:otherwise>
        </xsl:choose>

        <xsl:call-template name="print-metadata-entries">
          <xsl:with-param name="entries" select="g:metadata/g:entry|$rootStepRun/g:step/g:metadata/g:entry" />
        </xsl:call-template>

        <div class="stepRun">
          <xsl:attribute name="id">stepRun-<xsl:value-of select="g:step/@id" /></xsl:attribute>
          <xsl:apply-templates select="$rootStepRun" mode="details-content" />
        </div>

        <xsl:if test="g:children/g:test[@id=/g:report/g:packageRun/g:testRuns/g:testRun/@id]">
          <ul class="testRunContainer">
            <xsl:apply-templates select="g:children/g:test[@id=/g:report/g:packageRun/g:testRuns/g:testRun/@id]" mode="details" />
          </ul>
        </xsl:if>
      </div>
    </li>
  </xsl:template>

  <xsl:template match="g:stepRun" mode="details-content">
    <xsl:apply-templates select="g:executionLog">
      <xsl:with-param name="stepId" select="g:step/@id" />
    </xsl:apply-templates>

    <xsl:if test="g:children/g:stepRun">
      <ul class="stepRunContainer">
        <xsl:apply-templates select="g:children/g:stepRun" mode="details" />
      </ul>
    </xsl:if>
  </xsl:template>

  <xsl:template match="g:stepRun" mode="details">
    <xsl:variable name="allStepResults" select="descendant-or-self::g:result" />
    <xsl:variable name="assertions" select="sum($allStepResults/@assertCount)" />

    <li class="stepRun">
      <xsl:attribute name="id">stepRun-<xsl:value-of select="g:step/@id" /></xsl:attribute>

      <span class="stepRunHeading">
        <img src="img/Minus.gif" class="toggleAbs">
          <xsl:attribute name="id">toggle-stepRunPanel-<xsl:value-of select="g:step/@id" /></xsl:attribute>
          <xsl:attribute name="onclick">toggle('stepRunPanel-<xsl:value-of select="g:step/@id" />');</xsl:attribute>
        </img>

        <xsl:value-of select="g:step/@fullName" />
        
        <xsl:call-template name="outcomeBar">
          <xsl:with-param name="outcome" select="g:result/@outcome" />
        </xsl:call-template>
        
        (Duration: <xsl:value-of select="format-number(g:result/@duration, '0.00')" />s, Assertions: <xsl:value-of select="$assertions"/>)
      </span>

      <div class="stepRunPanel">
        <xsl:attribute name="id">stepRunPanel-<xsl:value-of select="g:step/@id" /></xsl:attribute>

        <xsl:apply-templates select="g:step/g:metadata" />
        
        <xsl:apply-templates select="." mode="details-content" />
      </div>
    </li>
  </xsl:template>

  <xsl:template match="g:metadata">
    <xsl:call-template name="print-metadata-entries">
      <xsl:with-param name="entries" select="g:entry" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="print-metadata-entries">
    <xsl:param name="entries" />
    <xsl:variable name="visibleEntries" select="$entries[@key != 'ComponentKind']" />

    <xsl:if test="$visibleEntries">
      <ul class="metadata">
        <xsl:apply-templates select="$visibleEntries">
          <xsl:sort select="translate(@key, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')" lang="en" data-type="text" />
          <xsl:sort select="translate(@value, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')" lang="en" data-type="text" />
        </xsl:apply-templates>
      </ul>
    </xsl:if>
  </xsl:template>

  <xsl:template match="g:entry">
    <li>
      <xsl:value-of select="@key" />: <xsl:value-of select="g:value" />
    </li>
  </xsl:template>

  <xsl:template match="g:executionLog">
    <xsl:param name="stepId" />

    <xsl:if test="g:streams/g:stream">
      <div class="log">
        <xsl:attribute name="id">log-<xsl:value-of select="$stepId" /></xsl:attribute>

        <xsl:apply-templates select="g:streams/g:stream" mode="stream">
          <xsl:with-param name="attachments" select="g:attachments" />
        </xsl:apply-templates>
      </div>
    </xsl:if>
  </xsl:template>

  <xsl:template match="g:streams/g:stream" mode="stream">
    <xsl:param name="attachments" />

    <div>
      <xsl:attribute name="class">logStream logStream-<xsl:value-of select="@name" /></xsl:attribute>

      <span class="logStreamHeading">
        <xsl:value-of select="@name" />
      </span>

      <xsl:apply-templates select="g:body" mode="stream">
        <xsl:with-param name="attachments" select="$attachments" />
      </xsl:apply-templates>
    </div>
  </xsl:template>

  <xsl:template match="g:body" mode="stream">
    <xsl:param name="attachments" />

    <div class="logStreamBody">
      <xsl:apply-templates select="g:contents" mode="stream">
        <xsl:with-param name="attachments" select="$attachments" />
      </xsl:apply-templates>
    </div>
  </xsl:template>

  <xsl:template match="g:section" mode="stream">
    <xsl:param name="attachments" />

    <div class="logStreamSection">
      <span class="logStreamSectionHeading">
        <xsl:value-of select="@name"/>
      </span>
      <div>
        <xsl:apply-templates select="g:contents" mode="stream">
          <xsl:with-param name="attachments" select="$attachments" />
        </xsl:apply-templates>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="g:contents" mode="stream">
    <xsl:param name="attachments" />

    <xsl:apply-templates select="child::node()[self::g:text or self::g:section or self::g:embed]" mode="stream">
      <xsl:with-param name="attachments" select="$attachments" />
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="g:text" mode="stream">
    <xsl:param name="attachments" />

    <div>
      <xsl:call-template name="print-text-with-line-breaks">
        <xsl:with-param name="text" select="text()" />
      </xsl:call-template>
    </div>
  </xsl:template>

  <xsl:template match="g:embed" mode="stream">
    <xsl:param name="attachments" />
    <xsl:variable name="attachmentName" select="@attachmentName" />
    <xsl:variable name="attachment" select="$attachments/g:attachment[@name=$attachmentName]" />

    <div class="logAttachment">
      <xsl:choose>
        <xsl:when test="$attachment/@contentPath">
          <xsl:choose>
            <xsl:when test="starts-with($attachment/@contentType, 'image/')">
              <img>
                <xsl:attribute name="src"><xsl:value-of select="$contentRoot"/><xsl:text>/</xsl:text><xsl:value-of select="$attachment/@contentPath"/></xsl:attribute>
                <xsl:attribute name="alt"><xsl:value-of select="$attachmentName"/></xsl:attribute>
              </img>
            </xsl:when>
            <xsl:otherwise>
              <em>Attachment: </em>
              <a>
                <xsl:attribute name="href"><xsl:value-of select="$contentRoot"/><xsl:text>/</xsl:text><xsl:value-of select="$attachment/@contentPath"/></xsl:attribute>
                <xsl:value-of select="$attachmentName" />
              </a>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:otherwise>
          <em>Attachment: </em>
          <xsl:value-of select="$attachmentName" />
          (content not available)
        </xsl:otherwise>
      </xsl:choose>
    </div>
  </xsl:template>

  <xsl:template name="print-text-with-line-breaks">
    <xsl:param name="text" />
    <xsl:variable name="tail" select="substring-after($text, '&#10;')" />

    <xsl:choose>
      <xsl:when test="$tail!=''">
        <xsl:value-of select="substring-before($text, '&#10;')" />
        <br/>
        <xsl:call-template name="print-text-with-line-breaks">
          <xsl:with-param name="text" select="$tail" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="icon">
    <xsl:param name="kind" />

    <!--<img>
      <xsl:choose>
        <xsl:when test="$kind = 'Fixture'">
          <xsl:attribute name="src">img/Fixture.png</xsl:attribute>
          <xsl:attribute name="alt">Fixture Icon</xsl:attribute>
        </xsl:when>
        <xsl:when test="$kind = 'Test'">
          <xsl:attribute name="src">img/Test.png</xsl:attribute>
          <xsl:attribute name="alt">Test Icon</xsl:attribute>
        </xsl:when>
        <xsl:otherwise>
          <xsl:attribute name="src">img/Container.png</xsl:attribute>
          <xsl:attribute name="alt">Container Icon</xsl:attribute>
        </xsl:otherwise>
      </xsl:choose>
    </img>-->
  </xsl:template>

  <!-- Outcome bar -->
  <xsl:template name="outcomeBar">
    <xsl:param name="outcome" />

    <table class="outcomeBar">
      <tr>
        <td>
          <div>
            <xsl:attribute name="class">outcomeBar outcome-<xsl:value-of select="$outcome" /></xsl:attribute>
            <xsl:text> </xsl:text>
          </div>
        </td>
      </tr>
    </table>
  </xsl:template>
  
  <!-- Progress bar -->
  <xsl:template name="progressBar">
    <xsl:param name="passed" select="0"/>
    <xsl:param name="failed" select="0"/>
    <xsl:param name="inconclusive" select="0"/>
    <xsl:param name="class" select="''" />

    <xsl:variable name="total" select="$passed + $failed + $inconclusive" />

    <table class="progressBar">
      <tr>
        <td>
          <div>
            <xsl:attribute name="class">progressBar $class</xsl:attribute>
            <xsl:if test="$passed > 0">
              <div class="progress-passed">
                <xsl:attribute name="style">width:<xsl:value-of select="100.0 * $passed div $total"/>%</xsl:attribute>
              </div>
            </xsl:if>
            <xsl:if test="$failed > 0">
              <div class="progress-failed">
                <xsl:attribute name="style">width:<xsl:value-of select="100.0 * $failed div $total"/>%</xsl:attribute>
              </div>
            </xsl:if>
            <xsl:if test="$inconclusive > 0">
              <div class="progress-inconclusive" style="width:50%">
                <xsl:attribute name="style">width:<xsl:value-of select="100.0 * $inconclusive div $total"/>%</xsl:attribute>
              </div>
            </xsl:if>
          </div>
        </td>
      </tr>
    </table>
  </xsl:template>

  <!-- Pretty print date time values -->
  <xsl:template name="format-datetime">
    <xsl:param name="datetime" />
    <xsl:value-of select="substring($datetime, 12, 8)" />, <xsl:value-of select="substring($datetime, 1, 10)" />
  </xsl:template>
</xsl:stylesheet>