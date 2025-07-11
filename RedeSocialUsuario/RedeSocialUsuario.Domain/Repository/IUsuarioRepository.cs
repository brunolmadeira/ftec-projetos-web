// Arquivo: RedeSocialUsuario\RedeSocialUsuario.Domain\Repository\IUsuarioRepository.cs
using System;
using System.Threading.Tasks;
using RedeSocialUsuario.Domain.Entities;

namespace RedeSocialUsuario.Domain.Repository
{
    /// <summary>
    /// Interface para operações de persistência relacionadas ao usuário.
    /// </summary>
    public interface IUsuarioRepository
    {
        /// <summary>
        /// Cria um novo usuário de forma assíncrona.
        /// </summary>
        /// <param name="usuario">Entidade do usuário a ser criada.</param>
        /// <returns>Usuário criado.</returns>
        Task<Usuario> CriarUsuarioAsync(Usuario usuario);

        /// <summary>
        /// Busca um usuário pelo Id de forma assíncrona.
        /// </summary>
        /// <param name="id">Identificador único do usuário.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        Task<Usuario> BuscarPorIdAsync(Guid id);

        /// <summary>
        /// Busca um usuário pelo e-mail de forma assíncrona.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        Task<Usuario> BuscarPorEmailAsync(string email);

        /// <summary>
        /// Busca um usuário pelo username de forma assíncrona.
        /// </summary>
        /// <param name="username">Nome de usuário.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        Task<Usuario> BuscarPorUsernameAsync(string username);

        /// <summary>
        /// Atualiza os dados de um usuário de forma assíncrona.
        /// </summary>
        /// <param name="usuario">Entidade do usuário com dados atualizados.</param>
        /// <returns>Usuário atualizado.</returns>
        Task<Usuario> AtualizarUsuarioAsync(Usuario usuario);

        /// <summary>
        /// Deleta um usuário de forma assíncrona.
        /// </summary>
        /// <param name="id">Identificador único do usuário.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        Task DeletarUsuarioAsync(Guid id);

        /// <summary>
        /// Verifica se um e-mail já está cadastrado de forma assíncrona.
        /// </summary>
        /// <param name="email">E-mail a ser verificado.</param>
        /// <returns>True se o e-mail existe, caso contrário false.</returns>
        Task<bool> VerificarEmailExistenteAsync(string email);

        /// <summary>
        /// Verifica se um username já está cadastrado de forma assíncrona.
        /// </summary>
        /// <param name="username">Username a ser verificado.</param>
        /// <returns>True se o username existe, caso contrário false.</returns>
        Task<bool> VerificarUsernameExistenteAsync(string username);

        /// <summary>
        /// Salva ou atualiza o código de recuperação de senha para o e-mail informado.
        /// </summary>
        Task SalvarCodigoRecuperacaoAsync(string email, string codigo, DateTimeOffset expiracao);

        /// <summary>
        /// Obtém o código e a data de expiração associados ao e-mail informado.
        /// </summary>
        Task<(string Codigo, DateTimeOffset Expiracao)?> ObterCodigoRecuperacaoAsync(string email);

        /// <summary>
        /// Invalida (remove) o código de recuperação de senha após uso ou expiração.
        /// </summary>
        Task InvalidarCodigoRecuperacaoAsync(string email);

        /// <summary>
        /// Atualiza a senha do usuário com base no e-mail.
        /// </summary>
        Task AtualizarSenhaAsync(string email, string senhaHash);

        Task<List<Usuario>> PesquisarUsuariosAsync(string termo);

        Task InserirSeguidorAsync(Guid idSeguidor, Guid idParaSeguir);

        Task DeletarSeguidorAsync(Guid idSeguidor, Guid idParaParar);

        Task<List<Usuario>> PesquisarUsuariosPorTermoAsync(string termo);

        Task<List<Usuario>> BuscarTodosUsuariosAsync();



    }
}