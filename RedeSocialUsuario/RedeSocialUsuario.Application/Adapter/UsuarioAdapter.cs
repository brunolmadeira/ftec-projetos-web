// Arquivo: RedeSocialUsuario\RedeSocialUsuario.Application\Adapter\UsuarioAdapter.cs

using System;
using RedeSocialUsuario.Domain.Entities;
using RedeSocialUsuario.Application.Dto;
using RedeSocialUsuario.Application.DTO;

namespace RedeSocialUsuario.Application.Adapter
{
    /// <summary>
    /// Adaptador para conversão entre Usuario, UsuarioDto e CriarUsuarioRequest.
    /// </summary>
    public static class UsuarioAdapter
    {
        /// <summary>
        /// Converte um objeto Usuario para UsuarioDto.
        /// </summary>
        /// <param name="usuario">Entidade Usuario.</param>
        /// <returns>DTO UsuarioDto.</returns>
        public static UsuarioDto ToDto(Usuario usuario)
        {
            if (usuario == null)
                return null;

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Username = usuario.Username,
                Email = usuario.Email,
                Celular = usuario.Celular,
                DataNascimento = usuario.DataNascimento,
                DataCriacao = usuario.CriadoEm,
                FotoBase64 = usuario.FotoBase64,
                Ativo = usuario.Ativo
            };
        }

        /// <summary>
        /// Converte um objeto CriarUsuarioRequest para Usuario.
        /// </summary>
        /// <param name="request">Modelo de requisição para criação de usuário.</param>
        /// <param name="senhaHash">Hash da senha do usuário.</param>
        /// <returns>Entidade Usuario.</returns>
        public static Usuario FromCriarRequest(CriarUsuarioRequest request, string senhaHash)
        {
            if (request == null)
                return null;

            return new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Email = request.Email,
                Username = request.Username,
                DataNascimento = request.DataNascimento,
                SenhaHash = senhaHash,
                CriadoEm = DateTime.UtcNow,
                Ativo = true
            };
        }

        /// <summary>
        /// Converte um objeto UsuarioDto para Usuario.
        /// </summary>
        /// <param name="dto">DTO UsuarioDto.</param>
        /// <returns>Entidade Usuario.</returns>
        public static Usuario FromDto(UsuarioDto dto)
        {
            if (dto == null)
                return null;

            return new Usuario
            {
                Id = dto.Id,
                Nome = dto.Nome,
                Username = dto.Username,
                Email = dto.Email,
                Celular = dto.Celular,
                DataNascimento = dto.DataNascimento,
                FotoBase64 = dto.FotoBase64,
                CriadoEm = dto.DataCriacao,
                Ativo = dto.Ativo
            };
        }

        /// <summary>
        /// Converte um objeto UsuarioDto para CriarUsuarioRequest (parcial).
        /// </summary>
        /// <param name="usuario">DTO do usuário.</param>
        /// <returns>Modelo CriarUsuarioRequest.</returns>
        public static CriarUsuarioRequest ToCriarUsuarioRequest(UsuarioDto usuario)
        {
            if (usuario == null)
                return null;

            return new CriarUsuarioRequest
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Username = usuario.Username,
                DataNascimento = usuario.DataNascimento ?? default,
                // Senha não pode ser recuperada — mantenha como null
                Senha = null
            };
        }
    }
}