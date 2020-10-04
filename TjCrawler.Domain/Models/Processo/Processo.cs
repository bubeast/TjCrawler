using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TjCrawler.Domain.Attributes;
using TjCrawler.Domain.Models.Processo;

namespace TjCrawler.Domain.Models.Processo
{
    [DotnetCrawlerEntity(XPath = "/html/body/div/table[4]/tr/td")]
    public class Processo
    {
        public Processo()
        {
            FlagGrauRecurso = false;
            PartesProcesso = new HashSet<ParteProcesso>();
            Movimentacoes = new HashSet<Movimentacao>();
        }

        [DisplayName("Classe")]
        [DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[2]/td[2]/table/tr/td/span[1]/span", SelectorType = SelectorType.XPath)]
        public string Classe { get; set; }

        [DisplayName("Área")]
        [DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[3]/td[2]/table/tr/td", SelectorType = SelectorType.XPath)]
        public string Area { get; set; }

        [DisplayName("Assunto")]
        [DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[4]/td[2]/span", SelectorType = SelectorType.XPath)]
        public string Assunto { get; set; }

        [DisplayName("Distribuição")]
        [DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[6]/td[2]/span", SelectorType = SelectorType.XPath)]
        public string DataDistribuicao { get; set; }

        [DisplayName("Juiz")]
        [DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[9]/td[2]/span", SelectorType = SelectorType.XPath)]
        public string Juiz { get; set; }

        [DisplayName("Valor da Ação")]
        [DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[10]/td[2]/span", SelectorType = SelectorType.XPath)]
        public string ValorAcao { get; set; }

        [DisplayName("Em grau de recurso")]
        [DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[1]/td[2]/table/tr/td/span[3]", SelectorType = SelectorType.XPath)]
        public bool FlagGrauRecurso { get; set; } = false;

        [DisplayName("Processo")]
        [DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[1]/td[2]/table/tr/td/span[1]", SelectorType = SelectorType.XPath)]
        public string NumeroProcessoCompleto { get; set; }

        [DisplayName("Partes do processo")]
        public ICollection<ParteProcesso> PartesProcesso { get; set; }

        [DisplayName("Movimentações")]
        public ICollection<Movimentacao> Movimentacoes { get; set; }
    }
}
