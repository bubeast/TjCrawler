using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TjCrawler.Domain.Attributes;

namespace TjCrawler.Domain.Models.TJ_MS
{
    [DotnetCrawlerEntity(XPath = "//*[@id='tableTodasPartes']", XPathAlt = "//*[@id='tablePartesPrincipais']")]
    public class ParteProcesso
    {
        [DisplayName("Tipo")]
        [DotnetCrawlerField(Expression = "tr > td:nth-child(1) > span", SelectorType = SelectorType.CssSelector)]
        public string TipoParte { get; set; }
        [DisplayName("Nome")]
        [DotnetCrawlerField(Expression = "tr:nth-child({0}) > td:nth-child(2)", SelectorType = SelectorType.CssSelector)]
        public string NomeParte { get; set; }

        [DisplayName("Advogado")]
        [DotnetCrawlerField(Expression = "tr:nth-child({0}) > td:nth-child(2) > span", SelectorType = SelectorType.CssSelector)]
        public List<Advogado> Advogados { get; set; }
    }
}
