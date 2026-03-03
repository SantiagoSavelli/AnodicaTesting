document.addEventListener("DOMContentLoaded", () => {
    cargarTabla();
});

async function cargarTabla() {
    const tbody = document.querySelector("#tblInsumos tbody");
    if (!tbody) return;
    tbody.innerHTML = `<tr><td colspan="6" class="text-center text-secondary py-4">Cargando...</td></tr>`;

    try {
        const resp = await fetch("/Insumos/GetAll");
        const json = await resp.json();

        const data = json.data ?? [];
        if (data.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" class="text-center text-secondary py-4">No hay insumos cargados.</td></tr>`;
            return;
        }

        tbody.innerHTML = data.map(i => {
            const id = i.InsumoID ?? i.insumoID ?? i.id;
            const codigo = i.CodigoInsumo ?? i.codigoInsumo ?? "";
            const nombre = i.InsumoNombre ?? i.insumoNombre ?? i.NombreInsumo ?? i.nombreInsumo ?? "";
            const unidad = i.UnidadMedida ?? i.unidadMedida ?? "";

            const bajoMin = Number(i.CantidadStock ?? i.cantidadStock ?? 0) <= Number(i.CantMinimaStock ?? i.cantMinimaStock ?? 0);

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
                    <a class="btn btn-sm btn-warning" href="/Insumos/Edit/${id}" title="Editar">
                      <i class="bi bi-pencil"></i>
                    </a>
                    <button class="btn btn-sm btn-danger"
                            data-id="${id}"
                            data-nombre="${escapeHtml(nombre)}"
                            onclick="abrirModalEliminar(this)">
                        <i class="bi bi-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>`;
        }).join("");

    } catch (e) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger py-4">Error cargando insumos.</td></tr>`;
        console.error(e);
    }
}

let idAEliminar = null;
let modalEliminar = null;

document.addEventListener("DOMContentLoaded", () => {
    cargarTabla();

    modalEliminar = new bootstrap.Modal(document.getElementById("modalEliminar"));

    document.getElementById("btnConfirmarEliminar")
        .addEventListener("click", async () => {

            if (!idAEliminar) return;

            await borrarInsumo(idAEliminar);
            idAEliminar = null;
            modalEliminar.hide();
        });
});

function abrirModalEliminar(btn) {
    idAEliminar = btn.getAttribute("data-id");

    const nombre = btn.getAttribute("data-nombre");
    document.getElementById("modalEliminarDetalle").textContent =
        nombre ? `Insumo: ${nombre}` : "";

    modalEliminar.show();
}

async function borrarInsumo(id) {
    try {
        const resp = await fetch(`/Insumos/Delete/${id}`, { method: "DELETE" });
        const json = await resp.json();

        if (!json.success) {
            alert(json.message || "No se pudo borrar.");
            return;
        }

        await cargarTabla();
    } catch (e) {
        console.error(e);
        alert("Error al borrar.");
    }
}

function formatNumber(n) {
    if (n === null || n === undefined) return "";
    return Number(n).toLocaleString("es-AR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function escapeHtml(str) {
    return String(str)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}