using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Insumo insumo)
        {
            if (ModelState.IsValid) 
            { 
                _contenedorTrabajo.Insumo.Add(insumo);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(short id)
        {
            var insumo = _contenedorTrabajo.Insumo.Get(id);

            if (insumo == null)
            {
                return NotFound();
            }

            return View(insumo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, Insumo insumo)
        {
            if (id != insumo.InsumoID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(insumo);

            var existe = _contenedorTrabajo.Insumo.Get(id);
            if (existe == null)
                return NotFound();

            _contenedorTrabajo.Insumo.Update(insumo);
            _contenedorTrabajo.Save();

            return RedirectToAction(nameof(Index));
        }

        #region Llamadas a la API
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Insumo.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(short id)
        {
            var objFromDb = _contenedorTrabajo.Insumo.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error no se encontro el insumo" });
            }

            _contenedorTrabajo.Insumo.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Insumo borrado correctamente" });
        }
        #endregion
    }
}
