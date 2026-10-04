using cvhub.Data;
using cvhub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace cvhub.Controllers
{
    [ApiController]
    [Route("api/candidatos")]
    
    
    public class CandidatosController : ControllerBase
    {
        private readonly cvhubDbContext _context;

        public CandidatosController(cvhubDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult<Candidato>> Criar([FromBody] Candidato candidato, CancellationToken cancellationToken)
        {
            // Estes valores são definidos pelo backend.
            candidato.Id = 0;
            candidato.DataCadastro = DateTime.UtcNow;

            candidato.NomeCompleto = candidato.NomeCompleto.Trim();
            candidato.Email = candidato.Email.Trim();
            candidato.Telefone = candidato.Telefone?.Trim();
            candidato.AreaInteresse = candidato.AreaInteresse?.Trim();
            candidato.ResumoProfissional = candidato.ResumoProfissional?.Trim();

            _context.Candidatos.Add(candidato);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = candidato.Id },
                candidato);
        }

        [HttpGet]
        public async Task<ActionResult<List<Candidato>>> Listar(CancellationToken cancellationToken)
        {
            var candidatos = await _context.Candidatos
                .AsNoTracking()
                .OrderByDescending(c => c.DataCadastro)
                .ThenByDescending(c => c.Id)
                .ToListAsync(cancellationToken);

            return Ok(candidatos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Candidato>> ObterPorId(int id, CancellationToken cancellationToken)
        {
            var candidato = await _context.Candidatos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    c => c.Id == id,
                    cancellationToken);

            if (candidato is null)
            {
                return NotFound(new
                {
                    mensagem = "Candidato não encontrado."
                });
            }

            return Ok(candidato);
        }
    }
}
