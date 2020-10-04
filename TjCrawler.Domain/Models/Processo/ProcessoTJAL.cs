using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TjCrawler.Domain.Attributes;

namespace TjCrawler.Domain.Models.Processo
{
    [DotnetCrawlerEntity(XPath = "/html/body/div/table[4]/tr/td/div[1]/table[2]")]
    public class ProcessoTJAL
    {
        public ProcessoTJAL()
        {
            FlagGrauRecurso = false;
            PartesProcesso = new HashSet<ParteProcesso>();
            Movimentacoes = new HashSet<Movimentacao>();
        }

        [DisplayName("Classe")]
        [DotnetCrawlerField(Expression = "//*[text()[contains(., 'Classe')]]/../../td//span", SelectorType = SelectorType.XPath)]
        public string Classe { get; set; }

        [DisplayName("Área")]
        [DotnetCrawlerField(Expression = "tr:nth-child(3) > td:nth-child(2) > table > tr > td", SelectorType = SelectorType.CssSelector)]
        public string Area { get; set; }

        [DisplayName("Assunto")]
        [DotnetCrawlerField(Expression = "//*[text()[contains(., 'Assunto')]]/../../td//span", SelectorType = SelectorType.XPath)]
        public string Assunto { get; set; }

        [DisplayName("Distribuição")]
        //[DotnetCrawlerField(Expression = "/html/body/div/table[4]/tr/td/div[1]/table[2]/tr[./label[contains(., 'Distribuição')]]", SelectorType = SelectorType.XPath)]
        [DotnetCrawlerField(Expression = "//*[text()[contains(., 'Distribuição')]]/../../td//span", SelectorType = SelectorType.XPath)]
        public string DataDistribuicao { get; set; }

        [DisplayName("Juiz")]
        [DotnetCrawlerField(Expression = "//*[text()[contains(., 'Juiz')]]/../../td//span", SelectorType = SelectorType.XPath)]
        public string Juiz { get; set; }

        [DisplayName("Valor da Ação")]
        [DotnetCrawlerField(Expression = "//*[text()[contains(., 'Valor da ação')]]/../../td//span", SelectorType = SelectorType.XPath)]
        public string ValorAcao { get; set; }

        [DisplayName("Em grau de recurso")]
        [DotnetCrawlerField(Expression = "//*[text()[contains(., 'Em grau de recurso')]]/../../td//span", SelectorType = SelectorType.XPath)]
        public bool? FlagGrauRecurso { get; set; } = false;

        [DisplayName("Processo")]
        [DotnetCrawlerField(Expression = "tr:nth-child(1) > td:nth-child(2) > table > tr > td > span:nth-child(1)", SelectorType = SelectorType.CssSelector)]
        public string NumeroProcessoCompleto { get; set; }

        [DisplayName("Partes do processo")]
        public ICollection<ParteProcesso> PartesProcesso { get; set; }

        [DisplayName("Movimentações")]
        public ICollection<Movimentacao> Movimentacoes { get; set; }
    }
}
