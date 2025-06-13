using Ftec.ProjetosWeb.CadCliente.Aplicacao.Adapter;
using Ftec.ProjetosWeb.CadCliente.Aplicacao.DTO;
using Ftec.ProjetosWeb.CadCliente.Dominio.Entidades;
using Ftec.ProjetosWeb.CadCliente.Dominio.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftec.ProjetosWeb.CadCliente.Aplicacao
{
    public class ClienteAplicacao
    {
        private IClienteRepository ClienteRepository;

        public ClienteAplicacao(IClienteRepository clienteRepository)
        {
            this.ClienteRepository = clienteRepository;
        }

        public ClienteDto Pesquisar(Guid id)
        {
            var cliente = ClienteRepository.Procurar(id);
            return ClienteAdapter.ParaDto(cliente);
        }

        public List<ClienteDto> PesquisarTodos()
        {
            var clientes = ClienteRepository.ProcurarTodos();
            return ClienteAdapter.ParaDto(clientes);
        }

        public bool Excluir(ClienteDto cliente)
        {
            return ClienteRepository.Excluir(cliente.Id);
        }

        public void Alterar(ClienteDto cliente)
        {
            var clienteEntidade = ClienteAdapter.ParaDomain(cliente);
            ClienteRepository.Alterar(clienteEntidade);
        }

        public Guid Inserir(ClienteDto cliente)
        {
            if (cliente == null)
            {
                throw new ApplicationException("Cliente não informado!");
            }

   
            cliente.Id = Guid.NewGuid();

            var clienteEntidade = ClienteAdapter.ParaDomain(cliente);
            ClienteRepository.Inserir(clienteEntidade);

            return cliente.Id;
        }
    }
}
