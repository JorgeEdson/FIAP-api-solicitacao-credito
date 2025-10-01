using api.solicitacao.credito.Dominio;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace api.solicitacao.credito.Infraestrutura
{
    public class Contexto : DbContext
    {
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
