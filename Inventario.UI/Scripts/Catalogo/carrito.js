/**
 * Sistema de Carrito de Compras con AJAX
 * Maneja todas las operaciones del carrito y actualización de stock en tiempo real
 */

// Estado global del carrito
let carritoItems = [];
let stockOriginal = {}; // Guardar el stock original de cada producto

// Inicialización cuando el documento está listo
$(document).ready(function () {
    inicializarCarrito();
});

function inicializarCarrito() {
    // Guarda el stock original de cada producto
    $('.producto-card').each(function () {
        const productoId = $(this).data('producto-id');
        const stockInicial = parseInt($(this).find('.stock-valor').text());
        stockOriginal[productoId] = stockInicial;
    });

    // Carga el carrito existente
    cargarCarrito();
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
        success: function (data) {
            mostrarExito('¡Agregado!', nombre + ' agregado al carrito');
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

    const stockOriginalProducto = stockOriginal[productoId];
    const totalEnCarrito = calcularTotalEnCarritoExceptoItem(productoId, carritoId);
    const stockDisponibleParaEsteItem = stockOriginalProducto - totalEnCarrito;

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
        text: "Se eliminará este producto del carrito",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e74c3c',
        cancelButtonColor: '#95a5a6',
        confirmButtonText: 'Sí, eliminar',
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
        text: "Se eliminarán todos los productos del carrito",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e74c3c',
        cancelButtonColor: '#95a5a6',
        confirmButtonText: 'Sí, vaciar',
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

        html += `
            <div class="carrito-item">
                <div class="carrito-item-header">
                    <div class="carrito-item-nombre">${item.NombreProducto}</div>
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
                    ₡${item.PrecioUnitario.toFixed(2)} x ${item.Cantidad} = ₡${item.Subtotal.toFixed(2)}
                </div>
            </div>
        `;
    });

    container.html(html);
    $('#btn-vaciar').show();
}

//Mostrar el carrito vacío
function mostrarCarritoVacio() {
    $('#carrito-items').html(`
        <div class="carrito-vacio">
            <i class="fas fa-shopping-cart"></i>
            <p>Tu carrito está vacío</p>
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
            $('#total-precio').text('₡' + data.Total.toFixed(2));

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
        const stockOriginalProducto = stockOriginal[productoId];
        $(this).find('.stock-valor').text(stockOriginalProducto);

        // Habilitar el botón si hay stock
        const btnAgregar = $(this).find('.btn-agregar');
        btnAgregar.prop('disabled', false);
        $(this).find('.producto-stock').removeClass('stock-bajo');
    });

    // Calcular y mostrar el stock disponible restando lo que está en el carrito
    carritoItems.forEach(function (item) {
        const card = $(`.producto-card[data-producto-id="${item.ProductoId}"]`);
        if (card.length > 0) {
            const stockOriginalProducto = stockOriginal[item.ProductoId];
            const totalEnCarrito = calcularTotalEnCarrito(item.ProductoId);
            const stockDisponible = stockOriginalProducto - totalEnCarrito;

            card.find('.stock-valor').text(stockDisponible);

            // Deshabilitar botón si no hay stock disponible
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


    //Calcular total en carrito excepto un item específico

function calcularTotalEnCarritoExceptoItem(productoId, carritoIdExcluir) {
    return carritoItems
        .filter(item => item.ProductoId === productoId && item.Id !== carritoIdExcluir)
        .reduce((total, item) => total + item.Cantidad, 0);
}

//Calcular la cantidad máxima permitida para un item del carrito
function calcularMaxCantidadPermitida(productoId, carritoId) {
    const stockOriginalProducto = stockOriginal[productoId];
    const totalOtrosItems = calcularTotalEnCarritoExceptoItem(productoId, carritoId);
    return stockOriginalProducto - totalOtrosItems;
}


//Finalizar compra y descontar del stock en BD
function finalizarCompra() {
    Swal.fire({
        title: '¿Finalizar compra?',
        text: "Se procesará tu pedido y se descontará del inventario",
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#27ae60',
        cancelButtonColor: '#95a5a6',
        confirmButtonText: 'Sí, finalizar',
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

//Mostrar mensaje de éxito
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