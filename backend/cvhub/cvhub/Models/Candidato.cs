using System.ComponentModel.DataAnnotations;

namespace cvhub.Models
{
    public class Candidato
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome completo.")]
        [MaxLength(200)]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Telefone { get; set; }

        [MaxLength(150)]
        public string? AreaInteresse { get; set; }

        [MaxLength(4000)]
        public string? ResumoProfissional { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
