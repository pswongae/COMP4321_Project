using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Search_Engine
{
    public class TestProgram
    {
        private static string baseUriStr = "http://www.cse.ust.hk";

        public TestProgram() { }

        public void printResult(string fileName)
        {
            Crawler crawler = new Crawler(baseUriStr);
            crawler.loadTableFromDB();
            crawler.printCrawlerResult(fileName);
        }

        public static void Main(string[] args)
        {
            TestProgram ts = new TestProgram();
            ts.printResult("spider_result.txt");
        }

    }
}