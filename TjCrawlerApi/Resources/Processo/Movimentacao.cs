using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TjCrawler.Api.Resources.Processo
{
    public class Movimentacao
    {
        public string Data { get; set; }
        public string TituloMovimento { get; set; }
        public string TextoMovimento { get; set; }
        public string LinkMovimento { get; set; }
    }
}
