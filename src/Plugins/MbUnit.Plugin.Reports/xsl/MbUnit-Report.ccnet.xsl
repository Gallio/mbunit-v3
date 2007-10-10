<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:g="http://www.mbunit.com/gallio">
  <xsl:output method="html" indent="yes" encoding="utf-8" omit-xml-declaration="yes" />
  <xsl:param name="contentRoot" select="''" />
  <xsl:param name="resourceRoot" select="''" />

  <xsl:variable name="contentDir"></xsl:variable>
  <xsl:variable name="cssDir"></xsl:variable>
  <xsl:variable name="jsDir"></xsl:variable>
  <xsl:variable name="imgDir">/images/</xsl:variable>

  <xsl:template match="/">
    <xsl:apply-templates select="/" mode="html-fragment" />
  </xsl:template>
  
  <!-- Include the base HTML / XHTML report template -->
  <xsl:include href="MbUnit-Report.html+xhtml.xsl" />  
</xsl:stylesheet>