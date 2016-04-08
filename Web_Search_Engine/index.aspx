<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Web_Search_Engine.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web Search Engine</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2><asp:Label id="abc" runat="server" Text=""/></h2>
        <asp:Label id="def" runat="server" Text=""/>
        <asp:Button id="ts" OnClick="submit" Text="Test" runat="server"/>
        <br />
        <asp:Label id="ghi" runat="server" Text=""/>
    </div>
    </form>
</body>
</html>
