<%@ Page Title=".: Movies :." Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Movies.aspx.cs" Inherits="SkipSee.Admin.Movies" %>
<asp:Content ID="Movies" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div class="row" style="text-align: center">
                <h2>Select Movie</h2>
                <asp:DropDownList ID="ddlMovie" runat="server" AutoPostBack="True" DataSourceID="dsDropDown" DataTextField="Title" DataValueField="MovieID" OnSelectedIndexChanged="ddlMovie_SelectedIndexChanged" OnDataBound="ddlMovie_DataBound">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="row">
                <h2>Movie Details</h2>
                <asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="dsDetails" ForeColor="#333333" GridLines="None">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundField DataField="MovieID" HeaderText="MovieID" SortExpression="MovieID" />
                        <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                        <asp:BoundField DataField="Year" HeaderText="Year" SortExpression="Year" />
                        <asp:BoundField DataField="MPAA_Rating" HeaderText="MPAA_Rating" SortExpression="MPAA_Rating" />
                        <asp:BoundField DataField="RunTime" HeaderText="RunTime" SortExpression="RunTime" />
                        <asp:BoundField DataField="InTheaters_Date" HeaderText="InTheaters_Date" SortExpression="InTheaters_Date" />
                        <asp:BoundField DataField="Date_Added" HeaderText="Date_Added" SortExpression="Date_Added" />
                        <asp:BoundField DataField="Last_Updated" HeaderText="Last_Updated" SortExpression="Last_Updated" />
                        <asp:BoundField DataField="Times_Updated" HeaderText="Times_Updated" SortExpression="Times_Updated" />
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
                <h2>Tags</h2>
                <asp:Label ID="lblAddTag" runat="server" Text="Enter Tag:"></asp:Label>
                <asp:TextBox ID="txtAddTag" runat="server"></asp:TextBox>
                <asp:Button ID="btnAddTag" runat="server" Text="Add" OnClick="btnAddTag_Click" />
                <asp:GridView ID="gvTags" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="ID" DataSourceID="dsTags" ForeColor="#333333" GridLines="None">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
                        <asp:BoundField DataField="Tag" HeaderText="Tag" SortExpression="Tag" />
                        <asp:BoundField DataField="Image_Count" ReadOnly="True" HeaderText="Image_Count" SortExpression="Image_Count" />
                        <asp:BoundField DataField="Video_Count" HeaderText="Video_Count" SortExpression="Video_Count" ReadOnly="True" />
                        <asp:BoundField DataField="Date_Added" HeaderText="Date_Added" ReadOnly="True" SortExpression="Date_Added" />
                        <asp:BoundField DataField="Last_Updated" HeaderText="Last_Updated" ReadOnly="True" SortExpression="Last_Updated" />
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

                <br /><br />
                <asp:GridView ID="gvPopTags" runat="server" CellPadding="4" DataKeyNames="Tag" DataSourceID="dsPopTags" ForeColor="#333333" GridLines="None" AllowPaging="True" AllowSorting="True">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
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
                <h2>Reviews</h2>
                <asp:GridView ID="gvReviews" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="dsReviews" ForeColor="#333333" GridLines="None">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundField DataField="Critics_Score" HeaderText="Critics_Score" SortExpression="Critics_Score" />
                        <asp:BoundField DataField="Critics_Rating" HeaderText="Critics_Rating" SortExpression="Critics_Rating" />
                        <asp:BoundField DataField="Audience_Score" HeaderText="Audience_Score" SortExpression="Audience_Score" />
                        <asp:BoundField DataField="Audience_Rating" HeaderText="Audience_Rating" SortExpression="Audience_Rating" />
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
                <h2>Characters</h2>
                <asp:GridView ID="gvCharacters" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="dsCharacters" ForeColor="#333333" GridLines="None">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                        <asp:BoundField DataField="Character" HeaderText="Character" SortExpression="Character" />
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
                <h2>Posters</h2>
                <asp:GridView ID="gvPosters" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="dsPosters" ForeColor="#333333" GridLines="None">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:TemplateField HeaderText="Thumbnail" SortExpression="Thumbnail">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Thumbnail") %>' Target="_blank">
                                    <img src='<%# Eval("Thumbnail") %>'  alt="Open" />
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Profile" SortExpression="Profile">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Profile") %>' Target="_blank">
                                    <img src='<%# Eval("Profile") %>'  alt="Open" />
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Detailed" SortExpression="Detailed">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Detailed") %>' Target="_blank">
                                    <img src='<%# Eval("Detailed") %>'  alt="Open" />
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Original" SortExpression="Original">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Original") %>' Target="_blank">
                                    <img src='<%# Eval("Original") %>'  alt="Open" height="480" width="320" />
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
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
                <h2>Blurbs</h2>
                <asp:GridView ID="gvBlurbs" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="dsBlurbs" ForeColor="#333333" GridLines="None">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundField DataField="Synopsis" HeaderText="Synopsis" SortExpression="Synopsis" />
                        <asp:BoundField DataField="Critics_Consensus" HeaderText="Critics_Consensus" SortExpression="Critics_Consensus" />
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
                <h2>Videos</h2>
                <asp:PlaceHolder runat="server" ID="VideoPlaceholder"></asp:PlaceHolder>
                <br />
                <br />
                <h2>Images</h2>
                <asp:PlaceHolder runat="server" ID="ImagePlaceholder"></asp:PlaceHolder>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:SqlDataSource ID="dsDropDown" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="SELECT [MovieID], [Title] FROM [tblMovies] WHERE Last_Updated >= DATEADD(dd, -1, GETDATE()) ORDER BY [Title]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="dsDetails" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="SELECT tblMovies.MovieID, tblMovies.Title, tblMovies.Year, tblMovies.MPAA_Rating, tblMovies.RunTime, tblMovies.Date_Added, tblMovies.Last_Updated, tblMovies.Times_Updated, tblReleaseDates.InTheaters_Date, tblReleaseDates.OnDVD_Date FROM tblMovies INNER JOIN tblReleaseDates ON tblMovies.MovieID = tblReleaseDates.MovieID WHERE (tblMovies.MovieID = @MovieID)">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlMovie" Name="MovieID" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsTags" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" DeleteCommand="DELETE FROM [tblTags] WHERE [ID] = @ID" SelectCommand="SELECT * FROM [tblTags] WHERE ([MovieID] = @MovieID)" UpdateCommand="UPDATE [tblTags] SET [Tag] = @Tag, [Last_Updated] = NOW() WHERE [ID] = @ID">
        <DeleteParameters>
            <asp:Parameter Name="ID" Type="Int32" />
        </DeleteParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlMovie" Name="MovieID" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Tag" Type="String" />
            <asp:Parameter Name="ID" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPopTags" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="SELECT Tag, Count(*) as [Count] FROM [tblInst_Tags] WHERE ([MovieID] = @MovieID) Group by Tag Order by [Count] desc">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlMovie" Name="MovieID" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsReviews" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="SELECT [Critics_Score], [Critics_Rating], [Audience_Score], [Audience_Rating] FROM [tblRatings] WHERE ([MovieID] = @MovieID)">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlMovie" Name="MovieID" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsCharacters" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="Select tblCast.Name, tblCharacters.Character from [tblCast]
Left Outer Join [tblCharacters]
on tblCast.ID = tblCharacters.CastID
Where tblCast.MovieID = @MovieID">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlMovie" Name="MovieID" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPosters" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="SELECT [Thumbnail], [Profile], [Detailed], [Original] FROM [tblPosters] WHERE ([MovieID] = @MovieID)">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlMovie" Name="MovieID" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsBlurbs" runat="server" ConnectionString="<%$ ConnectionStrings:SkipSee %>" SelectCommand="SELECT [Synopsis], [Critics_Consensus] FROM [tblBlurbs] WHERE ([MovieID] = @MovieID)">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlMovie" Name="MovieID" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
