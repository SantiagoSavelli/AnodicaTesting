using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace AnodicaInsumos.Controllers
{
    public class InsumosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public InsumosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


    }
}
