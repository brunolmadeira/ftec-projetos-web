// Arquivo: RedeSocialUsuario\RedeSocialUsuario.API\Controller\LoginController.cs

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using RedeSocialUsuario.Application.Application;
using RedeSocialUsuario.Application.Dto;
using RedeSocialUsuario.API.Services;


namespace RedeSocialUsuario.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo login de usuários.
    /// </summary>
    [ApiController]
    [Route("api/v1/usuarios/login")]
    public class LoginController : ControllerBase
    {
        private readonly UsuarioApplication _usuarioApplication;
        private readonly TokenService _tokenService;

        /// <summary>
        /// Construtor do LoginController.
        /// </summary>
        /// <param name="usuarioApplication">Aplicação de usuário.</param>
        /// <param name="tokenService">Serviço de geração de token JWT.</param>
        public LoginController(UsuarioApplication usuarioApplication, TokenService tokenService)
        {
            _usuarioApplication = usuarioApplication;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Realiza o login do usuário e retorna um token JWT se autenticado.
        /// </summary>
        /// <param name="request">Objeto contendo login (email ou username) e senha.</param>
        /// <returns>Token JWT e dados do usuário autenticado.</returns>
        [HttpPost]
        [SwaggerOperation(
    Summary = "Autentica um usuário no sistema",
    Description = @"Realiza o login do usuário verificando as credenciais e retorna um token JWT para autenticação.
                    <br><br><b>Pré-condições:</b>
                    <br>- O header Authorization deve conter a chave secreta 'ProjetoWebRedeSocial'
                    <br>- Login e senha devem ser fornecidos no corpo da requisição
                    <br><br><b>Pós-condições:</b>
                    <br>- Retorna token JWT válido para autenticação
                    <br>- Retorna dados básicos do usuário autenticado")]
        [SwaggerResponse(200, "Autenticação bem-sucedida")]
        [SwaggerResponse(400, "Dados de login inválidos ou incompletos")]
        [SwaggerResponse(401, "Chave secreta inválida ou credenciais incorretas")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            // Verifica se o header Authorization existe
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return Unauthorized("Authorization header não encontrado.");
            }

            var tokenHerader = authorizationHeader.ToString();
            if (!tokenHerader.StartsWith("Bearer ") || tokenHerader.Substring("Bearer ".Length).Trim() != "ProjetoWebRedeSocial")
            {
                return Unauthorized("Chave secreta inválida.");
            }

            // Valida se o request está preenchido
            if (request == null || string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Senha))
                return BadRequest(new { mensagem = "Login e senha são obrigatórios." });

            // Tenta autenticar o usuário
            UsuarioDto usuario = await _usuarioApplication.LoginUsuario(request.Login, request.Senha);

            if (usuario == null)
                return Unauthorized(new { mensagem = "Login ou senha inválidos." });

            // Gera o token JWT
            string token = _tokenService.GenerateToken(usuario.Id.ToString(), usuario.Username);

            // Retorna o token e dados do usuário autenticado
            return Ok(new
            {
                token,
                usuario = new
                {
                    usuario.Id,
                    usuario.Username,
                    usuario.Email
                }
            });
        }

        /// <summary>
        /// Realiza o login do usuário e retorna um token JWT se autenticado.
        /// </summary>
        /// <param name="request">Objeto contendo login (email ou username) e senha.</param>
        /// <returns>Token JWT e dados do usuário autenticado.</returns>
        [HttpPost("alterar-senha-token")]
        [SwaggerOperation(
            Summary = "Solicita alteração de senha por token",
            Description = @"Envia um token de recuperação para o e-mail do usuário.
                           <br><br><b>Fluxo:</b>
                           <br>1. Sistema gera um token numérico de 6 dígitos
                           <br>2. Token é válido por 15 minutos
                           <br>3. Token é enviado para o e-mail do usuário
                           <br><br><b>Pré-condições:</b>
                           <br>- Header Authorization com chave secreta válida
                           <br>- E-mail do usuário deve ser fornecido
                           <br><br><b>Pós-condições:</b>
                           <br>- Token é armazenado no banco de dados.
                           <br>- Webhook é acionado com os dados de recuperação")]
        [SwaggerResponse(200, "Token gerado e enviado com sucesso")]
        [SwaggerResponse(400, "E-mail inválido ou usuário não encontrado")]
        [SwaggerResponse(401, "Chave secreta inválida ou não fornecida")]
        public async Task<IActionResult> SolicitarAlteracaoSenha([FromBody] SolicitarAlteracaoSenhaRequest request)
        {

            // Verifica se o header Authorization existe
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return Unauthorized("Authorization header não encontrado.");
            }

            var tokenHerader = authorizationHeader.ToString();
            if (!tokenHerader.StartsWith("Bearer ") || tokenHerader.Substring("Bearer ".Length).Trim() != "ProjetoWebRedeSocial")
            {
                return Unauthorized("Chave secreta inválida.");
            }

            // Valida se o request está preenchido
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { mensagem = "Email é obrigatório" });
            }

            var usuario = await _usuarioApplication.BuscarUsuarioPorEmail(request.Email);
            if (usuario == null)
                return BadRequest(new { mensagem = "Dados inválidos ou usuário não encontrado." });

            // Gera código e hora de expiração
            string codigo = new Random().Next(000000, 999999).ToString();
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset horaBrasilia = utcNow.ToOffset(new TimeSpan(-3, 0, 0));
            DateTimeOffset expiracao = horaBrasilia.AddMinutes(15);

            // Salvar no banco
            await _usuarioApplication.SalvarCodigoRecuperacao(request.Email, codigo, expiracao);

            // Dispara webhook externo (você pode substituir por envio de email real futuramente)
            using var httpClient = new HttpClient();
            await httpClient.PostAsJsonAsync("https://webhook.XXXXXXXXX/webhook/ftec-rede-social-usuario", new
            {
                email = request.Email,
                codigo,
                expiracao
            });

            return Ok("Token enviado para o email do usuário.");
        }

        /// <summary>
        /// Realiza o login do usuário e retorna um token JWT se autenticado.
        /// </summary>
        /// <param name="request">Objeto contendo login (email ou username) e senha.</param>
        /// <returns>Token JWT e dados do usuário autenticado.</returns>
        [HttpPost("alterar-senha")]
        [SwaggerOperation(
            Summary = "Altera a senha do usuário usando token de recuperação",
            Description = @"Realiza a alteração da senha após validação do token de recuperação.
                           <br><br><b>Fluxo:</b>
                           <br>1. Valida o token de recuperação (código de 6 dígitos)
                           <br>2. Verifica se o token ainda está dentro do prazo de validade
                           <br>3. Atualiza a senha com hash BCrypt
                           <br>4. Invalida o token após uso
                           <br><br><b>Pré-condições:</b>
                           <br>- Header Authorization com chave secreta válida
                           <br>- E-mail, nova senha e código de recuperação devem ser fornecidos
                           <br>- Token deve ter sido previamente solicitado e ainda ser válido
                           <br><br><b>Segurança:</b>
                           <br>- A senha é armazenada com hash BCrypt
                           <br>- O token de recuperação é invalidado após uso")]
        [SwaggerResponse(200, "Senha alterada com sucesso")]
        [SwaggerResponse(400, "Campos obrigatórios não fornecidos ou token inválido")]
        [SwaggerResponse(401, "Chave secreta inválida ou não fornecida")]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequest request)
        {

            // Verifica se o header Authorization existe
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return Unauthorized("Authorization header não encontrado.");
            }

            var tokenHerader = authorizationHeader.ToString();
            if (!tokenHerader.StartsWith("Bearer ") || tokenHerader.Substring("Bearer ".Length).Trim() != "ProjetoWebRedeSocial")
            {
                return Unauthorized("Chave secreta inválida.");
            }

            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Senha) ||
                string.IsNullOrWhiteSpace(request.CodigoRecuperacaoSenha))
            {
                return BadRequest("Todos os campos são obrigatórios.");
            }

            var validacao = await _usuarioApplication.ValidarCodigoRecuperacao(request.Email, request.CodigoRecuperacaoSenha);
            if (!validacao.Sucesso)
                return BadRequest(validacao.Mensagem);

            // Atualiza senha
            string novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            await _usuarioApplication.AtualizarSenhaUsuario(request.Email, novaSenhaHash);

            // Invalida código (pode remover ou marcar como usado)
            await _usuarioApplication.InvalidarCodigoRecuperacao(request.Email);

            return Ok("Senha alterada com sucesso");
        }



        /// <summary>
        /// Modelo de request para login.
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// Email ou nome de usuário.
            /// </summary>
            public string Login { get; set; }

            /// <summary>
            /// Senha em texto puro.
            /// </summary>
            public string Senha { get; set; }
        }

        public class SolicitarAlteracaoSenhaRequest
        {
            /// <summary>
            /// Email ou nome de usuário.
            /// </summary>
            public string Email { get; set; }

        }

        public class AlterarSenhaRequest
        {
            /// <summary>
            /// Email ou nome de usuário.
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Senha em texto puro.
            /// </summary>
            public string Senha { get; set; }

            /// <summary>
            /// Código recebido no email
            /// </summary>
            public string CodigoRecuperacaoSenha { get; set; }
        }
    }
}