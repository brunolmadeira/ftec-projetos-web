// Arquivo: RedeSocialUsuario\RedeSocialUsuario.API\Adapter\Mapping.cs

using AutoMapper;
using RedeSocialUsuario.Domain.Entities;
using RedeSocialUsuario.Application.Dto;
using RedeSocialUsuario.Application.DTO;


namespace RedeSocialUsuario.API.Adapter
{
    /// <summary>
    /// Perfil de configuração do AutoMapper para mapeamento entre entidades, DTOs e requests de usuário.
    /// </summary>
    public class Mapping : Profile
    {
        public Mapping()
        {
            // Mapeamento bidirecional entre Usuario e UsuarioDto
            CreateMap<Usuario, UsuarioDto>()
                // Mapeia Usuario.UsuarioNome para UsuarioDto.UserName
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                // Mapeia Usuario.CriadoEm para UsuarioDto.DataCriacao
                .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => src.CriadoEm))
                // Mapeia Usuario.FotoBase64 para UsuarioDto.FotoPerfilUrl (ajuste conforme lógica de conversão/armazenamento de imagem)
                .ForMember(dest => dest.FotoBase64, opt => opt.MapFrom(src => src.FotoBase64))
                // Ignora propriedades não existentes em Usuario
                .ReverseMap();

            CreateMap<UsuarioDto, Usuario>()
                // Mapeia UsuarioDto.UserName para Usuario.UsuarioNome
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                // Mapeia UsuarioDto.DataCriacao para Usuario.CriadoEm
                .ForMember(dest => dest.CriadoEm, opt => opt.MapFrom(src => src.DataCriacao))
                // Mapeia UsuarioDto.FotoPerfilUrl para Usuario.FotoBase64 (ajuste conforme lógica de conversão/armazenamento de imagem)
                .ForMember(dest => dest.FotoBase64, opt => opt.MapFrom(src => src.FotoBase64))
                // Ignora propriedades não existentes em UsuarioDto
                .ReverseMap();

            // Mapeamento de CriarUsuarioRequest para Usuario (apenas para criação)
            CreateMap<CriarUsuarioRequest, Usuario>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.DataNascimento, opt => opt.MapFrom(src => src.DataNascimento))
                // SenhaHash deve ser preenchido posteriormente (hash da senha)
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
                // Propriedades não presentes na request são ignoradas ou inicializadas conforme regras de negócio
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Pronome, opt => opt.Ignore())
                .ForMember(dest => dest.Bio, opt => opt.Ignore())
                .ForMember(dest => dest.Link, opt => opt.Ignore())
                .ForMember(dest => dest.Genero, opt => opt.Ignore())
                .ForMember(dest => dest.GeneroCustomizado, opt => opt.Ignore())
                .ForMember(dest => dest.FotoBase64, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Ativo, opt => opt.Ignore());
        }
    }
}