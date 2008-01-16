<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:g="http://www.gallio.org/"
                xmlns="http://www.w3.org/1999/xhtml">
  <xsl:template match="g:report" mode="xhtml-document">
    <html xml:lang="en" lang="en" dir="ltr">
      <head>
        <title>Gallio Test Report</title>
        <link rel="stylesheet" type="text/css" href="{$cssDir}Gallio-Report.css" />
        <script type="text/javascript" src="{$jsDir}Gallio-Report.js">
          <xsl:comment> comment inserted for Internet Explorer </xsl:comment>
        </script>
      </head>
      <body class="gallio-report">
        <xsl:apply-templates select="." mode="xhtml-body" />
      </body>
    </html>
  </xsl:template>
  
  <xsl:template match="g:report" mode="html-document">
    <xsl:call-template name="strip-namespace">
      <xsl:with-param name="nodes"><xsl:apply-templates select="." mode="xhtml-document" /></xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="g:report" mode="xhtml-fragment">
    <div class="gallio-report">
      <!-- Technically a link element should not appear outside of the "head"
           but most browsers tolerate it and this gives us better out of the box
           support in embedded environments like CCNet since no changes need to
           be made to the stylesheets of the containing application.
      -->
      <link rel="stylesheet" type="text/css" href="{$cssDir}Gallio-Report.css" />
      <script type="text/javascript" src="{$jsDir}Gallio-Report.js">
        <xsl:comment> comment inserted for Internet Explorer </xsl:comment>
      </script>
      
      <xsl:apply-templates select="." mode="xhtml-body" />
    </div>
  </xsl:template>

  <xsl:template match="g:report" mode="html-fragment">
    <xsl:call-template name="strip-namespace">
      <xsl:with-param name="nodes"><xsl:apply-templates select="." mode="xhtml-fragment" /></xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  
  <xsl:template match="g:report" mode="xhtml-body">
    <div id="header">
      <h1>Gallio Test Report</h1>
    </div>
    <xsl:apply-templates select="g:package/g:assemblyFiles" />
    <xsl:apply-templates select="g:packageRun" mode="statistics" />
    <xsl:apply-templates select="g:packageRun" mode="summary"/>
    <xsl:apply-templates select="g:packageRun" mode="details"/>
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

  <xsl:template match="g:packageRun" mode="statistics">
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

  <xsl:template match="g:packageRun" mode="summary">
    <div id="Summary" class="section">
      <h2>Summary</h2>
      <div class="section-content">
        <xsl:choose>
          <xsl:when test="g:testInstanceRun/g:children/g:testInstanceRun">
            <ul>
              <xsl:apply-templates select="g:testInstanceRun/g:children/g:testInstanceRun" mode="summary" />
            </ul>
          </xsl:when>
          <xsl:otherwise>
            <em>This report does not contain any test runs.</em>
          </xsl:otherwise>
        </xsl:choose>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="g:testInstanceRun" mode="summary">
    <xsl:variable name="id" select="g:testInstance/@id" />
    <xsl:variable name="testId" select="g:testInstance/@testId" />
    <xsl:variable name="test" select="ancestor::g:report/g:testModel/descendant::g:test[@id = $testId]" />
    
    <xsl:if test="$test/@isTestCase = 'false'">
      <xsl:variable name="testCases" select="$test/descendant-or-self::g:test[@isTestCase='true']" />
      <xsl:variable name="testCaseResults" select="descendant-or-self::g:testInstanceRun[g:testInstance/@testId = $testCases/@id]/g:testStepRun/g:result" />

      <xsl:variable name="passedOutcomeCount" select="count($testCaseResults[@outcome = 'passed'])" />
      <xsl:variable name="failedOutcomeCount" select="count($testCaseResults[@outcome = 'failed'])" />
      <xsl:variable name="inconclusiveOutcomeCount" select="count($testCaseResults[@outcome = 'inconclusive'])" />

      <li>
        <div>
          <xsl:choose>
            <xsl:when test="g:children/g:testInstanceRun">
              <xsl:call-template name="toggle">
                <xsl:with-param name="href">summaryPanel-<xsl:value-of select="$id"/></xsl:with-param>
              </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
              <xsl:call-template name="toggle-stop" />
            </xsl:otherwise>
          </xsl:choose>

          <!--
          <xsl:call-template name="icon">
            <xsl:with-param name="kind" select="$kind" />
          </xsl:call-template>
          -->

          <a href="#testInstanceRun-{$id}"><xsl:value-of select="g:testInstance/@name" /></a>

          <xsl:call-template name="progressBar">
            <xsl:with-param name="passed">
              <xsl:value-of select="$passedOutcomeCount" />
            </xsl:with-param>
            <xsl:with-param name="failed">
              <xsl:value-of select="$failedOutcomeCount" />
            </xsl:with-param>
            <xsl:with-param name="inconclusive">
              <xsl:value-of select="$inconclusiveOutcomeCount" />
            </xsl:with-param>
          </xsl:call-template>
          (<xsl:value-of select="$passedOutcomeCount" />/<xsl:value-of select="$failedOutcomeCount" />/<xsl:value-of select="$inconclusiveOutcomeCount" />)
        </div>

        <xsl:if test="g:children/g:testInstanceRun">
          <ul id="summaryPanel-{$id}">
            <xsl:apply-templates select="g:children/g:testInstanceRun" mode="summary" />
          </ul>
        </xsl:if>
      </li>
    </xsl:if>
  </xsl:template>

  <xsl:template match="g:packageRun" mode="details">
    <div id="Details" class="section">
      <h2>Details</h2>
      <div class="section-content">
        <xsl:choose>
          <xsl:when test="g:testInstanceRun/g:children/g:testInstanceRun">
            <ul class="testInstanceRunContainer">
              <xsl:apply-templates select="g:testInstanceRun/g:children/g:testInstanceRun" mode="details" />
            </ul>
          </xsl:when>
          <xsl:otherwise>
            <em>This report does not contain any test runs.</em>
          </xsl:otherwise>
        </xsl:choose>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="g:testInstanceRun" mode="details">
    <xsl:variable name="id" select="g:testInstance/@id" />
    <xsl:variable name="testId" select="g:testInstance/@testId" />
    <xsl:variable name="test" select="ancestor::g:report/g:testModel/descendant::g:test[@id = $testId]" />
    
    <xsl:variable name="metadataEntries" select="$test/g:metadata/g:entry|g:testInstance/g:metadata/g:entry|g:testStepRun/g:testStep/g:metadata/g:entry" />
    <xsl:variable name="kind" select="$metadataEntries[@key='TestKind']/g:value" />
    
    <xsl:variable name="testCases" select="$test/descendant-or-self::g:test[@isTestCase='true']" />
    <xsl:variable name="testCaseResults" select="descendant-or-self::g:testInstanceRun[g:testInstance/@testId = $testCases/@id]/g:testStepRun/g:result" />

    <xsl:variable name="passedOutcomeCount" select="count($testCaseResults[@outcome = 'passed'])" />
    <xsl:variable name="failedOutcomeCount" select="count($testCaseResults[@outcome = 'failed'])" />
    <xsl:variable name="inconclusiveOutcomeCount" select="count($testCaseResults[@outcome = 'inconclusive'])" />
    
    <xsl:variable name="executedStatusCount" select="count($testCaseResults[@status = 'executed'])" />
    <xsl:variable name="ignoredStatusCount" select="count($testCaseResults[@status = 'ignored'])" />
    <xsl:variable name="skippedStatusCount" select="count($testCaseResults[@status = 'skipped'])" />

    <xsl:variable name="totalAssertCount" select="sum($testCaseResults/@assertCount)" />

    <xsl:variable name="nestingLevel" select="count(ancestor::g:testInstanceRun)" />

    <li id="testInstanceRun-{$id}">
      <div class="testInstanceRunHeading testInstanceRunHeading-Level{$nestingLevel}">
        <xsl:call-template name="toggle">
          <xsl:with-param name="href">testInstanceRunPanel-<xsl:value-of select="$id"/></xsl:with-param>
        </xsl:call-template>
        <!--
        <xsl:call-template name="icon">
          <xsl:with-param name="kind" select="$kind" />
        </xsl:call-template>
        -->

        <xsl:value-of select="g:testInstance/@name" />

        <xsl:choose>
          <xsl:when test="g:children/g:testInstanceRun">
          <xsl:call-template name="progressBar">
            <xsl:with-param name="passed">
              <xsl:value-of select="$passedOutcomeCount" />
            </xsl:with-param>
            <xsl:with-param name="failed">
              <xsl:value-of select="$failedOutcomeCount" />
            </xsl:with-param>
            <xsl:with-param name="inconclusive">
              <xsl:value-of select="$inconclusiveOutcomeCount" />
            </xsl:with-param>
          </xsl:call-template>
          (<xsl:value-of select="$passedOutcomeCount" />/<xsl:value-of select="$failedOutcomeCount" />/<xsl:value-of select="$inconclusiveOutcomeCount" />)
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="outcomeBar">
              <xsl:with-param name="outcome" select="g:testStepRun/g:result/@outcome" />
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </div>

      <div id="testInstanceRunPanel-{$id}" class="testInstanceRunPanel">
        <xsl:choose>
          <xsl:when test="$kind = 'Assembly' or $kind = 'Framework'">
            <table class="statistics-table">
              <tr class="alternate-row">
                <td>Results:</td>
                <td>
                  <xsl:value-of select="$executedStatusCount" /> run,
                  <xsl:value-of select="$passedOutcomeCount" /> passed,
                  <xsl:value-of select="$failedOutcomeCount" /> failed,
                  <xsl:value-of select="$inconclusiveOutcomeCount" /> inconclusive (<xsl:value-of select="$ignoredStatusCount" /> ignored / <xsl:value-of select="$skippedStatusCount" /> skipped)
                </td>
              </tr>
              <tr>
                <td>Duration:</td>
                <td>
                  <xsl:value-of select="format-number(g:testStepRun/g:result/@duration, '0.00')" />s
                </td>
              </tr>
              <tr class="alternate-row">
                <td>
                  Assertions:
                </td>
                <td>
                  <xsl:value-of select="$totalAssertCount" />
                </td>
              </tr>
            </table>
          </xsl:when>
          <xsl:otherwise>
            Duration: <xsl:value-of select="format-number(g:testStepRun/g:result/@duration, '0.00')" />s,
            Assertions: <xsl:value-of select="$totalAssertCount"/>.
          </xsl:otherwise>
        </xsl:choose>

        <xsl:call-template name="print-metadata-entries">
          <xsl:with-param name="entries" select="$metadataEntries" />
        </xsl:call-template>

        <div id="testStepRun-{g:testStepRun/g:testStep/@id}" class="testStepRun">
          <xsl:apply-templates select="g:testStepRun" mode="details-content" />
        </div>

        <xsl:if test="g:children/g:testInstanceRun">
          <ul class="testInstanceRunContainer">
            <xsl:apply-templates select="g:children/g:testInstanceRun" mode="details" />
          </ul>
        </xsl:if>
      </div>
    </li>
  </xsl:template>

  <xsl:template match="g:testStepRun" mode="details-content">
    <xsl:apply-templates select="g:executionLog">
      <xsl:with-param name="stepId" select="g:testStep/@id" />
    </xsl:apply-templates>

    <xsl:if test="g:children/g:testStepRun">
      <ul class="testStepRunContainer">
        <xsl:apply-templates select="g:children/g:testStepRun" mode="details" />
      </ul>
    </xsl:if>
  </xsl:template>

  <xsl:template match="g:testStepRun" mode="details">
    <xsl:variable name="allStepResults" select="descendant-or-self::g:result" />
    <xsl:variable name="assertions" select="sum($allStepResults/@assertCount)" />

    <li id="testStepRun-{g:testStep/@id}" class="testStepRun">
      <div class="testStepRunHeading">
        <xsl:call-template name="toggle">
          <xsl:with-param name="href">testStepRunPanel-<xsl:value-of select="g:testStep/@id"/></xsl:with-param>
        </xsl:call-template>

        <xsl:value-of select="g:testStep/@fullName" />
        
        <xsl:call-template name="outcomeBar">
          <xsl:with-param name="outcome" select="g:result/@outcome" />
        </xsl:call-template>
        
        (Duration: <xsl:value-of select="format-number(g:result/@duration, '0.00')" />s, Assertions: <xsl:value-of select="$assertions"/>)
      </div>

      <div id="testStepRunPanel-{g:testStep/@id}" class="testStepRunPanel">
        <xsl:apply-templates select="g:testStep/g:metadata" />
        
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
    <xsl:variable name="visibleEntries" select="$entries[@key != 'TestKind']" />

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
      <div id="log-{$stepId}" class="log">
        <xsl:apply-templates select="g:streams/g:stream" mode="stream">
          <xsl:with-param name="attachments" select="g:attachments" />
        </xsl:apply-templates>
        
        <xsl:if test="g:attachments/g:attachment">
          <div class="logAttachmentList">
            Attachments: <xsl:for-each select="g:attachments/g:attachment">
              <xsl:apply-templates select="." mode="link" /><xsl:if test="position() != last()">, </xsl:if>
            </xsl:for-each>.
          </div>
        </xsl:if>
      </div>
    </xsl:if>
  </xsl:template>

  <xsl:template match="g:streams/g:stream" mode="stream">
    <xsl:param name="attachments" />

    <div class="logStream logStream-{@name}">
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
    
    <div class="logAttachmentEmbedding">
      <xsl:apply-templates select="$attachments/g:attachment[@name=$attachmentName]" mode="embed" />
    </div>
  </xsl:template>

  <xsl:template match="g:attachment" mode="embed">
    <xsl:variable name="isImage" select="starts-with(@contentType, 'image/')" />
    <xsl:choose>
      <xsl:when test="$attachmentBrokerUrl != ''">
        <xsl:variable name="attachmentBrokerQuery"><xsl:value-of select="$attachmentBrokerUrl"/>testStepId=<xsl:value-of select="../../g:testStep/@id"/>&amp;attachmentName=<xsl:value-of select="@name"/></xsl:variable>
        <xsl:choose>
          <xsl:when test="$isImage">
            <img src="{$attachmentBrokerQuery}" alt="Attachment: {@name}" />
          </xsl:when>
          <xsl:otherwise>
            Attachment: <a href="{$attachmentBrokerQuery}"><xsl:value-of select="@name" /></a>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:when test="@contentDisposition = 'link'">
        <xsl:variable name="attachmentUri" select="translate(@contentPath, '\', '/')" />
        <xsl:choose>
          <xsl:when test="$isImage">
            <img src="{$attachmentUri}" alt="Attachment: {@name}" />
          </xsl:when>
          <xsl:otherwise>
            Attachment: <a href="{$attachmentUri}"><xsl:value-of select="@name" /></a>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:when test="@contentDisposition = 'inline' and $isImage and @encoding = 'base64'">
        <img src="data:{@contentType};base64,{text()}" alt="Attachment: {@name}" />
      </xsl:when>
      <xsl:otherwise>
        Attachment: <xsl:value-of select="@name" /> (n/a)
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template match="g:attachment" mode="link">
    <xsl:choose>
      <xsl:when test="$attachmentBrokerUrl != ''">
        <xsl:variable name="attachmentBrokerQuery"><xsl:value-of select="$attachmentBrokerUrl"/>testStepId=<xsl:value-of select="../../../g:testStep/@id"/>&amp;attachmentName=<xsl:value-of select="@name"/></xsl:variable>
        <a href="{$attachmentBrokerQuery}"><xsl:value-of select="@name" /></a>
      </xsl:when>
      <xsl:when test="@contentDisposition = 'link'">
        <xsl:variable name="attachmentUri" select="translate(@contentPath, '\', '/')" />        
        <a href="{$attachmentUri}"><xsl:value-of select="@name" /></a>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="@name" /> (n/a)
      </xsl:otherwise>
    </xsl:choose>
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

  <!--
  <xsl:template name="icon">
    <xsl:param name="kind" />

    <img>
      <xsl:choose>
        <xsl:when test="$kind = 'Fixture'">
          <xsl:attribute name="src">{$imgDir}Fixture.png</xsl:attribute>
          <xsl:attribute name="alt">Fixture Icon</xsl:attribute>
        </xsl:when>
        <xsl:when test="$kind = 'Test'">
          <xsl:attribute name="src">{$imgDir}Test.png</xsl:attribute>
          <xsl:attribute name="alt">Test Icon</xsl:attribute>
        </xsl:when>
        <xsl:otherwise>
          <xsl:attribute name="src">{$imgDir}Container.png</xsl:attribute>
          <xsl:attribute name="alt">Container Icon</xsl:attribute>
        </xsl:otherwise>
      </xsl:choose>
    </img>
  </xsl:template>
  -->

  <!-- Outcome bar -->
  <xsl:template name="outcomeBar">
    <xsl:param name="outcome" />

    <table class="outcomeBar">
      <tr>
        <td>
          <div class="outcomeBar outcome-{$outcome}">
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

    <xsl:variable name="total" select="$passed + $failed + $inconclusive" />

    <table class="progressBar">
      <tr>
        <td>
          <div class="progressBar">
            <xsl:if test="$passed > 0">
              <div class="progress-passed" style="width:{100.0 * $passed div $total}%" />
            </xsl:if>
            <xsl:if test="$failed > 0">
              <div class="progress-failed" style="width:{100.0 * $failed div $total}%" />
            </xsl:if>
            <xsl:if test="$inconclusive > 0">
              <div class="progress-inconclusive" style="width:{100.0 * $inconclusive div $total}%" />
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
  
  <!-- Toggle buttons -->
  <xsl:template name="toggle">
    <xsl:param name="href" />
    
    <img src="{$imgDir}Minus.gif" class="toggle" id="toggle-{$href}" onclick="toggle('{$href}');" alt="Toggle Button" />
  </xsl:template>
  
  <xsl:template name="toggle-stop">
    <img src="{$imgDir}FullStop.gif" alt="Toggle Placeholder" />
  </xsl:template>
  
  <!-- Namespace stripping adapted from http://www.xml.com/pub/a/2004/05/05/tr.html -->
  <xsl:template name="strip-namespace" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
    <xsl:param name="nodes" />
    <xsl:apply-templates select="msxsl:node-set($nodes)" mode="strip-namespace" />
  </xsl:template>
  
  <xsl:template match="*" mode="strip-namespace">
    <xsl:element name="{local-name()}" namespace="">
      <xsl:apply-templates select="@*|node()" mode="strip-namespace"/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="@*" mode="strip-namespace">
    <xsl:attribute name="{local-name()}" namespace="">
      <xsl:value-of select="."/>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="processing-instruction()|comment()" mode="strip-namespace">
    <xsl:copy>
      <xsl:apply-templates select="node()" mode="strip-namespace"/>
    </xsl:copy>
  </xsl:template>  
</xsl:stylesheet>
