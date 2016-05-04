<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="Web_Search_Engine.History" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web Search Engine</title>
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"/>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.0/jquery.min.js"></script>
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="historyForm" style="text-align:center" runat="server">
    <div>
        <h1>COMP 4321 Project Search History Page</h1>
        <br/>
        <div id="left" style="float:left; width:20%" >
            <asp:Table id="LeftTable" class="table" style="text-align:left" width="100%" runat="server">
                <asp:TableHeaderRow>
                </asp:TableHeaderRow>
            </asp:Table>
        </div>
        <div id="right" style="float:right; width:80%">
            <h3 id="term" style="text-align:center" runat="server"></h3>
            <asp:Table id="ResultTable" class="table" style="text-align:left" width="100%" runat="server">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell Width="200">Score</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Result</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
            <a id="backbtn" class="btn btn-default btn-md" href="search.aspx">Back To Search</a>
        </div>
        <br/>
        <br/> 
    </div>
    </form>
</body>
</html>
