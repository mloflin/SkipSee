<%@ Page Title=".: Logs :." Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Logs.aspx.cs" Inherits="SkipSee.Admin.Logs" %>
<asp:Content ID="Logs" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <h2>Parameters</h2>
        <asp:GridView ID="gvParameters" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="ID" DataSourceID="dsParameters" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value" />
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
    </div>
    <div class="row">
        <h2>Logs</h2>
        <table style="width: 50%; ">
            <tr>
                <td><asp:Label ID="lblRole" runat="server" Text="Role"/></td>
                <td><asp:Label ID="lblOperation" runat="server" Text="Operation"/></td>
                <td><asp:Label ID="lblStatus" runat="server" Text="Status"/></td>
                <td><asp:Label ID="lblUserIdentity" runat="server" Text="UserIdentity"/></td>
            </tr>
            <tr>
                <td><asp:DropDownList ID="ddlRole" runat="server"></asp:DropDownList></td>
                <td><asp:DropDownList ID="ddlOperation" runat="server"></asp:DropDownList></td>
                <td><asp:DropDownList ID="ddlStatus" runat="server"></asp:DropDownList></td>
                <td><asp:DropDownList ID="ddlUserIdentity" runat="server"></asp:DropDownList></td>
            </tr>
        </table>
        <asp:GridView ID="gvLogs" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="ID" DataSourceID="dsLogs" ForeColor="#333333" GridLines="None" PageSize="30">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" />
                <asp:BoundField DataField="TIMESTAMP" HeaderText="TIMESTAMP" SortExpression="TIMESTAMP" />
                <asp:BoundField DataField="Role" HeaderText="Role" SortExpression="Role" />
                <asp:BoundField DataField="Operation" HeaderText="Operation" SortExpression="Operation" />
                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                <asp:BoundField DataField="UserIdentity" HeaderText="UserIdentity" SortExpression="UserIdentity" />
                <asp:BoundField DataField="CallDurationMS" HeaderText="CallDurationMS" SortExpression="CallDurationMS" />
                <asp:BoundField DataField="Notes" HeaderText="Notes" SortExpression="Notes" />
                <asp:BoundField DataField="Field1" HeaderText="Field1" SortExpression="Field1" />
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
    </div>
    <asp:SqlDataSource ID="dsParameters" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="SELECT * FROM [Parameter]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="dsLogs" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="SELECT Top 200 * FROM [Tracing] ORDER BY [ID] DESC"></asp:SqlDataSource>
</asp:Content>
