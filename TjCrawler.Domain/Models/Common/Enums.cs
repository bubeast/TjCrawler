using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TjCrawler.Domain.Models.Common
{
    public class Enums
    {
        public enum CodigosTribunaisEstaduais
        {
            [Description("Alagoas")]
            AL = 02,
            [Description("Mato Grosso do Sul")]
            MS = 12
        };

        public enum ScrappingMode
        {
            Table,
            Forms
        }
    }
}
