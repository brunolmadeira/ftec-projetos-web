// Arquivo: RedeSocialUsuario\RedeSocialUsuario.API\Services\TokenService.cs

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace RedeSocialUsuario.API.Services
{
    // Serviço responsável por gerar tokens JWT com claims customizadas
    public class TokenService
    {
        // Chave secreta para assinatura do token (em produção, use configuração segura)
        private readonly string _jwtSecret;
        // Tempo de expiração do token em minutos
        private readonly int _jwtExpirationMinutes;

        public TokenService(string jwtSecret, int jwtExpirationMinutes)
        {
            _jwtSecret = jwtSecret;
            _jwtExpirationMinutes = jwtExpirationMinutes;
        }

        // Gera um JWT contendo as claims sub, username e exp
        public string GenerateToken(string userId, string username)
        {
            // Define a hora de expiração do token
            var expiration = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);

            // Cria as claims do token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId), // Identificador do usuário
                new Claim("username", username),                // Nome de usuário
                new Claim(JwtRegisteredClaimNames.Exp,
                    new DateTimeOffset(expiration).ToUnixTimeSeconds().ToString()) // Expiração em Unix Time
            };

            // Cria a chave de segurança a partir da chave secreta
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Cria o token JWT
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            // Serializa o token para string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidarToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                // Adicione esta linha:
                SecurityToken validatedToken;

                return tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch
            {
                return null;
            }
        }
    }
}