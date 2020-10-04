using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TjCrawler.Api.Resources.Processo
{
    public class ParteProcesso
    {
        public string TipoParte { get; set; }
        public string NomeParte { get; set; }
        public List<Advogado> Advogados { get; set; }
    }
}
