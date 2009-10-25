<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
				xmlns="http://schemas.microsoft.com/wix/2006/wi">

  <xsl:template match="wix:Wix">
    <Include>
      <xsl:apply-templates/>
	</Include>
  </xsl:template>
  
  <xsl:template match="wix:Fragment/wix:DirectoryRef">
    <DirectoryRef Id="DLR">
      <xsl:apply-templates select="*"/>
    </DirectoryRef>
  </xsl:template>

  <xsl:template match="wix:Component">
    <xsl:copy>
      <xsl:attribute name="Win64">no</xsl:attribute>
      <xsl:apply-templates select="@*|node()"/>      
    </xsl:copy>
  </xsl:template>

  <xsl:template match="@Id">
    <xsl:attribute name="Id">DLR.scripts.<xsl:value-of select="." /></xsl:attribute>
  </xsl:template>
				
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
