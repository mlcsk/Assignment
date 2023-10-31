namespace CrawlWebApp.Models
{
    public class CrawlResponse
    {
        public int CrawlId { get; }
        public HashSet<string> VisitedUrls { get; }
        public Dictionary<string, string> PageTitles { get; }
        public HashSet<string> LinkedUrls { get; }


        public CrawlResponse(int crawlId)
        {
            CrawlId = crawlId;
            VisitedUrls = new HashSet<string>();
            PageTitles = new Dictionary<string, string>();
            LinkedUrls = new HashSet<string>();
        }

        public void AddPage(string url, string pageTitle)
        {
            VisitedUrls.Add(url);
            if (!PageTitles.ContainsKey(url))
            {
                PageTitles.Add(url, pageTitle);
            }
        }
        public void AddLinkedUrl(string url)
        {
            LinkedUrls.Add(url);
        }
    }
}
