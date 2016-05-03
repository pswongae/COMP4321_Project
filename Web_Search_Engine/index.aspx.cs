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
        //private static string baseUriStr = "http://www.cse.ust.hk/~ericzhao/COMP4321/TestPages/testpage.htm";
        //private static string baseUriStr = "http://www.cse.ust.hk";
        //private static string baseUriStr = "http://www.cse.ust.hk/~ericzhao/COMP4321/TestPages/Movie/1.html";
        private static string baseUriStr = "http://ihome.ust.hk/~pswongae/se/D1.html";

        private int num = 500;

        public static Crawler crawler;

        protected void Page_Load(object sender, EventArgs e)
        {
            initCrawler();
        }

        public static void initCrawler()
        {
            crawler = new Crawler(baseUriStr);
            //crawler.loadStopwordList("stopwords.txt");
            crawler.loadTableFromDB();
        }

        protected void crawl(object sender, EventArgs e)
        {
            Uri baseUri = new Uri(baseUriStr);

            //Dictionary<int, Page> oriPageProperties = new Dictionary<int, Page>();

            Dictionary<int, Page> pageProperties = crawler.fetchPages(baseUri, num, crawler.PageProperties);
            Dictionary<int, List<int>> linkChildren = crawler.getLinkChildren(pageProperties);
            Dictionary<int, List<int>> linkParent = crawler.getLinkParent(linkChildren);
            Dictionary<int, List<string>> keywords = crawler.getKeywords(pageProperties);
            Dictionary<int, List<string>> keywordsInverted = crawler.getKeywordsInverted(keywords);
            Dictionary<int, List<string>> keywordsT = crawler.getKeywordsT(pageProperties);
            Dictionary<int, List<string>> keywordsTInverted = crawler.getKeywordsTInverted(keywordsT);

            crawler.updateTableIntoDB();
        }

        protected void test(object sender, EventArgs e)
        {
            TestProgram ts = new TestProgram();
            ts.printResult("spider_result.txt");
        }

    }

}