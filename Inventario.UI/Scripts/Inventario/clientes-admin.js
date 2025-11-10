(function ($) {
    const estado = {
        request: null,
        debounce: null
    };

    $(document).ready(function () {
        const buscador = $('#busqueda-clientes');
        if (!buscador.length) {
            return;
        }

        buscador.on('input', function () {
            const termino = $(this).val();
            if (estado.debounce) {
                clearTimeout(estado.debounce);
            }
            estado.debounce = setTimeout(function () {
                buscarClientes(termino);
            }, 300);
        });

        buscarClientes('');
    });

    function buscarClientes(termino) {
        const loading = $('#clientes-loading');
        const error = $('#clientes-error');
        const empty = $('#clientes-empty');
        const tabla = $('#contenedor-tabla-clientes');

        if (estado.request) {
            estado.request.abort();
        }

        error.addClass('d-none');
        loading.removeClass('d-none');

        estado.request = $.ajax({
            url: '/api/clientes/buscar',
            method: 'GET',
            data: { termino: termino || '' },
            success: function (data) {
                renderClientes(data || []);
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

    function renderClientes(clientes) {
        const tbody = $('#tabla-clientes-body');
        const tabla = $('#contenedor-tabla-clientes');
        const empty = $('#clientes-empty');
        const error = $('#clientes-error');

        if (!clientes.length) {
            tbody.empty();
            tabla.addClass('d-none');
            error.addClass('d-none');
            empty.removeClass('d-none');
            return;
        }

        const filas = clientes.map(crearFilaCliente).join('');
        tbody.html(filas);
        tabla.removeClass('d-none');
        empty.addClass('d-none');
        error.addClass('d-none');
    }

    function crearFilaCliente(cliente) {
        const estadoBadge = cliente.Estado
            ? '<span class="badge bg-success">Activo</span>'
            : '<span class="badge bg-secondary">Inactivo</span>';

        return `
            <tr>
                <td>${escaparHtml(cliente.CedulaJuridica)}</td>
                <td>${escaparHtml(cliente.Nombre)}</td>
                <td>${escaparHtml(cliente.Correo)}</td>
                <td>${escaparHtml(cliente.Telefono)}</td>
                <td class="text-center">${estadoBadge}</td>
                <td class="text-center action-icons">
                    <a href="/Clientes/EditarCliente/${cliente.Id}" class="text-warning" title="Editar">
                        <i class="fas fa-pencil-alt"></i>
                    </a>
                    <a href="/Clientes/DetallesCliente/${cliente.Id}" class="text-info" title="Ver Detalles">
                        <i class="fas fa-eye"></i>
                    </a>
                    <a href="/Clientes/EliminarCliente/${cliente.Id}" class="text-danger" title="Eliminar">
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
