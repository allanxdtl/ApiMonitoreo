$(document).ready(function () {
    const apiBase = "http://localhost:5241/api"; // Ajusta tu URL base aquí

    // 1. Cargar productos al cargar la página
    function cargarProductos() {
        $.get(`${apiBase}/ProductoTerminado/List`, function (data) {
            let select = $("#producto");
            select.empty();
            select.append('<option value="">-- Seleccionar --</option>');
            data.forEach(p => {
                select.append(`<option value="${p.productoId}">${p.nombre}</option>`);
            });
        }).fail(function () {
            Swal.fire("Error", "No se pudieron cargar los productos", "error");
        });
    }

    // Llamar a cargar productos al inicio
    cargarProductos();

    // 2. Enviar formulario para crear producción
    $("#formP").on("submit", function (e) {
        e.preventDefault();

        let productoId = $("#producto").val();
        let cantidad = $("#cantidad").val();

        if (!productoId || !cantidad) {
            Swal.fire("Atención", "Seleccione un producto y cantidad", "warning");
            return;
        }

        $.ajax({
            url: `${apiBase}/Produccion/CrearProduccion?productoId=${productoId}&cantidad=${cantidad}`,
            type: "POST",
            success: function (mensaje) {
                Swal.fire("Éxito", mensaje, "success").then(() => {
                    // Descargar el PDF del BOM después de producir
                    descargarPDF(productoId);
                });
            },
            error: function (xhr) {
                let errorMsg = xhr.responseText || "Error al crear producción";
                Swal.fire("Error", errorMsg, "error");
            }
        });
    });

    // 3. Función para descargar el PDF del BOM
    function descargarPDF(productoId) {
        $.ajax({
            url: `${apiBase}/BOM/GetByProducto/${productoId}`,
            method: "GET",
            xhrFields: {
                responseType: "blob" // Para manejar el archivo binario
            },
            success: function (data) {
                // Crear URL para descargar PDF
                let blob = new Blob([data], { type: "application/pdf" });
                let link = document.createElement("a");
                link.href = window.URL.createObjectURL(blob);
                link.download = `BOM_${productoId}.pdf`;
                link.click();
            },
            error: function () {
                Swal.fire("Error", "No se pudo generar el PDF", "error");
            }
        });
    }
});
