namespace CrawlWebApp.Models
{
    public class CrawlPageRelation
    {
        public CrawlPageRelation(string url, string title, int linkedurlcount, Dictionary<string, string> linkedurlstitles)
        {
            Url = url;
            Title = title;
            LinkedURLSCount = linkedurlcount;
            LinkedUrlsTitles = linkedurlstitles;
        }

        public string Url { get; set; }
        public string Title { get; set; }
        public int LinkedURLSCount { get; set; }
        public Dictionary<string, string> LinkedUrlsTitles { get; set; }
       
    }
}
