using AASPA.Models.Model;
using AASPA.Repository;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AASPA.Controllers
{
    public class SituacaoOcorrencia : PrivateController
    {
        private readonly MysqlContexto _contexto;

        public SituacaoOcorrencia(MysqlContexto contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        [Route("/api/SituacaoOcorrencia")]
        public IActionResult AllSituacaoOcorrencia()
        {
            try
            {
                var situacaoOcorrencia = _contexto.situacao_ocorrencia.ToList();
                return Ok(situacaoOcorrencia);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/api/SituacaoOcorrencia")]
        public IActionResult EditarSituacaoOcorrencia([FromBody] NomeId request)
        {
            try
            {
                var situacaoOcorrencia = _contexto.situacao_ocorrencia.FirstOrDefault(x => x.Nome.ToUpper() == request.Nome.ToUpper());
                    if (situacaoOcorrencia != null)
                    throw new Exception("Situação Ocorrencia já cadastrada");

                _contexto.situacao_ocorrencia.Add(new Repository.Maps.SituacaoOcorrencia
                {
                    Nome = request.Nome,
                });

                _contexto.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/api/SituacaoOcorrencia/Editar/{id}")]
        public IActionResult EditarSituacaoOcorrencia([FromBody] NomeId request, [FromRoute] int id)
        {
            try
            {
                var situacaoOcorrencia = _contexto.situacao_ocorrencia.FirstOrDefault(x => x.Id == id)
                    ?? throw new Exception("Nenhuma Situação Ocorrencia foi encontrada");

                situacaoOcorrencia.Nome = request.Nome;

                _contexto.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/api/SituacaoOcorrencia/Deletar/{id}")]
        public IActionResult DeletarSituacaoOcorrencia([FromRoute] int id)
        {
            try
            {
                var situacaoOcorrencia = _contexto.situacao_ocorrencia.FirstOrDefault(x => x.Id == id)
                    ?? throw new Exception("Nenhuma Situação Ocorrencia foi encontrada");

                _contexto.situacao_ocorrencia.Remove(situacaoOcorrencia);

                _contexto.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
