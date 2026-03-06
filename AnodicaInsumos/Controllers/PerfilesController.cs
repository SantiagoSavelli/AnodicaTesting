using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace AnodicaInsumos.Controllers
{
    public class PerfilesController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PerfilesController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
