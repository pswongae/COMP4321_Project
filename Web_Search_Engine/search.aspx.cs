using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_Search_Engine
{
    public partial class search : System.Web.UI.Page
    {
        private Crawler crawler;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (index.crawler == null)
            {
                index.initCrawler();
            }
            crawler = index.crawler;

            string queryTerm = Request.Form.AllKeys.FirstOrDefault(s => s.Contains("query") && s != "query");
            if (queryTerm != null && queryTerm.Length > 0)
            {
                string[] queryTermArr = queryTerm.Split('_');
                if (queryTermArr.Length > 1)
                {
                    query.Text = queryTermArr[1].Replace('+', ' ');
                }
                else
                {
                    query.Text = Request.Form[queryTerm];
                }
                submitQuery(sender, e);
            }
        }

        protected void submitQuery(object sender, EventArgs e)
        {
            SearchEngine se = new SearchEngine(query.Text, crawler);
            se.search();
            IEnumerable<KeyValuePair<int, Tuple<double, bool>>> scoreTable = se.ScoreTable.ToList();
            //scoreTable = scoreTable.OrderByDescending(a => a.Value.Item2).ThenByDescending(b => b.Value.Item1);
            scoreTable = scoreTable.OrderByDescending(b => b.Value.Item1);
            foreach (KeyValuePair<int, Tuple<double, bool>> pair in scoreTable)
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
                lastModified.Text = "Last modified: " + lastModifiedDate.ToLocalTime()+", ";
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
                    freqWordListItem.Text += freqWord.Item1 + ": " + freqWord.Item2 +"; ";
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

                TableCell tCellC = new TableCell();
                Button similarPages = new Button();
                similarPages.CssClass = "btn btn-default btn-sm";
                similarPages.Text = "Get Similar Pages";
                similarPages.ID = "queryBut" + pair.Key;
                for (int i = 0; i < freqWords.Count; i++)
                {
                    similarPages.ID += (i > 0 ? "+" : "_") + freqWords[i].Item1;
                }
                tCellC.Controls.Add(similarPages);
                tRow.Cells.Add(tCellC);
            }
        }
    }
}