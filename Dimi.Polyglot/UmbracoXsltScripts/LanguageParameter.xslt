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

  <xsl:variable name="initlang">en</xsl:variable>
  <xsl:variable name="requestLang" select="umbraco.library:RequestQueryString('lang')" />
  <xsl:variable name="langISO">
    <xsl:choose>
      <xsl:when test="Exslt.ExsltRegularExpressions:test($requestLang, '^[a-zA-Z][a-zA-Z](-[a-zA-Z][a-zA-Z])$') = 1 ">
        <xsl:value-of select="Exslt.ExsltStrings:lowercase($requestLang)" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="Exslt.ExsltStrings:lowercase($initlang)" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
</xsl:stylesheet>