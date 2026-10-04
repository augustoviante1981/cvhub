using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace cvhub.Controllers
{
    [ApiController]
    [Route("api/curriculos")]
    public class CurriculosController : ControllerBase
    {
        private const long LimiteArquivo = 5 * 1024 * 1024;
        private readonly ILogger<CurriculosController> _logger;

        public CurriculosController(ILogger<CurriculosController> logger)
        {
            _logger = logger;
        }

        [HttpPost("extrair")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> Extrair(IFormFile? arquivo, CancellationToken cancellationToken)
        {
            if (arquivo is null || arquivo.Length == 0)
            {
                return BadRequest(new { mensagem = "Selecione um arquivo PDF." });
            }

            if (arquivo.Length > LimiteArquivo)
            {
                return BadRequest(new { mensagem = "O PDF deve ter no máximo 5 MB." });
            }

            if (!string.Equals(Path.GetExtension(arquivo.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    mensagem = "Envie um arquivo com extensão PDF."
                });
            }

            using var stream = new MemoryStream();
            await arquivo.CopyToAsync(stream, cancellationToken);

            // Confere também o conteúdo, não apenas a extensão.
            var assinatura = Encoding.ASCII.GetString(stream.GetBuffer(), 0, (int)Math.Min(stream.Length, 5));

            if (assinatura != "%PDF-")
            {
                return BadRequest(new { mensagem = "O arquivo enviado não é um PDF válido." });
            }

            stream.Position = 0;
            string texto;

            try
            {
                using var documento = PdfDocument.Open(stream);
                var conteudo = new StringBuilder();

                foreach (var pagina in documento.GetPages())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    conteudo.AppendLine(ContentOrderTextExtractor.GetText(pagina));
                }

                texto = conteudo.ToString();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha na leitura do currículo PDF.");

                return UnprocessableEntity(new { mensagem = "Não foi possível ler o PDF. " + "Preencha o formulário manualmente." });
            }

            if (string.IsNullOrWhiteSpace(texto))
            {
                return UnprocessableEntity(new { mensagem = "O PDF não contém texto extraível. " + "Preencha o formulário manualmente." });
            }

            var email = Encontrar(texto, @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b");

            var telefone = Encontrar(texto, @"(?<!\d)(?:\+?55[\s.-]*)?(?:\(\d{2}\)|\d{2})[\s.-]*9?\d{4}[\s.-]*\d{4}(?!\d)");

            var nome = IdentificarNome(texto);

            return Ok(new
            {
                nomeCompleto = nome,
                email,
                telefone,
                mensagem = "Leitura concluída. Revise os dados " +
                           "e complete os campos não identificados."
            });
        }

        private static string? Encontrar(string texto, string padrao)
        {
            var resultado = Regex.Match(
                texto,
                padrao,
                RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(1));

            return resultado.Success ? resultado.Value.Trim() : null;
        }

        private static string? IdentificarNome(string texto)
        {
            var linhas = texto.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            // Procura primeiro uma identificação explícita.
            foreach (var linha in linhas)
            {
                var resultado = Regex.Match(
                    linha.Trim(),
                    @"^Nome(?:\s+completo)?\s*:\s*(.+)$",
                    RegexOptions.IgnoreCase,
                    TimeSpan.FromSeconds(1));

                if (resultado.Success)
                {
                    var nome = resultado.Groups[1].Value.Trim();

                    if (NomePlausivel(nome))
                        return nome;
                }
            }

            // Alternativa simples: nome próximo ao início do documento.
            foreach (var linha in linhas.Take(5))
            {
                var nome = linha.Trim();

                if (NomePlausivel(nome) &&
                    !nome.Contains("currículo",
                        StringComparison.OrdinalIgnoreCase) &&
                    !nome.Contains("curriculo",
                        StringComparison.OrdinalIgnoreCase) &&
                    !nome.Contains("curriculum",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return nome;
                }
            }

            return null;
        }

        private static bool NomePlausivel(string nome)
        {
            return nome.Length <= 200 && Regex.IsMatch(
                nome,
                @"^\p{L}+(?:['’-]\p{L}+)*(?:\s+\p{L}+(?:['’-]\p{L}+)*){1,7}$",
                RegexOptions.None,
                TimeSpan.FromSeconds(1));
        }
    }
}
