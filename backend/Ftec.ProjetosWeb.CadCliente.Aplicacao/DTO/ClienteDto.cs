using Ftec.ProjetosWeb.CadCliente.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftec.ProjetosWeb.CadCliente.Aplicacao.DTO
{
    public class ClienteDto
    {
        public Guid Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Username { get; set; }
        public string Telefone { get; set; }
        public string Genero { get; set; }
        public string Descricao { get; set; }
        public string Foto { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
