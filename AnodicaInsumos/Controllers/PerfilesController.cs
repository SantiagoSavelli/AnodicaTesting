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
        public IActionResult Create(Perfil perfil, IFormFile? archivoImagen, List<byte>? tratamientoIds, 
            List<short?>? ubicacionesTratamiento, List<decimal>? stockMinimo, List<string>? equivalenciasCodigo,List<string>? equivalenciasDescripcion)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Campo = x.Key,
                        Errores = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                CargarCombos();
                return View(perfil);
            }

            var existeCodigo = _contenedorTrabajo.Perfil.GetFirstOrDefault(x => x.PerfilCodigoAlcemar == perfil.PerfilCodigoAlcemar);

            if (existeCodigo != null)
            {
                ModelState.AddModelError("PerfilCodigoAlcemar", "Ya existe un perfil con ese código.");
                CargarCombos();
                return View(perfil);
            }

            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    archivoImagen.CopyTo(memoryStream);
                    perfil.ImagenPerfil = memoryStream.ToArray();
                }
            }

            _contenedorTrabajo.Perfil.Add(perfil);
            _contenedorTrabajo.Save();

            if (tratamientoIds != null)
            {
                for (int i = 0; i < tratamientoIds.Count; i++)
                {
                    var detalle = new PerfilTratamiento
                    {
                        PerfilRef = perfil.PerfilID,
                        TratamientoRef = tratamientoIds[i],
                        UbicacionRef = (ubicacionesTratamiento != null && ubicacionesTratamiento.Count > i && ubicacionesTratamiento[i] > 0)
                                            ? ubicacionesTratamiento[i]
                                            : null,
                        CantMinimaTirasStock = (stockMinimo != null && stockMinimo.Count > i)
                                            ? Convert.ToInt16(stockMinimo[i])
                                            : (short)0,
                        CantidadStock = 0
                    };

                    _contenedorTrabajo.PerfilTratamiento.Add(detalle);
                }
            }

            if (equivalenciasCodigo != null)
            {
                for (int i = 0; i < equivalenciasCodigo.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(equivalenciasCodigo[i]))
                    {
                        var perfilEquivalente = _contenedorTrabajo.Perfil
                            .GetFirstOrDefault(x => x.PerfilCodigoAlcemar == equivalenciasCodigo[i]);

                        if (perfilEquivalente != null)
                        {
                            var eq = new PerfilEquivalencia
                            {
                                PerfilRef = perfil.PerfilID,
                                PerfilEquivalenteRef = perfilEquivalente.PerfilID
                            };

                            _contenedorTrabajo.PerfilEquivalencia.Add(eq);
                        }
                    }
                }
            }

            _contenedorTrabajo.Save();

            return RedirectToAction(nameof(Index));
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

            var perfilTratamientos = _contenedorTrabajo.PerfilTratamiento.GetAll(includeProperties: "Tratamiento,Ubicacion")
                .Where(x => x.PerfilRef == id)
                .ToList();

            var perfilEquivalencias = _contenedorTrabajo.PerfilEquivalencia.GetAll()
                .Where(x => x.PerfilRef == id)
                .Select(x => new
                {
                    Codigo = _contenedorTrabajo.Perfil.GetFirstOrDefault(p => p.PerfilID == x.PerfilEquivalenteRef)?.PerfilCodigoAlcemar ?? x.PerfilEquivalenteRef.ToString(),
                    Descripcion = _contenedorTrabajo.Perfil.GetFirstOrDefault(p => p.PerfilID == x.PerfilEquivalenteRef)?.Descripcion ?? ""
                })
                .ToList();

            ViewBag.PerfilTratamientos = perfilTratamientos;
            ViewBag.PerfilEquivalencias = perfilEquivalencias;

            return View(perfil);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var perfil = _contenedorTrabajo.Perfil.GetFirstOrDefault(
                x => x.PerfilID == id,
                includeProperties: "Linea,Ubicacion"
            );

            if (perfil == null)
                return NotFound();

            CargarCombos();

            var perfilTratamientos = _contenedorTrabajo.PerfilTratamiento.GetAll()
                .Where(x => x.PerfilRef == id)
                .ToList();

            var perfilEquivalencias = _contenedorTrabajo.PerfilEquivalencia.GetAll()
                .Where(x => x.PerfilRef == id)
                .Select(x => new
                {
                    Codigo = _contenedorTrabajo.Perfil
                        .GetFirstOrDefault(p => p.PerfilID == x.PerfilEquivalenteRef)?.PerfilCodigoAlcemar ?? "",

                    Descripcion = _contenedorTrabajo.Perfil
                        .GetFirstOrDefault(p => p.PerfilID == x.PerfilEquivalenteRef)?.Descripcion ?? ""
                })
                .ToList();

            ViewBag.PerfilTratamientos = perfilTratamientos;
            ViewBag.PerfilEquivalencias = perfilEquivalencias;

            return View(perfil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Perfil perfil, IFormFile? archivoImagen, List<short>? tratamientoIds, List<short?>? ubicacionesTratamiento, 
            List<decimal>? stockMinimo, List<string>? equivalenciasCodigo, List<string>? equivalenciasDescripcion)
        {
            if (id != perfil.PerfilID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                CargarCombos();
                var perfilTratamientosView = _contenedorTrabajo.PerfilTratamiento.GetAll()
                    .Where(x => x.PerfilRef == id)
                    .ToList();

                var perfilEquivalenciasView = _contenedorTrabajo.PerfilEquivalencia.GetAll()
                    .Where(x => x.PerfilRef == id)
                    .Select(x => new
                    {
                        Codigo = x.PerfilEquivalenteRef.ToString(),
                        Descripcion = _contenedorTrabajo.Perfil.GetFirstOrDefault(p => p.PerfilID == x.PerfilEquivalenteRef)?.Descripcion ?? ""
                    })
                    .ToList();

                ViewBag.PerfilTratamientos = perfilTratamientosView;
                ViewBag.PerfilEquivalencias = perfilEquivalenciasView;
                return View(perfil);
            }

            var perfilDb = _contenedorTrabajo.Perfil.Get(id);
            if (perfilDb == null)
                return NotFound();

            var existeCodigo = _contenedorTrabajo.Perfil.GetFirstOrDefault(x =>
                x.PerfilCodigoAlcemar == perfil.PerfilCodigoAlcemar && x.PerfilID != id);

            if (existeCodigo != null)
            {
                ModelState.AddModelError("PerfilCodigoAlcemar", "Ya existe un perfil con ese código.");
                CargarCombos();
                return View(perfil);
            }

            perfilDb.PerfilCodigoAlcemar = perfil.PerfilCodigoAlcemar;
            perfilDb.LineaRef = perfil.LineaRef;
            perfilDb.UbicacionRef = perfil.UbicacionRef;
            perfilDb.Descripcion = perfil.Descripcion;
            perfilDb.PesoXmetro = perfil.PesoXmetro;
            perfilDb.LongTiraMts = perfil.LongTiraMts;
            perfilDb.PesoXtira = perfil.PesoXtira;
            perfilDb.CantTirasPaquete = perfil.CantTirasPaquete;
            perfilDb.ManejaStockPropio = perfil.ManejaStockPropio;

            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    archivoImagen.CopyTo(memoryStream);
                    perfilDb.ImagenPerfil = memoryStream.ToArray();
                }
            }

            _contenedorTrabajo.Perfil.Update(perfilDb);
            _contenedorTrabajo.Save();

            var tratamientosExistentes = _contenedorTrabajo.PerfilTratamiento.GetAll()
                .Where(x => x.PerfilRef == id)
                .ToList();

            foreach (var item in tratamientosExistentes)
            {
                _contenedorTrabajo.PerfilTratamiento.Remove(item);
            }

            if (tratamientoIds != null)
            {
                for (int i = 0; i < tratamientoIds.Count; i++)
                {
                    var detalle = new PerfilTratamiento
                    {
                        PerfilRef = perfilDb.PerfilID,
                        TratamientoRef = tratamientoIds[i],
                        UbicacionRef = (ubicacionesTratamiento != null && ubicacionesTratamiento.Count > i && ubicacionesTratamiento[i].HasValue)
                            ? ubicacionesTratamiento[i].Value
                            : null,
                        CantMinimaTirasStock = (stockMinimo != null && stockMinimo.Count > i)
                            ? Convert.ToInt16(stockMinimo[i])
                            : (short)0,
                        CantidadStock = 0
                    };
                    _contenedorTrabajo.PerfilTratamiento.Add(detalle);
                }
            }
            var equivalenciasExistentes = _contenedorTrabajo.PerfilEquivalencia.GetAll()
                .Where(x => x.PerfilRef == id)
                .ToList();

            foreach (var item in equivalenciasExistentes)
            {
                _contenedorTrabajo.PerfilEquivalencia.Remove(item);
            }

            if (equivalenciasCodigo != null)
            {
                var equivalentesAgregados = new HashSet<int>();

                for (int i = 0; i < equivalenciasCodigo.Count; i++)
                {
                    var codigoEquivalente = equivalenciasCodigo[i]?.Trim();

                    if (string.IsNullOrWhiteSpace(codigoEquivalente))
                        continue;

                    var perfilEquivalente = _contenedorTrabajo.Perfil
                        .GetFirstOrDefault(x => x.PerfilCodigoAlcemar == codigoEquivalente);

                    if (perfilEquivalente == null)
                        continue;
                    if (perfilEquivalente.PerfilID == perfilDb.PerfilID)
                        continue;
                    if (equivalentesAgregados.Contains(perfilEquivalente.PerfilID))
                        continue;

                    equivalentesAgregados.Add(perfilEquivalente.PerfilID);

                    var eq = new PerfilEquivalencia
                    {
                        PerfilRef = perfilDb.PerfilID,
                        PerfilEquivalenteRef = perfilEquivalente.PerfilID
                    };
                    _contenedorTrabajo.PerfilEquivalencia.Add(eq);
                }
            }
            _contenedorTrabajo.Save();

            return RedirectToAction(nameof(Index));
        }

        private void CargarCombos()
        {
            ViewBag.Proveedores = _contenedorTrabajo.Proveedor.GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.ProveedorNombre,
                    Value = x.ProveedorID.ToString()
                }).ToList();

            ViewBag.Lineas = _contenedorTrabajo.Linea.GetAll(includeProperties: "Proveedor")
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

            ViewBag.Tratamientos = _contenedorTrabajo.Tratamiento.GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.TratamientoNombre,
                    Value = x.TratamientoID.ToString()
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

        [HttpGet]
        public IActionResult GetLineasPorProveedor(int proveedorId)
        {
            var lineas = _contenedorTrabajo.Linea.GetAll()
                .Where(x => x.ProveedorRef == proveedorId)
                .Select(x => new
                {
                    value = x.LineaID,
                    text = x.LineaNombre
                })
                .ToList();

            return Json(lineas);
        }

        [HttpGet]
        public IActionResult GetPerfilPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Json(null);

            var perfil = _contenedorTrabajo.Perfil
                .GetFirstOrDefault(p => p.PerfilCodigoAlcemar == codigo);

            if (perfil == null)
                return Json(null);

            return Json(new
            {
                codigo = perfil.PerfilCodigoAlcemar,
                descripcion = perfil.Descripcion,
                id = perfil.PerfilID
            });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var perfil = _contenedorTrabajo.Perfil.Get(id);
            if (perfil == null)
            {
                return Json(new { success = false, message = "Error: no se encontró el perfil." });
            }

            var tratamientos = _contenedorTrabajo.PerfilTratamiento.GetAll()
                .Where(x => x.PerfilRef == id)
                .ToList();

            foreach (var item in tratamientos)
            {
                _contenedorTrabajo.PerfilTratamiento.Remove(item);
            }

            var equivalenciasComoPerfil = _contenedorTrabajo.PerfilEquivalencia.GetAll()
                .Where(x => x.PerfilRef == id)
                .ToList();

            foreach (var item in equivalenciasComoPerfil)
            {
                _contenedorTrabajo.PerfilEquivalencia.Remove(item);
            }

            var equivalenciasComoEquivalente = _contenedorTrabajo.PerfilEquivalencia.GetAll()
                .Where(x => x.PerfilEquivalenteRef == id)
                .ToList();

            foreach (var item in equivalenciasComoEquivalente)
            {
                _contenedorTrabajo.PerfilEquivalencia.Remove(item);
            }

            _contenedorTrabajo.Perfil.Remove(perfil);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Perfil borrado correctamente." });
        }
        #endregion
    }
}