namespace api.solicitacao.credito.Aplicacao.Requests
{
    public record SolicitarCreditoRequest(
        string Cpf,
        decimal RendaMensal,
        decimal ValorSolicitado,
        int PrazoMeses,
        string? Canal
    );
}
