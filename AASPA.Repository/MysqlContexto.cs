using AASPA.Repository.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogAlteracaoDb>(entity =>
            {
                entity.Property(e => e.log_tipo)
                      .HasConversion<int>();
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClienteDb>()
                .HasMany(c => c.LogStatus)
                .WithOne()
                .HasForeignKey(l => l.log_status_cliente_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogBeneficioDb>()
                .HasOne<ClienteDb>()
                .WithMany(c => c.LogBeneficios)
                .HasForeignKey(lb => lb.log_beneficios_cliente_id)
                .OnDelete(DeleteBehavior.Cascade);
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
        public DbSet<RetornoRemessaDb> retornos_remessa { get; set; }
        public DbSet<RegistroRetornoRemessaDb> registros_retorno_remessa { get; set; }
        public DbSet<RetornoFinanceiroDb> retorno_financeiro { get; set; }
        public DbSet<RegistroRetornoFinanceiroDb> registro_retorno_financeiro { get; set; }
        public DbSet<CodigoRetornoDb> codigo_retorno { get; set; }
        public DbSet<SolicitacaoReembolsoDb> solicitacaoreembolso { get; set; }
        public DbSet<ElegivelReembolsoDb> elegivelreembolso { get; set; }
        public DbSet<AnexosDb> anexos { get; set; }
        public DbSet<LogAlteracaoDb> log_alteracao { get; set; }
        public DbSet<LogCancelamentoClienteDb> log_cancelamento_cliente {  get; set; }
    }
}
