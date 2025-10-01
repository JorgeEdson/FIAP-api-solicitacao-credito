namespace api.solicitacao.credito.Aplicacao.Responses
{
    public record SolicitarCreditoResponse(
        string TrackingId,
        string Status,        
        string Message
);
}
