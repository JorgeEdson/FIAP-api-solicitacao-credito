using api.solicitacao.credito.Aplicacao.Requests;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace api.solicitacao.credito.Servicos
{
    public class IdempotencyService
    {        
        public string GerarIdempotencyKey(SolicitarCreditoRequest req, DateTime? logicalClock = null)
        {            
            var cpf = SomenteDigitos(req.Cpf);
            var valor = req.ValorSolicitado.ToString(CultureInfo.InvariantCulture);  
            var prazo = req.PrazoMeses.ToString(CultureInfo.InvariantCulture);
            var canal = (req.Canal ?? "default").Trim().ToLowerInvariant();
            
            var canonical = $"cpf={cpf}|valor={valor}|prazo={prazo}|canal={canal}";
            
            return Sha256Hex(canonical);
        }

        private static string SomenteDigitos(string input)
        {
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
                if (char.IsDigit(ch)) sb.Append(ch);
            return sb.ToString();
        }

        private static string Sha256Hex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes); 
        }

        public string GerarCorrelationId(SolicitarCreditoRequest req)
        {   
            var seed = $"{SomenteDigitos(req.Cpf)}-{req.PrazoMeses}-{req.ValorSolicitado.ToString(CultureInfo.InvariantCulture)}";
            var prefix = Sha256Hex(seed)[..16]; 
            return $"{prefix}-{Guid.NewGuid()}";
        }
    }
}
