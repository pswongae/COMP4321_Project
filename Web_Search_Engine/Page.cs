using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

namespace Web_Search_Engine
{
    public class Page
    {
        public int Id { get; set; }

        public string Url { get; set; } //

        public HtmlDocument Doc { get; set; }

        public string LastModified { get; set; } //

        public int ContentLength { get; set; } //

        public string Title { get; set; } //

        // public string AllText { get; set; }

        public List<string> WordList { get; set; }

        public List<string> WordTList { get; set; }

        // public List<string> ParentLinks { get; set; }

        public List<string> ChildLinks { get; set; }

        public Page(int id, string url)
        {
            Id = id;
            Url = url;
            updateHeaderInfo();
        }

        public Page(int id, string url, string lastModified, int contentLength, string title)
        {
            Id = id;
            Url = url;
            LastModified = lastModified;
            ContentLength = contentLength;
            Title = title;
        }
        public void updateHeaderInfo()
        {
            string[] headerInfo = getHeaderInfo(new Uri(Url));
            LastModified = headerInfo[0];
            ContentLength = int.Parse(headerInfo[1]);
            //System.Diagnostics.Debug.WriteLine(Url);
            //System.Diagnostics.Debug.WriteLine(LastModified);
        }
        /*
        public void fetch()
        {
            Doc = new HtmlWeb().Load(Url);
            HtmlNode titleNode = Doc.DocumentNode.SelectSingleNode("//head/title");
            if (titleNode != null)
            {
                Title = titleNode.InnerText;
            }
            // ParentLinks = new List<string>();
            ChildLinks = getChildList(new Uri(Url));
            // AllText = getAllText(Url);
            WordList = getWords(Url);
        }
        */
        public string[] getHeaderInfo(Uri url)
        {
            string[] headerInfo = new string[2];
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Last-Modified
                if (response.Headers.GetValues("Last-Modified") != null)
                {
                    headerInfo[0] = response.Headers.GetValues("Last-Modified")[0];
                }
                else
                {
                    headerInfo[0] = response.Headers.GetValues("Date")[0];
                }

                // Content-Length
                if (response.Headers.GetValues("Content-Length") != null)
                {
                    headerInfo[1] = response.Headers.GetValues("Content-Length")[0];
                }
                else
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string htmlText = reader.ReadToEnd();
                    headerInfo[1] = htmlText.Length + "";
                }
                response.Close();
            }
            catch (WebException ex)
            {
                headerInfo[0] = null;
                headerInfo[1] = "-1";
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            return headerInfo;
        }

        public List<string> getKeywordDetails(string keyword)
        {
            List<string> keywordDetails = new List<string>();
            for (int i = 0; i < WordTList.Count; i++)
            {
                if (WordTList[i].ToLower().Equals(keyword.ToLower()))
                {
                    keywordDetails.Add(Id + "," + i);
                }
            }
            for (int i = 0; i < WordList.Count; i++)
            {
                if (WordList[i].ToLower().Equals(keyword.ToLower()))
                {
                    keywordDetails.Add(Id + "," + i);
                }
            }
            return keywordDetails;
        }

        public int getKeywordFrequency(string keyword)
        {
            int count = 0;
            for (int i = 0; i < WordTList.Count; i++)
            {
                if (WordTList[i].ToLower().Equals(keyword.ToLower()))
                {
                    count++;
                }
            }
            for (int i = 0; i < WordList.Count; i++)
            {
                if (WordList[i].ToLower().Equals(keyword.ToLower()))
                {
                    count++;
                }
            }
            return count;
        }

        public List<Tuple<string, int>> getFreqWords(int num)
        {
            List<string> wordList = WordTList.Concat(WordList).OrderByDescending(w => getKeywordFrequency(w)).ThenBy(w => w).Distinct().ToList();
            List<Tuple<string, int>> freqWords = new List<Tuple<string, int>>();
            for (int i = 0; i < num && i < wordList.Count; i++)
            {
                freqWords.Add(new Tuple<string, int>(wordList[i], getKeywordFrequency(wordList[i])));
            }
            return freqWords;
        }

        /*
        public List<string> getChildList(Uri url)
        {
            List<string> childList = new List<string>();
            HtmlNodeCollection linkNodes = Doc.DocumentNode.SelectNodes("//a[@href]");
            if (linkNodes != null)
            {
                foreach (HtmlNode link in linkNodes)
                {
                    HtmlAttribute att = link.Attributes["href"];
                    string page = new Uri(url, att.Value).AbsoluteUri;
                    if (!childList.Contains(page))
                    {
                        childList.Add(page);
                    }
                }
            }
            return childList;
        }

        public List<string> getWords(string url)
        {
            List<string> wordList = new List<string>();
            HtmlNodeCollection textNodes = Doc.DocumentNode.SelectNodes("//body//*[not(self::script)]/text()");
            char[] delimiters = new char[] { ' ', '\n' };
            if (textNodes != null)
            {
                foreach (HtmlNode text in textNodes)
                {
                    Regex rgx = new Regex("\\s+\n+|[^\\w+]");
                    string words = rgx.Replace(text.InnerText, " ").Trim();
                    if (!words.Equals(""))
                    {
                        wordList.Add(words);
                    }
                }
                System.Diagnostics.Debug.WriteLine(string.Join(" ", wordList));
            }
            return wordList;
        }

        public string getAllText(string url)
        {
            string htmlText = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                htmlText = reader.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(htmlText);
                response.Close();
            }
            catch (WebException)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
            return htmlText;
        }
        */
    }
}