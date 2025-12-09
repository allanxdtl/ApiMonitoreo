/// <reference path="jquery-3.7.1.min.js" />
import apiRoute from "./api.js";

$(document).ready(function () {
  cargarOrdenesListas();

  // Cargar órdenes y llenar el select
  function cargarOrdenesListas() {
    $.get(`${apiRoute}Envios`, (data) => {
      const select = $("#ordenSelect");
      select.empty();
      select.append(`<option value="">-- Seleccionar orden --</option>`);

      data.forEach((item) => {
        select.append(`<option value="${item.idorden}">
                    Orden #${item.idorden} - Cliente ${item.idcliente}
                </option>`);
      });
    }).fail(() => {
      Swal.fire("Error", "No se pudieron cargar las órdenes", "error");
    });
  }

  // Descargar etiquetas de envío
  $("#descargarEtiquetas").on("click", function () {
    const id = $("#ordenSelect").val();

    if (!id) {
      Swal.fire("Advertencia", "Selecciona una orden primero", "warning");
      return;
    }

    Swal.fire({
      title: "Generando etiquetas...",
      text: "Por favor espera...",
      allowOutsideClick: false,
      didOpen: () => {
        Swal.showLoading();
      },
    });

    fetch(`${apiRoute}Envios/DescargarEtiquetasDeEmbalaje?ordenid=${id}`)
      .then((response) => {
        if (!response.ok) throw new Error("Error al generar etiquetas");
        return response.blob();
      })
      .then((blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = `Etiquetas_Orden_${id}.pdf`;
        document.body.appendChild(a);
        a.click();
        a.remove();

        Swal.close();
        Swal.fire("Éxito", "Etiquetas generadas correctamente", "success");
      })
      .catch(() => {
        Swal.close();
        Swal.fire("Error", "No se pudieron generar las etiquetas", "error");
      });
  });

  // Evento botón buscar
  $("#btnBuscarOrden").on("click", function () {
    const id = $("#ordenSelect").val();

    if (!id) {
      Swal.fire("Advertencia", "Selecciona una orden primero", "warning");
      return;
    }

    obtenerOrden(id);
  });

  // Obtener orden seleccionada
  function obtenerOrden(id) {
    $.get(`${apiRoute}Envios/${id}`, (data) => {
      console.log(data);
      // Si esperas un solo objeto del endpoint, usamos data directo
      const orden = Array.isArray(data) ? data[0] : data;

      // Mostrar sección
      $("#resultado").removeClass("invisible");

      // Mapear campos, si alguno no existe lo dejamos comentado en pantalla
      $("#ordenNumero").text(orden.idorden ?? "/* idorden no enviado */");
      $("#ordenCliente").text(orden.idcliente ?? "/* idcliente no enviado */");
      $("#ordenTotal").text(
        `$${
          (orden.precio / orden.cantidadPedida) * orden.cantidadProductosPasados
        }` ?? "/* precio no enviado */"
      );
      $("#ordenPruebas").text(orden.estatus ?? "/* estatus no enviado */");
      $("#ordenProducto").text(
        orden.idproducto ?? "/* idproducto no enviado */"
      );
      $("#ordenCantidad").text(
        orden.cantidadPedida ?? "/* cantidad no enviada */"
      );
      $("#ordenCantidadPasada").text(orden.cantidadProductosPasados);
      $("#ordenCajas").text(Math.ceil(orden.cantidadProductosPasados / 8));
    }).fail(() => {
      Swal.fire(
        "Error",
        "No se pudo obtener la información de la orden",
        "error"
      );
    });
  }
});
