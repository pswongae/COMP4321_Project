using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_Search_Engine
{
    public partial class index : System.Web.UI.Page
    {
        //private string baseUriStr = "http://www.cse.ust.hk/~ericzhao/COMP4321/TestPages/testpage.htm";
        private string baseUriStr = "http://www.cse.ust.hk";
        //private string baseUriStr = "http://www.cse.ust.hk/~ericzhao/COMP4321/TestPages/Movie/1.html";

        private int num = 30;

        private Crawler crawler;

        protected void Page_Load(object sender, EventArgs e)
        {
            cr.Enabled = true;
            ts.Enabled = true;
        }

        protected void crawl(object sender, EventArgs e)
        {
            Uri baseUri = new Uri(baseUriStr);
            crawler = new Crawler(baseUriStr);
            crawler.loadStopwordList("stopwords.txt");

            Dictionary<int, Page> oriPageProperties = new Dictionary<int, Page>();
            // should be loaded from db
            crawler.loadTableFromDB();

            Dictionary<int, Page> pageProperties = crawler.fetchPages(baseUri, num, crawler.PageProperties);
            Dictionary<int, List<int>> linkChildren = crawler.getLinkChildren(pageProperties);
            Dictionary<int, List<int>> linkParent = crawler.getLinkParent(linkChildren);
            Dictionary<int, List<string>> keywords = crawler.getKeywords(pageProperties);
            Dictionary<int, List<string>> keywordsInverted = crawler.getKeywordsInverted(keywords);
            Dictionary<int, List<string>> keywordsT = crawler.getKeywordsT(pageProperties);
            Dictionary<int, List<string>> keywordsTInverted = crawler.getKeywordsTInverted(keywordsT);

            crawler.updateTableIntoDB();

            abc.Text = "Finished Crawling!";
        }

        protected void test(object sender, EventArgs e)
        {
            TestProgram ts = new TestProgram();
            ts.printResult("spider_result.txt");
            ghi.Text = "Please check the spider_result.txt";
            //crawler.loadTableFromDB();
            //crawler.printCrawlerResult("spider_result.txt");
        }
    }

}