using porter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            Crawler = crawler;/*
            foreach (List<string> ls in getStemmedQueryTerms(QueryTerm))
            {

                foreach (string s in ls)
                {
                    System.Diagnostics.Debug.Write(s + ", ");
                }
                System.Diagnostics.Debug.WriteLine("");
            }
                foreach (List<string> ls in getStemmedQueryTerms(QueryTerm))
            {
                if (ls.Count > 1)
                {
                    getPageWordTableForPhrase(ls);
                }
            }*/
        }

        public void search()
        { 

            ScoreTable = new Dictionary<int, Tuple<double, bool>>();

            Dictionary<int, Dictionary<int, List<int>>> pageWordTable = new Dictionary<int, Dictionary<int, List<int>>>();
            Dictionary<int, List<int>> dfTable = new Dictionary<int, List<int>>();
            // stem query
            /*Stemmer stemmer = new Stemmer();
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
            }*/
            List<List<string>> query = getStemmedQueryTerms(QueryTerm);
            foreach (List<string> ls in query)
            {
                var tuple = getPageWordDfTableForPhrase(ls);
                List<KeyValuePair<int, Dictionary<int, List<int>>>> pwList = tuple.Item1.ToList();
                foreach (KeyValuePair<int, Dictionary<int, List<int>>> pwPair in pwList){
                    if (!pageWordTable.ContainsKey(pwPair.Key))
                    {
                        pageWordTable.Add(pwPair.Key, pwPair.Value);
                    }
                    else
                    {
                        List<KeyValuePair<int, List<int>>> pwSubList = pwPair.Value.ToList();
                        foreach (KeyValuePair<int, List<int>> pwSubPair in pwSubList)
                        {
                            if (!pageWordTable[pwPair.Key].ContainsKey(pwSubPair.Key))
                            {
                                pageWordTable[pwPair.Key].Add(pwSubPair.Key, pwSubPair.Value);
                            }
                        }
                    }
                }
                List<KeyValuePair<int, List<int>>> dfList = tuple.Item2.ToList();
                foreach (KeyValuePair<int, List<int>> dfPair in dfList)
                {
                    if (!dfTable.ContainsKey(dfPair.Key))
                    {
                        dfTable.Add(dfPair.Key, dfPair.Value);
                    }
                    else
                    {
                        dfTable[dfPair.Key].Union(dfPair.Value);
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("######");
            List<KeyValuePair<int, Dictionary<int, List<int>>>> pageWordList = pageWordTable.ToList();
            foreach (KeyValuePair<int, Dictionary<int, List<int>>> pair in pageWordList)
            {
                System.Diagnostics.Debug.WriteLine(pair.Key + " : ");
                List<KeyValuePair<int, List<int>>> pageWordSubList = pair.Value.ToList();
                /*int maxtf = 0;
                foreach (KeyValuePair<int, List<int>> pair2 in pageWordSubList)
                {
                    if (pair2.Value.Count > maxtf)
                    {
                        maxtf = pair2.Value.Count;
                    }
                }*/
                int maxtf = Crawler.getPageById(pair.Key).getFreqWords(1)[0].Item2;
                double sum_dq = 0, sum_dq2 = 0;
                foreach (KeyValuePair<int, List<int>> pair2 in pageWordSubList)
                {
                    int tf = Crawler.getPageById(pair.Key).getKeywordFrequency(Crawler.getWordById(pair2.Key));//pair2.Value.Count;
                    int df = dfTable[pair2.Key].Count;
                    double idf = Math.Log(((double) Crawler.PageProperties.Count / df), 2);
                    sum_dq += (double)tf * idf / maxtf;
                    sum_dq2 += (double)tf * idf;
                    System.Diagnostics.Debug.WriteLine("\t" + pair2.Key + " : " + pair2.Value.Count + " [ "+tf+"("+maxtf+"),"+df+","+idf+"; "+ 
                        Crawler.PageProperties.Count+","+ ((double)Crawler.PageProperties.Count / df)+" ]");
                }
                System.Diagnostics.Debug.WriteLine("sum_dq = " + sum_dq + " [without maxtf: "+(sum_dq2)+"]");
                // 
                
                double sum_dsq = 0, sum_dsq2 = 0;
                /*int maxtf2 = 0;
                foreach (string word in Crawler.KeywordsT[pair.Key].Concat(Crawler.Keywords[pair.Key]))
                {
                    int wordFreq = Crawler.PageProperties[pair.Key].getKeywordFrequency(word);
                    if (wordFreq > maxtf2)
                    {
                        maxtf2 = wordFreq;
                    }
                }*/
                List<string> checkedWords = new List<string>();
                foreach (string word in Crawler.KeywordsT[pair.Key].Concat(Crawler.Keywords[pair.Key]))
                {
                    if (!checkedWords.Contains(word))
                    {
                        //System.Diagnostics.Debug.WriteLine(word);
                        int wordFreq = Crawler.PageProperties[pair.Key].getKeywordFrequency(word);
                        List<int> temp = new List<int>();
                        List<string> allPagesInfo2 = new List<string>();
                        int wordId = Crawler.getWordId(word);
                        if (Crawler.KeywordsTInverted.ContainsKey(wordId))
                        {
                            allPagesInfo2.AddRange(Crawler.KeywordsTInverted[wordId]);
                        }
                        if (Crawler.KeywordsInverted.ContainsKey(wordId))
                        {
                            allPagesInfo2.AddRange(Crawler.KeywordsInverted[wordId]);
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
                        sum_dsq += Math.Pow(tf * idf / maxtf, 2); // old: maxtf2
                        sum_dsq2 += Math.Pow(tf * idf, 2);
                        //System.Diagnostics.Debug.WriteLine(tf+" "+df+" "+idf+" "+Math.Pow(tf * idf, 2));
                        //System.Diagnostics.Debug.WriteLine(word + " " + wordFreq + "(" + maxtf2+ ") " + temp.Count);
                        checkedWords.Add(word);
                    }
                }
                System.Diagnostics.Debug.WriteLine("sum_dsq = "+sum_dsq+" [without maxtf: "+sum_dsq2+"]");
                double sum_qsq = 0; //query.Count; //double sum_qsq = queryTerms.Count;
                List<string> tempQ = new List<string>();
                foreach (List<string> ls in query)
                {
                    foreach (string s in ls)
                    {
                        if (!tempQ.Contains(s))
                        {
                            tempQ.Add(s);
                        }
                    }
                }
                sum_qsq = tempQ.Count;
                System.Diagnostics.Debug.WriteLine("sum_qsq = " + sum_qsq);
                double cosineSim = sum_dq / (Math.Sqrt(sum_dsq) * Math.Sqrt(sum_qsq));
                double cosineSim2 = sum_dq2 / (Math.Sqrt(sum_dsq2) * Math.Sqrt(sum_qsq));
                System.Diagnostics.Debug.WriteLine("cosineSim = " + cosineSim + " [without maxtf: " + cosineSim2 + "]");
                bool inTitle = false;//bool inTitle = Crawler.KeywordsT[pair.Key].Any(x => queryTerms.Any(y => y == x));
                foreach (List<string> str in query)
                {
                    foreach (string sub in str)
                    {
                        if (Crawler.KeywordsT[pair.Key].Contains(sub))
                        {
                            inTitle = true;
                        }
                    }
                }
                    
                System.Diagnostics.Debug.WriteLine(inTitle);
                if (inTitle)
                {
                    cosineSim *= 1.5;
                }
                ScoreTable.Add(pair.Key, new Tuple<double, bool>(cosineSim, inTitle));
            }
        }
        
        public Tuple<Dictionary<int, Dictionary<int, List<int>>>, Dictionary<int, List<int>>> getPageWordDfTableForPhrase(List<string> terms)
        {
            List<int> keys = new List<int>();
            Dictionary<int, Dictionary<int, List<int>>> pageWordTable = new Dictionary<int, Dictionary<int, List<int>>>();
            Dictionary<int, List<int>> dfTable = new Dictionary<int, List<int>>();
            foreach (string term in terms)
            {
                System.Diagnostics.Debug.WriteLine(term);
                int wordId = Crawler.getWordId(term);
                if (wordId < 0)
                {
                    break;
                }
                List<string> allPagesInfo = new List<string>();
                if (Crawler.KeywordsTInverted.ContainsKey(wordId))
                {
                    allPagesInfo.AddRange(Crawler.KeywordsTInverted[wordId]);
                }
                if (Crawler.KeywordsInverted.ContainsKey(wordId))
                {
                    allPagesInfo.AddRange(Crawler.KeywordsInverted[wordId]);
                }
                /*if (Crawler.KeywordsInverted.ContainsKey(wordId))
                {
                    allPagesInfo = Crawler.KeywordsInverted[wordId];
                }*/
                foreach (string allInfo in allPagesInfo)
                {
                    string[] pageInfo = allInfo.Split(',');
                    int pageIndex = Int32.Parse(pageInfo[0]);
                    int wordPos = Int32.Parse(pageInfo[1]);

                    if (!pageWordTable.ContainsKey(pageIndex))
                    {
                        pageWordTable.Add(pageIndex, new Dictionary<int, List<int>>());
                    }
                    if (!pageWordTable[pageIndex].ContainsKey(wordId))
                    {
                        pageWordTable[pageIndex].Add(wordId, new List<int>());
                    }
                    if (!pageWordTable[pageIndex][wordId].Contains(wordPos))
                    {
                        pageWordTable[pageIndex][wordId].Add(wordPos);
                    }

                    if (!dfTable.ContainsKey(wordId))
                    {
                        dfTable.Add(wordId, new List<int>());
                    }
                    if (!dfTable[wordId].Contains(pageIndex))
                    {
                        dfTable[wordId].Add(pageIndex);
                    }
                }
            }
            List<KeyValuePair<int, Dictionary<int, List<int>>>> rpage = pageWordTable.ToList();
            foreach (KeyValuePair<int, Dictionary<int, List<int>>> pair in rpage)
            {
                SortedDictionary<int, string> wordPosList = new SortedDictionary<int, string>();
                List<KeyValuePair<int, List<int>>> posList = pair.Value.ToList();
                foreach (KeyValuePair<int, List<int>> posPair in posList)
                {
                    foreach (int pos in posPair.Value)
                    {
                        if (!wordPosList.ContainsKey(pos)) { 
                            wordPosList.Add(pos, Crawler.getWordById(posPair.Key));
                        }
                    }
                }
                SortedDictionary<int, string>.KeyCollection keyColl = wordPosList.Keys;
                SortedDictionary<int, string>.ValueCollection valueColl = wordPosList.Values;
                for (int i=0; i<valueColl.ToList().Count-terms.Count+1; i++)
                {
                    int startPos = keyColl.ToList()[i];
                    int k = 0;
                    bool valid = true;
                    for (int j=i; j<i+terms.Count; j++, k++)
                    {
                        System.Diagnostics.Debug.WriteLine(i+" "+j);
                        if (!(valueColl.ToList()[j].Equals(terms[k]) && keyColl.ToList()[j] == startPos + k)){
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        System.Diagnostics.Debug.WriteLine("------------" + pair.Key);
                        keys.Add(pair.Key);
                        break;
                    }
                }
            }
            List<int> exceptkeys = pageWordTable.Keys.Except(keys).ToList();
            foreach (int key in exceptkeys) {
                pageWordTable.Remove(key);
            }
            return new Tuple<Dictionary<int, Dictionary<int, List<int>>>, Dictionary<int, List<int>>>(pageWordTable, dfTable);
        }

        public List<List<string>> getStemmedQueryTerms(string line)
        {
            List<List<string>> terms = new List<List<string>>();
            Stemmer stemmer = new Stemmer();
            foreach (Match match in Regex.Matches(line, "\"([^\"]*)\""))
            {
                List<string> words = match.ToString().Replace('"', ' ').Trim().Split(' ')
                    .Where(wrd => (!Crawler.StopwordList.Contains(wrd.ToLower()) && !wrd.Trim().Equals(""))).ToList();
                List<string> stemmedWords = new List<string>();
                foreach (string w in words)
                {
                    string str = w.Trim().ToLower();
                    stemmer.add(str.ToCharArray(), str.Length);
                    stemmer.stem();
                    string word = stemmer.ToString().ToLower();
                    //if (!stemmedWords.Contains(word))// && Crawler.getWordId(word) >= 0)
                    //{
                        stemmedWords.Add(word);
                    //}
                }
                terms.Add(stemmedWords);
            }
            string remaining = Regex.Replace(line, "\"([^\"]*)\"", " ");
            remaining = Regex.Replace(remaining, @"\s+", " ");
            List<string> remWords = new List<string>();
            foreach (string word in remaining.Split(' ').Where(wrd => (!Crawler.StopwordList.Contains(wrd.ToLower()) && !wrd.Trim().Equals(""))))
            {
                string w = word.ToLower().Trim();
                stemmer.add(w.ToCharArray(), w.Length);
                stemmer.stem();
                string word2 = stemmer.ToString().ToLower();
                if (!remWords.Contains(word2))// && Crawler.getWordId(word2) >= 0)
                {
                    terms.Add(new List<string>() { word2 });
                    remWords.Add(word2);
                }
            }
            return terms;
        }
    }
}