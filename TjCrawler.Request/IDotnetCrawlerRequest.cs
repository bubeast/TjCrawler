using System;
using System.Collections.Generic;
using System.Text;

namespace TjCrawler.Request
{
    public interface IDotnetCrawlerRequest
    {
        string Url { get; set; }
        string Regex { get; set; }
        long TimeOut { get; set; }
    }
}
