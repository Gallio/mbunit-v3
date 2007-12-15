<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:g="http://www.mbunit.com/gallio">
  <xsl:param name="resourceRoot" select="''" />
  
  <xsl:param name="show-passed-tests">true</xsl:param>
  <xsl:param name="show-failed-tests">true</xsl:param>
  <xsl:param name="show-inconclusive-tests">true</xsl:param>
  <xsl:param name="show-ignored-tests">true</xsl:param>
  <xsl:param name="show-skipped-tests">true</xsl:param>

  <xsl:output method="text" encoding="utf-8"/>

  <xsl:template match="/">
    <xsl:apply-templates select="//g:packageRun" />
  </xsl:template>

  <xsl:template match="g:packageRun">
		<xsl:apply-templates select="g:testInstanceRuns" />
    <xsl:apply-templates select="g:statistics" />
  </xsl:template>
  
	<xsl:template match="g:statistics">
    <xsl:text>Run: </xsl:text>
		<xsl:value-of select="@runCount"/>
    <xsl:text>, Passed: </xsl:text>
    <xsl:value-of select="@passCount"/>
    <xsl:text>, Failed: </xsl:text>
		<xsl:value-of select="@failureCount"/>
    <xsl:text>, Inconclusive: </xsl:text>
    <xsl:value-of select="@inconclusiveCount"/>
    <xsl:text>, Ignored: </xsl:text>
    <xsl:value-of select="@ignoreCount"/>
    <xsl:text>, Skipped: </xsl:text>
		<xsl:value-of select="@skipCount"/>
		<xsl:text>.&#xA;</xsl:text>
	</xsl:template>
  
	<xsl:template match="g:testInstanceRuns">
    <xsl:variable name="passed" select="descendant::g:testStepRun[g:result/@status='executed' and g:result/@outcome='passed']" />
    <xsl:variable name="failed" select="descendant::g:testStepRun[g:result/@status='executed' and g:result/@outcome='failed']" />
    <xsl:variable name="inconclusive" select="descendant::g:testStepRun[g:result/@status='executed' and g:result/@outcome='inconclusive']" />
    <xsl:variable name="ignored" select="descendant::g:testStepRun[g:result/@status='ignored']" />
    <xsl:variable name="skipped" select="descendant::g:testStepRun[g:result/@status='skipped']" />

    <xsl:if test="$show-passed-tests and $passed">
      <xsl:text>* Passed:&#xA;&#xA;</xsl:text>
      <xsl:apply-templates select="$passed" />
      <xsl:text>&#xA;</xsl:text>
    </xsl:if>

    <xsl:if test="$show-failed-tests and $failed">
      <xsl:text>* Failed:&#xA;&#xA;</xsl:text>
      <xsl:apply-templates select="$failed" />
      <xsl:text>&#xA;</xsl:text>
    </xsl:if>
    
    <xsl:if test="$show-inconclusive-tests and $inconclusive">
      <xsl:text>* Inconclusive:&#xA;&#xA;</xsl:text>
      <xsl:apply-templates select="$inconclusive" />
      <xsl:text>&#xA;</xsl:text>
    </xsl:if>

    <xsl:if test="$show-skipped-tests and $skipped">
      <xsl:text>* Skipped:&#xA;&#xA;</xsl:text>
      <xsl:apply-templates select="$skipped" />
      <xsl:text>&#xA;</xsl:text>
    </xsl:if>
    
		<xsl:if test="$show-ignored-tests and $ignored">
			<xsl:text>* Ignored:&#xA;&#xA;</xsl:text>
		  <xsl:apply-templates select="$ignored" />
      <xsl:text>&#xA;</xsl:text>
    </xsl:if>
	</xsl:template>
  
	<xsl:template match="g:testInstanceRun">
    <xsl:apply-templates select="g:testStepRun" />
	</xsl:template>

  <xsl:template match="g:testStepRun">
    <xsl:variable name="testInstanceRun" select="ancestor::g:testInstanceRun" />
    <xsl:variable name="testId" select="$testInstanceRun/g:testInstance/@testId" />
    <xsl:variable name="test" select="//g:test[@id=$testId]" />

    <xsl:text>[</xsl:text>
    <xsl:value-of select="$test/g:metadata/g:entry[@key='ComponentKind']/g:value" />
    <xsl:text>] </xsl:text>
    <xsl:value-of select="@fullName" />
    <xsl:text>&#xA;</xsl:text>
    <xsl:apply-templates select="g:executionLog" />
    <xsl:text>&#xA;</xsl:text>

    <xsl:apply-templates select="g:children/g:testStepRun" />
  </xsl:template>

  <xsl:template match="g:executionLog">
    <xsl:apply-templates select="g:streams" />
  </xsl:template>

  <xsl:template match="g:streams">
    <xsl:apply-templates select="g:stream" />
  </xsl:template>
  
  <xsl:template match="g:stream">
    <xsl:param name="prefix" select="'  '" />

    <xsl:value-of select="$prefix"/>
    <xsl:text>&lt;Stream: </xsl:text>
    <xsl:value-of select="@name" />
    <xsl:text>&gt;&#xA;</xsl:text>
    <xsl:apply-templates select="g:body">
      <xsl:with-param name="prefix" select="concat($prefix, '  ')" />
    </xsl:apply-templates>
    <xsl:value-of select="$prefix"/>
    <xsl:text>&lt;End Stream&gt;&#xA;</xsl:text>
  </xsl:template>
  
  <xsl:template match="g:body">
    <xsl:param name="prefix" select="''" />

    <xsl:apply-templates select="g:contents">
      <xsl:with-param name="prefix" select="$prefix" />
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="g:contents">
    <xsl:param name="prefix" select="''"  />
    
    <xsl:apply-templates select="child::node()[self::g:text or self::g:section or self::g:embed]">
      <xsl:with-param name="prefix" select="$prefix" />
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="g:text">
    <xsl:param name="prefix" select="''"  />
    
    <xsl:call-template name="indent">
      <xsl:with-param name="str" select="text()" />
      <xsl:with-param name="prefix" select="$prefix" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="g:section">
    <xsl:param name="prefix" select="''"  />
    
    <xsl:value-of select="$prefix"/>
    <xsl:text>&lt;Section: </xsl:text>
    <xsl:value-of select="@name" />
    <xsl:text>&gt;&#xA;</xsl:text>
    <xsl:apply-templates select="g:contents">
      <xsl:with-param name="prefix" select="concat($prefix, '  ')" />
    </xsl:apply-templates>
    <xsl:value-of select="$prefix"/>
    <xsl:text>&lt;End Section&gt;&#xA;</xsl:text>
  </xsl:template>

  <xsl:template match="g:embed">
    <xsl:param name="prefix" select="''"  />
    
    <xsl:value-of select="$prefix"/>
    <xsl:text>&lt;Attachment: </xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text>&gt;&#xA;</xsl:text>
  </xsl:template>

  <xsl:template match="*">
  </xsl:template>

  <xsl:template name="indent">
    <xsl:param name="str" />
    <xsl:param name="prefix" select="'  '" />

    <xsl:if test="$str!=''">
      <xsl:call-template name="indent-recursive">
        <xsl:with-param name="str" select="translate($str, '&#9;&#xA;&#xD;', ' &#xA;')" />
        <xsl:with-param name="prefix" select="$prefix" />
      </xsl:call-template>
    </xsl:if>
  </xsl:template>

  <xsl:template name="indent-recursive">
    <xsl:param name="str" />
    <xsl:param name="prefix" />

    <xsl:variable name="line" select="substring-before($str, '&#xA;')" />
    <xsl:choose>
      <xsl:when test="$line!=''">
        <xsl:value-of select="$prefix"/>
        <xsl:value-of select="$line"/>
        <xsl:text>&#xA;</xsl:text>
        <xsl:call-template name="indent-recursive">
          <xsl:with-param name="str" select="substring-after($str, '&#xA;')" />
          <xsl:with-param name="prefix" select="$prefix" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$prefix"/>
        <xsl:value-of select="$str"/>
        <xsl:text>&#xA;</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
</xsl:stylesheet>