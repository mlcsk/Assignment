namespace CrawlWebApp.Models
{
    public class CrawlRequest
    {
        public required List<string> Urls { get; set; }
        public int MaxDepth { get; set; }
    }
}
