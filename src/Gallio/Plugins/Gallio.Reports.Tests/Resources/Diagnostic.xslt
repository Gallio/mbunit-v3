<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:g="http://www.gallio.org/"
                xmlns="http://www.w3.org/1999/xhtml">
  <xsl:output method="xml" indent="yes" encoding="utf-8" />
  <xsl:param name="resourceRoot" select="''" />
  
  <xsl:template match="/">
    <diagnostics>
      <resourceRoot>
        <xsl:value-of select="$resourceRoot"/>
      </resourceRoot>
    
      <source>
        <xsl:copy-of select="report"/>
      </source>
    </diagnostics>
  </xsl:template>
</xsl:stylesheet>
