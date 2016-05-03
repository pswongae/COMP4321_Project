using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using porter;
using System.Configuration;

namespace Web_Search_Engine
{
    public class Crawler
    {
        public Uri BaseUri { get; set; }

        public HtmlWeb Web { get; set; }
        
        public Dictionary<int, Page> PageProperties { get; set; }

        public Dictionary<int, string> WordProperties { get; set; }

        public Dictionary<int, List<int>> LinkChildren { get; set; }

        public Dictionary<int, List<int>> LinkParent { get; set; }

        public Dictionary<int, List<string>> Keywords { get; set; }

        public Dictionary<int, List<string>> KeywordsInverted { get; set; }

        public Dictionary<int, List<string>> KeywordsT { get; set; }

        public Dictionary<int, List<string>> KeywordsTInverted { get; set; }

        public List<string> StopwordList { get; set; }

        public bool bDebug = true;

        public Crawler(string uri)
        {
            BaseUri = new Uri(uri);
            Web = new HtmlWeb();
            PageProperties = new Dictionary<int, Page>();
            WordProperties = new Dictionary<int, string>();
            LinkChildren = new Dictionary<int, List<int>>();
            LinkParent = new Dictionary<int, List<int>>();
            Keywords = new Dictionary<int, List<string>>();
            KeywordsT = new Dictionary<int, List<string>>();
            KeywordsInverted = new Dictionary<int, List<string>>();
            KeywordsTInverted = new Dictionary<int, List<string>>();
            StopwordList = new List<string>();
        }

        public Page getPageById(int id)
        {
            if (PageProperties.ContainsKey(id))
            {
                return PageProperties[id];
            }
            return null;
        }

        public int getPageId(string url)
        {
            List<KeyValuePair<int, Page>> list = PageProperties.ToList();
            foreach (KeyValuePair<int, Page> pair in list)
            {
                if (pair.Value.Url.Equals(url))
                {
                    return pair.Key;
                }
            }
            return -1;
        }

        public string getWordById(int id)
        {
            if (WordProperties.ContainsKey(id))
            {
                return WordProperties[id];
            }
            return null;
        }

        public int getWordId(string word)
        {
            List<KeyValuePair<int, string>> list = WordProperties.ToList();
            foreach (KeyValuePair<int, string> pair in list)
            {
                if (pair.Value.ToLower().Equals(word.ToLower()))
                {
                    return pair.Key;
                }
            }
            return -1;
        }

        public void loadStopwordList(string fileName)
        {
            //StopwordList = new List<string>();
            try
            {
                string line;
                StreamReader file = File.OpenText(HttpContext.Current.Server.MapPath(fileName));
                while ((line = file.ReadLine()) != null)
                {
                    if (!(line.Trim().Equals(""))){
                        StopwordList.Add(line);
                    }
                }
                file.Close();
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        public Dictionary<int, Page> fetchPages(Uri url, int num, Dictionary<int, Page> pageProperties)
        {
            int count = 0;
            List<string> allUrl = new List<string>();
            allUrl.Add(url.AbsoluteUri);
            while (count < allUrl.Count)
            {
                Page page = new Page(count, allUrl[count]);
                if (getPageById(count) == null ||
                    getPageById(count) != null && !pageProperties[count].LastModified.Equals(page.LastModified))
                {
                    fetchPage(page, StopwordList);
                    if (getPageById(count) == null)
                    {
                        pageProperties.Add(count, page);
                    }
                    else
                    {
                        pageProperties[count] = page;
                    }
                    if (bDebug)
                    {
                        System.Diagnostics.Debug.WriteLine(count + ": " + page.Url);
                    }
                    foreach (string childLink in page.ChildLinks)
                    {
                        if (allUrl.Count < num)
                        {
                            if (!allUrl.Contains(childLink))
                            {
                                allUrl.Add(childLink);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                count++;
            }
            PageProperties = pageProperties;
            return pageProperties;
        }

        public Dictionary<int, List<int>> getLinkChildren(Dictionary<int, Page> pageProperties)
        {
            Dictionary<int, List<int>> linkChildren = new Dictionary<int, List<int>>();
            List<KeyValuePair<int, Page>> list = pageProperties.ToList();
            foreach (KeyValuePair<int, Page> pair in list)
            {
                List<int> childList = new List<int>();
                foreach (string childLink in pair.Value.ChildLinks)
                {
                    int childId = getPageId(childLink);
                    if (childId >= 0)
                    {
                        childList.Add(childId);
                    }
                }
                linkChildren.Add(pair.Key, childList);
            }
            if (bDebug)
            {
                List<KeyValuePair<int, List<int>>> list2 = linkChildren.ToList();
                System.Diagnostics.Debug.WriteLine("LINK CHILDREN");
                foreach (KeyValuePair<int, List<int>> pair in list2)
                {
                    System.Diagnostics.Debug.Write(pair.Key + ": ");
                    foreach (int childId in pair.Value)
                    {
                        System.Diagnostics.Debug.Write(childId + " ");
                    }
                    System.Diagnostics.Debug.WriteLine(null);
                }
                System.Diagnostics.Debug.WriteLine("------------------------------------------");
            }
            LinkChildren = linkChildren;
            return linkChildren;

        }

        public Dictionary<int, List<int>> getLinkParent(Dictionary<int, List<int>> linkChildren)
        {
            Dictionary<int, List <int>> linkParent = new Dictionary<int, List<int>>();
            var keys = linkChildren.Values.SelectMany(v => v).Distinct();
            foreach (var key in keys)
            {
                var vals = linkChildren.Keys.Where(k => linkChildren[k].Contains(key));
                linkParent.Add(key, vals.ToList());
            }
            if (bDebug)
            {
                List<KeyValuePair<int, List<int>>> list3 = linkParent.ToList();
                System.Diagnostics.Debug.WriteLine("LINK PARENT");
                foreach (KeyValuePair<int, List<int>> pair in list3)
                {
                    System.Diagnostics.Debug.Write(pair.Key + ": ");
                    foreach (int parentId in pair.Value)
                    {
                        System.Diagnostics.Debug.Write(parentId + " ");
                    }
                    System.Diagnostics.Debug.WriteLine(null);
                }
                System.Diagnostics.Debug.WriteLine("------------------------------------------");
            }
            LinkParent = linkParent;
            return linkParent;
        }

        public Dictionary<int, List<string>> getKeywords(Dictionary<int, Page> pageProperties)
        {
            Dictionary<int, List<string>> keywords = new Dictionary<int, List<string>>();
            List<KeyValuePair<int, Page>> list = pageProperties.ToList();
            foreach (KeyValuePair<int, Page> pair in list)
            { 
                keywords.Add(pair.Key, pair.Value.WordList);
            }
            if (bDebug)
            {
                List<KeyValuePair<int, List<string>>> list2 = keywords.ToList();
                System.Diagnostics.Debug.WriteLine("KEYWORDS");
                foreach (KeyValuePair<int, List<string>> pair in list2)
                {
                    System.Diagnostics.Debug.Write(pair.Key + ": ");
                    foreach (string word in pair.Value)
                    {
                        System.Diagnostics.Debug.Write(word + " ");
                    }
                    System.Diagnostics.Debug.WriteLine(null);
                }
                System.Diagnostics.Debug.WriteLine("------------------------------------------");
            }
            Keywords = keywords;
            return keywords;
        }

        public Dictionary<int, List<string>> getKeywordsInverted(Dictionary<int, List<string>> keywords)
        {
            Dictionary<int, List<string>> keywordsInverted = new Dictionary<int, List<string>>();
            var keys = keywords.Values.SelectMany(v => v).Distinct(StringComparer.CurrentCultureIgnoreCase);
            foreach (var key in keys)
            {
                //System.Diagnostics.Debug.WriteLine(getWordId(key));
                var vals = keywords.Keys.Where(k => keywords[k].Contains(key, StringComparer.CurrentCultureIgnoreCase));
                foreach (int pageId in vals)
                {
                    if (!keywordsInverted.ContainsKey(getWordId(key)))
                    {
                        keywordsInverted.Add(getWordId(key), getPageById(pageId).getKeywordDetails(key));
                    }
                    else
                    {
                        keywordsInverted[getWordId(key)].AddRange(getPageById(pageId).getKeywordDetails(key));
                    }
                    //System.Diagnostics.Debug.Write(getPageById(pageId).Title + "; ");//getPageById(pageId).
                }
                //System.Diagnostics.Debug.WriteLine(null);
                //keywordsInverted.Add(getWordId(key), null);
            }
            if (bDebug)
            {
                List<KeyValuePair<int, List<string>>> list2 = keywordsInverted.ToList();
                System.Diagnostics.Debug.WriteLine("KEYWORDS INVERTED");
                foreach (KeyValuePair<int, List<string>> pair in list2)
                {
                    System.Diagnostics.Debug.Write(pair.Key + ": ");
                    foreach (string word in pair.Value)
                    {
                        System.Diagnostics.Debug.Write(word + " ");
                    }
                    System.Diagnostics.Debug.WriteLine(null);
                }
                System.Diagnostics.Debug.WriteLine("------------------------------------------");
            }
            KeywordsInverted = keywordsInverted;
            return keywordsInverted;
        }

        public Dictionary<int, List<string>> getKeywordsT(Dictionary<int, Page> pageProperties)
        {
            Dictionary<int, List<string>> keywordsT = new Dictionary<int, List<string>>();
            List<KeyValuePair<int, Page>> list = pageProperties.ToList();
            foreach (KeyValuePair<int, Page> pair in list)
            {
                keywordsT.Add(pair.Key, pair.Value.WordTList);
            }
            if (bDebug)
            {
                List<KeyValuePair<int, List<string>>> list2 = keywordsT.ToList();
                System.Diagnostics.Debug.WriteLine("KEYWORDS");
                foreach (KeyValuePair<int, List<string>> pair in list2)
                {
                    System.Diagnostics.Debug.Write(pair.Key + ": ");
                    foreach (string word in pair.Value)
                    {
                        System.Diagnostics.Debug.Write(word + " ");
                    }
                    System.Diagnostics.Debug.WriteLine(null);
                }
                System.Diagnostics.Debug.WriteLine("------------------------------------------");
            }
            KeywordsT = keywordsT;
            return keywordsT;
        }

        public Dictionary<int, List<string>> getKeywordsTInverted(Dictionary<int, List<string>> keywordsT)
        {
            Dictionary<int, List<string>> keywordsTInverted = new Dictionary<int, List<string>>();
            var keys = keywordsT.Values.SelectMany(v => v).Distinct(StringComparer.CurrentCultureIgnoreCase);
            foreach (var key in keys)
            {
                //System.Diagnostics.Debug.WriteLine(getWordId(key));
                var vals = keywordsT.Keys.Where(k => keywordsT[k].Contains(key, StringComparer.CurrentCultureIgnoreCase));
                foreach (int pageId in vals)
                {
                    if (!keywordsTInverted.ContainsKey(getWordId(key)))
                    {
                        keywordsTInverted.Add(getWordId(key), getPageById(pageId).getKeywordDetails(key));
                    }
                    else
                    {
                        keywordsTInverted[getWordId(key)].AddRange(getPageById(pageId).getKeywordDetails(key));
                    }
                    //System.Diagnostics.Debug.Write(getPageById(pageId).Title + "; ");//getPageById(pageId).
                }
                //System.Diagnostics.Debug.WriteLine(null);
                //keywordsInverted.Add(getWordId(key), null);
            }
            if (bDebug)
            {
                List<KeyValuePair<int, List<string>>> list2 = keywordsTInverted.ToList();
                System.Diagnostics.Debug.WriteLine("KEYWORDS T INVERTED");
                foreach (KeyValuePair<int, List<string>> pair in list2)
                {
                    System.Diagnostics.Debug.Write(pair.Key + ": ");
                    foreach (string word in pair.Value)
                    {
                        System.Diagnostics.Debug.Write(word + " ");
                    }
                    System.Diagnostics.Debug.WriteLine(null);
                }
                System.Diagnostics.Debug.WriteLine("------------------------------------------");
            }
            KeywordsTInverted = keywordsTInverted;
            return keywordsTInverted;
        }

        /*
        public void printAllPages(Label abc, Dictionary<string, Page> allPages)
        {
            List<KeyValuePair<string, Page>> list = allPages.ToList();
            foreach (KeyValuePair<string, Page> pair in list)
            {
                abc.Text += pair.Key+"  "+pair.Value.Id+"<br>";
            }
        }

        public void printLinkRelation(Label def, Dictionary<string, Page> allPages)
        {
            List<KeyValuePair<string, Page>> list = allPages.ToList();
            foreach (KeyValuePair<string, Page> pair in list)
            {
                def.Text += pair.Value.Id + ": ";
                foreach (string childLink in pair.Value.ChildLinks)
                {
                    if (allPages.ContainsKey(childLink))
                    {
                        def.Text += allPages[childLink].Id + " ";
                    }
                }
                def.Text += "<br>";
            }
        }
        */
        public void fetchPage(Page page, List<string> stopword)
        {
            page.Doc = new HtmlWeb().Load(page.Url);
            page.Doc.OptionWriteEmptyNodes = true;
            //HtmlNode.ElementsFlags["br"] = HtmlElementFlag.Empty;
            HtmlNode titleNode = page.Doc.DocumentNode.SelectSingleNode("//head/title");
            if (titleNode != null)
            {
                page.Title = titleNode.InnerText;
            }
            page.ChildLinks = extractChildLinks(page);
            page.WordTList = extractKeywordsT(page, stopword);
            page.WordList = extractKeywords(page, stopword);
        }

        public List<string> extractChildLinks(Page page)
        {
            List<string> childList = new List<string>();
            HtmlNodeCollection linkNodes = page.Doc.DocumentNode.SelectNodes("//a[@href]");
            if (linkNodes != null)
            {
                foreach (HtmlNode link in linkNodes)
                {
                    HtmlAttribute att = link.Attributes["href"];
                    string childLink = new Uri(new Uri(page.Url), att.Value).AbsoluteUri;
                    if (!childList.Contains(childLink))
                    {
                        childList.Add(childLink);
                    }
                }
            }
            return childList;
        }

        public List<string> extractKeywords(Page page, List<string> stopword)
        {
            List<string> wordList = new List<string>();
            List<string> wordList2 = new List<string>();
            HtmlNodeCollection textNodes = page.Doc.DocumentNode.SelectNodes("//body//*[not(self::script)]/text() | //body//*[not(self::script)]/following-sibling::text() | //body//*[not(self::script)]/preceding-sibling::text()");
            Stemmer stemmer = new Stemmer();
            if (textNodes != null)
            {
                foreach (HtmlNode text in textNodes)
                {
                    Regex rgx = new Regex("\\s+\n+|[^\\w+]");
                    string words = rgx.Replace(text.InnerText, " ").Trim();
                    if (!words.Equals(""))
                    {
                        //System.Diagnostics.Debug.WriteLine(">>>"+words);
                        List<string> wordArr = words.Split(' ').Where(wrd => (!stopword.Contains(wrd.ToLower()) && !wrd.Trim().Equals(""))).ToList();
                        wordList2.AddRange(wordArr);
                        foreach (string s in wordArr)
                        {
                            stemmer.add(s.ToCharArray(), s.Length);
                            stemmer.stem();
                            string finalWord = stemmer.ToString().ToLower();
                            wordList.Add(finalWord);
                            if (getWordId(finalWord) < 0)
                            {
                                if (bDebug)
                                {
                                    System.Diagnostics.Debug.WriteLine(WordProperties.Count + ": " + finalWord);
                                }
                                WordProperties.Add(WordProperties.Count, finalWord);
                            }
                        }
                    }
                }
                string keywords = string.Join(" ", wordList);
                //System.Diagnostics.Debug.WriteLine(string.Join(" ", wordList2));
                //System.Diagnostics.Debug.WriteLine(keywords);
            }
            return wordList;
        }

        public List<string> extractKeywordsT(Page page, List<string> stopword)
        {
            List<string> wordList = new List<string>();
            Stemmer stemmer = new Stemmer();
            Regex rgx = new Regex("\\s+\n+|[^\\w+]");
            string words = rgx.Replace(page.Title, " ").Trim();
            if (!words.Equals(""))
            {
                List<string> wordArr = words.Split(' ').Where(wrd => (!stopword.Contains(wrd.ToLower()) && !wrd.Trim().Equals(""))).ToList();
                foreach (string s in wordArr)
                {
                    stemmer.add(s.ToCharArray(), s.Length);
                    stemmer.stem();
                    string finalWord = stemmer.ToString().ToLower();
                    wordList.Add(finalWord);
                    if (getWordId(finalWord) < 0)
                    {
                        if (bDebug)
                        {
                            System.Diagnostics.Debug.WriteLine(WordProperties.Count + ": " + finalWord);
                        }
                        WordProperties.Add(WordProperties.Count, finalWord);
                    }
                }
            }
            return wordList;
        }

        public List<int> getLinkListById(Dictionary<int, List<int>> linkRelation, int id)
        {
            if (linkRelation.ContainsKey(id))
            {
                return linkRelation[id];
            }
            return null;
        }

        public void loadTableFromDB()
        {
            String conn = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            Database db = new Database(conn);
            LinkParent = db.obtainDictFromTable(1);
            LinkChildren = db.obtainDictFromTable(0);
            Keywords = db.obtainDictFromTableListString(3);
            KeywordsT = db.obtainDictFromTableListString(5);
            KeywordsInverted = db.obtainDictFromTableListString(4);
            KeywordsTInverted = db.obtainDictFromTableListString(6);
            WordProperties = db.obtainDictFromTableString(2);
            PageProperties = db.obtainDictFromPage();

            List<KeyValuePair<int, Page>> pagePropertiesList = PageProperties.ToList();
            foreach (KeyValuePair<int, Page> pageProperty in pagePropertiesList)
            {
                List<string> childList = new List<string>();
                foreach (int pageId in LinkChildren[pageProperty.Key])
                {
                    childList.Add(getPageById(pageId).Url);
                }
                pageProperty.Value.ChildLinks = childList;

                List<string> keywords = new List<string>();
                foreach (string keyword in Keywords[pageProperty.Key])
                {
                    if (keyword.Trim().Length > 0) { 
                        keywords.Add(keyword);
                    }
                }
                pageProperty.Value.WordList = keywords;

                List<string> keywordsT = new List<string>();
                foreach (string keywordT in KeywordsT[pageProperty.Key])
                {
                    if (keywordT.Trim().Length > 0)
                    {
                        keywordsT.Add(keywordT);
                    }
                }
                pageProperty.Value.WordTList = keywordsT;
            }

            
        }

        public void updateTableIntoDB()
        {
            String conn = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            Database db = new Database(conn);
            db.insertDictToTable(LinkParent, 1);
            db.insertDictToTable(LinkChildren, 0);
            db.insertDictToTable(Keywords, 3);
            db.insertDictToTable(KeywordsT, 5);
            db.insertDictToTable(KeywordsInverted, 4);
            db.insertDictToTable(KeywordsTInverted, 6);
            db.insertDictToTable(WordProperties, 2);
            db.insertDictToPage(PageProperties, 7);
            
        }

        public void printCrawlerResult(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(HttpContext.Current.Server.MapPath(fileName)))
            {
                List<KeyValuePair<int, Page>> pagePropertiesList = PageProperties.ToList();
                foreach (KeyValuePair<int, Page> pageProperty in pagePropertiesList)
                {
                    writer.WriteLine(pageProperty.Value.Title);
                    writer.WriteLine(pageProperty.Value.Url);
                    writer.WriteLine(pageProperty.Value.LastModified + ", " + pageProperty.Value.ContentLength);
                    List<string> printedKeywords = new List<string>();
                    foreach (string keywordT in KeywordsT[pageProperty.Key])
                    {
                        if (!printedKeywords.Contains(keywordT, StringComparer.CurrentCultureIgnoreCase) && keywordT.Trim().Length > 0)
                        {
                            writer.Write(keywordT + " " + pageProperty.Value.getKeywordFrequency(keywordT) + "; ");
                            printedKeywords.Add(keywordT);
                        }
                    }
                    foreach (string keyword in Keywords[pageProperty.Key])
                    {
                        if (!printedKeywords.Contains(keyword, StringComparer.CurrentCultureIgnoreCase) && keyword.Trim().Length > 0) { 
                            writer.Write(keyword + " " + pageProperty.Value.getKeywordFrequency(keyword) + "; ");
                            printedKeywords.Add(keyword);
                        }
                    }
                    writer.WriteLine("");
                    foreach (int linkChild in getLinkListById(LinkChildren, pageProperty.Key))
                    {
                        writer.WriteLine(PageProperties[linkChild].Url);
                    }
                    writer.WriteLine("-------------------------------------------------------------------------------------------");
                }
            }
        }
    }
}
 