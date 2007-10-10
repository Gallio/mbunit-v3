<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:g="http://www.mbunit.com/gallio">
  <xsl:output method="html" doctype-system="http://www.w3.org/TR/html4/strict.dtd"
              doctype-public="-//W3C//DTD HTML 4.01//EN" indent="yes" encoding="utf-8" omit-xml-declaration="yes" />
  <xsl:param name="contentRoot" select="''" />
  <xsl:param name="resourceRoot" select="''" />

  <xsl:variable name="contentDir"><xsl:if test="$contentRoot != ''"><xsl:value-of select="$contentRoot"/>/</xsl:if></xsl:variable>
  <xsl:variable name="cssDir"><xsl:if test="$resourceRoot != ''"><xsl:value-of select="$resourceRoot"/>/</xsl:if>css/</xsl:variable>
  <xsl:variable name="jsDir"><xsl:if test="$resourceRoot != ''"><xsl:value-of select="$resourceRoot"/>/</xsl:if>js/</xsl:variable>
  <xsl:variable name="imgDir"><xsl:if test="$resourceRoot != ''"><xsl:value-of select="$resourceRoot"/>/</xsl:if>img/</xsl:variable>

  <xsl:template match="/">
    <xsl:apply-templates select="/" mode="html-document" />
  </xsl:template>
  
  <!-- Include the base HTML / XHTML report template -->
  <xsl:include href="MbUnit-Report.html+xhtml.xsl" />
</xsl:stylesheet>