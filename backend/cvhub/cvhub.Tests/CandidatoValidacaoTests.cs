using System.ComponentModel.DataAnnotations;
using cvhub.Models;
using Xunit;

namespace cvhub.Tests;

public class CandidatoValidacaoTests
{
    [Fact]
    public void CadastroComCamposObrigatorios_DeveSerValido()
    {
        var candidato = new Candidato
        {
            NomeCompleto = "Mariana Souza",
            Email = "mariana@example.com"
        };

        var erros = Validar(candidato);

        Assert.Empty(erros);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NomeAusente_DeveSerRejeitado(string nome)
    {
        var candidato = new Candidato
        {
            NomeCompleto = nome,
            Email = "mariana@example.com"
        };

        var erros = Validar(candidato);

        Assert.Contains(erros, erro => erro.MemberNames.Contains(nameof(Candidato.NomeCompleto)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    [InlineData("mariana@")]
    public void EmailInvalido_DeveSerRejeitado(string email)
    {
        var candidato = new Candidato
        {
            NomeCompleto = "Mariana Souza",
            Email = email
        };

        var erros = Validar(candidato);

        Assert.Contains(erros, erro => erro.MemberNames.Contains(nameof(Candidato.Email)));
    }

    [Fact]
    public void NomeAcimaDoLimite_DeveSerRejeitado()
    {
        var candidato = new Candidato
        {
            NomeCompleto = new string('A', 201),
            Email = "mariana@example.com"
        };

        var erros = Validar(candidato);

        Assert.Contains(erros, erro => erro.MemberNames.Contains(nameof(Candidato.NomeCompleto)));
    }

    private static List<ValidationResult> Validar(Candidato candidato)
    {
        var erros = new List<ValidationResult>();

        Validator.TryValidateObject(candidato, new ValidationContext(candidato), erros, validateAllProperties: true);

        return erros;
    }
}