let paginaActual = 1;
const pageSize = 20;

document.addEventListener("DOMContentLoaded", () => {
    cargarTabla();

    const recargar = debounce(() => {
        paginaActual = 1;
        cargarTabla();
    }, 300);

    document.getElementById("fCodigo")?.addEventListener("input", recargar);
    document.getElementById("fProveedor")?.addEventListener("input", recargar);
    document.getElementById("fLinea")?.addEventListener("input", recargar);
    document.getElementById("fDescripcion")?.addEventListener("input", recargar);

    document.getElementById("btnLimpiar")?.addEventListener("click", () => {
        document.getElementById("fCodigo").value = "";
        document.getElementById("fProveedor").value = "";
        document.getElementById("fLinea").value = "";
        document.getElementById("fDescripcion").value = "";
        paginaActual = 1;
        cargarTabla();
    });

    document.getElementById("tblPerfiles")?.addEventListener("click", (e) => {
        const btn = e.target.closest(".btn-eliminar");
        if (!btn) return;

        const id = btn.getAttribute("data-id");
        const nombre = btn.getAttribute("data-nombre");

        confirmarEliminacion(`/Perfiles/Delete/${id}`, nombre, "perfil", cargarTabla);
    });
});

async function cargarTabla() {
    const tbody = document.querySelector("#tblPerfiles tbody");
    if (!tbody) return;

    tbody.innerHTML = `<tr><td colspan="6" class="text-center text-secondary py-4">Cargando...</td></tr>`;

    try {
        const codigo = document.getElementById("fCodigo")?.value?.trim() ?? "";
        const proveedor = document.getElementById("fProveedor")?.value?.trim() ?? "";
        const linea = document.getElementById("fLinea")?.value?.trim() ?? "";
        const descripcion = document.getElementById("fDescripcion")?.value?.trim() ?? "";

        const qs = new URLSearchParams({
            codigo,
            proveedor,
            linea,
            descripcion,
            page: String(paginaActual),
            pageSize: String(pageSize)
        });

        const resp = await fetch(`/Perfiles/GetAll?${qs.toString()}`);
        const json = await resp.json();

        const data = json.data ?? [];
        const total = json.total ?? 0;

        if (data.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" class="text-center text-secondary py-4">No hay perfiles cargados.</td></tr>`;
            renderPaginacion({
                total: 0,
                pageSize,
                paginaActual,
                onPageChange: (p) => {
                    paginaActual = p;
                    cargarTabla();
                }
            });
            return;
        }

        tbody.innerHTML = data.map(p => {
            const id = p.perfilID ?? p.PerfilID ?? p.id;
            const codigo = p.codigo ?? p.PerfilCodigoAlcemar ?? "";
            const proveedor = p.proveedor ?? "";
            const linea = p.linea ?? "";
            const descripcion = p.descripcion ?? "";
            const imagen = p.imagenPerfil ?? p.ImagenPerfil ?? null;

            return `
                <tr>
                    <td class="ps-3 fw-semibold">${escapeHtml(codigo)}</td>
                    <td>${escapeHtml(proveedor)}</td>
                    <td>${escapeHtml(linea)}</td>
                    <td>${escapeHtml(descripcion)}</td>
                    <td class="text-center">
                        ${renderImagenPerfil(imagen, codigo)}
                    </td>
                    <td class="text-end pe-3">
                        <div class="btn-group" role="group">
                            <a class="btn btn-sm btn-outline-info" href="/Perfiles/Details/${id}" title="Detalle">
                                <i class="bi bi-eye"></i>
                            </a>
                            <a class="btn btn-sm btn-warning" href="/Perfiles/Edit/${id}" title="Editar">
                                <i class="bi bi-pencil"></i>
                            </a>
                            <button class="btn btn-sm btn-danger btn-eliminar"
                                    type="button"
                                    data-id="${id}"
                                    data-nombre="${escapeHtml(codigo)}"
                                    title="Eliminar">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    </td>
                </tr>
            `;
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
        console.error(e);
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger py-4">Error cargando perfiles.</td></tr>`;
    }
}

function renderImagenPerfil(imagenBase64, codigo) {
    if (!imagenBase64) {
        return `<div class="text-secondary small">Sin imagen</div>`;
    }

    return `
        <img src="data:image/png;base64,${imagenBase64}"
             alt="Perfil ${escapeHtml(codigo)}"
             class="perfil-img"
             style="max-height:70px; max-width:140px; object-fit:contain; border-radius:6px; background-color:#0f172a; padding:4px;" />
    `;
}