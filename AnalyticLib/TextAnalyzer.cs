using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticLib
{
    public class TextAnalyzer
    {
        private List<News> _news;
        private List<string> _words = new List<string>();


        public TextAnalyzer(List<News> news)
        {
            _news = news;
        }

        //Split text into words and may remove stopwords
        public TextAnalyzer SplitText(bool removeStopWords = false)
        {
            HashSet<string> stopWords = new HashSet<string>() { "is", "the", "a", };

            foreach (News news in _news)
            {
                foreach (string word in news.Contents.Split())
                {
                    if (!string.IsNullOrEmpty(word) && !word.ToLower().IsStopWord()) _words.Add(word);
                }
            }

            return this;
        }

        //Count the occurance of words in a list
        public Dictionary<string, int> WordCount()
        {
            Dictionary<string, int> countedWords = new Dictionary<string, int>();

            foreach (string word in _words)
            {
                string wrd = word.ToLower();

                if (countedWords.ContainsKey(wrd))
                {
                    countedWords[wrd] = countedWords[wrd] + 1;
                }
                else
                {
                    countedWords.Add(wrd, 1);
                }
            }

            return countedWords;
        }
    }
}
