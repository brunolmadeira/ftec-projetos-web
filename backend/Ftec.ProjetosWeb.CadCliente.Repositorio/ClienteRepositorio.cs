using Ftec.ProjetosWeb.CadCliente.Dominio.Entidades;
using Ftec.ProjetosWeb.CadCliente.Dominio.Repositorio;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftec.ProjetosWeb.CadCliente.Repositorio
{
    public class ClienteRepositorio : IClienteRepository
    {
        private string strConexao;
        public ClienteRepositorio(string strConexao)
        {
            this.strConexao = strConexao;
        }

        public void Alterar(Cliente cliente)
        {
            using (var conexao = new NpgsqlConnection(strConexao))
            {
                conexao.Open();
                using (var transacao = conexao.BeginTransaction())
                {
                    try
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        cmd.Connection = conexao;
                        cmd.Transaction = transacao;
                        cmd.CommandText = @"
                                          UPDATE public.clientes
                                            SET nomecompleto = @nomecompleto,
                                                email = @email,
                                                senha = @senha,
                                                datanascimento = @datanascimento,
                                                username = @username,
                                                telefone = @telefone,
                                                genero = @genero,
                                                descricao = @descricao,
                                                foto = @foto
                                            WHERE id = @id";
                        cmd.Parameters.AddWithValue("nomecompleto", cliente.NomeCompleto);
                        cmd.Parameters.AddWithValue("email", cliente.Email);
                        cmd.Parameters.AddWithValue("senha", cliente.Senha);
                        cmd.Parameters.AddWithValue("datanascimento", cliente.DataNascimento);
                        cmd.Parameters.AddWithValue("username", cliente.Username);
                        cmd.Parameters.AddWithValue("telefone", cliente.Telefone);
                        cmd.Parameters.AddWithValue("genero", cliente.Genero);
                        cmd.Parameters.AddWithValue("descricao", cliente.Descricao);
                        cmd.Parameters.AddWithValue("foto", cliente.Foto);
                        cmd.ExecuteNonQuery();

                        transacao.Commit();
                    }
                    catch (Exception ex)
                    {
                        transacao.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public bool Excluir(Guid id)
        {
            using (var conexao = new NpgsqlConnection(strConexao))
            {
                conexao.Open();
                using (var transacao = conexao.BeginTransaction())
                {
                    try
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        cmd.Connection = conexao;
                        cmd.Transaction = transacao;
                        cmd.CommandText = " DELETE FROM public.clientes " +
                                          " Where id = @id"; 
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.ExecuteNonQuery();
                        transacao.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transacao.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void Inserir(Cliente cliente)
        {
            using (var conexao = new NpgsqlConnection(strConexao))
            {
                conexao.Open();
                using (var transacao = conexao.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conexao;
                            cmd.Transaction = transacao;
                            cmd.CommandText = @"
                                                INSERT INTO public.clientes
                                                (nomecompleto, email, senha, datanascimento, username, telefone, genero, descricao, foto)
                                                VALUES
                                                (@nomecompleto, @email, @senha, @datanascimento, @username, @telefone, @genero, @descricao, @foto)";

                            cmd.Parameters.AddWithValue("nomecompleto", cliente.NomeCompleto);
                            cmd.Parameters.AddWithValue("email", cliente.Email);
                            cmd.Parameters.AddWithValue("senha", cliente.Senha);
                            cmd.Parameters.AddWithValue("datanascimento", cliente.DataNascimento);
                            cmd.Parameters.AddWithValue("username", cliente.Username);
                            cmd.Parameters.AddWithValue("telefone", cliente.Telefone);
                            cmd.Parameters.AddWithValue("genero", cliente.Genero);
                            cmd.Parameters.AddWithValue("descricao", cliente.Descricao);
                            cmd.Parameters.AddWithValue("foto", cliente.Foto);

                            cmd.ExecuteNonQuery();
                            transacao.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        transacao.Rollback();
                        throw;
                    }
                }
            }
        }

        public Cliente Procurar(Guid id)
        {
            Cliente cliente = null;
            try
            {
                using (var con = new NpgsqlConnection(strConexao))
                {
                    con.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM clientes WHERE id = @id";
                    cmd.Parameters.AddWithValue("id", id);
                    var leitor = cmd.ExecuteReader();
                    while (leitor.Read())
                    {
                        cliente = new Cliente()
                        {
                            Id = Guid.Parse(leitor["id"].ToString()),
                            NomeCompleto = leitor["nomecompleto"].ToString(),
                            Email = leitor["email"].ToString(),
                            Senha = leitor["senha"].ToString(),
                            DataNascimento = Convert.ToDateTime(leitor["datanascimento"]),
                            Username = leitor["username"].ToString(),
                            Telefone = leitor["telefone"].ToString(),
                            Genero = leitor["genero"].ToString(),
                            Descricao = leitor["descricao"].ToString(),
                            Foto = leitor["foto"].ToString(),
                        };
                    }
                    leitor.Close();
                    
                }
                return cliente;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Cliente> ProcurarTodos()
        {
            List<Cliente> clientes = new List<Cliente>();

            try
            {
                using (var con = new NpgsqlConnection(strConexao))
                {
                    con.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM clientes";
                    var leitor = cmd.ExecuteReader();
                    while (leitor.Read())
                    {
                        var cliente = new Cliente()
                        {
                            Id = Guid.Parse(leitor["id"].ToString()),
                            NomeCompleto = leitor["nomecompleto"].ToString(),
                            Email = leitor["email"].ToString(),
                            Senha = leitor["senha"].ToString(),
                            DataNascimento = Convert.ToDateTime(leitor["datanascimento"]),
                            Username = leitor["username"].ToString(),
                            Telefone = leitor["telefone"].ToString(),
                            Genero = leitor["genero"].ToString(),
                            Descricao = leitor["descricao"].ToString(),
                            Foto = leitor["foto"].ToString(),
                        };
                        clientes.Add(cliente);
                    }
                    leitor.Close();

                }
                return clientes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
