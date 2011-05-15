<%@ Page Language="C#" MasterPageFile ="../../masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="TranslationCreation.aspx.cs" Inherits="Dimi.Polyglot.Web.Backoffice.TranslationCreation" %>
<%@ Register Namespace="umbraco.uicontrols" Assembly="controls" TagPrefix="umb" %>
<asp:Content ID="Content" ContentPlaceHolderID="body" runat="server">
<umb:UmbracoPanel ID="Panel1" runat="server" hasMenu="true" Text="Create Language Documents">
        <umb:Pane ID="Pane1" runat="server">
            <umb:PropertyPanel ID="DocumentDescription" runat="server" Text="Document details">
            </umb:PropertyPanel>
            <umb:PropertyPanel ID="PropertyPanel1" runat="server" Text="Languages to be created">
                <asp:CheckBox ID="CheckBoxMultiLanguageSelect" runat="server" /> <b>All</b>
                <asp:CheckBoxList ID="CheckBoxList1" runat="server"></asp:CheckBoxList>
            </umb:PropertyPanel>
            <umb:PropertyPanel ID="PropertyPanel2" runat="server" Text="There are no translation folder document types (x_TranslationFolder) or translation document types (x_Translation) for your currently selected document." Visible="false">
            </umb:PropertyPanel>
        </umb:Pane>
    </umb:UmbracoPanel>
</asp:Content>