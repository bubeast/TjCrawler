using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TjCrawler.Core;
using TjCrawler.Core.Services.Interfaces;
using TjCrawler.Domain.Models.Processo;
using TjCrawler.Request;
using DotnetCrawler.Downloader;
using DotnetCrawler.Processor;
using TjCrawler.Domain.Models;
using TjCrawler.Domain.Models.Common;

namespace TjCrawler.Api.Services
{
    public class ProcessoService : IProcessoService
    {
        public const string SEQUENCIAL = "Sequencial";
        public const string DIGITO_VERIFICADOR = "DigitoVerificador";
        public const string ANO_AJUIZAMENTO = "AnoAjuizamento";
        public const string NUMERO_ORGAO = "NumeroOrgao";
        public const string NUMERO_TRIBUNAL = "NumeroTribunal";
        public const string NUMERO_UNIDADE_ORIGEM = "NumeroUnidadeOrigem";
        public const string URL_TJ_AL_1G = "https://www2.tjal.jus.br/cpopg/search.do?cbPesquisa=NUMPROC&dadosConsulta.tipoNuProcesso=UNIFICADO&dadosConsulta.valorConsultaNuUnificado={0}";
        public const string URL_TJ_AL_2G = "https://www2.tjal.jus.br/cposg5/search.do?paginaConsulta=1&cbPesquisa=NUMPROC&tipoNuProcesso=UNIFICADO&dePesquisaNuUnificado={0}";
        public const string URL_TJ_MS_1G = "https://www2.tjal.jus.br/cpopg/search.do?cbPesquisa=NUMPROC&dadosConsulta.tipoNuProcesso=UNIFICADO&dadosConsulta.valorConsultaNuUnificado={0}";
        public const string URL_TJ_MS_2G = "https://esaj.tjms.jus.br/cposg5/search.do?cbPesquisa=NUMPROC&dePesquisaNuUnificado={0}&tipoNuProcesso=UNIFICADO";

        public ProcessoService()
        {
        }

        public async Task<Processo> ObterProcesso(string numeroProcesso, int? codigoTribunal)
        {
            Processo dadosProcesso = null;
            string tribunal;
            Dictionary<string, string> processoInfo = DecodificarNumeroProcesso(numeroProcesso);

            if (codigoTribunal == null)
            {
                codigoTribunal = Convert.ToInt32(processoInfo.GetValueOrDefault(NUMERO_TRIBUNAL));
            }

            if (ValidarNumeroProcesso(processoInfo))
            {
                tribunal = ObterEstadoDeCodigoTribunal(codigoTribunal.Value);

                switch (tribunal)
                {
                    case "AL":
                        dadosProcesso = await ConsultarProcessoTJAL(numeroProcesso, URL_TJ_AL_1G, URL_TJ_AL_2G);
                        break;
                    case "MS":
                        dadosProcesso = await ConsultarProcessoTJMS(numeroProcesso, URL_TJ_AL_1G, URL_TJ_AL_2G);
                        break;
                    default:
                        throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }

            return dadosProcesso;
        }

        private async Task<Processo> ConsultarProcessoTJMS(string numeroProcesso, string urlTribunalPrimeiroGrau, string urlTribunalSegundoGrau)
        {
            throw new NotImplementedException();
        }

        private async Task<Processo> ConsultarProcessoTJAL(string numeroProcesso, string urlTribunalPrimeiroGrau, string urlTribunalSegundoGrau)
        {
            List<ProcessoTJAL> dadosProcesso = null;
            List<ParteProcesso> partesProcesso = null;
            List<Movimentacao> movimentacoes = null;

            var crawlerDadosProcesso = new DotnetCrawler<ProcessoTJAL>()
                                 .AddRequest(new DotnetCrawlerRequest { Url = String.Format(urlTribunalPrimeiroGrau, numeroProcesso), TimeOut = 5000 })
                                 .AddDownloader(new DotnetCrawlerDownloader { DownloaderType = DotnetCrawlerDownloaderType.FromMemory, DownloadPath = @"C:\DotnetCrawler" })
                                 .AddProcessor(new TJCrawlerProcessor<ProcessoTJAL> { });

            dadosProcesso = await crawlerDadosProcesso.CrawleProcessoAL(String.Format(urlTribunalPrimeiroGrau, numeroProcesso));

            if (dadosProcesso != null)
            {
                var crawlePartesProcesso = new DotnetCrawler<ParteProcesso>()
                                    .AddRequest(new DotnetCrawlerRequest { Url = String.Format(urlTribunalPrimeiroGrau, numeroProcesso), TimeOut = 5000 })
                                    .AddDownloader(new DotnetCrawlerDownloader { DownloaderType = DotnetCrawlerDownloaderType.FromMemory, DownloadPath = @"C:\DotnetCrawler" })
                                    .AddProcessor(new TJCrawlerProcessor<ParteProcesso> { });

                partesProcesso = await crawlePartesProcesso.CrawlePartesProcessoAL(String.Format(urlTribunalPrimeiroGrau, numeroProcesso));

                if (partesProcesso != null)
                    dadosProcesso.FirstOrDefault().PartesProcesso = partesProcesso;

                var crawlerMovimentacoesProcesso = new DotnetCrawler<Movimentacao>()
                                 .AddRequest(new DotnetCrawlerRequest { Url = String.Format(urlTribunalPrimeiroGrau, numeroProcesso), TimeOut = 5000 })
                                 .AddDownloader(new DotnetCrawlerDownloader { DownloaderType = DotnetCrawlerDownloaderType.FromMemory, DownloadPath = @"C:\DotnetCrawler" })
                                 .AddProcessor(new TJCrawlerProcessor<Movimentacao> { });

                movimentacoes = await crawlerMovimentacoesProcesso.CrawleMovimentacoes(String.Format(urlTribunalPrimeiroGrau, numeroProcesso));

                if (movimentacoes != null)
                    dadosProcesso.FirstOrDefault().Movimentacoes = movimentacoes;
            }

            var resultado = dadosProcesso.FirstOrDefault();

            Processo processoResult = null;

            if (resultado != null)
            {
                processoResult = new Processo
                {
                    Area = resultado.Area,
                    Assunto = resultado.Assunto,
                    Classe = resultado.Classe,
                    DataDistribuicao = resultado.DataDistribuicao,
                    FlagGrauRecurso = resultado.FlagGrauRecurso.GetValueOrDefault(),
                    Juiz = resultado.Juiz,
                    ValorAcao = resultado.ValorAcao,
                    NumeroProcessoCompleto = resultado.NumeroProcessoCompleto,
                    PartesProcesso = new List<ParteProcesso>(),
                    Movimentacoes = resultado.Movimentacoes
                };

                foreach (var parte in resultado.PartesProcesso.ToList())
                {
                    processoResult.PartesProcesso.Add(new ParteProcesso
                    {
                        TipoParte = parte.TipoParte,
                        NomeParte = parte.NomeParte,
                        Advogados = parte.Advogados
                    });
                }

                foreach (var movimentacao in resultado.Movimentacoes.ToList())
                {
                    processoResult.Movimentacoes.Add(new Movimentacao
                    {
                        Data = movimentacao.Data,
                        TituloMovimento = movimentacao.TituloMovimento,
                        LinkMovimento = movimentacao.LinkMovimento,
                        TextoMovimento = movimentacao.TextoMovimento
                    });
                }

                //_mapper.Map(resultado, processoResult, typeof(ProcessoTJAL), typeof(Processo));
            }

            return processoResult;
        }

        private Dictionary<string, string> DecodificarNumeroProcesso(string numeroProcessoString)
        {
            Dictionary<string, string> processoInfo = new Dictionary<string, string>();
            try
            {
                processoInfo.Add(SEQUENCIAL, numeroProcessoString.Substring(0, 7));
                processoInfo.Add(DIGITO_VERIFICADOR, numeroProcessoString.Substring(7, 2));
                processoInfo.Add(ANO_AJUIZAMENTO, numeroProcessoString.Substring(9, 4));
                processoInfo.Add(NUMERO_ORGAO, numeroProcessoString.Substring(13, 1));
                processoInfo.Add(NUMERO_TRIBUNAL, numeroProcessoString.Substring(14, 2));
                processoInfo.Add(NUMERO_UNIDADE_ORIGEM, numeroProcessoString.Substring(16, 4));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return processoInfo;
        }
        private string ObterEstadoDeCodigoTribunal(int codigoTribunal)
        {
            string estado = String.Empty;

            if (Enum.IsDefined(typeof(Enums.CodigosTribunaisEstaduais), codigoTribunal))
                estado = ((Enums.CodigosTribunaisEstaduais)codigoTribunal).ToString();

            return estado;
        }
        private bool ValidarNumeroProcesso(Dictionary<string, string> processoInfo)
        {
            bool valido = true;

            if (Convert.ToInt32(processoInfo.GetValueOrDefault(SEQUENCIAL)) < 1)
                return false;
            if (Convert.ToInt32(processoInfo.GetValueOrDefault(DIGITO_VERIFICADOR)) < 1)
                return false;
            if (Convert.ToInt32(processoInfo.GetValueOrDefault(ANO_AJUIZAMENTO)) > DateTime.Now.Year)
                return false;
            if (Convert.ToInt32(processoInfo.GetValueOrDefault(NUMERO_ORGAO)) < 1 || Convert.ToInt32(processoInfo.GetValueOrDefault(NUMERO_ORGAO)) > 9)
                return false;
            if (Convert.ToInt32(processoInfo.GetValueOrDefault(NUMERO_TRIBUNAL)) < 1)
                return false;
            if (Convert.ToInt32(processoInfo.GetValueOrDefault(NUMERO_UNIDADE_ORIGEM)) < 1 || Convert.ToInt32(processoInfo.GetValueOrDefault(NUMERO_UNIDADE_ORIGEM)) > 8999)
                return false;

            return valido;
        }
    }
}
