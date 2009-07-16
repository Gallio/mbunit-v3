<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
				xmlns="http://schemas.microsoft.com/wix/2006/wi">

  <xsl:template match="wix:Fragment">
    <Fragment>
      <DirectoryRef Id="RSpec">
        <xsl:apply-templates/>
      </DirectoryRef>
    </Fragment>
  </xsl:template>

  <xsl:template match="@Id">
    <xsl:attribute name="Id">RSpec-scripts-<xsl:value-of select="." /></xsl:attribute>
  </xsl:template>
				
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
