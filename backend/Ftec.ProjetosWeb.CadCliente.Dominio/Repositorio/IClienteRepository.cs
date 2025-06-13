using Ftec.ProjetosWeb.CadCliente.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftec.ProjetosWeb.CadCliente.Dominio.Repositorio
{
    public interface IClienteRepository
    {
        Cliente Procurar(Guid id);
        List<Cliente> ProcurarTodos();
        void Inserir(Cliente cliente);
        void Alterar(Cliente cliente);
        bool Excluir(Guid id);
    }
}
