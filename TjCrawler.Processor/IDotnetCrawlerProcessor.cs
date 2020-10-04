using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotnetCrawler.Processor
{
    public interface IDotnetCrawlerProcessor<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> Process(HtmlDocument document);
        Task<IEnumerable<TEntity>> ProcessarDados(HtmlDocument document);
        Task<IEnumerable<TEntity>> ProcessarPartes(HtmlDocument document);
        Task<IEnumerable<TEntity>> ProcessarMovimentacoes(HtmlDocument document);
        Task<IEnumerable<TEntity>> ProcessForm(HtmlDocument document);
    }
}
