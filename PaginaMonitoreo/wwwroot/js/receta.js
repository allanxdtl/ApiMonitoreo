$(document).ready(function () {
    $.get("http://localhost:5241/api/ProductoTerminado/List", function (data) {
        // data es un array de { productoId, nombre }
        const selectProducto = $("#producto");
        selectProducto.empty(); // Limpiar por si había opciones previas
        selectProducto.append('<option value="">-- Seleccionar --</option>');

        data.forEach(item => {
            selectProducto.append(`<option value="${item.productoId}">${item.nombre}</option>`);
        });
    }).fail(function () {
        alert("Error al cargar productos");
    });

    $.get("http://localhost:5241/api/MateriaPrima/List", function (data) {
        const selectMateria = $("#materiaPrima");
        selectMateria.empty();
        selectMateria.append('<option value="">-- Seleccionar --</option>');

        data.forEach(item => {
            selectMateria.append(`<option value="${item.materiaPrimaId}">${item.nombre} - ${item.unidadMedida}</option>`);
        });
    });
});