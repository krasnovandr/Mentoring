<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  exclude-result-prefixes="msxsl"
  xmlns:user="my:my-scripts"
  xmlns:ct="http://library.by/catalog"
  xmlns:Helpers="urn:Helpers"
>

  <msxsl:script implements-prefix='user' language='CSharp'>
    <![CDATA[
    public string CurrentDate() {
      return DateTime.Now.ToString();
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
        <xsl:variable name="allBooks" select="/ct:catalog/ct:book" />
        <xsl:for-each select="Helpers:Distinct(//ct:genre/text())">
          <xsl:variable name="item" select="." />
          <h3>
            <xsl:value-of select="."/>
          </h3>
          <table border="1">
            <tr bgcolor="#9acd32">
              <th>Автор</th>
              <th>Название</th>
              <th>Дата издания</th>
              <th>Дата регистрации</th>
            </tr>

            <xsl:for-each select="$allBooks">
              <xsl:if test="$item = ct:genre">
                <xsl:call-template name="bookItem">
                  <xsl:with-param name="book" select = "." />
                </xsl:call-template>
              </xsl:if>
            </xsl:for-each>
          </table>
          <h4>
            Общее число книг в жанре <xsl:value-of select="$item"/>:
            <b>
              <xsl:value-of select="count($allBooks[ct:genre=$item])"/>
            </b>
          </h4>
        </xsl:for-each>
        <b>
          <i>
            Общее число книг:<xsl:value-of select="count($allBooks)"/>
          </i>
        </b>
      </body>
    </html>
  </xsl:template>

  <xsl:template name = "bookItem" >
    <xsl:param name = "book" />
    <tr>
      <td>
        <xsl:value-of select="ct:author"/>
      </td>
      <td>
        <xsl:value-of select="ct:title"/>
      </td>
      <td>
        <xsl:value-of select="ct:publish_date"/>
      </td>
      <td>
        <xsl:value-of select="ct:registration_date"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
