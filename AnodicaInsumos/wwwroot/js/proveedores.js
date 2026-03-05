let idAEliminar = null;
let modalEliminar = null;

let paginaActual = 1;
const pageSize = 10;

document.addEventListener("DOMContentLoaded", () => {
    cargarTabla();

    modalEliminar = new bootstrap.Modal(document.getElementById("modalEliminar"));

    document.getElementById("btnConfirmarEliminar")
        ?.addEventListener("click", async () => {

            if (!idAEliminar) return;

            await borrarProveedor(idAEliminar);
            idAEliminar = null;
            modalEliminar.hide();
        });

    const recargar = debounce(() => cargarTabla(), 300);

    document.getElementById("fNombre")?.addEventListener("input", recargar);
    document.getElementById("fTelefono")?.addEventListener("input", recargar);
    document.getElementById("fEmail")?.addEventListener("input", recargar);
    document.getElementById("fProductos")?.addEventListener("input", recargar);

    document.getElementById("btnLimpiar")?.addEventListener("click", () => {
        document.getElementById("fNombre").value = "";
        document.getElementById("fTelefono").value = "";
        document.getElementById("fEmail").value = "";
        document.getElementById("fProductos").value = "";
        cargarTabla();
    });
});

async function cargarTabla() {

    const tbody = document.querySelector("#tblProveedores tbody");
    if (!tbody) return;

    tbody.innerHTML = `<tr><td colspan="5" class="text-center text-secondary py-4">Cargando...</td></tr>`;

    try {

        const nombre = document.getElementById("fNombre")?.value?.trim() ?? "";
        const telefono = document.getElementById("fTelefono")?.value?.trim() ?? "";
        const email = document.getElementById("fEmail")?.value?.trim() ?? "";
        const productos = document.getElementById("fProductos")?.value?.trim() ?? "";

        const qs = new URLSearchParams({
            nombre,
            telefono,
            email,
            productos,
            page: String(paginaActual),
            pageSize: String(pageSize)
        });

        const resp = await fetch(`/Proveedores/GetAll?${qs.toString()}`);
        const json = await resp.json();

        const data = json.data ?? [];
        const total = json.total ?? 0;

        if (data.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-secondary py-4">No hay proveedores cargados.</td></tr>`;
            return;
        }

        tbody.innerHTML = data.map(p => {

            const id = p.ProveedorID ?? p.proveedorID ?? p.id;

            const nombre = p.ProveedorNombre ?? p.proveedorNombre ?? "";
            const telefono = p.Telefonos ?? p.telefonos ?? "";
            const email = p.Email ?? p.email ?? "";
            const productos = p.Productos ?? p.productos ?? "";

            return `
              <tr>
                <td class="ps-3">${escapeHtml(nombre)}</td>
                <td>${escapeHtml(telefono)}</td>
                <td class="text-center">${escapeHtml(email)}</td>
                <td class="text-center">${escapeHtml(productos)}</td>

                <td class="text-end pe-3">
                  <div class="btn-group" role="group">

                    <a class="btn btn-sm btn-outline-info"
                       href="/Proveedores/Details/${id}" title="Detalle">
                       <i class="bi bi-eye"></i>
                    </a>

                    <a class="btn btn-sm btn-warning"
                       href="/Proveedores/Edit/${id}" title="Editar">
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
              </tr>
            `;
        }).join("");

        renderPaginacion(total);

    }
    catch (e) {
        console.error(e);
        tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger py-4">Error cargando proveedores.</td></tr>`;
    }
}

function abrirModalEliminar(btn) {

    idAEliminar = btn.getAttribute("data-id");

    const nombre = btn.getAttribute("data-nombre");

    document.getElementById("modalEliminarDetalle").textContent =
        nombre ? `Proveedor: ${nombre}` : "";

    modalEliminar.show();
}

async function borrarProveedor(id) {

    try {

        const resp = await fetch(`/Proveedores/Delete/${id}`, {
            method: "DELETE"
        });

        const json = await resp.json();

        if (!json.success) {
            alert(json.message || "No se pudo borrar.");
            return;
        }

        await cargarTabla();
    }
    catch (e) {

        console.error(e);
        alert("Error al borrar.");
    }
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

    if (paginaActual > totalPaginas) paginaActual = totalPaginas;

    const desde = total === 0 ? 0 : (paginaActual - 1) * pageSize + 1;
    const hasta = Math.min(paginaActual * pageSize, total);

    info.textContent =
        total === 0
            ? "0 resultados"
            : `Mostrando ${desde}–${hasta} de ${total}`;

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