using CrawlWebApp.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace CrawlWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlWebController : ControllerBase
    {

        private static int idCounter = 1;
        private static readonly ConcurrentDictionary<int, CrawlResponse> crawlResponses = new ConcurrentDictionary<int, CrawlResponse>();
        private readonly IHttpClientFactory _httpClientFactory;
        public CrawlWebController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [Route("crawlsave")]
        public async Task<IActionResult> CrawlSave([FromBody] CrawlRequest request)
        {
            int crawlId = idCounter++;
            var crawlResult = new CrawlResponse(crawlId);

            foreach (string url in request.Urls)
            {
                await CrawlUrls(crawlResult, url, request.MaxDepth);
            }

            crawlResponses.TryAdd(crawlId, crawlResult);

            return Ok(new { CrawlId = crawlId });
        }

        [HttpGet]
        [ResponseCache(Duration = 300)]
        [Route("crawlresponse/{crawlId}")]
        public IActionResult GetCrawlResult(int crawlId)
        {
            if (crawlResponses.TryGetValue(crawlId, out var crawlResult))
            {
                return Ok(crawlResult);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("pagerelations")]
        public IActionResult GetPageRelations(string url, int maxDepth)
        {
            var result = new List<CrawlPageRelation>();
            var visitedUrls = new HashSet<string>();
            PageRelations(result, visitedUrls, url, maxDepth);
            return Ok(result);
        }

        private async Task CrawlUrls(CrawlResponse crawlResponse, string url, int maxDepth)
        {
            if (maxDepth < 0)
            {
                return;
            }

            if (!crawlResponse.VisitedUrls.Contains(url))
            {
                crawlResponse.VisitedUrls.Add(url);

                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var html = await httpClient.GetStringAsync(url);
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    string pageTitle = document.DocumentNode.SelectSingleNode("//title").InnerHtml;

                    crawlResponse.AddPage(url, pageTitle);

                    var links = document.DocumentNode.SelectNodes("//a[@href]");
                    if (links != null)
                    {
                        foreach (var link in links)
                        {
                            if (maxDepth < 0)
                            {
                                return;
                            }

                            var href = link.GetAttributeValue("href", "");
                            if (!string.IsNullOrEmpty(href))
                            {
                                if (!Uri.TryCreate(href, UriKind.Absolute, out var absoluteUri))
                                {
                                    if (Uri.TryCreate(new Uri(url), href, out absoluteUri))
                                    {
                                        href = absoluteUri.AbsoluteUri;
                                    }
                                }

                                crawlResponse.AddLinkedUrl(href.ToString());
                                maxDepth--;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error crawling {url}: {ex.Message}");
                }
            }
        }

        private void PageRelations(List<CrawlPageRelation> response, HashSet<string> visitedUrls, string url, int maxDepth)
        {
            if (maxDepth < 0 || visitedUrls.Contains(url))
            {
                return;
            }

            visitedUrls.Add(url);

            foreach (var crawlResultPair in crawlResponses)
            {
                var crawlResponse = crawlResultPair.Value;
                if (crawlResponse.VisitedUrls.Contains(url))
                {
                    response.Add(new CrawlPageRelation(url, crawlResponse.PageTitles[url], crawlResponse.LinkedUrls.Count, crawlResponse.LinkedUrls));
                    break;

                }
            }
        }

    }

}

