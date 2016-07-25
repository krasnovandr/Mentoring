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
    public string CurrentDate() {
      return DateTime.UtcNow.ToString();
    }
    
     public object DistinctValues(object aaa) {
      return aaa;
    }
  
  ]]>
  </msxsl:script>
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">
    <html>
      <body>
        <h2>Текущие фонды по жанрам</h2>
        <h4>
          <xsl:value-of select="user:CurrentDate()"/>
        </h4>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th>Title</th>
            <th>Artist</th>
          </tr>
            <xsl:value-of select="user:DistinctValues(//ct:genre/text())"/>
          <xsl:for-each select="catalog/book[genre]">
            <xsl:choose>
              <xsl:when test="expression">
                ... some output ...
              </xsl:when>
              <xsl:otherwise>
                ... some output ....
              </xsl:otherwise>
            </xsl:choose>
            
            
            <tr>
              <td>
                <xsl:value-of select="title"/>
              </td>
              <td>
                <xsl:value-of select="genre"/>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
