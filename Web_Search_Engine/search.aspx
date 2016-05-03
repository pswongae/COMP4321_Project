<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="Web_Search_Engine.search" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web Search Engine</title>
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"/>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.0/jquery.min.js"></script>
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="searchForm" style="text-align:center" runat="server">
    <div>
            <h1>COMP 4321 Project Search Page</h1>
            <br/>
            <div>
                <label style="font-size:24px">Query: </label>
                <asp:TextBox ID="query" name="query" width="500" runat="server"/>
                <asp:Button id="submit" class="btn btn-success btn-md" Text="Submit" OnClick="submitQuery" runat="server"/>
                <a id="history" class="btn btn-warning btn-md" href="history.aspx">Search History</a>
            </div>
            <br/>
            <div>
                <label>Do not know what to search? Then Go to Stemmed Word List: </label>
                <a id="stemwords" class="btn btn-link" href="stemword.aspx">List</a>
            </div>
            <asp:Table id="ResultTable" class="table" style="text-align:left" width="100%" runat="server">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell Width="200">Score</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Result</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="100"></asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
            <br/>
            <a id="backbtn" class="btn btn-default btn-md" href="index.aspx">Back To Index</a>
    </div>
    </form>
</body>
</html>
