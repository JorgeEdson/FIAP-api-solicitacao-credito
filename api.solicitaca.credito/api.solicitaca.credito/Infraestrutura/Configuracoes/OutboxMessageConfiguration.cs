using api.solicitacao.credito.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.solicitaca.credito.Infraestrutura.Configuracoes
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("T344OUTB", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)                   
                   .ValueGeneratedNever();

            builder.Property(x => x.TipoMensagem)                   
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Payload)                   
                   .IsRequired();

            builder.Property(x => x.CorrelationId)                   
                   .HasMaxLength(64)
                   .IsRequired();

            builder.Property(x => x.IdempotencyKey)                   
                   .HasMaxLength(64)
                   .IsRequired();

            builder.Property(x => x.Status)                   
                   .HasConversion<string>()
                   .HasMaxLength(32)
                   .IsRequired();

            builder.Property(x => x.DataCadastro)                   
                   .IsRequired();

            builder.Property(x => x.DataUltimaAlteracao)
                   .IsRequired(false);

            builder.Property(x => x.Tentativas)                   
                   .HasDefaultValue(0);

            builder.HasIndex(x => x.IdempotencyKey).IsUnique();
            builder.HasIndex(x => new { x.Status, x.DataCadastro });
        }
    }
}
