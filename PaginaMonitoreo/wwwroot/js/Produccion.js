$(document).ready(function () {
    const apiBase = "http://localhost:5241/api";

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
            success: function (produccionId) {
                Swal.fire("Éxito", "Producción creada correctamente", "success").then(() => {
                    descargarPDFCodigosBarra(produccionId);
                });
            },
            error: function (xhr) {
                let errorMsg = xhr.responseText || "Error al crear producción";

                // ❌ CAMBIO AQUÍ: La redirección ahora está dentro de la promesa de SweetAlert
                Swal.fire("Error", errorMsg, "error").then(() => {
                    window.location.href = '../pages/Registrar_Lote.html';
                });
            }
        });
    });

    // 3. Función para descargar el PDF de códigos de barra
    function descargarPDFCodigosBarra(produccionId) {
        $.ajax({
            url: `${apiBase}/Produccion/GenerarCodigosBarraPDF/${produccionId}`,
            method: "GET",
            xhrFields: {
                responseType: "blob"
            },
            success: function (data) {
                let blob = new Blob([data], { type: "application/pdf" });
                let link = document.createElement("a");
                link.href = window.URL.createObjectURL(blob);
                link.download = `CodigosBarra_${produccionId}.pdf`;
                link.click();
            },
            error: function () {
                Swal.fire("Error", "No se pudo generar el PDF de códigos de barra", "error");
            }
        });
    }
});