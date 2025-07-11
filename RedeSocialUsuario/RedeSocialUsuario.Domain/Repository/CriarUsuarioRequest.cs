// Arquivo: RedeSocialUsuario\RedeSocialUsuario.Domain\Repository\CriarUsuarioRequest.cs

using System;

namespace RedeSocialUsuario.API.Models
{
    /// <summary>
    /// Modelo de requisição para criação de usuário.
    /// </summary>
    public class CriarUsuarioRequest
    {
        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Endereço de e-mail do usuário.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nome de usuário (login).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Número de celular do usuário.
        /// </summary>
        public string Celular { get; set; }

        /// <summary>
        /// Data de nascimento do usuário.
        /// </summary>
        public DateTime DataNascimento { get; set; }

        /// <summary>
        /// Senha do usuário.
        /// </summary>
        public string Senha { get; set; }
    }
}