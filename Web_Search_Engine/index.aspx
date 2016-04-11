<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Web_Search_Engine.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web Search Engine</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label id="jkl" runat="server" Text="Click the button to start the crawler"/>
        <asp:Button id="cr" OnClick="crawl" Text="Crawl" runat="server"/>
        <asp:Label id="abc" runat="server" Text=""/>
        <br />
        <asp:Label id="def" runat="server" Text="Click the button to execute test program"/>
        <asp:Button id="ts" OnClick="test" Text="Test" runat="server"/>
        <asp:Label id="ghi" runat="server" Text=""/>
    </div>
    </form>
</body>
</html>
