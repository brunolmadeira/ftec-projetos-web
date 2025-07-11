// Arquivo: RedeSocialUsuario\RedeSocialUsuario.API\Controller\UsuarioController.cs

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using RedeSocialUsuario.Application.Application;
using RedeSocialUsuario.Application.Dto;
using RedeSocialUsuario.Application.DTO;
using RedeSocialUsuario.API.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RedeSocialUsuario.API.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas ao usuário.
    /// </summary>
    [ApiController]
    [Route("api/v1/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioApplication _usuarioApplication;
        private readonly TokenService _tokenService;

        /// <summary>
        /// Construtor do UsuarioController.
        /// </summary>
        /// <param name="usuarioApplication">Instância da camada de aplicação de usuário.</param>
        public UsuarioController(UsuarioApplication usuarioApplication, TokenService tokenService)
        {
            _usuarioApplication = usuarioApplication;
            _tokenService = tokenService;
        }


        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria um novo usuário",
            Description = "Cria um novo usuário na rede social. Requer uma chave de autorização especial no header (Bearer ProjetoWebRedeSocial)."
        )]

        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioRequest request)
        {
            // Verifica se o header Authorization existe
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return Unauthorized("Authorization header não encontrado.");
            }

            var token = authorizationHeader.ToString();
            if (!token.StartsWith("Bearer ") || token.Substring("Bearer ".Length).Trim() != "ProjetoWebRedeSocial")
            {
                return Unauthorized("Chave secreta inválida.");
            }

            var usuarioCriado = await _usuarioApplication.CriarUsuario(request);
            if (usuarioCriado == null)
            {
                // Retorna 400 se dados inválidos ou email/username já existem
                return BadRequest("Dados inválidos ou email/username já cadastrados.");
            }
            return CreatedAtAction(nameof(BuscarUsuarioPorId), new { id = usuarioCriado.Id }, usuarioCriado);
        }

        /// <summary>
        /// Busca um usuário pelo Id.
        /// </summary>
        /// <param name="id">Id do usuário.</param>
        /// <returns>Usuário encontrado.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Busca um usuário por ID",
            Description = "Retorna os dados do usuário correspondente ao ID informado, se existir."
        )]

        public async Task<IActionResult> BuscarUsuarioPorId([FromRoute] Guid id)
        {
            var usuario = await _usuarioApplication.BuscarUsuarioPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpGet("pesquisar-usuario-id/{id}")]
        [Authorize]
        public async Task<IActionResult> PesquisarUsuario([FromRoute] Guid id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return Unauthorized("Authorization header não encontrado.");

            var token = authorizationHeader.ToString().Replace("Bearer ", "").Trim();

            var principal = _tokenService.ValidarToken(token);
            if (principal == null)
                return Unauthorized("Token inválido.");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??
                              principal.FindFirst("sub") ??
                              principal.FindFirst("id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("ID do usuário inválido no token.");

            if (id == Guid.Empty)
                return BadRequest("Informe o ID ou o Username para pesquisar.");

            UsuarioDto usuario = null;
            
            usuario = await _usuarioApplication.BuscarUsuarioPorId(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            // Retorna apenas dados não sensíveis
            return Ok(new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Username,
                usuario.Bio,
                usuario.Pronome,
                usuario.FotoBase64,
                usuario.Link
            });
        }

        [HttpGet("pesquisar-usuario-username/{username}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Busca um usuário por ID",
            Description = "Retorna os dados públicos de um usuário (nome, username, bio, pronome, foto e link) com base no ID informado. Requer autenticação via token JWT."
        )]

        public async Task<IActionResult> PesquisarUsuario([FromRoute] string username)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return Unauthorized("Authorization header não encontrado.");

            var token = authorizationHeader.ToString().Replace("Bearer ", "").Trim();

            var principal = _tokenService.ValidarToken(token);
            if (principal == null)
                return Unauthorized("Token inválido.");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??
                              principal.FindFirst("sub") ??
                              principal.FindFirst("id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("ID do usuário inválido no token.");

            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("Informe o Username para pesquisar.");

            UsuarioDto usuario = null;

            if (username != null)
                usuario = await _usuarioApplication.BuscarUsuarioPorUsername(username);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            // Retorna apenas dados não sensíveis
            return Ok(new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Username,
                usuario.Bio,
                usuario.Pronome,
                usuario.FotoBase64,
                usuario.Link
            });
        }

        [HttpGet("pesquisar-usuarios")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Pesquisa usuários por termo (nome ou username)",
            Description = "Retorna uma lista de usuários com os campos públicos (ID, nome, username e foto) que contenham o termo pesquisado. Caso o termo não seja informado, retorna todos os usuários. Requer autenticação via token JWT."
        )]

        public async Task<IActionResult> PesquisarUsuarios([FromQuery] string? termo=null)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return Unauthorized("Authorization header não encontrado.");

            var token = authorizationHeader.ToString().Replace("Bearer ", "").Trim();

            var principal = _tokenService.ValidarToken(token);
            if (principal == null)
                return Unauthorized("Token inválido.");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??
                              principal.FindFirst("sub") ??
                              principal.FindFirst("id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("ID do usuário inválido no token.");

            // Consulta de todos os usuários (filtrando os dados não sensíveis)
             var usuarios = await _usuarioApplication.PesquisarUsuariosPorTermoAsync(termo);

            var resultado = usuarios.Select(u => new
            {
                u.Id,
                u.Username,
                u.Nome,
                Foto = u.FotoBase64
            });

            return Ok(resultado);
        }
        

        [HttpPut()]
        [SwaggerOperation(
            Summary = "Atualiza os dados do usuário autenticado",
            Description = "Permite que o usuário autenticado atualize parcialmente seus dados, como nome, celular, pronome, bio, link, data de nascimento e foto. Todos os campos são opcionais. Requer token JWT no header Authorization."
        )]

        public async Task<IActionResult> AtualizarUsuario([FromBody] AtualizarUsuarioDto dto)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return Unauthorized("Authorization header não encontrado.");

            var token = authorizationHeader.ToString().Replace("Bearer ", "").Trim();

            var principal = _tokenService.ValidarToken(token);
            if (principal == null)
                return Unauthorized("Token inválido.");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??
                              principal.FindFirst("sub") ??
                              principal.FindFirst("id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("ID do usuário inválido no token.");

            var usuario = await _usuarioApplication.BuscarUsuarioPorId(userId);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            // Atualiza apenas os campos fornecidos
            if (!string.IsNullOrWhiteSpace(dto.Nome)) usuario.Nome = dto.Nome;
            if (!string.IsNullOrWhiteSpace(dto.Celular)) usuario.Celular = dto.Celular;
            if (!string.IsNullOrWhiteSpace(dto.Pronome)) usuario.Pronome = dto.Pronome;
            if (!string.IsNullOrWhiteSpace(dto.Bio)) usuario.Bio = dto.Bio;
            if (!string.IsNullOrWhiteSpace(dto.Link)) usuario.Link = dto.Link;
            if (dto.DataNascimento.HasValue) usuario.DataNascimento = dto.DataNascimento;
            if (!string.IsNullOrWhiteSpace(dto.FotoBase64)) usuario.FotoBase64 = dto.FotoBase64;

            usuario.CriadoEm = DateTime.UtcNow;

            var atualizado = await _usuarioApplication.AtualizarUsuarioAsync(userId, dto);
            if (!atualizado)
                return NotFound("Usuário não encontrado.");

            if (atualizado == null)
                return StatusCode(500, "Erro ao atualizar usuário.");

            return Ok(new { mensagem = "Usuário atualizado com sucesso." });
        }


        /// <summary>
        /// Exclui o usuário autenticado.
        /// </summary>
        /// <param name="id">Pega o ID pelo token de autenticação do usuário.</param>
        /// <returns>Status da exclusão.</returns>
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Exclui o usuário autenticado",
            Description = "Remove permanentemente o usuário autenticado do sistema com base no token JWT fornecido no header Authorization."
        )]

        public async Task<IActionResult> ExcluirUsuarioAutenticado()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return Unauthorized("Authorization header não encontrado.");

            var token = authorizationHeader.ToString().Replace("Bearer ", "").Trim();

            var principal = _tokenService.ValidarToken(token);
            if (principal == null)
                return Unauthorized("Token inválido.");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??  // Padrão ASP.NET
                              principal.FindFirst("sub") ??                     // Padrão JWT
                              principal.FindFirst("id");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("ID do usuário inválido no token.");

            var excluido = await _usuarioApplication.ExcluirUsuario(userId);
            if (!excluido)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            return Ok(new { mensagem = "Usuário deletado com sucesso." });
        }


        /// <summary>
        /// Verifica se um e-mail está disponível para cadastro.
        /// </summary>
        /// <param name="email">E-mail a ser verificado.</param>
        /// <returns>True se disponível, false se já existe.</returns>
        [HttpGet("verifica-email")]
        [SwaggerOperation(
            Summary = "Verifica disponibilidade de e-mail",
            Description = "Verifica se um e-mail já está cadastrado no sistema. Retorna true se estiver disponível."
        )]
                [SwaggerResponse(200, "Retorna true se o e-mail estiver disponível, false caso contrário.")]
                [SwaggerResponse(400, "E-mail não informado.")]
        public async Task<IActionResult> CheckEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("E-mail não informado.");
            }

            var disponivel = await _usuarioApplication.VerificarEmailDisponivel(email);
            return Ok(new { disponivel });
        }

        /// <summary>
        /// Verifica se um username está disponível para cadastro.
        /// </summary>
        /// <param name="username">Username a ser verificado.</param>
        /// <returns>True se disponível, false se já existe.</returns>
        [HttpGet("verifica-username")]
        [SwaggerOperation(
    Summary = "Verifica disponibilidade de username",
            Description = "Verifica se um username já está cadastrado no sistema. Retorna true se estiver disponível."
        )]
        [SwaggerResponse(200, "Retorna true se o username estiver disponível, false caso contrário.")]
        [SwaggerResponse(400, "Username não informado.")]
        public async Task<IActionResult> CheckUsername([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username não informado.");
            }

            var disponivel = await _usuarioApplication.VerificarUsernameDisponivel(username);
            return Ok(new { disponivel });
        }

        [HttpPost("seguir")]
        [Authorize]
        [Authorize]
        [SwaggerOperation(
            Summary = "Seguir um usuário",
            Description = "Permite que o usuário autenticado siga outro usuário pelo ID ou username. É necessário estar autenticado com token JWT."
        )]
        [SwaggerResponse(200, "Usuário seguido com sucesso.")]
        [SwaggerResponse(400, "Requisição malformada ou parâmetros inválidos.")]
        [SwaggerResponse(401, "Token ausente ou inválido.")]
        [SwaggerResponse(404, "Usuário a ser seguido não encontrado.")]
        public async Task<IActionResult> Seguir([FromQuery] Guid? id, [FromQuery] string username)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return Unauthorized("Authorization header não encontrado.");

            var token = authorizationHeader.ToString().Replace("Bearer ", "").Trim();

            var principal = _tokenService.ValidarToken(token);
            if (principal == null)
                return Unauthorized("Token inválido.");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??  // Padrão ASP.NET
                              principal.FindFirst("sub") ??                     // Padrão JWT
                              principal.FindFirst("id");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("ID do usuário inválido no token.");

            UsuarioDto usuarioParaSeguir = null;

            if (id != null)
                usuarioParaSeguir = await _usuarioApplication.BuscarUsuarioPorId(id.Value);
            else if (!string.IsNullOrWhiteSpace(username))
                usuarioParaSeguir = await _usuarioApplication.BuscarUsuarioPorUsername(username);

            if (usuarioParaSeguir == null)
                return NotFound("Usuário a ser seguido não encontrado.");

            await _usuarioApplication.SeguirUsuario(userId, usuarioParaSeguir.Id);

            return Ok("Agora você está seguindo este usuário.");
        }

        [HttpPost("parar-de-seguir")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Parar de seguir um usuário",
            Description = "Permite que o usuário autenticado deixe de seguir outro usuário pelo ID ou username. É necessário estar autenticado com token JWT."
        )]
        [SwaggerResponse(200, "Usuário seguido com sucesso.")]
        [SwaggerResponse(400, "Requisição malformada ou parâmetros inválidos.")]
        [SwaggerResponse(401, "Token ausente ou inválido.")]
        [SwaggerResponse(404, "Usuário a ser seguido não encontrado.")]
        [Authorize]
        public async Task<IActionResult> PararDeSeguir([FromQuery] Guid? id, [FromQuery] string username)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return Unauthorized("Authorization header não encontrado.");

            var token = authorizationHeader.ToString().Replace("Bearer ", "").Trim();

            var principal = _tokenService.ValidarToken(token);
            if (principal == null)
                return Unauthorized("Token inválido.");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??  // Padrão ASP.NET
                              principal.FindFirst("sub") ??                     // Padrão JWT
                              principal.FindFirst("id");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("ID do usuário inválido no token.");

            UsuarioDto usuarioParaParar = null;

            if (id != null)
                usuarioParaParar = await _usuarioApplication.BuscarUsuarioPorId(id.Value);
            else if (!string.IsNullOrWhiteSpace(username))
                usuarioParaParar = await _usuarioApplication.BuscarUsuarioPorUsername(username);

            if (usuarioParaParar == null)
                return NotFound("Usuário não encontrado.");

            await _usuarioApplication.PararDeSeguirUsuario(userId, usuarioParaParar.Id);

            return Ok("Você deixou de seguir este usuário.");
        }
    }
}