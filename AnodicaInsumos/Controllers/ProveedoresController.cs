using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AnodicaInsumos.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ProveedoresController(IContenedorTrabajo contenedorTrabajo)
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
            var tipos = _contenedorTrabajo.TipoProveedor.GetAll()
                .Select(t => new SelectListItem
                {
                    Text = t.TipoProveedorNombre,
                    Value = t.TipoProveedorID.ToString()
                }).ToList();

            ViewBag.TiposProveedor = tipos;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Proveedor proveedor, List<byte> tiposSeleccionados)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Proveedor.Add(proveedor);
                _contenedorTrabajo.Save();

                foreach (var tipoId in tiposSeleccionados)
                {
                    var relacion = new ProveedorTipoProveedor
                    {
                        ProveedorRef = proveedor.ProveedorID,
                        TipoProveedorRef = tipoId
                    };

                    _contenedorTrabajo.ProveedorTipoProveedor.Add(relacion);
                }

                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(proveedor);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var proveedor = _contenedorTrabajo.Proveedor.Get(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Proveedor proveedor)
        {
            if (id != proveedor.ProveedorID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(proveedor);

            var existe = _contenedorTrabajo.Proveedor.Get(id);
            if (existe == null)
                return NotFound();

            _contenedorTrabajo.Proveedor.Update(proveedor);
            _contenedorTrabajo.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var proveedor = _contenedorTrabajo.Proveedor.Get(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll(string? nombre, string? telefono, string? email, string? productos, 
            decimal? porcentaje, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _contenedorTrabajo.Proveedor.GetAll().AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(x => x.ProveedorNombre.Contains(nombre));

            if (!string.IsNullOrWhiteSpace(telefono))
                query = query.Where(x => x.Telefonos != null && x.Telefonos.Contains(telefono));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(x => x.Email != null && x.Email.Contains(email));

            if (!string.IsNullOrWhiteSpace(productos))
                query = query.Where(x => x.Productos != null && x.Productos.Contains(productos));

            if (porcentaje.HasValue)
                query = query.Where(x => x.PorcentajePesoTiraPerfil == porcentaje.Value);

            var total = query.Count();

            var data = query
                .OrderBy(x => x.ProveedorID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new { data, total, page, pageSize });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Proveedor.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error no se encontró el proveedor" });
            }

            _contenedorTrabajo.Proveedor.Remove(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Proveedor borrado correctamente" });
        }
        #endregion
    }
}
