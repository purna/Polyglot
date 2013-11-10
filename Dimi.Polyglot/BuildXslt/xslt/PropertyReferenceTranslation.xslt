<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet 
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:msxml="urn:schemas-microsoft-com:xslt"
  xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:dimi.polyglot="urn:dimi.polyglot" 
  exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets dimi.polyglot ">


<xsl:output method="xml" omit-xml-declaration="yes"/>

<xsl:template name="PropertyReferenceTranslation" match="/">
  <xsl:param name="nodeId" />
  <xsl:param name="Property" />
  <xsl:param name="langISO" />
    <xsl:variable name="currentPage" select="umbraco.library:GetXmlNodeById($nodeId)"/>
    <xsl:variable name="resultValue">
      <xsl:variable name="currentPageTypeAlias" select="name($currentPage)" />
      <xsl:choose>
        <xsl:when
            test="$currentPage/*[name() = concat($currentPageTypeAlias, '_TranslationFolder')]/
                    *[name() = concat($currentPageTypeAlias, '_Translation') and language = $langISO]/
                      * [name() = $Property and not(@isDoc)] != '' and
                  string-length($currentPage/*[name() = concat($currentPageTypeAlias, '_TranslationFolder')]/
                    *[name() = concat($currentPageTypeAlias, '_Translation') and language = $langISO]/
                      * [name() = $Property and not(@isDoc)]) != 0">
            <xsl:value-of disable-output-escaping="yes"
                        select="$currentPage/*[name() = concat($currentPageTypeAlias, '_TranslationFolder')]/
                          *[name() = concat($currentPageTypeAlias, '_Translation') and language = $langISO]/
                            * [name() = $Property and not(@isDoc)]" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:choose>
            <xsl:when test="string-length($currentPage/
                             *[name() = concat($Property, '_', $langISO) and not(@isDoc)]) = 0">
              <xsl:value-of disable-output-escaping="yes" 
                               select="$currentPage/* [name() = $Property and not(@isDoc)]" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of disable-output-escaping="yes"
                            select="$currentPage/*[name() = concat($Property, '_', $langISO) and not(@isDoc)]" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:value-of disable-output-escaping="yes" select="$resultValue" />
</xsl:template>
</xsl:stylesheet>