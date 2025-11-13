(function ($) {
    const estado = {
        request: null,
        debounce: null
    };

    const formatoMoneda = window.Intl
        ? new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC' })
        : null;

    $(document).ready(function () {
        const buscador = $('#busqueda-productos');
        if (!buscador.length) {
            return;
        }

        buscador.on('input', function () {
            const termino = $(this).val();
            if (estado.debounce) {
                clearTimeout(estado.debounce);
            }
            estado.debounce = setTimeout(function () {
                buscarProductos(termino);
            }, 300);
        });

        buscarProductos('');
    });

    function buscarProductos(termino) {
        const loading = $('#productos-loading');
        const error = $('#productos-error');
        const empty = $('#productos-empty');
        const tabla = $('#contenedor-tabla-productos');

        if (estado.request) {
            estado.request.abort();
        }

        error.addClass('d-none');
        loading.removeClass('d-none');

        estado.request = $.ajax({
            url: '/api/productos/buscar',
            method: 'GET',
            data: { termino: termino || '' },
            success: function (data) {
                renderProductos(data || []);
            },
            error: function (xhr, status) {
                if (status === 'abort') {
                    return;
                }
                error.removeClass('d-none');
                tabla.addClass('d-none');
                empty.addClass('d-none');
            },
            complete: function () {
                loading.addClass('d-none');
                estado.request = null;
            }
        });
    }

    function renderProductos(productos) {
        const tbody = $('#tabla-productos-body');
        const tabla = $('#contenedor-tabla-productos');
        const empty = $('#productos-empty');
        const error = $('#productos-error');

        if (!productos.length) {
            tbody.empty();
            tabla.addClass('d-none');
            error.addClass('d-none');
            empty.removeClass('d-none');
            return;
        }

        const filas = productos.map(crearFilaProducto).join('');
        tbody.html(filas);
        tabla.removeClass('d-none');
        empty.addClass('d-none');
        error.addClass('d-none');
    }

    function crearFilaProducto(producto) {
        const precio = Number(producto.Precio) || 0;
        const iva = Number(producto.PorcentajeIVA) || 0;
        const precioConIva = producto.PrecioConImpuestos
            ? Number(producto.PrecioConImpuestos)
            : precio + (precio * (iva / 100));
        const precioFormateado = formatoMoneda ? formatoMoneda.format(precio) : '\u20a1' + precio.toFixed(2);
        const precioIvaFormateado = formatoMoneda ? formatoMoneda.format(precioConIva) : '\u20a1' + precioConIva.toFixed(2);

        return `
            <tr>
                <td>${escaparHtml(producto.SKU)}</td>
                <td>${escaparHtml(producto.Nombre)}</td>
                <td>${escaparHtml(producto.Marca)}</td>
                <td>${precioFormateado}</td>
                <td>${iva.toFixed(2)}%</td>
                <td>${precioIvaFormateado}</td>
                <td class="text-center">${producto.CantidadEnStock || 0}</td>
                <td class="text-center action-icons">
                    <a href="/Productos/EditarProducto/${producto.Id}" class="text-warning" title="Editar">
                        <i class="fas fa-pencil-alt"></i>
                    </a>
                    <a href="/Productos/DetallesProducto/${producto.Id}" class="text-info" title="Ver Detalles">
                        <i class="fas fa-eye"></i>
                    </a>
                    <a href="/Productos/EliminarProducto/${producto.Id}" class="text-danger" title="Eliminar">
                        <i class="fas fa-trash-alt"></i>
                    </a>
                </td>
            </tr>`;
    }

    function escaparHtml(texto) {
        if (!texto) {
            return '';
        }

        const mapa = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;'
        };

        return texto.toString().replace(/[&<>"']/g, (caracter) => mapa[caracter] || caracter);
    }
})(jQuery);
