<%@ Page Title=".: Instagram :." Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Instagram.aspx.cs" Inherits="SkipSee.Admin.Instagram" %>
<asp:Content ID="Instagram" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div style="text-align: center">
            <h2>Enter Tag</h2>
            <asp:TextBox ID="txtTag" runat="server" AutoPostBack="True"></asp:TextBox>
            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
        </div>
        <br />
        <br />
        <h2>Videos</h2>
        <asp:PlaceHolder runat="server" ID="VideoPlaceholder"></asp:PlaceHolder>
        <br />
        <br />
        <h2>Images</h2>
        <asp:PlaceHolder runat="server" ID="ImagePlaceholder"></asp:PlaceHolder>
    </div>
</asp:Content>
