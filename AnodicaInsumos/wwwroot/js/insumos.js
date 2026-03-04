let idAEliminar = null;
let modalEliminar = null;

let paginaActual = 1;
const pageSize = 10;

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

    const recargar = debounce(() => cargarTabla(), 300);

    document.getElementById("fCodigo")?.addEventListener("input", recargar);
    document.getElementById("fNombre")?.addEventListener("input", recargar);
    document.getElementById("fUnidad")?.addEventListener("input", recargar);

    document.getElementById("btnLimpiar")?.addEventListener("click", () => {
        document.getElementById("fCodigo").value = "";
        document.getElementById("fNombre").value = "";
        document.getElementById("fUnidad").value = "";
        cargarTabla();
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
            codigo, nombre, unidad,
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
        renderPaginacion(total);
    } catch (e) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger py-4">Error cargando insumos.</td></tr>`;
        console.error(e);
    }
}

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

function debounce(fn, delay = 300) {
    let t;
    return (...args) => {
        clearTimeout(t);
        t = setTimeout(() => fn(...args), delay);
    };
}

function renderPaginacion(total) {
    const ul = document.getElementById("paginacion");
    const info = document.getElementById("paginacionInfo");
    if (!ul || !info) return;

    const totalPaginas = Math.max(1, Math.ceil(total / pageSize));

    // Ajuste si la página actual quedó fuera (por filtros)
    if (paginaActual > totalPaginas) paginaActual = totalPaginas;

    const desde = total === 0 ? 0 : (paginaActual - 1) * pageSize + 1;
    const hasta = Math.min(paginaActual * pageSize, total);
    info.textContent = total === 0
        ? "0 resultados"
        : `Mostrando ${desde}–${hasta} de ${total}`;

    // Limitar botones visibles (no llenar de números)
    const maxBtns = 5;
    let start = Math.max(1, paginaActual - Math.floor(maxBtns / 2));
    let end = Math.min(totalPaginas, start + maxBtns - 1);
    start = Math.max(1, end - maxBtns + 1);

    ul.innerHTML = `
      ${navBtn("«", 1, paginaActual === 1)}
      ${navBtn("‹", paginaActual - 1, paginaActual === 1)}
      ${rango(start, end).map(p => itemNumero(p, p === paginaActual)).join("")}
      ${navBtn("›", paginaActual + 1, paginaActual === totalPaginas)}
      ${navBtn("»", totalPaginas, paginaActual === totalPaginas)}
    `;

    // enganchar handlers
    ul.querySelectorAll("[data-page]").forEach(btn => {
        btn.addEventListener("click", () => {
            const p = Number(btn.getAttribute("data-page"));
            irPagina(p);
        });
    });
}

function irPagina(p) {
    paginaActual = p;
    cargarTabla();
}

function itemPagina(texto, disabled, onClick) {
    // Se renderiza como li/a; el click lo agregamos con data-page
    // Para « ‹ › » usamos data-page desde renderPaginacion
    return ""; // lo vamos a generar con itemNumero/rango para simplificar
}

function itemNumero(p, active) {
    return `
    <li class="page-item ${active ? "active" : ""}">
      <button class="page-link ${active ? "bg-primary border-primary" : "bg-dark text-light border-secondary"}"
              data-page="${p}" type="button">${p}</button>
    </li>`;
}

function rango(a, b) {
    const arr = [];
    for (let i = a; i <= b; i++) arr.push(i);
    return arr;
}

function navBtn(label, page, disabled) {
    return `
    <li class="page-item ${disabled ? "disabled" : ""}">
      <button class="page-link bg-dark text-light border-secondary"
              ${disabled ? "disabled" : ""} data-page="${page}" type="button">${label}</button>
    </li>`;
}