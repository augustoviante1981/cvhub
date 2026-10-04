using System.Text;
using cvhub.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace cvhub.Tests;

public class CurriculosControllerTests
{
    [Fact]
    public async Task SemArquivo_DeveRetornar400()
    {
        var controller = CriarController();

        var resultado = await controller.Extrair(null, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task ArquivoVazio_DeveRetornar400()
    {
        using var stream = new MemoryStream();
        var arquivo = CriarArquivo(stream, "curriculo.pdf");

        var resultado = await CriarController().Extrair(arquivo, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task ExtensaoInvalida_DeveRetornar400()
    {
        using var stream = new MemoryStream(Encoding.ASCII.GetBytes("%PDF-conteudo"));

        var arquivo = CriarArquivo(stream, "curriculo.txt");

        var resultado = await CriarController().Extrair(arquivo, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task TextoRenomeadoParaPdf_DeveRetornar400()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Este arquivo não é um PDF."));

        var arquivo = CriarArquivo(stream, "curriculo.pdf");

        var resultado = await CriarController().Extrair(arquivo, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task ArquivoMaiorQue5MB_DeveRetornar400()
    {
        var bytes = new byte[5 * 1024 * 1024 + 1];
        Encoding.ASCII.GetBytes("%PDF-").CopyTo(bytes, 0);

        using var stream = new MemoryStream(bytes);
        var arquivo = CriarArquivo(stream, "curriculo.pdf");

        var resultado = await CriarController().Extrair(arquivo, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task PdfCorrompido_DeveRetornar422()
    {
        using var stream = new MemoryStream(Encoding.ASCII.GetBytes("%PDF-1.7\narquivo corrompido"));

        var arquivo = CriarArquivo(stream, "curriculo.pdf");

        var resultado = await CriarController().Extrair(arquivo, CancellationToken.None);

        var resposta = Assert.IsType<UnprocessableEntityObjectResult>(resultado);

        Assert.Equal(422, resposta.StatusCode);
    }

    private static CurriculosController CriarController()
    {
        return new CurriculosController(NullLogger<CurriculosController>.Instance);
    }

    private static FormFile CriarArquivo(
        Stream stream, string nome)
    {
        return new FormFile(stream, 0, stream.Length, "arquivo", nome)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };
    }
}