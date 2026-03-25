using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace AnodicaInsumos.Controllers
{
    public class InsumosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly ILogger<InsumosController> _logger;

        public InsumosController(IContenedorTrabajo contenedorTrabajo, ILogger<InsumosController> logger)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Insumo insumo)
        {
            if (!ModelState.IsValid) 
                return View(insumo);

            try 
            {
                await _contenedorTrabajo.Insumo.AddAsync(insumo);
                await _contenedorTrabajo.SaveAsync();

                return View(nameof(Index));
            } catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al crear el insumo");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el insumo. Por favor, intente nuevamente.");
                return View(insumo);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(short id)
        {
            var insumo = await _contenedorTrabajo.Insumo.GetFirstOrDefaultAsync(x => x.InsumoID == id, NoTracking: true);

            if (insumo == null)
                return NotFound();

            return View(insumo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, Insumo insumo)
        {
            if (id != insumo.InsumoID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(insumo);

            try
            {
                var existe = await _contenedorTrabajo.Insumo.GetAsync(id);
                if (existe == null)
                    return NotFound();

                existe.CodigoInsumo = insumo.CodigoInsumo;
                existe.InsumoNombre = insumo.InsumoNombre;
                existe.UnidadMedida = insumo.UnidadMedida;

                _contenedorTrabajo.Insumo.Update(existe);
                await _contenedorTrabajo.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el insumo con ID {InsumoID}", id);
                ModelState.AddModelError(string.Empty, "Ocurrió un error al editar el insumo. Por favor, intente nuevamente.");
                return View(insumo);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(short id)
        {
            var insumo = await _contenedorTrabajo.Insumo.GetFirstOrDefaultAsync(x => x.InsumoID == id, NoTracking: true);
            if (insumo == null) return NotFound();
            return View(insumo);
        }

        #region Llamadas a la API
        [HttpGet]
        public async Task<IActionResult> GetAll(string? codigo, string? nombre, string? unidad, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var insumos = await _contenedorTrabajo.Insumo.GetAllAsync(
                filter: x =>
                    (string.IsNullOrWhiteSpace(codigo) || x.CodigoInsumo.Contains(codigo)) &&
                    (string.IsNullOrWhiteSpace(nombre) || x.InsumoNombre.Contains(nombre)) &&
                    (string.IsNullOrWhiteSpace(unidad) || x.UnidadMedida.Contains(unidad)),
                orderBy: q => q.OrderBy(x => x.InsumoID),
                NoTracking: true
            );

            var total = insumos.Count();

            var data = insumos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new { data, total, page, pageSize });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short id)
        {
            var insumo = await _contenedorTrabajo.Insumo.GetAsync(id);
            if (insumo == null)
            {
                return Json(new { success = false, message = "Error no se encontró el insumo." });
            }

            _contenedorTrabajo.Insumo.Remove(insumo);
            await _contenedorTrabajo.SaveAsync();

            return Json(new { success = true, message = "Insumo borrado correctamente." });
        }
        #endregion
    }
}
