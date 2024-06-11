using AASPA.Repository.Maps;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository
{
    public class MysqlContexto : DbContext
    {
        public MysqlContexto(DbContextOptions<MysqlContexto> options) : base(options)
        {
        }

        
        public DbSet<UsuarioDb> usuarios { get; set; }
        public DbSet<HistoricoContatosOcorrenciaDb> historico_contatos_ocorrencia { get; set; }
        public DbSet<OrigemDb> origem { get; set; }
        public DbSet<MotivoContatoDb> motivo_contato { get; set; }
        public DbSet<PagamentoDb> pagamentos { get; set; }
        public DbSet<LogBeneficioDb> log_beneficios { get; set; }
        public DbSet<LogStatusDb> log_status { get; set; }
        public DbSet<StatusDb> status { get; set; }
        public DbSet<BeneficioDb> beneficios { get; set; }
        public DbSet<ClienteDb> clientes { get; set; }
        public DbSet<CaptadorDb> captadores { get; set; }
        public DbSet<VinculoClienteCaptadorDb> vinculo_cliente_captador { get; set; }
        public DbSet<RegistroRemessaDb> registro_remessa { get; set; }
        public DbSet<RemessaDb> remessa { get; set; }
        public DbSet<RetornoRemessaDb> retornosremessa { get; set; }
        public DbSet<RegistroRetornoRemessaDb> registrosretornoremessa { get; set; }
    }
}
