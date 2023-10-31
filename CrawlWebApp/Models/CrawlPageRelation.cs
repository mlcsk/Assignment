namespace CrawlWebApp.Models
{
    public class CrawlPageRelation
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public int LinkedURLSCount { get; set; }
        public HashSet<string> LinkedURLs { get; set; }
        public CrawlPageRelation(string url, string title,int linkedurlcount, HashSet<string> linkedurls)
        {
            Url = url;
            Title = title;
            LinkedURLSCount = linkedurlcount;
            LinkedURLs = linkedurls;
        }
    }
}
