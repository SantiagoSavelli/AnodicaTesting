let paginaActual = 1;
const pageSize = 10;

document.addEventListener("DOMContentLoaded", () => {
    cargarTabla();

    const recargar = debounce(() => cargarTabla(), 300);

    document.getElementById("fCodigo")?.addEventListener("input", recargar);
    document.getElementById("fNombre")?.addEventListener("input", recargar);
    document.getElementById("fUnidad")?.addEventListener("input", recargar);

    document.getElementById("btnLimpiar")?.addEventListener("click", () => {
        document.getElementById("fCodigo").value = "";
        document.getElementById("fNombre").value = "";
        document.getElementById("fUnidad").value = "";
        paginaActual = 1;
        cargarTabla();
    });

    document.getElementById("tblInsumos")?.addEventListener("click", (e) => {
        const btn = e.target.closest(".btn-eliminar");
        if (!btn) return;

        const id = btn.getAttribute("data-id");
        const nombre = btn.getAttribute("data-nombre");

        confirmarEliminacion(`/Insumos/Delete/${id}`, nombre, "insumo", cargarTabla);
    });
});

async function cargarTabla() {
    const tbody = document.querySelector("#tblInsumos tbody");
    if (!tbody) return;

    tbody.innerHTML = `<tr><td colspan="6" class="text-center text-secondary py-4">Cargando...</td></tr>`;

    try {
        const codigo = document.getElementById("fCodigo")?.value?.trim() ?? "";
        const nombre = document.getElementById("fNombre")?.value?.trim() ?? "";
        const unidad = document.getElementById("fUnidad")?.value?.trim() ?? "";

        const qs = new URLSearchParams({
            codigo,
            nombre,
            unidad,
            page: String(paginaActual),
            pageSize: String(pageSize)
        });

        const resp = await fetch(`/Insumos/GetAll?${qs.toString()}`);
        const json = await resp.json();

        const data = json.data ?? [];
        const total = json.total ?? 0;

        if (data.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" class="text-center text-secondary py-4">No hay insumos cargados.</td></tr>`;
            return;
        }

        tbody.innerHTML = data.map(i => {
            const id = i.InsumoID ?? i.insumoID ?? i.id;
            const codigo = i.CodigoInsumo ?? i.codigoInsumo ?? "";
            const nombre = i.InsumoNombre ?? i.insumoNombre ?? i.NombreInsumo ?? i.nombreInsumo ?? "";
            const unidad = i.UnidadMedida ?? i.unidadMedida ?? "";

            return `
              <tr>
                <td class="ps-3">${escapeHtml(codigo)}</td>
                <td class="fw-semibold">${escapeHtml(nombre)}</td>
                <td class="text-center">
                  <span class="badge rounded-pill bg-secondary px-3 py-2">
                    ${escapeHtml(unidad)}
                  </span>
                </td>
                <td class="text-end pe-3">
                  <div class="btn-group" role="group">
                    <a class="btn btn-sm btn-outline-info" href="/Insumos/Details/${id}" title="Detalle">
                        <i class="bi bi-eye"></i>
                    </a>
                    <a class="btn btn-sm btn-warning" href="/Insumos/Edit/${id}" title="Editar">
                        <i class="bi bi-pencil"></i>
                    </a>
                    <button class="btn btn-sm btn-danger btn-eliminar"
                            type="button"
                            data-id="${id}"
                            data-nombre="${escapeHtml(nombre)}">
                        <i class="bi bi-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>`;
        }).join("");

        renderPaginacion({
            total,
            pageSize,
            paginaActual,
            onPageChange: (p) => {
                paginaActual = p;
                cargarTabla();
            }
        });

    } catch (e) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger py-4">Error cargando insumos.</td></tr>`;
        console.error(e);
    }
}