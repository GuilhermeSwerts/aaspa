using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class ClienteDb
    {
        [Key]
        public int cliente_id { get; set; }
        public string cliente_cpf { get; set; }
        public string cliente_nome { get; set; }
        public string cliente_cep { get; set; }
        public string cliente_logradouro { get; set; }
        public string cliente_bairro { get; set; }
        public string cliente_localidade { get; set; }
        public string cliente_uf { get; set; }
        public string cliente_numero { get; set; }
        public string? cliente_complemento { get; set; }
        public DateTime cliente_dataNasc { get; set; }
        public DateTime cliente_dataCadastro { get; set; } = DateTime.Now;
        public string? cliente_nrDocto { get; set; }
        public string? cliente_empregador { get; set; }
        public string cliente_matriculaBeneficio { get; set; }
        public string cliente_nomeMae { get; set; }
        public string? cliente_nomePai { get; set; }
        public string? cliente_telefoneFixo { get; set; }
        public string cliente_telefoneCelular { get; set; }
        public bool cliente_possuiWhatsapp { get; set; }
        public string cliente_funcaoAASPA { get; set; }
        public string? cliente_email { get; set; }
        //public int cliente_usuario_id_fk { get; set; }
        public bool cliente_situacao { get; set; }
        public int? cliente_sexo { get; set; }
        public int cliente_estado_civil { get; set; }
        public int? cliente_remessa_id { get; set; }
        public bool clientes_cadastro_externo { get; set; }
    }
}
