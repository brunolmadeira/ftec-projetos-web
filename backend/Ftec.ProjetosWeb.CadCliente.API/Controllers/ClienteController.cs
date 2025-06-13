using Ftec.ProjetosWeb.CadCliente.Aplicacao;
using Ftec.ProjetosWeb.CadCliente.Aplicacao.DTO;
using Ftec.ProjetosWeb.CadCliente.Dominio.Repositorio;
using Ftec.ProjetosWeb.CadCliente.Repositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ftec.ProjetosWeb.CadCliente.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private IClienteRepository clienteRepository;
        private ClienteAplicacao aplicacao;
        public ClienteController()
        {
            string strConexao =  "Server=localhost;Port=5432;Database=projetosweb;User Id=postgres;Password=217799;";
            clienteRepository = new ClienteRepositorio(strConexao);
            aplicacao = new ClienteAplicacao(clienteRepository);
        }

        [HttpGet]
        public List<ClienteDto> Get()
        {
            return aplicacao.PesquisarTodos();
        }

        [HttpPost]
        public void Post(ClienteDto clienteDto)
        {
            aplicacao.Inserir(clienteDto);
        }

        [HttpPut]
        public void Put(ClienteDto clienteDto)
        {
            aplicacao.Alterar(clienteDto);
        }

        [HttpDelete]    
        public void Delete(ClienteDto clienteDto)
        {
            aplicacao.Excluir(clienteDto);
        }

    }
}
