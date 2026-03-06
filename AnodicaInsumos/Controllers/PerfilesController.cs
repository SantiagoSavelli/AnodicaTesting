using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AnodicaInsumos.Controllers
{
    public class PerfilesController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PerfilesController(IContenedorTrabajo contenedorTrabajo)
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
            CargarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Perfil perfil)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Perfil.Add(perfil);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            CargarCombos();
            return View(perfil);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var perfil = _contenedorTrabajo.Perfil.GetFirstOrDefault(
                x => x.PerfilID == id,
                includeProperties: "Linea,Linea.Proveedor,Ubicacion"
            );

            if (perfil == null)
                return NotFound();

            return View(perfil);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var perfil = _contenedorTrabajo.Perfil.GetFirstOrDefault(
                x => x.PerfilID == id,
                includeProperties: "Linea,Linea.Proveedor,Ubicacion"
            );

            if (perfil == null)
                return NotFound();

            CargarCombos();
            return View(perfil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Perfil perfil)
        {
            if (id != perfil.PerfilID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(perfil);
            }

            var existe = _contenedorTrabajo.Perfil.Get(id);
            if (existe == null)
                return NotFound();

            _contenedorTrabajo.Perfil.Update(perfil);
            _contenedorTrabajo.Save();

            return RedirectToAction(nameof(Index));
        }

        private void CargarCombos()
        {
            ViewBag.Lineas = _contenedorTrabajo.Linea.GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.LineaNombre,
                    Value = x.LineaID.ToString()
                }).ToList();

            ViewBag.Ubicaciones = _contenedorTrabajo.Ubicacion.GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.UbicacionCodigo,
                    Value = x.UbicacionID.ToString()
                }).ToList();
        }

        #region Llamadas a la API

        [HttpGet]
        public IActionResult GetAll(string? codigo, string? proveedor, string? linea, string? descripcion, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _contenedorTrabajo.Perfil
                .GetAll(includeProperties: "Linea,Linea.Proveedor,Ubicacion")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(codigo))
                query = query.Where(x => x.PerfilCodigoAlcemar != null && x.PerfilCodigoAlcemar.Contains(codigo));

            if (!string.IsNullOrWhiteSpace(proveedor))
                query = query.Where(x => x.Linea != null &&
                                         x.Linea.Proveedor != null &&
                                         x.Linea.Proveedor.ProveedorNombre != null &&
                                         x.Linea.Proveedor.ProveedorNombre.Contains(proveedor));

            if (!string.IsNullOrWhiteSpace(linea))
                query = query.Where(x => x.Linea != null &&
                                         x.Linea.LineaNombre != null &&
                                         x.Linea.LineaNombre.Contains(linea));

            if (!string.IsNullOrWhiteSpace(descripcion))
                query = query.Where(x => x.Descripcion != null && x.Descripcion.Contains(descripcion));

            var total = query.Count();

            var data = query
                .OrderBy(x => x.PerfilCodigoAlcemar)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    perfilID = x.PerfilID,
                    codigo = x.PerfilCodigoAlcemar,
                    proveedor = x.Linea != null && x.Linea.Proveedor != null ? x.Linea.Proveedor.ProveedorNombre : "",
                    linea = x.Linea != null ? x.Linea.LineaNombre : "",
                    descripcion = x.Descripcion,
                    imagenPerfil = x.ImagenPerfil
                })
                .ToList();

            return Json(new { data, total, page, pageSize });
        }

        #endregion
    }
}