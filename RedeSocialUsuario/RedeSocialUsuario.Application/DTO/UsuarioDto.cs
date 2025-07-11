// Arquivo: RedeSocialUsuario\RedeSocialUsuario.Application\DTO\UsuarioDto.cs

using System;

namespace RedeSocialUsuario.Application.Dto
{
    /// <summary>
    /// DTO para transferência de dados não sensíveis do usuário.
    /// </summary>
    public class UsuarioDto
    {
        /// <summary>
        /// Identificador único do usuário.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Nome de usuário (username) utilizado para login e identificação pública.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// E-mail do usuário (campo não sensível, mas pode ser omitido conforme política de privacidade).
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Data de criação do usuário.
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Base64 da foto de perfil do usuário.
        /// </summary>
        public string FotoBase64 { get; set; }

        /// <summary>
        /// Data de nascimento do usuário.
        /// </summary>
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// Hash da senha do usuário.
        /// </summary>
        public string SenhaHash { get; set; }

        /// <summary>
        /// Indica se o usuário está ativo.
        /// </summary>
        public bool Ativo { get; set; }

        /// <summary>
        /// Número de celular do usuário.
        /// </summary>
        public string Celular { get; set; }

        /// <summary>
        /// Biografia ou descrição do usuário.
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Link pessoal ou de rede social do usuário.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Data de criação do registro do usuário.
        /// </summary>
        public DateTime CriadoEm { get; set; }

        /// <summary>
        /// Pronome de tratamento do usuário (ex: ele, ela, elu).
        /// </summary>
        public string Pronome { get; set; }

    }
}