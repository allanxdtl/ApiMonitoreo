import apiRoute from "./api.js";

$(document).ready(function () {
  const apiBase = apiRoute;

  // 1️⃣ Cargar las órdenes pendientes
  function cargarOrdenesPendientes() {
    $.get(`${apiBase}Orden/OrdenesPendientes`, function (data) {
      let tbody = $("#tablaOrdenes tbody");
      tbody.empty();

      if (data.length === 0) {
        tbody.append(`<tr><td colspan="5">No hay órdenes pendientes</td></tr>`);
        return;
      }

      data.forEach((o) => {
        let fila = `
                    <tr>
                        <td>${o.idorden}</td>
                        <td>${new Date(o.fechaOrden).toLocaleDateString()}</td>
                        <td>${o.nombre}</td>
                        <td>${o.cantidad}</td>
                        <td>${o.estatus}</td>
                        <td>
                            <button class="btn-producir" data-id="${
                              o.productoId
                            }" data-cantidad="${o.cantidad}" data-idorden=${
          o.idorden
        }>
                                Mandar a producción
                            </button>
                        </td>
                    </tr>`;
        tbody.append(fila);
      });
    }).fail(() => {
      Swal.fire(
        "Error",
        "No se pudieron cargar las órdenes pendientes",
        "error"
      );
    });
  }

  cargarOrdenesPendientes();

  // 2️⃣ Mandar a producción una orden
  $(document).on("click", ".btn-producir", function () {
    let productoId = $(this).data("id");
    let cantidad = $(this).data("cantidad");
    let idorden = $(this).data("idorden");

    Swal.fire({
      title: "¿Mandar a producción?",
      text: `Producto ID ${productoId} - Cantidad ${cantidad}`,
      icon: "question",
      showCancelButton: true,
      confirmButtonText: "Sí, producir",
      cancelButtonText: "Cancelar",
    }).then((result) => {
      if (result.isConfirmed) {
        // 🔹 Mostrar modal de "Creando producción..."
        Swal.fire({
          title: "Creando producción...",
          html: `
          <img src="../images/produccion.gif" alt="Produciendo el producto" width="100" height="100" />
          <p style="margin-top:10px;">Por favor, espere un momento... Estamos en produccion de la orden</p>
        `,
          allowOutsideClick: false,
          showConfirmButton: false,
          didOpen: () => {
            // Esperar 5 segundos antes de hacer la petición
            setTimeout(() => {
              $.ajax({
                url: `${apiBase}Produccion/CrearProduccion?productoId=${productoId}&cantidad=${cantidad}&orden=${idorden}`,
                type: "POST",
                success: function (produccionId) {
                  Swal.fire(
                    "Éxito",
                    "Producción creada correctamente",
                    "success"
                  ).then(() => {
                    descargarPDFCodigosBarra(produccionId);
                    cargarOrdenesPendientes(); // refrescar tabla
                  });
                },
                error: function (xhr) {
                  let errorMsg =
                    xhr.responseText || "Error al crear la producción";
                  Swal.fire("Error", errorMsg, "error");
                },
              });
            }, 5000); // ⏱ Espera de 5 segundos
          },
        });
      }
    });
  });

  // 3️⃣ Descargar PDF de códigos de barra
  function descargarPDFCodigosBarra(produccionId) {
    $.ajax({
      url: `${apiBase}Produccion/GenerarCodigosBarraPDF/${produccionId}`,
      method: "GET",
      xhrFields: { responseType: "blob" },
      success: function (data) {
        let blob = new Blob([data], { type: "application/pdf" });
        let link = document.createElement("a");
        link.href = window.URL.createObjectURL(blob);
        link.download = `CodigosBarra_${produccionId}.pdf`;
        link.click();
      },
      error: function () {
        Swal.fire(
          "Error",
          "No se pudo generar el PDF de códigos de barra",
          "error"
        );
      },
    });
  }
});
