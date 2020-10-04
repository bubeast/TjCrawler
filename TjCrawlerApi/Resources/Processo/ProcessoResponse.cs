using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TjCrawler.Api.Resources.Processo;

namespace TjCrawler.Api.Resources
{
    public class ProcessoResponse
    {
        public string Classe { get; set; }
        public string Area { get; set; }
        public string Assunto { get; set; }
        public DateTime DataDistribuicao { get; set; }
        public string Juiz { get; set; }
        public decimal ValorAcao { get; set; }
        public List<ParteProcesso> PartesProcesso { get; set; }
        public List<Movimentacao> Movimentacoes { get; set; }

        public string NumeroProcessoCompleto { get; set; }
    }
}
