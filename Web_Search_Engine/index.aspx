<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Web_Search_Engine.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web Search Engine</title>
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"/>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.0/jquery.min.js"></script>
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="indexForm" style="text-align:center" runat="server">
        <div class="container">
            <h1>COMP 4321 Project Index Page</h1>
            <br/>
            <div class="row">
                <div class="col-md-4">
                    <div class="panel panel-warning">
                        <div class="panel-heading">Start the Crawler</div>
                        <div class="panel-body">
                            <asp:Button id="crawlbutton" class="btn btn-sm btn-info" Text="Crawl" OnClick="crawl" runat="server"/>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="panel panel-info">
                        <div class="panel-heading">Test the Program</div>
                        <div class="panel-body">
                            <asp:Button id="testbutton" class="btn btn-sm btn-info" Text="Test" OnClick="test" runat="server"/>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="panel panel-success">
                        <div class="panel-heading">Go to Search Page</div>
                        <div class="panel-body">
                            <a id="searchbutton" class="btn btn-sm btn-info" href="search.aspx">Search</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
