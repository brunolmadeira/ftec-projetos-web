using Ftec.ProjetosWeb.CadCliente.Dominio.Entidades;
using Ftec.ProjetosWeb.CadCliente.Repositorio;

namespace Repositorio.Test
{
    [TestClass]
    public class ClienteRepositorioTest
    {
        string strConexao = "Server=localhost;Port=5432;Database=projetosweb;User Id=postgres;Password=217799;";
        [TestMethod]
        public void InserirTeste()
        {
            var cliente = new Cliente();
            var clienteRepositorio = new ClienteRepositorio(strConexao);
            try
            {
                cliente.NomeCompleto = "joao silva";
                cliente.Email = "joao@gmail.com";
                cliente.Senha = "1234";
                cliente.Username = "joaoS";
                cliente.Telefone = "99999999";
                cliente.Genero = "M";
                cliente.Descricao = "Um cara generico";
                cliente.Foto = "fotoBase64";
                cliente.DataNascimento = DateTime.Now;

                clienteRepositorio.Inserir(cliente);
                Assert.IsTrue(true);    
            }
            catch (Exception ex) 
            {
                Assert.Fail(ex.Message);
            }
            
        }
        [TestMethod]
        public void ExcluirTeste()
        {
            var cliente = new Cliente();
            var clienteRepositorio = new ClienteRepositorio(strConexao);
            try
            {
                cliente.NomeCompleto = "joao silva";
                cliente.Email = "joao@gmail.com";
                cliente.Senha = "1234";
                cliente.Username = "joaoS";
                cliente.Telefone = "99999999";
                cliente.Genero = "M";
                cliente.Descricao = "Um cara generico";
                cliente.Foto = "fotoBase64";
                cliente.DataNascimento = DateTime.Now;

                clienteRepositorio.Inserir(cliente);
                clienteRepositorio.Excluir(cliente.Id);
                var clienteRetorno = clienteRepositorio.Procurar(cliente.Id);

                Assert.IsTrue(clienteRetorno == null);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ProcurarTeste()
        {
            var cliente = new Cliente();
            var clienteRepositorio = new ClienteRepositorio(strConexao);
            try
            {
                cliente.NomeCompleto = "joao silva";
                cliente.Email = "joao@gmail.com";
                cliente.Senha = "1234";
                cliente.Username = "joaoS";
                cliente.Telefone = "99999999";
                cliente.Genero = "M";
                cliente.Descricao = "Um cara generico";
                cliente.Foto = "fotoBase64";
                cliente.DataNascimento = DateTime.Now;

                clienteRepositorio.Inserir(cliente);
                 var clienteRetorno = clienteRepositorio.Procurar(cliente.Id);

                Assert.IsTrue(clienteRetorno != null);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        public void ProcurarTodosTeste()
        {
            var clienteRepositorio = new ClienteRepositorio(strConexao);
            try
            {
                var clientes = clienteRepositorio.ProcurarTodos();  

                Assert.IsTrue(clientes.Count > 0);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void AlterarTeste()
        {
            var cliente = new Cliente()
            {
                NomeCompleto = "joao silva",
                Email = "joao@gmail.com",
                Senha = "1234",
                Username = "joaoS",
                Telefone = "99999999",
                Genero = "M",
                Descricao = "Um cara generico",
                Foto = "fotoBase64",
                DataNascimento = DateTime.Now,e
            };

            var clienteRepositorio = new ClienteRepositorio(strConexao);
            try
            {
                clienteRepositorio.Inserir(cliente);
                cliente.Username = "JoseS";
                clienteRepositorio.Alterar(cliente);

                cliente = clienteRepositorio.Procurar(cliente.Id);
                Assert.IsTrue(!cliente.Username.Equals("joaoS"));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}