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

            var perfil = _mapper.Map<Perfil>(vm.Perfil);
            perfil.UbicacionRef = null;

            if (vm.ArchivoImagen != null && vm.ArchivoImagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await vm.ArchivoImagen.CopyToAsync(memoryStream);
                perfil.ImagenPerfil = memoryStream.ToArray();
            }

            // Tratamientos
            foreach (var tratamiento in vm.PerfilTratamientos)
            {
                if (tratamiento.UbicacionRef == null && tratamiento.CantMinimaTirasStock == 0)
                    continue;

                perfil.Tratamientos.Add(new PerfilTratamiento
                {
                    TratamientoRef = tratamiento.TratamientoRef,
                    UbicacionRef = tratamiento.UbicacionRef,
                    CantMinimaTirasStock = tratamiento.CantMinimaTirasStock
                });
            }

            // Equivalencias
            var codigos = vm.PerfilEquivalencias
                .Where(x => !string.IsNullOrWhiteSpace(x.Codigo))
                .Select(x => x.Codigo.Trim())
                .Distinct()
                .ToList();

            if (codigos.Count > 0)
            {
                var perfilesEquivalentes = await _contenedorTrabajo.Perfil
                    .GetAllAsync(x => codigos.Contains(x.PerfilCodigoAlcemar), NoTracking: true);

                foreach (var perfilEquivalente in perfilesEquivalentes)
                {
                    perfil.Equivalencias.Add(new PerfilEquivalencia
                    {
                        PerfilEquivalenteRef = perfilEquivalente.PerfilID
                    });
                }
            }

            await _contenedorTrabajo.Perfil.AddAsync(perfil);
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
            var perfil = await _contenedorTrabajo.Perfil.GetFirstOrDefaultAsync(
                x => x.PerfilID == id,
                includeProperties: "Linea,Linea.Proveedor,Ubicacion,Tratamientos",
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
        public async Task<IActionResult> Edit(int id, PerfilVM vm, bool EliminarImagen)
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
                includeProperties: "Tratamientos,Equivalencias",
                NoTracking: false
            );

            if (perfilDb == null)
                return NotFound();

            _mapper.Map(vm.Perfil, perfilDb);
            perfilDb.UbicacionRef = null;

            if (EliminarImagen)
            {
                perfilDb.ImagenPerfil = null;
            }
            else if (vm.ArchivoImagen != null && vm.ArchivoImagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await vm.ArchivoImagen.CopyToAsync(memoryStream);
                perfilDb.ImagenPerfil = memoryStream.ToArray();
            }

            var tratamientosVm = vm.PerfilTratamientos
                .Where(x => x.UbicacionRef != null || x.CantMinimaTirasStock > 0)
                .ToDictionary(x => x.TratamientoRef);

            var tratamientosDb = perfilDb.Tratamientos.ToDictionary(x => x.TratamientoRef);

            foreach (var item in tratamientosVm)
            {
                if (tratamientosDb.TryGetValue(item.Key, out var existente))
                {
                    existente.UbicacionRef = item.Value.UbicacionRef;
                    existente.CantMinimaTirasStock = item.Value.CantMinimaTirasStock;
                }
                else
                {
                    perfilDb.Tratamientos.Add(new PerfilTratamiento
                    {
                        PerfilRef = id,
                        TratamientoRef = item.Value.TratamientoRef,
                        UbicacionRef = item.Value.UbicacionRef,
                        CantMinimaTirasStock = item.Value.CantMinimaTirasStock
                    });
                }
            }

            var tratamientosAEliminar = perfilDb.Tratamientos
                .Where(x => !tratamientosVm.ContainsKey(x.TratamientoRef))
                .ToList();

            foreach (var item in tratamientosAEliminar)
            {
                perfilDb.Tratamientos.Remove(item);
            }

            var codigos = vm.PerfilEquivalencias
                .Where(x => !string.IsNullOrWhiteSpace(x.Codigo))
                .Select(x => x.Codigo.Trim())
                .Distinct()
                .ToList();

            var perfilesEquivalentes = codigos.Count == 0
                ? new List<Perfil>()
                : (await _contenedorTrabajo.Perfil.GetAllAsync(
                    x => codigos.Contains(x.PerfilCodigoAlcemar) && x.PerfilID != id,
                    NoTracking: true
                )).ToList();

            var idsVm = perfilesEquivalentes
                .Select(x => x.PerfilID)
                .ToHashSet();

            var idsDb = perfilDb.Equivalencias
                .Select(x => x.PerfilEquivalenteRef)
                .ToHashSet();

            foreach (var perfilEquivalente in perfilesEquivalentes)
            {
                if (!idsDb.Contains(perfilEquivalente.PerfilID))
                {
                    perfilDb.Equivalencias.Add(new PerfilEquivalencia
                    {
                        PerfilRef = id,
                        PerfilEquivalenteRef = perfilEquivalente.PerfilID
                    });
                }
            }

            var equivalenciasAEliminar = perfilDb.Equivalencias
                .Where(x => !idsVm.Contains(x.PerfilEquivalenteRef))
                .ToList();

            foreach (var item in equivalenciasAEliminar)
            {
                perfilDb.Equivalencias.Remove(item);
            }

            _contenedorTrabajo.Perfil.Update(perfilDb);
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