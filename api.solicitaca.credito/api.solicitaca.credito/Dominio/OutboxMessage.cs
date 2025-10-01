namespace api.solicitacao.credito.Dominio
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string TipoMensagem { get; private set; } = default!;
        public string Payload { get; private set; } = default!;
        public string CorrelationId { get; private set; } = default!;
        public string IdempotencyKey { get; private set; } = default!;
        public OutboxStatus Status { get; private set; } = OutboxStatus.Pendente;
        public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;
        public DateTime? DataUltimaAlteracao { get; private set; }
        public int Tentativas { get; private set; }

        public static OutboxMessage Criar(string tipoMensagem, string payloadJson, string correlationId, string idempotencyKey)
            => new()
            {
                TipoMensagem = tipoMensagem,
                Payload = payloadJson,
                CorrelationId = correlationId,
                IdempotencyKey = idempotencyKey
            };

        public void IncrementaTentativa()
        {
            Tentativas++;
            DataUltimaAlteracao = DateTime.UtcNow;
        }

        public void AtualizaStatusProcessado()
        {
            Status = OutboxStatus.Processado;
            MarcarAlterado();
        }

        public void AtualizaStatusErro()
        {
            Status = OutboxStatus.Erro; 
            MarcarAlterado();
        }

        public void AtualizaStatusDeadLetter()
        {
            Status = OutboxStatus.DeadLetter;
            MarcarAlterado();
        }

        public void AtualizaStatusPendente()
        {
            Status = OutboxStatus.Pendente;
            MarcarAlterado();
        }

        private void MarcarAlterado()
            => DataUltimaAlteracao = DateTime.UtcNow;

    }

    public enum OutboxStatus
    {
        Pendente = 0,
        Processado = 1,
        Erro = 2,
        DeadLetter = 3
    }
}
