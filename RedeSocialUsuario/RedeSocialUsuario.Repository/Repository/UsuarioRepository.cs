// Arquivo: RedeSocialUsuario\RedeSocialUsuario.Repository\Repository\UsuarioRepository.cs

using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using RedeSocialUsuario.Domain.Entities;
using RedeSocialUsuario.Domain.Repository;

namespace RedeSocialUsuario.Repository.Repository
{
    /// <summary>
    /// Implementação de IUsuarioRepository utilizando Npgsql para acesso ao PostgreSQL.
    /// </summary>
    /// 
    
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Construtor recebe a connection string do banco PostgreSQL.
        /// </summary>
        /// <param name="connectionString">String de conexão com o banco.</param>
        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public async Task<Usuario> CriarUsuarioAsync(Usuario usuario)
        {
            // Cria um novo usuário no banco e retorna o usuário criado (com Id preenchido)
            const string sql = @"
                INSERT INTO usuarios (id, nome, email, username, senha_hash, criado_em)
                VALUES (@id, @nome, @email, @username, @senha_hash, @criado_em)
                RETURNING id, nome, email, username, senha_hash, criado_em;
            ";

            usuario.Id = Guid.NewGuid();
            usuario.CriadoEm = DateTime.UtcNow;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", usuario.Id);
                    cmd.Parameters.AddWithValue("nome", usuario.Nome);
                    cmd.Parameters.AddWithValue("email", usuario.Email);
                    cmd.Parameters.AddWithValue("username", usuario.Username);
                    cmd.Parameters.AddWithValue("senha_hash", usuario.SenhaHash);
                    cmd.Parameters.AddWithValue("criado_em", usuario.CriadoEm);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearUsuario(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <inheritdoc />
        public async Task<Usuario> BuscarPorIdAsync(Guid id)
        {
            const string sql = @"
                SELECT id, nome, email, username, senha_hash, criado_em
                FROM usuarios
                WHERE id = @id;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearUsuario(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <inheritdoc />
        public async Task<Usuario> BuscarPorEmailAsync(string email)
        {
            const string sql = @"
                SELECT id, nome, email, username, senha_hash, criado_em
                FROM usuarios
                WHERE email = @email;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("email", email);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearUsuario(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<Usuario>> PesquisarUsuariosPorTermoAsync(string termo)
        {
            const string sql = @"
                SELECT id, username, nome, foto_base64
                FROM usuarios
                WHERE (@termo IS NULL OR 
                       LOWER(nome) ILIKE LOWER('%' || @termo || '%') OR
                       LOWER(username) ILIKE LOWER('%' || @termo || '%'))
            ";

            var lista = new List<Usuario>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("termo", string.IsNullOrWhiteSpace(termo) ? DBNull.Value : termo);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Usuario
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Nome = reader.GetString(reader.GetOrdinal("nome")),
                                FotoBase64 = reader.IsDBNull(reader.GetOrdinal("foto_base64")) ? null :
                                             reader.GetString(reader.GetOrdinal("foto_base64"))
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<List<Usuario>> BuscarTodosUsuariosAsync()
        {
            var usuarios = new List<Usuario>();
            const string sql = "SELECT id, nome, username, foto_base64 FROM usuarios";

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                usuarios.Add(new Usuario
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    FotoBase64 = reader.IsDBNull(reader.GetOrdinal("foto_base64")) ? null : reader.GetString(reader.GetOrdinal("foto_base64"))
                });
            }

            return usuarios;
        }


        /// <inheritdoc />
        public async Task<Usuario> BuscarPorUsernameAsync(string username)
        {
            const string sql = @"
                SELECT id, nome, email, username, senha_hash, criado_em
                FROM usuarios
                WHERE username = @username;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearUsuario(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <inheritdoc />
        public async Task<Usuario> AtualizarUsuarioAsync(Usuario usuario)
        {
            const string sql = @"
        UPDATE usuarios
        SET 
            nome = @nome,
            celular = @celular,
            pronome = @pronome,
            bio = @bio,
            link = @link,
            foto_base64 = @foto_base64,
            data_nascimento = @data_nascimento,
            atualizado_em = @atualizado_em
        WHERE id = @id
        RETURNING 
            id, nome, email, username, celular, pronome, bio, link, 
            foto_base64, data_nascimento, atualizado_em;
    ";

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", usuario.Id);
            cmd.Parameters.AddWithValue("nome", (object?)usuario.Nome ?? DBNull.Value);
            cmd.Parameters.AddWithValue("celular", (object?)usuario.Celular ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pronome", (object?)usuario.Pronome ?? DBNull.Value);
            cmd.Parameters.AddWithValue("bio", (object?)usuario.Bio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("link", (object?)usuario.Link ?? DBNull.Value);
            cmd.Parameters.AddWithValue("foto_base64", (object?)usuario.FotoBase64 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("data_nascimento", (object?)usuario.DataNascimento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("atualizado_em", DateTime.UtcNow);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapearUsuarioSemSenha(reader);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task DeletarUsuarioAsync(Guid id)
        {
            const string sql = @"
                DELETE FROM usuarios
                WHERE id = @id;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> VerificarEmailExistenteAsync(string email)
        {
            const string sql = @"
                SELECT 1 FROM usuarios WHERE email = @email LIMIT 1;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("email", email);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync();
                    }
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> VerificarUsernameExistenteAsync(string username)
        {
            const string sql = @"
                SELECT 1 FROM usuarios WHERE username = @username LIMIT 1;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync();
                    }
                }
            }
        }

        public async Task<List<Usuario>> PesquisarUsuariosAsync(string termo)
        {
            const string sql = @"
                SELECT id, nome, email, username, senha_hash, criado_em
                FROM usuarios
                WHERE nome ILIKE @termo OR username ILIKE @termo
                LIMIT 20;
            ";

            var usuarios = new List<Usuario>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("termo", $"%{termo}%");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            usuarios.Add(MapearUsuario(reader));
                        }
                    }
                }
            }

            return usuarios;
        }

        public async Task InserirSeguidorAsync(Guid idSeguidor, Guid idParaSeguir)
        {
            const string sql = @"
                INSERT INTO seguidores (id_de_seguidor, id_para_seguido)
                VALUES (@seguidor, @seguido)
                ON CONFLICT DO NOTHING;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("seguidor", idSeguidor);
                    cmd.Parameters.AddWithValue("seguido", idParaSeguir);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeletarSeguidorAsync(Guid idSeguidor, Guid idParaParar)
        {
            const string sql = @"
                DELETE FROM seguidores
                WHERE id_de_seguidor = @seguidor AND id_para_seguido = @seguido;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("seguidor", idSeguidor);
                    cmd.Parameters.AddWithValue("seguido", idParaParar);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }




        public async Task SalvarCodigoRecuperacaoAsync(string email, string codigo, DateTimeOffset expiracao)
        {
            const string sql = @"
                INSERT INTO recuperacao_senha (email, codigo, expiracao)
                VALUES (@email, @codigo, @expiracao)
                ON CONFLICT (email) DO UPDATE
                SET codigo = EXCLUDED.codigo,
                    expiracao = EXCLUDED.expiracao;
            ";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("email", email);
                    cmd.Parameters.AddWithValue("codigo", codigo);
                    cmd.Parameters.AddWithValue("expiracao", expiracao.UtcDateTime);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<(string Codigo, DateTimeOffset Expiracao)?> ObterCodigoRecuperacaoAsync(string email)
        {
            const string sql = @"
                SELECT codigo, expiracao FROM recuperacao_senha
                WHERE email = @email;
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("email", email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string codigo = reader.GetString(reader.GetOrdinal("codigo"));
                DateTimeOffset expiracao = reader.GetDateTime(reader.GetOrdinal("expiracao"));
                return (codigo, expiracao);
            }

            return null;
        }

        public async Task InvalidarCodigoRecuperacaoAsync(string email)
        {
            const string sql = @"DELETE FROM recuperacao_senha WHERE email = @email;";

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("email", email);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task AtualizarSenhaAsync(string email, string senhaHash)
        {
            const string sql = @"
                UPDATE usuarios
                SET senha_hash = @senha_hash
                WHERE email = @email;
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("senha_hash", senhaHash);
            await cmd.ExecuteNonQueryAsync();
        }




        /// <summary>
        /// Mapeia os dados do NpgsqlDataReader para a entidade Usuario.
        /// </summary>
        /// <param name="reader">NpgsqlDataReader com os dados do usuário.</param>
        /// <returns>Instância de Usuario.</returns>
        private Usuario MapearUsuario(NpgsqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                SenhaHash = reader.GetString(reader.GetOrdinal("senha_hash")),
                CriadoEm = reader.GetDateTime(reader.GetOrdinal("criado_em"))
            };
        }

        private Usuario MapearUsuarioSemSenha(NpgsqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Nome = reader.IsDBNull("nome") ? null : reader.GetString(reader.GetOrdinal("nome")),
                Email = reader.IsDBNull("email") ? null : reader.GetString(reader.GetOrdinal("email")),
                Username = reader.IsDBNull("username") ? null : reader.GetString(reader.GetOrdinal("username")),
                Celular = reader.IsDBNull("celular") ? null : reader.GetString(reader.GetOrdinal("celular")),
                Pronome = reader.IsDBNull("pronome") ? null : reader.GetString(reader.GetOrdinal("pronome")),
                Bio = reader.IsDBNull("bio") ? null : reader.GetString(reader.GetOrdinal("bio")),
                Link = reader.IsDBNull("link") ? null : reader.GetString(reader.GetOrdinal("link")),
                FotoBase64 = reader.IsDBNull("foto_base64") ? null : reader.GetString(reader.GetOrdinal("foto_base64")),
                DataNascimento = reader.IsDBNull("data_nascimento") ? null : reader.GetDateTime(reader.GetOrdinal("data_nascimento")),
                AtualizadoEm = reader.IsDBNull("atualizado_em") ? null : reader.GetDateTime(reader.GetOrdinal("atualizado_em"))
            };
        }


    }
}