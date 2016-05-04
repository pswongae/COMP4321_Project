using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_Search_Engine
{
    public partial class stemword : System.Web.UI.Page
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
            loadAllStemwords();
        }

        protected void loadAllStemwords()
        {
            List<KeyValuePair<int, string>> wordPropertiesList = crawler.WordProperties.ToList();
            List<string> words = new List<string>();
            foreach (KeyValuePair<int, string> wordProperty in wordPropertiesList)
            {
                words.Add(wordProperty.Value);
            }
            int n = 10;
            int i = 0;
            TableRow tRow = new TableRow();
            WordTable.Rows.Add(tRow);
            for (; i < words.Count; i++)
            {
                if (i % n == 0)
                {
                    tRow = new TableRow();
                    WordTable.Rows.Add(tRow);
                }
                TableCell tCellA = new TableCell();
                tCellA.Width = 100;
                Button button = new Button();
                button.CssClass = "btn btn-link";
                button.ID = "queryBut"+i;
                button.Text = words[i];
                tCellA.Controls.Add(button);
                tRow.Cells.Add(tCellA);
            }
            while (i++ % n != 0)
            {
                TableCell tCellA = new TableCell();
                tCellA.Width = 100;
                tRow.Cells.Add(tCellA);
            }
        }
    }
}