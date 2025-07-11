using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedeSocialUsuario.Application.DTO
{
    public class AtualizarUsuarioDto
    {
        public string Nome { get; set; }
        public string Celular { get; set; }
        public string Pronome { get; set; }
        public string Bio { get; set; }
        public string Link { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string FotoBase64 { get; set; }
    }
}
