/**
 * Sistema de Carrito de Compras con AJAX
 * Maneja todas las operaciones del carrito y actualizacion de stock en tiempo real
 */

// Estado global del carrito
let carritoItems = [];
let stockOriginal = {}; // Guardar el stock original de cada producto
let catalogoRequest = null;
let catalogoBusquedaTimer = null;
const formateadorMoneda = window.Intl
    ? new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC' })
    : null;

function formatearMoneda(valor) {
    const numero = Number(valor) || 0;
    return formateadorMoneda ? formateadorMoneda.format(numero) : '\u20a1' + numero.toFixed(2);
}

// Inicializacion cuando el documento esta listo
$(document).ready(function () {
    inicializarCarrito();
    inicializarBusquedaCatalogo();
});

function inicializarCarrito() {
    registrarStockOriginalDesdeDom();
    cargarCarrito();
}

function registrarStockOriginalDesdeDom() {
    stockOriginal = {};
    $('.producto-card').each(function () {
        const productoId = $(this).data('producto-id');
        const stockInicial = parseInt($(this).find('.stock-valor').text(), 10);
        stockOriginal[productoId] = isNaN(stockInicial) ? 0 : stockInicial;
    });
}

// Cargar el carrito desde el servidor
function cargarCarrito() {
    $.ajax({
        url: '/api/carrito',
        type: 'GET',
        success: function (data) {
            carritoItems = data;
            renderizarCarrito();
            actualizarResumen();
            actualizarStockVisible();
        },
        error: function (xhr) {
            console.error('Error al cargar el carrito:', xhr);
            mostrarCarritoVacio();
        }
    });
}

// Inicializar la busqueda del catalogo
function inicializarBusquedaCatalogo() {
    const buscador = $('#busqueda-catalogo');
    if (!buscador.length) {
        return;
    }

    buscador.on('input', function () {
        const termino = $(this).val();
        if (catalogoBusquedaTimer) {
            clearTimeout(catalogoBusquedaTimer);
        }

        catalogoBusquedaTimer = setTimeout(function () {
            buscarProductosCatalogo(termino);
        }, 350);
    });
}

function buscarProductosCatalogo(termino) {
    const grid = $('#productos-grid');
    if (!grid.length) {
        return;
    }

    if (catalogoRequest) {
        catalogoRequest.abort();
    }

    toggleCatalogoLoading(true);
    mostrarCatalogoError(false);

    catalogoRequest = $.ajax({
        url: '/api/productos/buscar',
        type: 'GET',
        data: {
            termino: termino || '',
            soloActivos: true,
            soloConStock: true
        },
        success: function (data) {
            renderProductosCatalogo(data || []);
        },
        error: function (xhr, status) {
            if (status === 'abort') {
                return;
            }
            const mensaje = xhr.responseJSON && xhr.responseJSON.Message
                ? xhr.responseJSON.Message
                : 'No se pudo cargar el catalogo. Intenta nuevamente.';
            mostrarCatalogoError(true, mensaje);
            mostrarCatalogoVacio(false);
            mostrarGridCatalogo(false);
        },
        complete: function () {
            toggleCatalogoLoading(false);
            catalogoRequest = null;
        }
    });
}

function renderProductosCatalogo(productos) {
    const grid = $('#productos-grid');
    if (!grid.length) {
        return;
    }

    if (!Array.isArray(productos) || productos.length === 0) {
        grid.empty();
        mostrarGridCatalogo(false);
        mostrarCatalogoVacio(true);
        actualizarStockOriginalDesdeListado([]);
        return;
    }

    const tarjetas = productos.map(generarTarjetaProducto).join('');
    grid.html(tarjetas);

    mostrarCatalogoVacio(false);
    mostrarGridCatalogo(true);
    actualizarStockOriginalDesdeListado(productos);
    actualizarStockVisible();
}

function generarTarjetaProducto(producto) {
    const stockDisponible = producto.CantidadEnStock || 0;
    const claseStock = stockDisponible < 10 ? 'stock-bajo' : '';
    const botonDeshabilitado = stockDisponible === 0 ? 'disabled' : '';
    const precioNumero = Number(producto.Precio) || 0;
    const precioFormateado = formatearPrecio(precioNumero);
    const nombreSeguro = escaparTextoJs(producto.Nombre);
    const nombreVisible = escaparHtml(producto.Nombre) || 'Producto';
    const marcaVisible = escaparHtml(producto.Marca);

    return `
        <div class="producto-card" data-producto-id="${producto.Id}">
            <div class="producto-imagen">
                <i class="fas fa-box"></i>
            </div>
            <div class="producto-info">
                <h3>${nombreVisible}</h3>
                <div class="producto-marca">${marcaVisible}</div>
                <div class="producto-precio">${precioFormateado}</div>
                <div class="producto-stock ${claseStock}">
                    Stock: <span class="stock-valor">${stockDisponible}</span> unidades
                </div>
                <button class="btn-agregar"
                        onclick="agregarAlCarrito(${producto.Id}, '${nombreSeguro}', ${precioNumero}, ${stockDisponible})"
                        ${botonDeshabilitado}>
                    <i class="fas fa-cart-plus"></i> Agregar al Carrito
                </button>
            </div>
        </div>`;
}

function escaparTextoJs(texto) {
    if (!texto) {
        return '';
    }
    return texto
        .toString()
        .replace(/\\/g, '\\\\')
        .replace(/'/g, '\\\'')
        .replace(/"/g, '\\"');
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

function formatearPrecio(valor) {
    const numero = Number(valor) || 0;
    if (formateadorMoneda) {
        return formateadorMoneda.format(numero);
    }
    return '\u20a1' + numero.toFixed(2);
}

function toggleCatalogoLoading(mostrar) {
    const loading = $('#catalogo-loading');
    if (!loading.length) {
        return;
    }

    if (mostrar) {
        loading.removeClass('d-none');
    } else {
        loading.addClass('d-none');
    }
}

function mostrarCatalogoError(mostrar, mensaje) {
    const contenedor = $('#catalogo-error');
    if (!contenedor.length) {
        return;
    }

    if (mostrar) {
        if (mensaje) {
            contenedor.find('p').text(mensaje);
        }
        contenedor.removeClass('d-none');
    } else {
        contenedor.addClass('d-none');
    }
}

function mostrarCatalogoVacio(mostrar) {
    const contenedor = $('#catalogo-vacio');
    if (!contenedor.length) {
        return;
    }

    if (mostrar) {
        contenedor.removeClass('d-none');
    } else {
        contenedor.addClass('d-none');
    }
}

function mostrarGridCatalogo(mostrar) {
    const grid = $('#productos-grid');
    if (!grid.length) {
        return;
    }

    if (mostrar) {
        grid.removeClass('d-none');
    } else {
        grid.addClass('d-none');
    }
}

function actualizarStockOriginalDesdeListado(productos) {
    stockOriginal = {};
    productos.forEach(producto => {
        stockOriginal[producto.Id] = producto.CantidadEnStock || 0;
    });
}


//Agregar producto al carrito
function agregarAlCarrito(productoId, nombre, precio, stockDisponible) {
    // Verificar stock disponible visualmente
    const stockActualVisual = obtenerStockVisual(productoId);
    if (stockActualVisual <= 0) {
        mostrarError('No hay stock disponible para este producto');
        return;
    }

    $.ajax({
        url: '/api/carrito/agregar',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            ProductoId: productoId,
            Cantidad: 1
        }),
        success: function () {
            mostrarExito('Agregado', nombre + ' agregado al carrito');
            cargarCarrito(); // Recargar para que se actualice todo
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON && xhr.responseJSON.Message
                ? xhr.responseJSON.Message
                : 'No se pudo agregar el producto';
            mostrarError(errorMsg);
        }
    });
}

    //Actualizar cantidad en el carrito
function actualizarCantidad(carritoId, nuevaCantidad, productoId) {
    if (nuevaCantidad < 1) {
        eliminarDelCarrito(carritoId);
        return;
    }

    // Verificar stock disponible
    const itemCarrito = carritoItems.find(item => item.Id === carritoId);
    if (!itemCarrito) return;

    const stockOriginalProducto = stockOriginal[productoId] || 0;
    const totalEnCarrito = calcularTotalEnCarritoExceptoItem(productoId, carritoId);
    const stockDisponibleParaEsteItem = Math.max(stockOriginalProducto - totalEnCarrito, 0);

    if (nuevaCantidad > stockDisponibleParaEsteItem) {
        mostrarError(`Stock insuficiente. Disponible: ${stockDisponibleParaEsteItem} unidades`);
        // Revertir el valor en el input
        $(`input[data-carrito-id="${carritoId}"]`).val(itemCarrito.Cantidad);
        return;
    }

    $.ajax({
        url: '/api/carrito/actualizar',
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify({
            CarritoId: carritoId,
            Cantidad: nuevaCantidad
        }),
        success: function (data) {
            cargarCarrito();
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON && xhr.responseJSON.Message
                ? xhr.responseJSON.Message
                : 'No se pudo actualizar la cantidad';
            mostrarError(errorMsg);
            cargarCarrito(); // Recargar para revertir cambios
        }
    });
}

    //Eliminar producto del carrito
function eliminarDelCarrito(carritoId) {
    Swal.fire({
        title: '¿Eliminar producto?',
        text: "Se eliminara este producto del carrito",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e74c3c',
        cancelButtonColor: '#95a5a6',
        confirmButtonText: 'Si, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/api/carrito/eliminar/' + carritoId,
                type: 'DELETE',
                success: function () {
                    mostrarExito('Eliminado', 'Producto eliminado del carrito', 1500);
                    cargarCarrito();
                },
                error: function (xhr) {
                    mostrarError('No se pudo eliminar el producto');
                }
            });
        }
    });
}

    //Vaciar el carrito por completo
function vaciarCarrito() {
    Swal.fire({
        title: '¿Vaciar carrito?',
        text: "Se eliminaran todos los productos del carrito",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e74c3c',
        cancelButtonColor: '#95a5a6',
        confirmButtonText: 'Si, vaciar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/api/carrito/vaciar',
                type: 'DELETE',
                success: function () {
                    mostrarExito('Carrito vaciado', 'Todos los productos fueron eliminados', 1500);
                    cargarCarrito();
                },
                error: function (xhr) {
                    mostrarError('No se pudo vaciar el carrito');
                }
            });
        }
    });
}


//Renderizar los items del carrito
function renderizarCarrito() {
    const container = $('#carrito-items');

    if (carritoItems.length === 0) {
        mostrarCarritoVacio();
        return;
    }

    let html = '';
    carritoItems.forEach(function (item) {
        const maxCantidad = calcularMaxCantidadPermitida(item.ProductoId, item.Id);
        const cuponHtml = item.CodigoCupon
            ? `<span class="badge bg-success me-2">Cupon ${escaparHtml(item.CodigoCupon)}</span>
               <button class="btn btn-link text-danger p-0" onclick="removerCupon(${item.Id})">Quitar cupon</button>`
            : `<button class="btn btn-link p-0" onclick="mostrarModalCupon(${item.Id})">
                    <i class="fas fa-ticket-alt me-1"></i>Aplicar cupon
               </button>`;

        html += `
            <div class="carrito-item">
                <div class="carrito-item-header">
                    <div class="carrito-item-nombre">${escaparHtml(item.NombreProducto)}</div>
                    <button class="btn-eliminar" onclick="eliminarDelCarrito(${item.Id})">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
                <div class="carrito-item-controls">
                    <div class="cantidad-control">
                        <button class="btn-cantidad" onclick="actualizarCantidad(${item.Id}, ${item.Cantidad - 1}, ${item.ProductoId})">-</button>
                        <input type="number" class="cantidad-input" value="${item.Cantidad}"
                               data-carrito-id="${item.Id}"
                               onchange="actualizarCantidad(${item.Id}, parseInt(this.value), ${item.ProductoId})"
                               min="1" max="${maxCantidad}">
                        <button class="btn-cantidad" onclick="actualizarCantidad(${item.Id}, ${item.Cantidad + 1}, ${item.ProductoId})"
                                ${item.Cantidad >= maxCantidad ? 'disabled' : ''}>+</button>
                    </div>
                </div>
                <div class="carrito-item-precio">
                    <div class="small text-muted">${formatearMoneda(item.PrecioUnitario)} x ${item.Cantidad} = ${formatearMoneda(item.Subtotal)}</div>
                    <div class="small text-muted">IVA (${item.PorcentajeIVA.toFixed(2)}%): ${formatearMoneda(item.MontoIVA)}</div>
                    <div class="small ${item.MontoDescuento > 0 ? 'text-success' : 'text-muted'}">Descuento: -${formatearMoneda(item.MontoDescuento)}</div>
                    <div class="fw-bold fs-5 mt-1">Total: ${formatearMoneda(item.TotalLinea)}</div>
                </div>
                <div class="mt-2">
                    ${cuponHtml}
                </div>
            </div>
        `;
    });

    container.html(html);
    $('#btn-vaciar').show();
}

//Mostrar el carrito vacio
function mostrarCarritoVacio() {
    $('#carrito-items').html(`
        <div class="carrito-vacio">
            <i class="fas fa-shopping-cart"></i>
            <p>Tu carrito esta vacio</p>
        </div>
    `);
    $('#btn-vaciar').hide();
}

    //Actualizar el resumen del carrito
function actualizarResumen() {
    $.ajax({
        url: '/api/carrito/resumen',
        type: 'GET',
        success: function (data) {
            $('#carrito-badge').text(data.TotalItems);
            $('#total-productos').text(data.TotalProductos);
            $('#total-items').text(data.TotalItems);
            $('#subtotal-precio').text(formatearMoneda(data.Subtotal));
            $('#iva-precio').text(formatearMoneda(data.TotalIVA));
            $('#descuento-precio').text('-' + formatearMoneda(data.TotalDescuentos));
            $('#total-precio').text(formatearMoneda(data.Total));

            if (data.TotalProductos > 0) {
                $('#btn-finalizar').prop('disabled', false);
            } else {
                $('#btn-finalizar').prop('disabled', true);
            }
        }
    });
}

//Actualizar el stock visible en las tarjetas de productos
function actualizarStockVisible() {
    // Primero, restaurar todos los stocks a su valor original
    $('.producto-card').each(function () {
        const productoId = $(this).data('producto-id');
        const stockOriginalProducto = stockOriginal[productoId] || 0;
        $(this).find('.stock-valor').text(stockOriginalProducto);

        // Habilitar el boton si hay stock
        const btnAgregar = $(this).find('.btn-agregar');
        btnAgregar.prop('disabled', false);
        btnAgregar.html('<i class="fas fa-cart-plus"></i> Agregar al Carrito');
        $(this).find('.producto-stock').removeClass('stock-bajo');
    });

    // Calcular y mostrar el stock disponible restando lo que esta en el carrito
    carritoItems.forEach(function (item) {
        const card = $(`.producto-card[data-producto-id="${item.ProductoId}"]`);
        if (card.length > 0) {
            const stockOriginalProducto = stockOriginal[item.ProductoId] || 0;
            const totalEnCarrito = calcularTotalEnCarrito(item.ProductoId);
            const stockDisponible = Math.max(stockOriginalProducto - totalEnCarrito, 0);

            card.find('.stock-valor').text(stockDisponible);

            // Deshabilitar boton si no hay stock disponible
            const btnAgregar = card.find('.btn-agregar');
            if (stockDisponible <= 0) {
                btnAgregar.prop('disabled', true);
                btnAgregar.html('<i class="fas fa-ban"></i> Sin stock');
            }

            // Marcar como stock bajo si quedan menos de 10
            if (stockDisponible < 10) {
                card.find('.producto-stock').addClass('stock-bajo');
            }
        }
    });
}

//Obtener el stock visual actual de un producto
function obtenerStockVisual(productoId) {
    const card = $(`.producto-card[data-producto-id="${productoId}"]`);
    return parseInt(card.find('.stock-valor').text()) || 0;
}

//Calcular la cantidad total en carrito de un producto

function calcularTotalEnCarrito(productoId) {
    return carritoItems
        .filter(item => item.ProductoId === productoId)
        .reduce((total, item) => total + item.Cantidad, 0);
}


    //Calcular total en carrito excepto un item especifico

function calcularTotalEnCarritoExceptoItem(productoId, carritoIdExcluir) {
    return carritoItems
        .filter(item => item.ProductoId === productoId && item.Id !== carritoIdExcluir)
        .reduce((total, item) => total + item.Cantidad, 0);
}

//Calcular la cantidad maxima permitida para un item del carrito
function calcularMaxCantidadPermitida(productoId, carritoId) {
    const stockOriginalProducto = stockOriginal[productoId];
    const totalOtrosItems = calcularTotalEnCarritoExceptoItem(productoId, carritoId);
    return stockOriginalProducto - totalOtrosItems;
}


//Finalizar compra y descontar del stock en BD
function finalizarCompra() {
    Swal.fire({
        title: '¿Finalizar compra?',
        text: "Se procesara tu pedido y se descontara del inventario",
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#27ae60',
        cancelButtonColor: '#95a5a6',
        confirmButtonText: 'Si, finalizar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            // Mostrar loading
            Swal.fire({
                title: 'Procesando compra...',
                text: 'Por favor espera',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            // Realizar la compra
            $.ajax({
                url: '/api/carrito/finalizar-compra',
                type: 'POST',
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        title: '¡Pedido realizado!',
                        text: 'Tu pedido ha sido procesado exitosamente',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        // Recargar el stock original desde el servidor
                        location.reload();
                    });
                },
                error: function (xhr) {
                    const errorMsg = xhr.responseJSON && xhr.responseJSON.Message
                        ? xhr.responseJSON.Message
                        : 'No se pudo procesar la compra';

                    Swal.fire({
                        icon: 'error',
                        title: 'Error al procesar',
                        text: errorMsg
                    });
                }
            });
        }
    });
}

//Mostrar mensaje de exito
function mostrarExito(titulo, texto, timer = 1500) {
    Swal.fire({
        icon: 'success',
        title: titulo,
        text: texto,
        timer: timer,
        showConfirmButton: false
    });
}


//Mostrar mensaje de error
function mostrarError(mensaje) {
    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: mensaje
    });
}


function mostrarModalCupon(carritoId) {
    const item = carritoItems.find(i => i.Id === carritoId);
    if (!item) {
        mostrarError('No se encontro el producto en el carrito');
        return;
    }

    Swal.fire({
        title: `Cupon para ${item.NombreProducto}`,
        input: 'text',
        inputLabel: 'Ingresa el codigo del cupon',
        inputPlaceholder: 'CODIGO',
        showCancelButton: true,
        confirmButtonText: 'Aplicar',
        cancelButtonText: 'Cancelar',
        inputValidator: (value) => {
            if (!value) {
                return 'Debes ingresar un codigo';
            }
            return null;
        }
    }).then((result) => {
        if (result.isConfirmed && result.value) {
            aplicarCupon(carritoId, item.ProductoId, result.value.trim());
        }
    });
}

function aplicarCupon(carritoId, productoId, codigo) {
    $.ajax({
        url: '/api/carrito/aplicar-cupon',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            CarritoId: carritoId,
            ProductoId: productoId,
            CodigoCupon: codigo
        }),
        success: function () {
            mostrarExito('Cupon aplicado', 'Se agrego el descuento al producto');
            cargarCarrito();
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON && xhr.responseJSON.Message
                ? xhr.responseJSON.Message
                : 'No se pudo aplicar el cupon';
            mostrarError(errorMsg);
        }
    });
}

function aplicarCuponDesdeResumen() {
    const input = $('#input-cupon-global');
    const codigo = (input.val() || '').trim();
    if (!codigo) {
        mostrarError('Ingresa un codigo de cupon');
        return;
    }

    $.ajax({
        url: '/api/carrito/aplicar-cupon',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ CodigoCupon: codigo }),
        success: function () {
            mostrarExito('Cupon aplicado', 'Se intento aplicar el cupon disponible');
            input.val('');
            cargarCarrito();
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON && xhr.responseJSON.Message
                ? xhr.responseJSON.Message
                : 'No se pudo aplicar el cupon';
            mostrarError(errorMsg);
        }
    });
}

function removerCupon(carritoId) {
    $.ajax({
        url: '/api/carrito/remover-cupon/' + carritoId,
        type: 'DELETE',
        success: function () {
            mostrarExito('Cupon removido', 'Se quito el descuento del producto');
            cargarCarrito();
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON && xhr.responseJSON.Message
                ? xhr.responseJSON.Message
                : 'No se pudo remover el cupon';
            mostrarError(errorMsg);
        }
    });
}


