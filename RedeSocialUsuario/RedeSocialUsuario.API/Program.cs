using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RedeSocialUsuario.API.Adapter;
using RedeSocialUsuario.API.Middleware;
using RedeSocialUsuario.API.Services;
using RedeSocialUsuario.Application.Application;
using System.Text;
using AutoMapper;
using RedeSocialUsuario.Domain.Repository;
using RedeSocialUsuario.Repository.Repository;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. Obtenção das configurações =====
var jwtSecret = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Chave JWT não configurada no appsettings.json");
var jwtExpirationMinutes = builder.Configuration.GetValue<int>("Jwt:ExpirationMinutes", 60); // Valor padrão 60 se não configurado
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("String de conexão não configurada");
var key = Encoding.ASCII.GetBytes(jwtSecret);

// ===== 2. Configuração do AutoMapper =====
builder.Services.AddAutoMapper(typeof(Mapping).Assembly);

// ===== 3. Injeção de dependências =====
builder.Services.AddScoped<IUsuarioRepository>(_ => new UsuarioRepository(connectionString));
builder.Services.AddSingleton<TokenService>(_ => new TokenService(jwtSecret, jwtExpirationMinutes));
builder.Services.AddScoped<UsuarioApplication>();

// ===== 4. Configuração dos Controllers =====
builder.Services.AddControllers();

// ===== 5. Configuração do Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    options.EnableAnnotations();
});

// ===== 6. Configuração da Autenticação JWT =====
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
        // Adiciona validação de tempo de expiração
        ValidateLifetime = true
    };
});

// ===== 7. Build da aplicação =====
var app = builder.Build();

// ===== 8. Configuração do Pipeline HTTP =====

// Middleware de Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty; // Agora acesse direto na raiz (https://localhost:57169)
});

// Middlewares customizados
app.UseMiddleware<JwtMiddleware>();

// Middlewares padrão
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Mapeamento dos controllers
app.MapControllers();

// ===== 9. Execução da aplicação =====
app.Run();