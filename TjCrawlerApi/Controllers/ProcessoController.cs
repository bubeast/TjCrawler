using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TjCrawler.Core.Services.Interfaces;

namespace TjCrawlerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessoController : ControllerBase
    {
        private readonly IProcessoService _processoService;

        public ProcessoController(IProcessoService processoService)
        {
            _processoService = processoService;
        }

        // GET: api/<ProcessoController>
        [HttpGet]
        public ActionResult Get()
        {
            var result = new
            {
                Classe = "<MOCK>",
                Area = "<MOCK>",
                Assunto = "<MOCK>",
                DataDistribuicao = "<MOCK>",
                Juiz = "<MOCK>",
                ValorAcao = "<MOCK>",
                PartesProcesso = "<MOCK>",
                ListaMovimentacoes = new List<object>()
                {
                    new {Data = "B1", Movimento = "B1"},
                    new {Data = "B2", Movimento = "B2"}
                }
            };

            return Ok(result);
        }

        // GET api/<ProcessoController>/5
        [HttpGet("{numeroProcesso}")]
        public ActionResult Get(string numeroProcesso, int? codigoTribunal = null)
        {
            if (String.IsNullOrWhiteSpace(numeroProcesso) || numeroProcesso.Any(char.IsLetter))
            {
                return BadRequest("Número de processo inválido.");
            }

            try
            {
                numeroProcesso = string.Concat(numeroProcesso.Trim().Where(char.IsDigit));

                if (numeroProcesso.Length > 20)
                {
                    return BadRequest("Número de processo inválido.");
                }

                if (numeroProcesso.Length < 20)
                {
                    numeroProcesso = numeroProcesso.PadLeft(20, '0');
                }

                var dadosProcesso = _processoService.ObterProcesso(numeroProcesso, codigoTribunal);

                var resultMock = new
                {
                    Classe = "<MOCK>Procedimento Comum Cível",
                    Area = "<MOCK>Cível",
                    Assunto = "<MOCK>Dano Material",
                    DataDistribuicao = "<MOCK>02/05/2018 às 19:01",
                    Juiz = "<MOCK>José Cícero Alves da Silva",
                    ValorAcao = "<MOCK>R$ 281.178,42",
                    PartesProcesso = new List<object>()
                    {
                        new {Autor = "<MOCK>Luiz Pessoa", Advogado = "<MOCK>Jonatas Bahia"},
                        new {Reu = "<MOCK>Banco", Advogado = "<MOCK>Alberto Brasil"}
                    },
                    ListaMovimentacoes = new List<object>()
                    {
                        new {Data = "B1", Movimento = "B1"},
                        new {Data = "B2", Movimento = "B2"}
                    }
                };

                //return Ok(resultMock);

                if (dadosProcesso.Result != null)
                    return Ok(dadosProcesso.Result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Numero de processo inválido.");
            }
        }
    }
}
