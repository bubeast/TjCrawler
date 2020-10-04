using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TjCrawler.Domain.Models.Processo;

namespace TjCrawler.Core.Services.Interfaces
{
    public interface IProcessoService
    {
        public Task<Processo> ObterProcesso(string numeroProcesso, int? codigoTribunal);
    }
}
