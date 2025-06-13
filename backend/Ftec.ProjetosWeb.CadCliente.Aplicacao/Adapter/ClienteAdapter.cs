using Ftec.ProjetosWeb.CadCliente.Aplicacao.DTO;
using Ftec.ProjetosWeb.CadCliente.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ftec.ProjetosWeb.CadCliente.Aplicacao.Adapter
{
    public static class ClienteAdapter
    {
        public static List<ClienteDto> ParaDto(List<Cliente> clientes)
        {
            List<ClienteDto> clientes1 = new List<ClienteDto>();

            foreach (Cliente cliente in clientes)
            {
                clientes1.Add(ParaDto(cliente));
            }

            return clientes1;
        }
        public static List<Cliente> ParaDomain(List<ClienteDto> clientes)
        {
            List<Cliente> clientes1 = new List<Cliente>();

            foreach (ClienteDto cliente in clientes)
            {
                clientes1.Add(ParaDomain(cliente));
            }

            return clientes1;
        }
        public static Cliente ParaDomain(ClienteDto cliente)
        {
            return new Cliente()
            {
                NomeCompleto = cliente.NomeCompleto,
                Email = cliente.Email,
                DataNascimento = cliente.DataNascimento,
                Id = cliente.Id,
                Senha = cliente.Senha,
                Username= cliente.Username,
                Telefone=cliente.Telefone,
                Genero=cliente.Genero,
                Descricao=cliente.Descricao,
                Foto=cliente.Foto
             
            };
        }
        public static ClienteDto ParaDto(Cliente cliente)
        {
            return new ClienteDto()
            {
                NomeCompleto = cliente.NomeCompleto,
                Email = cliente.Email,
                DataNascimento = cliente.DataNascimento,
                Id = cliente.Id,
                Senha = cliente.Senha,
                Username = cliente.Username,
                Telefone = cliente.Telefone,
                Genero = cliente.Genero,
                Descricao = cliente.Descricao,
                Foto = cliente.Foto
            };


        }
    }
}
