using api.solicitacao.credito.Infraestrutura;
using api.solicitacao.credito.Aplicacao.Requests;
using api.solicitacao.credito.Servicos;
using Microsoft.EntityFrameworkCore;
using api.solicitacao.credito.Dominio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/solicitacoes", async (
    HttpRequest http,
    SolicitarCreditoRequest request,
    IdempotencyService idempotencyService,
    Contexto contexto, 
    CancellationToken ct) =>
{
    
    var idempotencyKey = http.Headers["Idempotency-Key"].FirstOrDefault()
                      ?? idempotencyService.GerarIdempotencyKey(request);

    var correlationId = http.Headers["X-Correlation-Id"].FirstOrDefault()
                     ?? idempotencyService.GerarCorrelationId(request);
    
    var jaExiste = await contexto.Set<OutboxMessage>()
        .AsNoTracking()
        .AnyAsync(x => x.IdempotencyKey == idempotencyKey, ct);

    if (jaExiste)
    {
        return Results.Ok(new
        {
            message = "Solicitação já recebida (idempotente).",
            idempotencyKey,
            correlationId
        });
    }
    
    using var tx = await contexto.Database.BeginTransactionAsync(ct);
    
    var payloadJson = System.Text.Json.JsonSerializer.Serialize(new
    {
        Type = "CreditoSolicitado",
        OccurredAtUtc = DateTime.UtcNow,
        Data = request,
        CorrelationId = correlationId
    });

    var outbox = OutboxMessage.Criar(
        tipoMensagem: "CreditoSolicitado",
        payloadJson: payloadJson,
        correlationId: correlationId,
        idempotencyKey: idempotencyKey
    );

    contexto.Set<OutboxMessage>().Add(outbox);
    await contexto.SaveChangesAsync(ct);
    await tx.CommitAsync(ct);

    return Results.Accepted($"/solicitacoes/{outbox.Id}", new
    {
        message = "Solicitação registrada com sucesso e será processada em breve.",
        trackingId = outbox.Id,
        idempotencyKey,
        correlationId
    });
});

app.Run();