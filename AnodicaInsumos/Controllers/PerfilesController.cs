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
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PerfilVM vm, List<int> tratamientoIds, List<int?> ubicacionesTratamiento,
            List<int> stockMinimo, List<string> equivalenciasCodigo)
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
            if (tratamientoIds != null && tratamientoIds.Count > 0)
            {
                for (int i = 0; i < tratamientoIds.Count; i++)
                {
                    var nuevoPerfilTratamiento = new PerfilTratamiento
                    {
                        PerfilRef = perfilId,
                        TratamientoRef = (short)tratamientoIds[i],
                        UbicacionRef = (short?)((ubicacionesTratamiento != null && i < ubicacionesTratamiento.Count)
                            ? ubicacionesTratamiento[i]
                            : null),
                        CantMinimaTirasStock = (short)((stockMinimo != null && i < stockMinimo.Count)
                            ? stockMinimo[i]
                            : 0)
                    };

                    await _contenedorTrabajo.PerfilTratamiento.AddAsync(nuevoPerfilTratamiento);
                }
            }

            // Guardar equivalencias
            if (equivalenciasCodigo != null && equivalenciasCodigo.Count > 0)
            {
                var codigosUnicos = equivalenciasCodigo
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct()
                    .ToList();

                foreach (var codigo in codigosUnicos)
                {
                    var perfilEquivalente = await _contenedorTrabajo.Perfil
                        .GetFirstOrDefaultAsync(p => p.PerfilCodigoAlcemar == codigo);

                    if (perfilEquivalente == null || perfilEquivalente.PerfilID == perfilId)
                        continue;

                    await _contenedorTrabajo.PerfilEquivalencia.AddAsync(new PerfilEquivalencia
                    {
                        PerfilRef = perfilId,
                        PerfilEquivalenteRef = perfilEquivalente.PerfilID
                    });
                }
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
                includeProperties: "Linea,Ubicacion,Tratamientos"
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
        public async Task<IActionResult> Edit(int id, PerfilVM vm, List<int> tratamientoIds, List<int?> ubicacionesTratamiento,
            List<int> stockMinimo, List<string> equivalenciasCodigo)
        {
            if (id != vm.Perfil.PerfilID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await CargarCombosAsync(vm);
                await CargarDatosEditAsync(vm);
                return View(vm);
            }

            var perfilDb = await _contenedorTrabajo.Perfil.GetAsync(id);
            if (perfilDb == null)
                return NotFound();

            // Mapear datos del perfil
            _mapper.Map(vm.Perfil, perfilDb);

            //Quitar los que ya no van
            //Agregar los nuevos
            //Actualizar los modificados

            foreach (var tratamiento in vm.TratamientosV2)
            {
                var tdb = perfilDb.Tratamientos?.FirstOrDefault(x => x.TratamientoRef == tratamiento.TratamientoRef);
                if (tdb != null)
                {
                    if (tratamiento.UbicacionRef == 1)
                    {
                        perfilDb.Tratamientos.Remove(tdb);
                    }
                    else
                    {
                        _mapper.Map(tratamiento, tdb);
                    }
                }
                else
                {
                    if (tratamiento.UbicacionRef != 1)
                    {
                        var nuevo = new PerfilTratamiento
                        {
                            PerfilRef = id,
                            TratamientoRef = tratamiento.TratamientoRef,
                            UbicacionRef = tratamiento.UbicacionRef,
                            CantMinimaTirasStock = tratamiento.CantMinimaTirasStock
                        };
                        perfilDb.Tratamientos.Add(nuevo);
                    }

                }
            }
            perfilDb.UbicacionRef = null;

            if (vm.ArchivoImagen != null && vm.ArchivoImagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await vm.ArchivoImagen.CopyToAsync(memoryStream);
                perfilDb.ImagenPerfil = memoryStream.ToArray();
            }

            _contenedorTrabajo.Perfil.Update(perfilDb);

            // BORRAR equivalencias anteriores
            var equivalenciasActuales = (await _contenedorTrabajo.PerfilEquivalencia.GetAllAsync())
                .Where(x => x.PerfilRef == id)
                .ToList();

            foreach (var item in equivalenciasActuales)
                _contenedorTrabajo.PerfilEquivalencia.Remove(item);

            // INSERTAR nuevas equivalencias
            foreach (var codigo in equivalenciasCodigo.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var perfilEquivalente = await _contenedorTrabajo.Perfil
                    .GetFirstOrDefaultAsync(p => p.PerfilCodigoAlcemar == codigo);

                if (perfilEquivalente == null || perfilEquivalente.PerfilID == id)
                    continue;

                await _contenedorTrabajo.PerfilEquivalencia.AddAsync(new PerfilEquivalencia
                {
                    PerfilRef = id,
                    PerfilEquivalenteRef = perfilEquivalente.PerfilID
                });
            }

            // BORRAR tratamientos anteriores
            var tratamientosActuales = (await _contenedorTrabajo.PerfilTratamiento.GetAllAsync())
                .Where(x => x.PerfilRef == id)
                .ToList();

            foreach (var item in tratamientosActuales)
                _contenedorTrabajo.PerfilTratamiento.Remove(item);

            // INSERTAR tratamientos nuevos
            for (int i = 0; i < tratamientoIds.Count; i++)
            {
                short? ubicacion = null;

                if (ubicacionesTratamiento != null && i < ubicacionesTratamiento.Count && ubicacionesTratamiento[i] != null)
                    ubicacion = (short)ubicacionesTratamiento[i];

                short stock = 0;

                if (stockMinimo != null && i < stockMinimo.Count)
                    stock = (short)stockMinimo[i];

                var nuevo = new PerfilTratamiento
                {
                    PerfilRef = id,
                    TratamientoRef = (short)tratamientoIds[i],
                    UbicacionRef = ubicacion,
                    CantMinimaTirasStock = stock
                };

                await _contenedorTrabajo.PerfilTratamiento.AddAsync(nuevo);
            }

            // GUARDAR TODO
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
                vm.TratamientosV2.Add(new PerfilTratamientoVM
                {
                    TratamientoRef = t.TratamientoID,
                    UbicacionRef = te?.UbicacionRef,
                    CantMinimaTirasStock = te?.CantMinimaTirasStock ?? 0,
                    CantidadStock = te?.CantidadStock ?? 0,
                    Descripcion = t.TratamientoNombre
                });

            }




            //var perfilTratamientos = await _contenedorTrabajo.PerfilTratamiento.GetAllAsync();
            //var perfilEquivalencias = await _contenedorTrabajo.PerfilEquivalencia.GetAllAsync(x => x.PerfilRef == vm.Perfil.PerfilID);
            //var perfiles = await _contenedorTrabajo.Perfil.GetAllAsync();

            //vm.PerfilTratamientos = perfilTratamientos
            //    .Where(x => x.PerfilRef == vm.Perfil.PerfilID)
            //    .ToList();
            //await _contenedorTrabajo.PerfilEquivalencia.GetAllAsync();
            //vm.PerfilEquivalencias = perfilEquivalencias
            //    .Select(x =>
            //    {
            //        var perfilEquivalente = perfiles.FirstOrDefault(p => p.PerfilID == x.PerfilEquivalenteRef);

            //        return new
            //        {
            //            Codigo = perfilEquivalente?.PerfilCodigoAlcemar ?? "",
            //            Descripcion = perfilEquivalente?.Descripcion ?? ""
            //        };
            //    })
            //    .ToList<dynamic>();
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