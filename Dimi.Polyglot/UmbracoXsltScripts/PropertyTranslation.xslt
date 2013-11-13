<?xml version="1.0" encoding="UTF-8"?>

<!DOCTYPE xsl:stylesheet [
  <!ENTITY nbsp "&#x00A0;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxml="urn:schemas-microsoft-com:xslt" xmlns:umbraco.library="urn:umbraco.library"
                xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes"
                xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions"
                xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets"
                exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets ">
  <xsl:output method="xml" omit-xml-declaration="yes" />
  <xsl:include href="../xslt/LanguageParameter.xslt" />
  <xsl:param name="currentPage" />
  <xsl:template match="/">
    <xsl:param name="Property" select="macro/Property" />
    <xsl:variable name="currentPageTypeAlias" select="name($currentPage)" />
    <xsl:choose>
      <xsl:when
          test="$currentPage/*[name() = concat($currentPageTypeAlias, 'TranslationFolder')]/
                    *[name() = concat($currentPageTypeAlias, 'Translation') and language = $langISO]/
                      * [name() = $Property and not(@isDoc)] != '' and
                  string-length($currentPage/*[name() = concat($currentPageTypeAlias, 'TranslationFolder')]/
                    *[name() = concat($currentPageTypeAlias, 'Translation') and language = $langISO]/
                      * [name() = $Property and not(@isDoc)]) != 0">
        <xsl:value-of disable-output-escaping="yes"
                     select="umbraco.library:RenderMacroContent($currentPage/*[name() = concat($currentPageTypeAlias, 'TranslationFolder')]/
                    *[name() = concat($currentPageTypeAlias, 'Translation') and language = $langISO]/
                      * [name() = $Property and not(@isDoc)], $currentPage/*[name() = concat($currentPageTypeAlias, 'TranslationFolder')]/
                          *[name() = concat($currentPageTypeAlias, 'Translation') and language = $langISO]/@id)" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="string-length($currentPage/
                             *[name() = concat($Property, '_', $langISO) and not(@isDoc)]) = 0">
            <xsl:value-of disable-output-escaping="yes"
                              select="umbraco.library:RenderMacroContent(
                               $currentPage/* [name() = $Property and not(@isDoc)], $currentPage/@id)" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of disable-output-escaping="yes"
                          select="umbraco.library:RenderMacroContent(
                                $currentPage/*[name() = concat($Property, '_', $langISO) and not(@isDoc)],
                                $currentPage/@id)" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>