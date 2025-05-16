using Ftec.ProjetosWeb.Social.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ftec.ProjetosWeb.Social.Controllers
{
    public class SocialController : Controller
    {
        private readonly ILogger<SocialController> _logger;

        public SocialController(ILogger<SocialController> logger)
        {
            _logger = logger;
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult Pesquisar()
        {
            return View();
        }

        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        public IActionResult Configuracoes()
        {
            return View();
        }

        public IActionResult Publicacao()
        {
            return View();
        }

        public IActionResult Stories()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
