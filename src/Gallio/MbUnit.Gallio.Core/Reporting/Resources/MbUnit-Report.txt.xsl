<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:gallio="http://www.mbunit.com/gallio">
  <xsl:param name="show-passed-tests">true</xsl:param>
  <xsl:param name="show-failed-tests">true</xsl:param>
  <xsl:param name="show-inconclusive-tests">true</xsl:param>
  <xsl:param name="show-ignored-tests">true</xsl:param>
  <xsl:param name="show-skipped-tests">true</xsl:param>

  <xsl:output method="text" encoding="utf-8"/>

  <xsl:template match="/">
    <xsl:apply-templates select="//gallio:packageRun" />
  </xsl:template>

  <xsl:template match="gallio:packageRun">
		<xsl:apply-templates select="gallio:testRuns" />
    <xsl:apply-templates select="gallio:statistics" />
  </xsl:template>
  
	<xsl:template match="gallio:statistics">
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
		<xsl:text>.
</xsl:text>
	</xsl:template>
  
	<xsl:template match="gallio:testRuns">
    <xsl:variable name="passed" select="gallio:testRun[gallio:result/@state='executed' and gallio:result/@outcome='passed']" />
    <xsl:variable name="failed" select="gallio:testRun[gallio:result/@state='executed' and gallio:result/@outcome='failed']" />
    <xsl:variable name="inconclusive" select="gallio:testRun[gallio:result/@state='executed' and gallio:result/@outcome='inconclusive']" />
    <xsl:variable name="ignored" select="gallio:testRun[gallio:result/@state='ignored']" />
    <xsl:variable name="skipped" select="gallio:testRun[gallio:result/@state='skipped']" />

    <xsl:if test="$show-passed-tests and $passed">
      <xsl:text>Passed:
</xsl:text>
      <xsl:apply-templates select="$passed" />
      <xsl:text>
</xsl:text>
    </xsl:if>

    <xsl:if test="$show-failed-tests and $failed">
			<xsl:text>Failed:
</xsl:text>
		<xsl:apply-templates select="$failed" />
      <xsl:text>
</xsl:text>
    </xsl:if>
    
    <xsl:if test="$show-inconclusive-tests and $inconclusive">
      <xsl:text>Inconclusive:
</xsl:text>
      <xsl:apply-templates select="$inconclusive" />
      <xsl:text>
</xsl:text>
    </xsl:if>
    
    <xsl:if test="$show-skipped-tests and $skipped">
			<xsl:text>Skipped:
</xsl:text>
		<xsl:apply-templates select="$skipped" />
      <xsl:text>
</xsl:text>
    </xsl:if>
    
		<xsl:if test="$show-ignored-tests and $ignored">
			<xsl:text>Ignored:
</xsl:text>
		<xsl:apply-templates select="$ignored" />
      <xsl:text>
</xsl:text>
    </xsl:if>
	</xsl:template>
  
	<xsl:template match="gallio:testRun">
    <xsl:variable name="testId" select="@id" />
    <xsl:variable name="test" select="//gallio:test[@id=$testId]" />
    
		<xsl:value-of select="position()" />
		<xsl:text>) [</xsl:text>
    <xsl:value-of select="$test/gallio:metadata/gallio:entry[@key='ComponentKind']/gallio:value" />
    <xsl:text>] </xsl:text>
    <xsl:value-of select="$test/@name" />
		<xsl:text>
</xsl:text>
    <xsl:apply-templates select="gallio:executionLog" />
	</xsl:template>

  <xsl:template match="gallio:executionLog">
    <xsl:apply-templates select="gallio:streams" />
  </xsl:template>

  <xsl:template match="gallio:streams">
    <xsl:apply-templates select="gallio:stream[@name='Failures']" />
  </xsl:template>
  
  <xsl:template match="gallio:stream">
    <xsl:apply-templates select="gallio:body" />
  </xsl:template>
  
  <xsl:template match="gallio:body">
    <xsl:apply-templates select="gallio:contents" />
  </xsl:template>

  <xsl:template match="gallio:contents">
    <xsl:apply-templates select="child::node()" />
  </xsl:template>

  <xsl:template match="gallio:text">
    <xsl:value-of select="text()" />
  </xsl:template>

  <xsl:template match="gallio:section">
    <xsl:text>[Section: </xsl:text>
    <xsl:value-of select="@name" />
    <xsl:text>]
</xsl:text>
    <xsl:apply-templates select="gallio:contents" />
  </xsl:template>

  <xsl:template match="gallio:embed">
    <xsl:text>[Attachment: </xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text>]
</xsl:text>
  </xsl:template>

  <xsl:template match="*">    
  </xsl:template>
  
</xsl:stylesheet>