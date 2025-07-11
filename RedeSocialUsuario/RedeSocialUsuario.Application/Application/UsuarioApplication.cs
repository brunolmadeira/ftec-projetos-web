// Arquivo: RedeSocialUsuario\RedeSocialUsuario.Application\Application\UsuarioApplication.cs

using System;
using System.Threading.Tasks;
using RedeSocialUsuario.Domain.Repository;
using RedeSocialUsuario.Domain.Entities;
using RedeSocialUsuario.Application.Adapter;
using RedeSocialUsuario.Application.Dto;
using RedeSocialUsuario.Application.DTO;
using RedeSocialUsuario.Application.Validators;
using FluentValidation.Results;

namespace RedeSocialUsuario.Application.Application
{
    /// <summary>
    /// Classe de aplica��o para opera��es relacionadas ao usu�rio.
    /// </summary>
    public class UsuarioApplication
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly CriarUsuarioRequestValidator _criarUsuarioValidator;

        /// <summary>
        /// Construtor da classe UsuarioApplication.
        /// </summary>
        /// <param name="usuarioRepository">Reposit�rio de usu�rio.</param>
        public UsuarioApplication(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _criarUsuarioValidator = new CriarUsuarioRequestValidator();
        }

        /// <summary>
        /// Cria um novo usu�rio ap�s valida��o e convers�o.
        /// </summary>
        /// <param name="request">Requisi��o de cria��o de usu�rio.</param>
        /// <returns>DTO do usu�rio criado ou null se inv�lido.</returns>
        public async Task<UsuarioDto> CriarUsuario(CriarUsuarioRequest request)
        {
            // Valida��o do request usando FluentValidation
            ValidationResult validationResult = _criarUsuarioValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                // Retorne null ou lance exce��o conforme pol�tica da aplica��o
                return null;
            }

            // Verifica se email ou username j� existem
            if (await _usuarioRepository.VerificarEmailExistenteAsync(request.Email) ||
                await _usuarioRepository.VerificarUsernameExistenteAsync(request.Username))
            {
                // Retorne null ou lance exce��o conforme pol�tica da aplica��o
                return null;
            }

            // Hash da senha (exemplo simples, substitua por hash seguro em produ��o)
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            // Adapta request para entidade Usuario
            Usuario usuario = UsuarioAdapter.FromCriarRequest(request, senhaHash);

            // Persiste usu�rio
            Usuario usuarioCriado = await _usuarioRepository.CriarUsuarioAsync(usuario);

            // Retorna DTO
            return UsuarioAdapter.ToDto(usuarioCriado);
        }

        /// <summary>
        /// Realiza login do usu�rio validando email/username e senha.
        /// </summary>
        /// <param name="login">Login (email ou username).</param>
        /// <param name="senha">Senha em texto puro.</param>
        /// <returns>DTO do usu�rio autenticado ou null se falhar.</returns>
        public async Task<UsuarioDto> LoginUsuario(string login, string senha)
        {
            Usuario usuario = null;

            // Tenta buscar por email
            usuario = await _usuarioRepository.BuscarPorEmailAsync(login);

            // Se n�o encontrou por email, tenta por username
            if (usuario == null)
                usuario = await _usuarioRepository.BuscarPorUsernameAsync(login);

            if (usuario == null)
                return null;

            // Verifica senha (substitua por hash seguro em produ��o)
            bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
            if (!senhaValida)
                return null;

            return UsuarioAdapter.ToDto(usuario);
        }

        public async Task SeguirUsuario(Guid idSeguidor, Guid idParaSeguir)
        {
            await _usuarioRepository.InserirSeguidorAsync(idSeguidor, idParaSeguir);
        }

        public async Task PararDeSeguirUsuario(Guid idSeguidor, Guid idParaParar)
        {
            await _usuarioRepository.DeletarSeguidorAsync(idSeguidor, idParaParar);
        }



        public async Task<List<UsuarioDto>> PesquisarUsuarios(string termo)
        {
            var usuarios = await _usuarioRepository.PesquisarUsuariosAsync(termo);
            return usuarios.Select(UsuarioAdapter.ToDto).ToList();
        }


        /// <summary>
        /// Busca um usu�rio pelo Id.
        /// </summary>
        /// <param name="id">Identificador do usu�rio.</param>
        /// <returns>DTO do usu�rio ou null se n�o encontrado.</returns>
        public async Task<UsuarioDto> BuscarUsuarioPorEmail(string email)
        {
            Usuario usuario = null;

            // Tenta buscar por email
            usuario = await _usuarioRepository.BuscarPorEmailAsync(email);
            if (usuario == null)
                return null;

            return UsuarioAdapter.ToDto(usuario);
        }

        public async Task<UsuarioDto> SalvarCodigoRecuperacao(string email, string codigo, DateTimeOffset expiracao)
        {
            await _usuarioRepository.SalvarCodigoRecuperacaoAsync(email, codigo, expiracao);
            return await BuscarUsuarioPorEmail(email);
        }

        public async Task<(bool Sucesso, string Mensagem)> ValidarCodigoRecuperacao(string email, string codigo)
        {
            var dados = await _usuarioRepository.ObterCodigoRecuperacaoAsync(email);
            if (dados == null)
                return (false, "C�digo de recupera��o n�o encontrado.");

            if (dados.Value.Codigo != codigo)
                return (false, "C�digo inv�lido.");

            if (dados.Value.Expiracao < DateTime.UtcNow)
                return (false, "C�digo expirado.");

            return (true, "C�digo v�lido.");
        }

        public async Task AtualizarSenhaUsuario(string email, string novaSenhaHash)
        {
            await _usuarioRepository.AtualizarSenhaAsync(email, novaSenhaHash);
        }

        public async Task InvalidarCodigoRecuperacao(string email)
        {
            await _usuarioRepository.InvalidarCodigoRecuperacaoAsync(email);
        }


        /// <summary>
        /// Busca um usu�rio pelo Id.
        /// </summary>
        /// <param name="id">Identificador do usu�rio.</param>
        /// <returns>DTO do usu�rio ou null se n�o encontrado.</returns>
        public async Task<UsuarioDto> BuscarUsuarioPorId(Guid id)
        {
            Usuario usuario = await _usuarioRepository.BuscarPorIdAsync(id);
            return UsuarioAdapter.ToDto(usuario);
        }

        /// <summary>
        /// Atualiza os dados de um usu�rio.
        /// </summary>
        /// <param name="usuarioDto">DTO com dados atualizados.</param>
        /// <returns>DTO do usu�rio atualizado ou null se n�o encontrado.</returns>
        public async Task<bool> AtualizarUsuarioAsync(Guid id, AtualizarUsuarioDto dto)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
            if (usuario == null)
                return false;

            // Atualiza somente os campos fornecidos
            if (!string.IsNullOrWhiteSpace(dto.Nome)) usuario.Nome = dto.Nome;
            if (!string.IsNullOrWhiteSpace(dto.Celular)) usuario.Celular = dto.Celular;
            if (!string.IsNullOrWhiteSpace(dto.Pronome)) usuario.Pronome = dto.Pronome;
            if (!string.IsNullOrWhiteSpace(dto.Bio)) usuario.Bio = dto.Bio;
            if (!string.IsNullOrWhiteSpace(dto.Link)) usuario.Link = dto.Link;
            if (dto.DataNascimento.HasValue) usuario.DataNascimento = dto.DataNascimento;
            if (!string.IsNullOrWhiteSpace(dto.FotoBase64)) usuario.FotoBase64 = dto.FotoBase64;

            usuario.AtualizadoEm = DateTime.UtcNow;

            await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            return true;
        }

        /// <summary>
        /// Exclui um usu�rio pelo Id.
        /// </summary>
        /// <param name="id">Identificador do usu�rio.</param>
        /// <returns>True se exclu�do, false se n�o encontrado.</returns>
        public async Task<bool> ExcluirUsuario(Guid id)
        {
            Usuario usuario = await _usuarioRepository.BuscarPorIdAsync(id);
            if (usuario == null)
                return false;

            await _usuarioRepository.DeletarUsuarioAsync(id);
            return true;
        }

        public async Task<UsuarioDto> BuscarUsuarioPorUsername(string username)
        {
            var usuario = await _usuarioRepository.BuscarPorUsernameAsync(username);
            if (usuario == null)
                return null;

            return UsuarioAdapter.ToDto(usuario);
        }


        /// <summary>
        /// Verifica se um e-mail est� dispon�vel para cadastro.
        /// </summary>
        /// <param name="email">E-mail a ser verificado.</param>
        /// <returns>True se dispon�vel, false se j� existe.</returns>
        public async Task<bool> VerificarEmailDisponivel(string email)
        {
            return !(await _usuarioRepository.VerificarEmailExistenteAsync(email));
        }

        /// <summary>
        /// Verifica se um username est� dispon�vel para cadastro.
        /// </summary>
        /// <param name="username">Username a ser verificado.</param>
        /// <returns>True se dispon�vel, false se j� existe.</returns>
        public async Task<bool> VerificarUsernameDisponivel(string username)
        {
            return !(await _usuarioRepository.VerificarUsernameExistenteAsync(username));
        }

        public async Task<List<UsuarioDto>> PesquisarUsuariosPorTermoAsync(string? termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return (await _usuarioRepository.BuscarTodosUsuariosAsync())
                       .Select(MapearUsuarioParaDto)
                       .ToList();

            return (await _usuarioRepository.PesquisarUsuariosPorTermoAsync(termo))
                   .Select(MapearUsuarioParaDto)
                   .ToList();
        }

        private UsuarioDto MapearUsuarioParaDto(Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Username = usuario.Username,
                FotoBase64 = usuario.FotoBase64,
                Bio = usuario.Bio,
                Pronome = usuario.Pronome,
                Link = usuario.Link
            };
        }
    }
}