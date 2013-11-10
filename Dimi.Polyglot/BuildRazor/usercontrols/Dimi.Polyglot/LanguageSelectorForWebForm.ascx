<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LanguageSelectorForWebForm.ascx.cs" Inherits="Dimi.Polyglot.Web.Frontoffice.LanguageSelectorForWebForm" %>
<asp:DropDownList ID="LanguageDropDownList" runat="server" 
    onselectedindexchanged="LanguageDropDownList_SelectedIndexChanged" 
    AutoPostBack="True"></asp:DropDownList><noscript><asp:Button ID="LanguageSubmitNoScript" runat="server" Text="&gt;" onclick="LanguageSubmitNoScript_Click" /></noscript>
