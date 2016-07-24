<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  exclude-result-prefixes="msxsl"
  xmlns:user="urn:my-scripts"
  xmlns:ct="http://library.by/catalog"

>

  <msxsl:script implements-prefix='user' language='CSharp'>
    <![CDATA[
    public string ToRfc822(string date) {
          DateTime parsedDate = DateTime.Parse(date);
          return parsedDate.ToString("r");
    }]]>
  </msxsl:script>
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="//ct:catalog">
    <xsl:element name="rss">
      <xsl:attribute name="version">2.0</xsl:attribute>
      <xsl:element name="channel">
        <xsl:element name="link">
          <xsl:text>http://library.by/catalog</xsl:text>
        </xsl:element>
        <xsl:element name="description"/>
        <xsl:element name="title">
          <xsl:text>Catalog</xsl:text>
        </xsl:element>
        <xsl:for-each select="//ct:book">
          <xsl:call-template name="bookItem">
            <xsl:with-param name="book" select = "." />
          </xsl:call-template>
        </xsl:for-each>
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template name = "bookItem" >
    <xsl:param name = "book" />
    <xsl:element name="item">
      <xsl:element name="title">
        <xsl:value-of select="ct:title"/>
      </xsl:element>
      <xsl:element name="description">
        <xsl:value-of select="ct:description"/>
      </xsl:element>
      <xsl:element name="pubDate">
        <xsl:value-of select="user:ToRfc822(ct:registration_date)"/>
      </xsl:element>
      <xsl:element name="guid">
        <xsl:text>http://library.by/catalog/</xsl:text>
        <xsl:value-of select="$book/@id"></xsl:value-of>
      </xsl:element>
      <xsl:if test="ct:isbn and ct:genre = 'Computer'">
        <xsl:element name="link">
          <xsl:text>http://my.safaribooksonline.com/</xsl:text>
          <xsl:value-of select="ct:isbn"/>
          <xsl:text>/</xsl:text>
        </xsl:element>
      </xsl:if>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
