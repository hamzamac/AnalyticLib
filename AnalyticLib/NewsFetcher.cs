using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AnalyticLib
{
    public class NewsFetcher
    {
        private WebClient _webClient;

        public NewsFetcher()
        {
            _webClient = new WebClient();
        }


        //fetch the news page source
        public string GetPage(string address)
        {
            string newsPage;

            using (Stream data = _webClient.OpenRead(address))
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    newsPage = reader.ReadToEnd();
                }
            }

            return newsPage;
        }

        //convert newspage to news object
        private News PageToNews(string newsPage)
        {
            string pattern = @"<article.+?</article>";
            var article = Regex.Match(newsPage, pattern, RegexOptions.Singleline);

            pattern = @"<time>.+?</time>";
            var time = Regex.Match(article.Value, pattern, RegexOptions.Singleline);

            pattern = @"<h1>.+?</h1>";
            var heading = Regex.Match(article.Value, pattern, RegexOptions.Singleline);

            pattern = @"<h2>.+?</h2>";
            var subheading = Regex.Match(article.Value, pattern, RegexOptions.Singleline);

            pattern = @"</h1>\s+?<div class.+?</div>";
            var content = Regex.Match(article.Value, pattern, RegexOptions.Singleline);

            return new News()
            {
                Date = DateTime.Parse(Clean(time, true)),
                Heading = Clean(heading),
                Subheading = Clean(subheading),
                Contents = Clean(content)
            };
        }

        private string Clean(Match html, bool isDate = false)
        {
            string pattern, cleaned;

            if (isDate)
            {
                pattern = @"(<.+?>|&.+?;|\n|[0-9]st|[0-9]nd|[0-9]rd|[0-9]th)";
                cleaned = Regex.Replace(html.Value, pattern, string.Empty);
            }
            else
            {
                pattern = @"(<.+?>|&.+?;|\n|\p{P})";
                cleaned = Regex.Replace(html.Value, pattern, " ");
            }

            return cleaned;
        }

        public HashSet<string> PlockUri(int max = 6)
        {
            HashSet<string> urls = new HashSet<string>();

            for (int i = 1; i <= max; i++)
            {
                try
                {
                    // get the news page
                    var page = GetPage(@"https://www.silobreaker.com/news-and-views/page/" + i);

                    //plock news uri
                    var pattern = @"<ul class=" + '"' + "all-news.+?</ul>";
                    var mainSection = Regex.Match(page, pattern, RegexOptions.Singleline);

                    pattern = @"https[:]//www[.]silobreaker[.]com/([a-z0-9]+|-)+?/";
                    var matchedUri = Regex.Matches(mainSection.Value, pattern, RegexOptions.Singleline);

                    //add the uri to the set
                    foreach (Match m in matchedUri)
                    {
                        urls.Add(m.Value);
                    }
                }
                catch (Exception)
                {
                    //
                }
            }

            return urls;
        }


        public List<News> GetNews(HashSet<string> urls)
        {
            List<News> news = new List<News>();

            foreach (string url in urls)
            {
                news.Add(PageToNews(GetPage(url)));
            }

            return news;
        }
    }
}
