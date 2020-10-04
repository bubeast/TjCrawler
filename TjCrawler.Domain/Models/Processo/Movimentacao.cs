using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TjCrawler.Domain.Attributes;

namespace TjCrawler.Domain.Models.Processo
{
    [DotnetCrawlerEntity(XPath = "//*[@id='tabelaTodasMovimentacoes']", XPathAlt = "//*[@id='tabelaUltimasMovimentacoes']")]
    public class Movimentacao
    {
        [DisplayName("Data")]
        [DotnetCrawlerField(Expression = "tr:nth-child({0}) > td:nth-child(1)", SelectorType = SelectorType.CssSelector)]
        public string Data { get; set; }

        [DisplayName("TituloMovimento")]
        [DotnetCrawlerField(Expression = "tr:nth-child({0}) > td:nth-child(3)", SelectorType = SelectorType.CssSelector)]
        public string TituloMovimento { get; set; }

        [DisplayName("Movimento")]
        [DotnetCrawlerField(Expression = "tr:nth-child({0}) > td:nth-child(3) > span", SelectorType = SelectorType.CssSelector)]
        public string TextoMovimento { get; set; }

        [DisplayName("Link")]
        [DotnetCrawlerField(Expression = "tr:nth-child({0}) > td:nth-child(2) > a", SelectorType = SelectorType.CssSelector)]
        public string LinkMovimento { get; set; }
    }
}
