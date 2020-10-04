using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetCrawler.Processor
{
    public interface ITJCrawlerProcessor<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> Process(HtmlDocument document);
    }
}
