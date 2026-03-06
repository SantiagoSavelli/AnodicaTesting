let modalEliminarGlobal = null;
let deleteCallbackActual = null;
let deleteUrlActual = null;

document.addEventListener("DOMContentLoaded", () => {
    const modalEl = document.getElementById("modalEliminar");

    if (modalEl) {
        modalEliminarGlobal = bootstrap.Modal.getOrCreateInstance(modalEl);

        modalEl.addEventListener("hidden.bs.modal", () => {
            limpiarBackdrop();
            deleteCallbackActual = null;
            deleteUrlActual = null;
        });
    }

    document.getElementById("btnConfirmarEliminar")?.addEventListener("click", async () => {
        if (!deleteUrlActual) return;

        try {
            const resp = await fetch(deleteUrlActual, { method: "DELETE" });
            const json = await resp.json();

            if (!json.success) {
                alert(json.message || "No se pudo eliminar.");
                return;
            }

            modalEliminarGlobal?.hide();
            setTimeout(limpiarBackdrop, 50);

            if (typeof deleteCallbackActual === "function") {
                deleteCallbackActual();
            }
        }
        catch (e) {
            console.error(e);
            alert("Error al eliminar.");
        }
    });
});

function confirmarEliminacion(url, nombre = "", tipo = "registro", callback = null) {
    deleteUrlActual = url;
    deleteCallbackActual = callback;

    const detalle = document.getElementById("modalEliminarDetalle");
    if (detalle) {
        detalle.textContent = nombre ? `${capitalizar(tipo)}: ${nombre}` : "";
    }

    const texto = document.querySelector("#modalEliminar .modal-body");
    if (texto) {
        const primerTexto = texto.childNodes[0];
        if (primerTexto && primerTexto.nodeType === Node.TEXT_NODE) {
            primerTexto.textContent = `¿Seguro que querés eliminar este ${tipo}?`;
        }
    }

    modalEliminarGlobal?.show();
}

function limpiarBackdrop() {
    document.body.classList.remove("modal-open");
    document.body.style.removeProperty("overflow");
    document.body.style.removeProperty("padding-right");
    document.querySelectorAll(".modal-backdrop").forEach(b => b.remove());
}

function debounce(fn, delay = 300) {
    let t;
    return (...args) => {
        clearTimeout(t);
        t = setTimeout(() => fn(...args), delay);
    };
}

function escapeHtml(str) {
    return String(str ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

function formatNumber(n, decimals = 2) {
    if (n === null || n === undefined || n === "") return "";
    return Number(n).toLocaleString("es-AR", {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
    });
}

function capitalizar(texto) {
    if (!texto) return "";
    return texto.charAt(0).toUpperCase() + texto.slice(1);
}

function renderPaginacion({ total, pageSize, paginaActual, onPageChange, paginacionId = "paginacion", infoId = "paginacionInfo" }) {
    const ul = document.getElementById(paginacionId);
    const info = document.getElementById(infoId);

    if (!ul || !info) return;

    const totalPaginas = Math.max(1, Math.ceil(total / pageSize));
    const pagina = paginaActual > totalPaginas ? totalPaginas : paginaActual;

    const desde = total === 0 ? 0 : (pagina - 1) * pageSize + 1;
    const hasta = Math.min(pagina * pageSize, total);

    info.textContent = total === 0
        ? "0 resultados"
        : `Mostrando ${desde}–${hasta} de ${total}`;

    const maxBtns = 5;
    let start = Math.max(1, pagina - Math.floor(maxBtns / 2));
    let end = Math.min(totalPaginas, start + maxBtns - 1);
    start = Math.max(1, end - maxBtns + 1);

    ul.innerHTML = `
        ${navBtn("«", 1, pagina === 1)}
        ${navBtn("‹", pagina - 1, pagina === 1)}
        ${rango(start, end).map(p => itemNumero(p, p === pagina)).join("")}
        ${navBtn("›", pagina + 1, pagina === totalPaginas)}
        ${navBtn("»", totalPaginas, pagina === totalPaginas)}
    `;

    ul.querySelectorAll("[data-page]").forEach(btn => {
        btn.addEventListener("click", () => {
            const p = Number(btn.getAttribute("data-page"));
            if (typeof onPageChange === "function") {
                onPageChange(p);
            }
        });
    });
}

function itemNumero(p, active) {
    return `
        <li class="page-item ${active ? "active" : ""}">
            <button class="page-link ${active ? "bg-primary border-primary" : "bg-dark text-light border-secondary"}"
                    data-page="${p}" type="button">${p}</button>
        </li>`;
}

function navBtn(label, page, disabled) {
    return `
        <li class="page-item ${disabled ? "disabled" : ""}">
            <button class="page-link bg-dark text-light border-secondary"
                    ${disabled ? "disabled" : ""} data-page="${page}" type="button">${label}</button>
        </li>`;
}

function rango(a, b) {
    const arr = [];
    for (let i = a; i <= b; i++) arr.push(i);
    return arr;
}