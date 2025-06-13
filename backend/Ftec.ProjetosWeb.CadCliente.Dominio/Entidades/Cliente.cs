using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftec.ProjetosWeb.CadCliente.Dominio.Entidades
{
    public class Cliente
    {
        public Guid Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Username { get; set; }
        public string Telefone {  get; set; }
        public string Genero {  get; set; }
        public string Descricao { get; set; }
        public string Foto { get; set; }
        public DateTime DataNascimento { get; set; }
        public Cliente()
        {
            this.Id = Guid.NewGuid();
            this.NomeCompleto = string.Empty;
            this.Email = string.Empty;
            this.Senha = string.Empty;
            this.Username = string.Empty;
            this.Telefone = string.Empty;
            this.Genero = string.Empty;
            this.Descricao = string.Empty;
            this.Foto = string.Empty;
            this.DataNascimento = DateTime.MinValue;
            
        }

       

    }
}
