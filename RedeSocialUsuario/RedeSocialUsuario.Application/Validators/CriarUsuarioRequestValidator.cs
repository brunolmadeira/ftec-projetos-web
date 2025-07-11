// Arquivo: RedeSocialUsuario.API/Application/Validators/CriarUsuarioRequestValidator.cs

using System;
using FluentValidation;
using RedeSocialUsuario.Application.DTO;

namespace RedeSocialUsuario.Application.Validators
{
    /// <summary>
    /// Validador FluentValidation para CriarUsuarioRequest.
    /// </summary>
    public class CriarUsuarioRequestValidator : AbstractValidator<CriarUsuarioRequest>
    {
        public CriarUsuarioRequestValidator()
        {
            // Nome: obrigatório, mínimo 3 e máximo 100 caracteres
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

            // Email: obrigatório, formato válido
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado não é válido.");

            // UsuarioNome: obrigatório, mínimo 3 e máximo 30 caracteres, sem espaços
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("O nome de usuário é obrigatório.")
                .MinimumLength(3).WithMessage("O nome de usuário deve ter pelo menos 3 caracteres.")
                .MaximumLength(30).WithMessage("O nome de usuário deve ter no máximo 30 caracteres.")
                .Matches(@"^\S+$").WithMessage("O nome de usuário não pode conter espaços.");

            // DataNascimento: obrigatório, maior de 13 anos
            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
                .Must(SerMaiorDe13Anos).WithMessage("O usuário deve ter pelo menos 13 anos.");

            // Senha: obrigatório, mínimo 6 e máximo 50 caracteres
            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.")
                .MaximumLength(50).WithMessage("A senha deve ter no máximo 50 caracteres.");
        }

        /// <summary>
        /// Verifica se a data corresponde a um usuário com pelo menos 13 anos.
        /// </summary>
        private bool SerMaiorDe13Anos(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;
            if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
            return idade >= 13;
        }
    }
}