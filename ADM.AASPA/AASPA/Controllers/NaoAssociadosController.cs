using AASPA.Models.Requests;
using AASPA.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AASPA.Controllers
{
    public class NaoAssociadosController : PrivateController
    {
        private readonly MysqlContexto _mysql;

        public NaoAssociadosController(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        [HttpGet]
        [Route("/api/NaoAssociados/{id}")]
        public IActionResult GetPorId([FromRoute] int id)
        {
            try
            {
                var item = _mysql.NaoAssociados.Find(id);
                if (item == null)
                    return BadRequest("Não encontrado.");

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/NaoAssociados/history/{cpf}")]
        public IActionResult GetByCpf([FromRoute] string cpf)
        {
            try
            {
                var origens = _mysql.origem.ToList();
                var motivos = _mysql.motivo_contato.ToList();
                var ocorrencias = _mysql.situacao_ocorrencia.ToList();

                var res = _mysql.NaoAssociados.Where(x => x.cpf_nao_associados == cpf).ToList().Select(x => new
                {
                    Id = x.nao_associado_id,
                    Origem = origens.FirstOrDefault(c => c.origem_id == x.origem_id)?.origem_nome,
                    data = x.data_ocorrencia,
                    Motivo = motivos.FirstOrDefault(c => c.motivo_contato_id == x.motivo_contato_id)?.motivo_contato_nome,
                    Ocorrencia = ocorrencias.FirstOrDefault(c => c.Id == x.situacao_ocorrencia_id)?.Nome,
                    Telefone = x.telefone,
                    Descricao = x.descricao_nao_associado
                }).ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/NaoAssociados")]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _mysql.NaoAssociados.ToList()
                    .GroupBy(x => x.cpf_nao_associados)
                    .Select(x => new
                    {
                        Nome = x.First().nome_nao_associados,
                        Cpf = x.Key,
                        Telefone = x.First().telefone,
                        Qtd = x.Count()
                    }).ToList();

                var total = query.Count();

                var paginated = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new
                {
                    TotalItems = total,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = paginated
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("/api/NaoAssociados/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var item = _mysql.NaoAssociados.Find(id);
                if (item == null)
                    return BadRequest("Não encontrado.");

                _mysql.NaoAssociados.Remove(item);
                _mysql.SaveChanges();

                return Ok("Removido com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("/api/NaoAssociados/{id}")]
        public IActionResult Put([FromRoute] int id, [FromBody] NaoAssociadosNovoAtendimentoRequest request)
        {
            try
            {
                var existente = _mysql.NaoAssociados.Find(id);
                if (existente == null)
                    return BadRequest("Não encontrado.");

                existente.cpf_nao_associados = request.Cpf.Replace(".", "").Replace("-", "").Replace(" ", "");
                existente.data_ocorrencia = request.DataHora;
                existente.descricao_nao_associado = request.Descricao;
                existente.motivo_contato_id = request.Motivo;
                existente.nome_nao_associados = request.Nome;
                existente.origem_id = request.Origem;
                existente.situacao_ocorrencia_id = request.Situacao;
                existente.telefone = request.Telefone;

                _mysql.SaveChanges();

                return Ok(existente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/api/NaoAssociados")]
        public IActionResult PostNovoContato([FromBody] NaoAssociadosNovoAtendimentoRequest request)
        {
            try
            {
                _mysql.NaoAssociados.Add(new Repository.Maps.NaoAssociados
                {
                    cpf_nao_associados = request.Cpf.Replace(".", "").Replace("-", "").Replace(" ", ""),
                    data_ocorrencia = request.DataHora,
                    descricao_nao_associado = request.Descricao,
                    motivo_contato_id = request.Motivo,
                    nao_associado_dt_cadastro = DateTime.Now,
                    nome_nao_associados = request.Nome,
                    origem_id = request.Origem,
                    situacao_ocorrencia_id = request.Situacao,
                    telefone = request.Telefone
                });
                _mysql.SaveChanges();

                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
