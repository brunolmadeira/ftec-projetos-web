// Arquivo: RedeSocialUsuario\RedeSocialUsuario.Domain\Entities\Usuario.cs

using System;

namespace RedeSocialUsuario.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um usuário da rede social.
    /// </summary>
    public class Usuario
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
        /// Endereço de e-mail do usuário.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Número de celular do usuário.
        /// </summary>
        public string Celular { get; set; }

        /// <summary>
        /// Nome de usuário (username) utilizado para login ou identificação pública.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Pronome de tratamento do usuário (ex: ele, ela, elu).
        /// </summary>
        public string Pronome { get; set; }

        /// <summary>
        /// Biografia ou descrição do usuário.
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Link pessoal ou de rede social do usuário.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gênero do usuário (ex: Masculino, Feminino, Outro).
        /// </summary>
        public string Genero { get; set; }

        /// <summary>
        /// Gênero customizado, caso o usuário deseje especificar.
        /// </summary>
        public string GeneroCustomizado { get; set; }

        /// <summary>
        /// Data de nascimento do usuário.
        /// </summary>
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// Foto do usuário em formato Base64.
        /// </summary>
        public string FotoBase64 { get; set; }

        /// <summary>
        /// Hash da senha do usuário.
        /// </summary>
        public string SenhaHash { get; set; }

        /// <summary>
        /// Data de criação do registro do usuário.
        /// </summary>
        public DateTime CriadoEm { get; set; }

        /// <summary>
        /// Data da última atualização do registro do usuário.
        /// </summary>
        public DateTime? AtualizadoEm { get; set; }

        /// <summary>
        /// Indica se o usuário está ativo.
        /// </summary>
        public bool Ativo { get; set; }
    }
}