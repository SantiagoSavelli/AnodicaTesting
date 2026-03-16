using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using AnodicaInsumos.Modelos.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.Intrinsics.X86;

namespace AnodicaInsumos.Controllers
{
    public class PerfilesController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IMapper _mapper;

        public PerfilesController(IContenedorTrabajo contenedorTrabajo, IMapper mapper)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new PerfilVM();
            await CargarCombosAsync(vm);
            var tratamientos = await _contenedorTrabajo.Tratamiento.GetAllAsync();

            vm.PerfilTratamientos = tratamientos
                .Select(t => new PerfilTratamientoVM
                {
                    TratamientoRef = t.TratamientoID,
                    Descripcion = t.TratamientoNombre,
                    UbicacionRef = null,
                    CantMinimaTirasStock = 0,
                    CantidadStock = 0
                })
                .ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PerfilVM vm)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombosAsync(vm);
                return View(vm);
            }

            if (vm.ArchivoImagen != null && vm.ArchivoImagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await vm.ArchivoImagen.CopyToAsync(memoryStream);
                vm.Perfil.ImagenPerfil = memoryStream.ToArray();
            }

            var perfil = _mapper.Map<Perfil>(vm.Perfil);
            perfil.UbicacionRef = null;

            await _contenedorTrabajo.Perfil.AddAsync(perfil);
            await _contenedorTrabajo.SaveAsync();

            var perfilId = perfil.PerfilID;

            // Guardar tratamientos
            foreach (var tratamiento in vm.PerfilTratamientos)
            {
                if (tratamiento.UbicacionRef == null && tratamiento.CantMinimaTirasStock == 0)
                    continue;

                await _contenedorTrabajo.PerfilTratamiento.AddAsync(new PerfilTratamiento
                {
                    PerfilRef = perfilId,
                    TratamientoRef = tratamiento.TratamientoRef,
                    UbicacionRef = tratamiento.UbicacionRef,
                    CantMinimaTirasStock = tratamiento.CantMinimaTirasStock
                });
            }

            // Guardar equivalencias
            foreach (var eq in vm.PerfilEquivalencias.Where(x => !string.IsNullOrWhiteSpace(x.Codigo))
                .GroupBy(x => x.Codigo.Trim()).Select(x => x.First()))
            {
                var perfilEquivalente = await _contenedorTrabajo.Perfil
                    .GetFirstOrDefaultAsync(p => p.PerfilCodigoAlcemar == eq.Codigo.Trim());

                if (perfilEquivalente == null || perfilEquivalente.PerfilID == perfilId)
                    continue;

                await _contenedorTrabajo.PerfilEquivalencia.AddAsync(new PerfilEquivalencia
                {
                    PerfilRef = perfilId,
                    PerfilEquivalenteRef = perfilEquivalente.PerfilID
                });
            }

            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var perfil = await _contenedorTrabajo.Perfil.GetFirstOrDefaultAsync(
                x => x.PerfilID == id,
                includeProperties: "Linea,Ubicacion"
            );

            if (perfil == null)
                return NotFound();

            var vm = new PerfilVM
            {
                Perfil = perfil,
                SoloLectura = true
            };

            await CargarCombosAsync(vm);
            await CargarDatosEditAsync(vm);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //NoTracking
            var perfil = await _contenedorTrabajo.Perfil.GetFirstOrDefaultAsync(
                x => x.PerfilID == id,
                includeProperties: "Linea,Ubicacion,Tratamientos",
                NoTracking: true
            );

            if (perfil == null)
                return NotFound();

            var vm = new PerfilVM
            {
                Perfil = perfil
            };

            await CargarCombosAsync(vm);
            await CargarDatosEditAsync(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PerfilVM vm)
        {
            if (id != vm.Perfil.PerfilID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await CargarCombosAsync(vm);
                await CargarDatosEditAsync(vm);
                return View(vm);
            }

            var perfilDb = await _contenedorTrabajo.Perfil.GetFirstOrDefaultAsync(
                x => x.PerfilID == id,
                includeProperties: "Tratamientos",
                NoTracking: false
            );
            if (perfilDb == null)
                return NotFound();

            _mapper.Map(vm.Perfil, perfilDb);

            //foreach (var tratamiento in vm.PerfilTratamientos)
            //{
            //    var tdb = perfilDb.Tratamientos?.FirstOrDefault(x => x.TratamientoRef == tratamiento.TratamientoRef);
            //    if (tdb != null)
            //    {
            //        if (tratamiento.UbicacionRef == 1)
            //        {
            //            _contenedorTrabajo.PerfilTratamiento.Remove(tdb);
            //        }
            //        else
            //        {
            //            _mapper.Map(tratamiento, tdb);
            //        }
            //    }
            //    else
            //    {
            //        if (tratamiento.UbicacionRef != 1)
            //        {
            //            var nuevo = new PerfilTratamiento
            //            {
            //                PerfilRef = id,
            //                TratamientoRef = tratamiento.TratamientoRef,
            //                UbicacionRef = tratamiento.UbicacionRef,
            //                CantMinimaTirasStock = tratamiento.CantMinimaTirasStock
            //            };
            //            await _contenedorTrabajo.PerfilTratamiento.AddAsync(nuevo);
            //        }

            //    }
            //}
            perfilDb.UbicacionRef = null;

            if (vm.ArchivoImagen != null && vm.ArchivoImagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await vm.ArchivoImagen.CopyToAsync(memoryStream);
                perfilDb.ImagenPerfil = memoryStream.ToArray();
            }

            _contenedorTrabajo.Perfil.Update(perfilDb);

            var equivalenciasActuales = (await _contenedorTrabajo.PerfilEquivalencia
                .GetAllAsync(x => x.PerfilRef == id))
                .ToList();

            var codigos = vm.PerfilEquivalencias
                .Where(x => !string.IsNullOrWhiteSpace(x.Codigo))
                .Select(x => x.Codigo.Trim())
                .Distinct()
                .ToList();

            var perfilesEquivalentes = (await _contenedorTrabajo.Perfil
                .GetAllAsync(x => codigos.Contains(x.PerfilCodigoAlcemar)))
                .Where(x => x.PerfilID != id)
                .ToList();

            var idsNuevos = perfilesEquivalentes
                .Select(x => x.PerfilID)
                .ToList();

            var idsActuales = equivalenciasActuales
                .Select(x => x.PerfilEquivalenteRef)
                .ToList();

            foreach (var actual in equivalenciasActuales)
            {
                if (!idsNuevos.Contains(actual.PerfilEquivalenteRef))
                {
                    _contenedorTrabajo.PerfilEquivalencia.Remove(actual);
                }
            }

            foreach (var nuevoId in idsNuevos)
            {
                if (!idsActuales.Contains(nuevoId))
                {
                    await _contenedorTrabajo.PerfilEquivalencia.AddAsync(new PerfilEquivalencia
                    {
                        PerfilRef = id,
                        PerfilEquivalenteRef = nuevoId
                    });
                }
            }

            var tratamientosActuales = (await _contenedorTrabajo.PerfilTratamiento
                .GetAllAsync(x => x.PerfilRef == id))
                .ToDictionary(x => x.TratamientoRef);

            foreach (var tratamiento in vm.PerfilTratamientos)
            {
                if (tratamientosActuales.TryGetValue(tratamiento.TratamientoRef, out var existente))
                {
                    existente.UbicacionRef = tratamiento.UbicacionRef;
                    existente.CantMinimaTirasStock = tratamiento.CantMinimaTirasStock;
                }
                else
                {
                    await _contenedorTrabajo.PerfilTratamiento.AddAsync(new PerfilTratamiento
                    {
                        PerfilRef = id,
                        TratamientoRef = tratamiento.TratamientoRef,
                        UbicacionRef = tratamiento.UbicacionRef,
                        CantMinimaTirasStock = tratamiento.CantMinimaTirasStock
                    });
                }
            }

            // eliminar los que ya no están
            foreach (var actual in tratamientosActuales.Values)
            {
                if (!vm.PerfilTratamientos.Any(x => x.TratamientoRef == actual.TratamientoRef))
                {
                    _contenedorTrabajo.PerfilTratamiento.Remove(actual);
                }
            }

            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombosAsync(PerfilVM vm)
        {
            var proveedores = await _contenedorTrabajo.Proveedor.GetAllAsync();
            var lineas = await _contenedorTrabajo.Linea.GetAllAsync();
            var ubicaciones = await _contenedorTrabajo.Ubicacion.GetAllAsync();
            var tratamientos = await _contenedorTrabajo.Tratamiento.GetAllAsync();

            vm.Proveedores = proveedores.Select(x => new SelectListItem
            {
                Text = x.ProveedorNombre,
                Value = x.ProveedorID.ToString()
            }).ToList();

            vm.Lineas = lineas.Select(x => new SelectListItem
            {
                Text = x.LineaNombre,
                Value = x.LineaID.ToString()
            }).ToList();

            vm.Ubicaciones = ubicaciones.Select(x => new SelectListItem
            {
                Text = x.UbicacionCodigo,
                Value = x.UbicacionID.ToString()
            }).ToList();

            vm.Tratamientos = tratamientos.Select(x => new SelectListItem
            {
                Text = x.TratamientoNombre,
                Value = x.TratamientoID.ToString()
            }).ToList();
        }

        private async Task CargarDatosEditAsync(PerfilVM vm)
        {
            if (vm.Perfil.Tratamientos == null)
                throw new Exception("Error: No se cargaron los tratamientos.");
            var tratamientos = await _contenedorTrabajo.Tratamiento.GetAllAsync();
            foreach (var t in tratamientos)
            {
                var te = vm.Perfil.Tratamientos.FirstOrDefault(x => x.TratamientoRef == t.TratamientoID);
                vm.PerfilTratamientos.Add(new PerfilTratamientoVM
                {
                    TratamientoRef = t.TratamientoID,
                    UbicacionRef = te?.UbicacionRef,
                    CantMinimaTirasStock = te?.CantMinimaTirasStock ?? 0,
                    CantidadStock = te?.CantidadStock ?? 0,
                    Descripcion = t.TratamientoNombre
                });

            }

            var equivalencias = await _contenedorTrabajo.PerfilEquivalencia
                .GetAllAsync(x => x.PerfilRef == vm.Perfil.PerfilID);

            var idsEquivalentes = equivalencias
                .Select(x => x.PerfilEquivalenteRef)
                .Distinct()
                .ToList();

            var perfilesEquivalentes = await _contenedorTrabajo.Perfil
                .GetAllAsync(x => idsEquivalentes.Contains(x.PerfilID));

            vm.PerfilEquivalencias = perfilesEquivalentes
                .Select(x => new PerfilEquivalenciaVM
                {
                    PerfilEquivalenteRef = x.PerfilID,
                    Codigo = x.PerfilCodigoAlcemar ?? "",
                    Descripcion = x.Descripcion ?? ""
                })
                .ToList();
        }

        #region Llamadas a la API

        [HttpGet]
        public async Task<IActionResult> GetAll(string? codigo, string? proveedor, string? linea, string? descripcion, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var perfiles = await _contenedorTrabajo.Perfil.GetAllAsync(includeProperties: "Linea,Linea.Proveedor,Ubicacion");
            var query = perfiles.AsQueryable();

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
        public async Task<IActionResult> GetLineasPorProveedor(int proveedorId)
        {
            var lineas = await _contenedorTrabajo.Linea.GetAllAsync();

            var resultado = lineas
                .Where(x => x.ProveedorRef == proveedorId)
                .Select(x => new
                {
                    value = x.LineaID,
                    text = x.LineaNombre
                })
                .ToList();

            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> GetPerfilPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Json(null);

            var perfil = await _contenedorTrabajo.Perfil
                .GetFirstOrDefaultAsync(p => p.PerfilCodigoAlcemar == codigo);

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
        public async Task<IActionResult> Delete(int id)
        {
            var perfil = await _contenedorTrabajo.Perfil.GetAsync(id);
            if (perfil == null)
            {
                return Json(new { success = false, message = "Error: no se encontró el perfil." });
            }

            var tratamientos = (await _contenedorTrabajo.PerfilTratamiento.GetAllAsync())
                .Where(x => x.PerfilRef == id)
                .ToList();

            foreach (var item in tratamientos)
            {
                _contenedorTrabajo.PerfilTratamiento.Remove(item);
            }

            var equivalenciasComoPerfil = (await _contenedorTrabajo.PerfilEquivalencia.GetAllAsync())
                .Where(x => x.PerfilRef == id)
                .ToList();

            foreach (var item in equivalenciasComoPerfil)
            {
                _contenedorTrabajo.PerfilEquivalencia.Remove(item);
            }

            var equivalenciasComoEquivalente = (await _contenedorTrabajo.PerfilEquivalencia.GetAllAsync())
                .Where(x => x.PerfilEquivalenteRef == id)
                .ToList();

            foreach (var item in equivalenciasComoEquivalente)
            {
                _contenedorTrabajo.PerfilEquivalencia.Remove(item);
            }

            _contenedorTrabajo.Perfil.Remove(perfil);
            await _contenedorTrabajo.SaveAsync();

            return Json(new { success = true, message = "Perfil borrado correctamente." });
        }

        #endregion
    }
}