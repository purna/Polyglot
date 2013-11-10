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
  <xsl:param name="currentPage" />
  <xsl:include href="../xslt/LanguageParameter.xslt" />
  <xsl:include href="../xslt/PropertyReferenceTranslation.xslt" />
  <xsl:template match="/">
    <xsl:param name="UlCssId" select="macro/UlCssId" />
    <xsl:param name="Level" select="macro/Level" />
    <xsl:param name="NaviHideProperty" select="macro/NaviHideProperty" />
    <xsl:param name="CurrentItemCssClass" select="macro/CurrentItemCssClass" />
    <xsl:param name="TitlePropertyAlias" select="macro/TitlePropertyAlias" />
    <ul id="{$UlCssId}">
      <xsl:for-each select="$currentPage/ancestor-or-self::*[@level=$Level and @isDoc]/child::*[@isDoc]">
        <xsl:variable name="hide">
          <xsl:choose>
            <xsl:when test="string-length($NaviHideProperty)!=0">
              <xsl:value-of select="child::*[name()=$NaviHideProperty]" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="child::*[name()='umbracoNaviHide']" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:if test="$hide!='1' or string-length($hide)=0">
          <xsl:variable name="link">
            <xsl:choose>
              <xsl:when test="string-length($langISO)=0">
                <xsl:value-of disable-output-escaping="yes" select="umbraco.library:NiceUrl(@id)" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of disable-output-escaping="yes" select="concat(umbraco.library:NiceUrl(@id), '?lang=', $langISO)" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:variable>
          <xsl:variable name="currentPageTypeAlias" select="name()" />
          <xsl:variable name="title">
            <xsl:call-template name="PropertyReferenceTranslation">
              <xsl:with-param name="nodeId" select="@id" />
              <xsl:with-param name="Property" select="$TitlePropertyAlias" />
              <xsl:with-param name="langISO" select="$langISO" />
            </xsl:call-template>
          </xsl:variable>
          <xsl:variable name="currentCss">
            <xsl:if test="contains(umbraco.library:NiceUrl($currentPage/@id),umbraco.library:NiceUrl(@id))">
              <xsl:value-of select="$CurrentItemCssClass" />
            </xsl:if>
          </xsl:variable>
          <li class="{$currentCss}">
            <a href="{$link}" title="{$title}">
              <xsl:value-of disable-output-escaping="yes" select="$title" />
            </a>
          </li>
        </xsl:if>
      </xsl:for-each>
    </ul>
  </xsl:template>
</xsl:stylesheet>