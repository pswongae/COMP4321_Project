using porter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Search_Engine
{
    public class SearchEngine
    {
        public string QueryTerm { get; set; }

        public Crawler Crawler { get; set; }

        public Dictionary<int, Tuple<double, bool>> ScoreTable { get; set; }

        public SearchEngine(string queryTerm, Crawler crawler)
        {
            QueryTerm = queryTerm;
            Crawler = crawler;
        }

        public void search()
        {
            ScoreTable = new Dictionary<int, Tuple<double, bool>>();
            // stem query
            Stemmer stemmer = new Stemmer();
            List<string> terms = QueryTerm.Split(' ').Where(wrd => (!Crawler.StopwordList.Contains(wrd.ToLower()) && !wrd.Trim().Equals(""))).ToList();
            List<string> queryTerms = new List<string>();
            Dictionary<int, Dictionary<int, List<int>>> pageWordTable = new Dictionary<int, Dictionary<int, List<int>>>();
            Dictionary<int, List<int>> dfTable = new Dictionary<int, List<int>>();
            for (int i = 0; i < terms.Count; i++)
            {
                stemmer.add(terms[i].ToCharArray(), terms[i].Length);
                stemmer.stem();
                string word = stemmer.ToString().ToLower();
                if (!queryTerms.Contains(word) && Crawler.getWordId(word) >= 0)
                {
                    queryTerms.Add(word);
                }
            }
            foreach (string word in queryTerms)
            {
                int wordIndex = Crawler.getWordId(word);
                System.Diagnostics.Debug.WriteLine(word + " " + wordIndex);
                if (wordIndex >= 0)
                {
                    List<string> allPagesInfo = new List<string>();
                    if (Crawler.KeywordsTInverted.ContainsKey(wordIndex))
                    {
                        allPagesInfo.AddRange(Crawler.KeywordsTInverted[wordIndex]);
                    }
                    if (Crawler.KeywordsInverted.ContainsKey(wordIndex))
                    {
                        allPagesInfo.AddRange(Crawler.KeywordsInverted[wordIndex]);
                    }
                    foreach (string pagesInfo in allPagesInfo)
                    {
                        string[] pageInfo = pagesInfo.Split(',');
                        int pageIndex = Int32.Parse(pageInfo[0]);
                        int wordPos = Int32.Parse(pageInfo[1]);
                        System.Diagnostics.Debug.WriteLine(pageInfo[0] + " " + pageInfo[1]);

                        if (!pageWordTable.ContainsKey(pageIndex))
                        {
                            pageWordTable.Add(pageIndex, new Dictionary<int, List<int>>());
                        }
                        if (!pageWordTable[pageIndex].ContainsKey(wordIndex))
                        {
                            pageWordTable[pageIndex].Add(wordIndex, new List<int>());
                        }
                        pageWordTable[pageIndex][wordIndex].Add(wordPos);

                        if (!dfTable.ContainsKey(wordIndex))
                        {
                            dfTable.Add(wordIndex, new List<int>());
                        }
                        if (!dfTable[wordIndex].Contains(pageIndex))
                        {
                            dfTable[wordIndex].Add(pageIndex);
                        } 
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("######");
            List<KeyValuePair<int, Dictionary<int, List<int>>>> pageWordList = pageWordTable.ToList();
            foreach (KeyValuePair<int, Dictionary<int, List<int>>> pair in pageWordList)
            {
                System.Diagnostics.Debug.WriteLine(pair.Key + " : ");
                List<KeyValuePair<int, List<int>>> pageWordSubList = pair.Value.ToList();
                int maxtf = 0;
                foreach (KeyValuePair<int, List<int>> pair2 in pageWordSubList)
                {
                    if (pair2.Value.Count > maxtf)
                    {
                        maxtf = pair2.Value.Count;
                    }
                }
                double sum_dq = 0, sum_dq2 = 0;
                foreach (KeyValuePair<int, List<int>> pair2 in pageWordSubList)
                {
                    int tf = pair2.Value.Count;
                    int df = dfTable[pair2.Key].Count;
                    double idf = Math.Log(((double) Crawler.PageProperties.Count / df), 2);
                    sum_dq += tf * idf / maxtf;
                    sum_dq2 += tf * idf;
                    System.Diagnostics.Debug.WriteLine("\t" + pair2.Key + " : " + pair2.Value.Count + " [ "+tf+"("+maxtf+"),"+df+","+idf+"; "+ 
                        Crawler.PageProperties.Count+","+ ((double)Crawler.PageProperties.Count / df)+" ]");
                }
                System.Diagnostics.Debug.WriteLine("sum_dq = " + sum_dq + " [without maxtf: "+(sum_dq2)+"]");
                // 
                
                double sum_dsq = 0, sum_dsq2 = 0;
                int maxtf2 = 0;
                foreach (string word in Crawler.KeywordsT[pair.Key].Concat(Crawler.Keywords[pair.Key]))
                {
                    int wordFreq = Crawler.PageProperties[pair.Key].getKeywordFrequency(word);
                    if (wordFreq > maxtf2)
                    {
                        maxtf2 = wordFreq;
                    }
                }
                List<string> checkedWords = new List<string>();
                foreach (string word in Crawler.KeywordsT[pair.Key].Concat(Crawler.Keywords[pair.Key]))
                {
                    if (!checkedWords.Contains(word))
                    {
                        //System.Diagnostics.Debug.WriteLine(word);
                        int wordFreq = Crawler.PageProperties[pair.Key].getKeywordFrequency(word);
                        List<int> temp = new List<int>();
                        List<string> allPagesInfo2 = new List<string>();
                        if (Crawler.KeywordsTInverted.ContainsKey(Crawler.getWordId(word)))
                        {
                            allPagesInfo2.AddRange(Crawler.KeywordsTInverted[Crawler.getWordId(word)]);
                        }
                        if (Crawler.KeywordsInverted.ContainsKey(Crawler.getWordId(word)))
                        {
                            allPagesInfo2.AddRange(Crawler.KeywordsInverted[Crawler.getWordId(word)]);
                        }
                        foreach (string pagesInfo in allPagesInfo2)
                        {
                            string[] pageInfo = pagesInfo.Split(',');
                            int pageIndex = Int32.Parse(pageInfo[0]);
                            int wordPos = Int32.Parse(pageInfo[1]);
                            if (!temp.Contains(pageIndex))
                            {
                                temp.Add(pageIndex);
                            }
                        }
                        int tf = wordFreq;
                        int df = temp.Count;
                        double idf = Math.Log(((double)Crawler.PageProperties.Count / df), 2);
                        sum_dsq += Math.Pow(tf * idf / maxtf2, 2);
                        sum_dsq2 += Math.Pow(tf * idf, 2);
                        //System.Diagnostics.Debug.WriteLine(tf+" "+df+" "+idf+" "+Math.Pow(tf * idf, 2));
                        //System.Diagnostics.Debug.WriteLine(word + " " + wordFreq + "(" + maxtf2+ ") " + temp.Count);
                        checkedWords.Add(word);
                    }
                }
                System.Diagnostics.Debug.WriteLine("sum_dsq = "+sum_dsq+" [without maxtf: "+sum_dsq2+"]");
                double sum_qsq = queryTerms.Count;
                System.Diagnostics.Debug.WriteLine("sum_qsq = " + sum_qsq);
                double cosineSim = sum_dq / (Math.Sqrt(sum_dsq) * Math.Sqrt(sum_qsq));
                double cosineSim2 = sum_dq2 / (Math.Sqrt(sum_dsq2) * Math.Sqrt(sum_qsq));
                System.Diagnostics.Debug.WriteLine("cosineSim = " + cosineSim + " [without maxtf: " + cosineSim2 + "]");
                bool inTitle = Crawler.KeywordsT[pair.Key].Any(x => queryTerms.Any(y => y == x));
                System.Diagnostics.Debug.WriteLine(inTitle);
                if (inTitle)
                {
                    cosineSim *= 1.5;
                }
                ScoreTable.Add(pair.Key, new Tuple<double, bool>(cosineSim, inTitle));
            }
        }
    }
}