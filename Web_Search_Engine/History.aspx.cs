using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_Search_Engine
{
    public partial class History : System.Web.UI.Page
    {
        private Crawler crawler;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["crawler"] == null)
            {
                index.initCrawler();
                Session["crawler"] = index.crawler;
            }
            else
            {
                crawler = (Crawler)Session["crawler"];
            }
            if (Session["history"] != null)
            {
                var history = (List<Tuple<string, string, IEnumerable<KeyValuePair<int, Tuple<double, bool>>>>>) Session["history"];
                history = history.OrderByDescending(x => x.Item1).ToList();
                foreach (Tuple<string, string, IEnumerable<KeyValuePair<int, Tuple<double, bool>>>> result in history)
                {
                    TableRow tRow = new TableRow();
                    tRow.VerticalAlign = VerticalAlign.Top;
                    LeftTable.Rows.Add(tRow);

                    TableCell tCellA = new TableCell();
                    tRow.Cells.Add(tCellA);

                    LinkButton linkButton = new LinkButton();
                    linkButton.Text = result.Item2;
                    linkButton.Click += new EventHandler(showResult);
                    linkButton.CommandName = result.Item1;

                    tCellA.Controls.Add(linkButton);
                }
            }
        }

        protected void showResult(object sender, EventArgs e)
        {
            for (int i = 1; i < ResultTable.Rows.Count; i++)
            {
                ResultTable.Rows.RemoveAt(i);
            }
            LinkButton linkButton = (LinkButton)sender;
            var history = (List<Tuple<string, string, IEnumerable<KeyValuePair<int, Tuple<double, bool>>>>>)Session["history"];
            foreach (Tuple<string, string, IEnumerable<KeyValuePair<int, Tuple<double, bool>>>> result in history)
            { 
                if (result.Item1.Equals(linkButton.CommandName)){
                    term.InnerText = result.Item2 + " - " + result.Item1;
                    foreach (KeyValuePair<int, Tuple<double, bool>> pair in result.Item3)
                    {
                        updateTable(pair);
                    }
                }
            }
        }

        protected void updateTable(KeyValuePair<int, Tuple<double, bool>>  pair)
        {
            TableRow tRow = new TableRow();
            tRow.VerticalAlign = VerticalAlign.Top;
            ResultTable.Rows.Add(tRow);

            TableCell tCellA = new TableCell();
            tCellA.Text = pair.Value.Item1 + "";
            tRow.Cells.Add(tCellA);

            TableCell tCellB = new TableCell();
            Page page = crawler.getPageById(pair.Key);

            HyperLink title = new HyperLink();
            title.Text = page.Title;
            title.CssClass = "h4";
            title.NavigateUrl = page.Url;
            tCellB.Controls.Add(title);
            tCellB.Controls.Add(new LiteralControl("<br />"));

            HyperLink url = new HyperLink();
            url.Font.Bold = true;
            url.Text = page.Url;
            url.NavigateUrl = page.Url;
            tCellB.Controls.Add(url);
            tCellB.Controls.Add(new LiteralControl("<br />"));

            DateTime lastModifiedDate = Convert.ToDateTime(page.LastModified);
            Label lastModified = new Label();
            lastModified.ForeColor = System.Drawing.Color.DarkGray;
            lastModified.Text = "Last modified: " + lastModifiedDate.ToLocalTime() + ", ";
            tCellB.Controls.Add(lastModified);
            Label contentLength = new Label();
            contentLength.ForeColor = System.Drawing.Color.DarkGray;
            contentLength.Text = "Content length: " + page.ContentLength;
            tCellB.Controls.Add(contentLength);
            tCellB.Controls.Add(new LiteralControl("<br />"));

            Label freqWordsText = new Label();
            freqWordsText.Text = "Keywords:";
            tCellB.Controls.Add(freqWordsText);
            tCellB.Controls.Add(new LiteralControl("<br />"));
            BulletedList freqWordList = new BulletedList();
            freqWordList.DisplayMode = BulletedListDisplayMode.Text;
            freqWordList.Font.Size = 10;
            ListItem freqWordListItem = new ListItem();
            freqWordListItem.Text = "";
            List<Tuple<string, int>> freqWords = page.getFreqWords(5);
            foreach (Tuple<string, int> freqWord in freqWords)
            {
                freqWordListItem.Text += freqWord.Item1 + ": " + freqWord.Item2 + "; ";
            }
            freqWordList.Items.Add(freqWordListItem);
            tCellB.Controls.Add(freqWordList);

            Label parentLink = new Label();
            parentLink.Text = "Parent link(s):";
            tCellB.Controls.Add(parentLink);
            tCellB.Controls.Add(new LiteralControl("<br />"));
            if (crawler.LinkParent.ContainsKey(pair.Key) && crawler.LinkParent[pair.Key].Count > 0)
            {
                BulletedList bl = new BulletedList();
                bl.DisplayMode = BulletedListDisplayMode.HyperLink;
                bl.Font.Size = 10;
                foreach (int pageId in crawler.LinkParent[pair.Key])
                {
                    Page parentPage = crawler.getPageById(pageId);
                    ListItem li = new ListItem();
                    li.Text = parentPage.Url;
                    li.Value = parentPage.Url;
                    bl.Items.Add(li);
                }
                tCellB.Controls.Add(bl);
            }
            else
            {
                BulletedList bl = new BulletedList();
                bl.DisplayMode = BulletedListDisplayMode.Text;
                bl.Font.Size = 10;
                Label noParentLink = new Label();
                ListItem li = new ListItem();
                li.Text = "None";
                bl.Items.Add(li);
                tCellB.Controls.Add(bl);
            }

            Label childLink = new Label();
            childLink.Text = "Child link(s):";
            tCellB.Controls.Add(childLink);
            tCellB.Controls.Add(new LiteralControl("<br />"));
            if (crawler.LinkChildren.ContainsKey(pair.Key) && crawler.LinkChildren[pair.Key].Count > 0)
            {
                BulletedList bl = new BulletedList();
                bl.DisplayMode = BulletedListDisplayMode.HyperLink;
                bl.Font.Size = 10;
                foreach (int pageId in crawler.LinkChildren[pair.Key])
                {
                    Page childPage = crawler.getPageById(pageId);
                    ListItem li = new ListItem();
                    li.Text = childPage.Url;
                    li.Value = childPage.Url;
                    bl.Items.Add(li);
                }
                tCellB.Controls.Add(bl);
            }
            else
            {
                BulletedList bl = new BulletedList();
                bl.DisplayMode = BulletedListDisplayMode.Text;
                bl.Font.Size = 10;
                Label noChildLink = new Label();
                ListItem li = new ListItem();
                li.Text = "None";
                bl.Items.Add(li);
                tCellB.Controls.Add(bl);
            }

            tRow.Cells.Add(tCellB);
        }
    }
}