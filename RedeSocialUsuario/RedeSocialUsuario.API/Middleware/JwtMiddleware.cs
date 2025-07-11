// Arquivo: RedeSocialUsuario\RedeSocialUsuario.API\Middleware\JwtMiddleware.cs

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RedeSocialUsuario.API.Services;

namespace RedeSocialUsuario.API.Middleware
{
    // Middleware responsável por ler o token JWT do header Authorization,
    // validar o token e injetar as claims no contexto do usuário (HttpContext.User)
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenService _tokenService;

        public JwtMiddleware(RequestDelegate next, TokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            var principal = _tokenService.ValidarToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
        }
    }

}