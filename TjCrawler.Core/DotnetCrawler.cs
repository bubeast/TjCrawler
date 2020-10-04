using DotnetCrawler.Downloader;
using DotnetCrawler.Processor;
using System.Collections.Generic;
using System.Threading.Tasks;
using TjCrawler.Domain.Models.Processo;
using TjCrawler.Request;

namespace TjCrawler.Core
{
    public class DotnetCrawler<TEntity> : IDotnetCrawler where TEntity : class
    {
        public IDotnetCrawlerRequest Request { get; private set; }
        public IDotnetCrawlerDownloader Downloader { get; private set; }
        public IDotnetCrawlerProcessor<TEntity> Processor { get; private set; }
        public HtmlAgilityPack.HtmlDocument Document { get; private set; }

        public DotnetCrawler()
        {

        }

        public DotnetCrawler<TEntity> AddRequest(IDotnetCrawlerRequest request)
        {
            Request = request;
            return this;
        }

        public DotnetCrawler<TEntity> AddDownloader(IDotnetCrawlerDownloader downloader)
        {
            Downloader = downloader;
            return this;
        }

        public DotnetCrawler<TEntity> AddProcessor(IDotnetCrawlerProcessor<TEntity> processor)
        {
            Processor = processor;
            return this;
        }

        public async Task Crawle()
        {
            var linkReader = new DotnetCrawlerPageLinkReader(Request);
            var links = await linkReader.GetLinks(Request.Url, 0);

            foreach (var url in links)
            {
                var document = await Downloader.Download(url);
                var entity = await Processor.Process(document);
            }
        }

        public async Task<List<Processo>> CrawleProcesso(string url)
        {
            var linkReader = new DotnetCrawlerPageLinkReader(Request);

            var document = await linkReader.GetPageRedirect(url);
            var entity = await Processor.Process(document);

            return (List<Processo>)entity;
        }

        public async Task<ParteProcesso> CrawlePartesProcesso(string url)
        {
            var linkReader = new DotnetCrawlerPageLinkReader(Request);

            var document = await linkReader.GetPageRedirect(url);
            var entity = await Processor.ProcessForm(document);

            return (ParteProcesso)entity;
        }

        public async Task<List<ProcessoTJAL>> CrawleProcessoAL(string url)
        {
            var linkReader = new DotnetCrawlerPageLinkReader(Request);

            var document = await linkReader.GetPageRedirect(url);
            var entity = await Processor.ProcessarDados(document);

            return (List<ProcessoTJAL>)entity;
        }

        public async Task<List<ParteProcesso>> CrawlePartesProcessoAL(string url)
        {
            var linkReader = new DotnetCrawlerPageLinkReader(Request);

            var document = await linkReader.GetPageRedirect(url);
            var entity = await Processor.ProcessarPartes(document);

            return (List<ParteProcesso>)entity;
        }

        public async Task<List<Movimentacao>> CrawleMovimentacoes(string url)
        {

            var linkReader = new DotnetCrawlerPageLinkReader(Request);

            var document = await linkReader.GetPageRedirect(url);
            var entity = await Processor.ProcessarMovimentacoes(document);

            return (List<Movimentacao>)entity;
        }
    }
}
