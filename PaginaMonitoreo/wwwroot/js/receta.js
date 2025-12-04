import apiRoute from "./api";
$(document).ready(function () {
  $.get(`${apiRoute}ProductoTerminado/List`, function (data) {
    // data es un array de { productoId, nombre }
    const selectProducto = $("#producto");
    selectProducto.empty(); // Limpiar por si había opciones previas
    selectProducto.append('<option value="">-- Seleccionar --</option>');

    data.forEach((item) => {
      selectProducto.append(
        `<option value="${item.productoId}">${item.nombre}</option>`
      );
    });
  }).fail(function () {
    alert("Error al cargar productos");
  });

  $.get(`${apiRoute}MateriaPrima/List`, function (data) {
    const selectMateria = $("#materiaPrima");
    selectMateria.empty();
    selectMateria.append('<option value="">-- Seleccionar --</option>');

    data.forEach((item) => {
      selectMateria.append(
        `<option value="${item.materiaPrimaId}">${item.nombre} - ${item.unidadMedida}</option>`
      );
    });
  });

  $("#Add").click(function () {
    $.ajax({
      url: `${apiRoute}BOM/Insert`,
      type: "POST",
      data: JSON.stringify({
        productoId: $("#producto").val(),
        materiaPrimaId: $("#materiaPrima").val(),
        cantidadNecesaria: $("#cantidad").val(),
      }),
      contentType: "application/json; charset=utf-8",
      success: function () {
        Swal.fire({
          title: "Materia prima añadida con éxito al producto!",
          icon: "success",
          draggable: true,
        });
      },
      error: function () {
        Swal.fire({
          title: "Error al añadir producto al BOM",
          icon: "error",
          draggable: true,
        });
      },
    });
  });
});

document.getElementById("bom").addEventListener("click", function () {
  const productoId = $("#producto").val(); // ejemplo, puede venir dinámico
  const url = `${apiRoute}BOM/GetByProducto/${productoId}`;

  fetch(url)
    .then((response) => {
      if (!response.ok) {
        throw new Error("Error al generar PDF");
      }
      return response.blob(); // Recibimos archivo binario
    })
    .then((blob) => {
      // Crear URL temporal para descarga
      const urlBlob = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = urlBlob;
      a.download = `BOM_${productoId}.pdf`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(urlBlob); // Liberar memoria
    })
    .catch((error) => {
      console.log(error);
      Swal.fire("Error", error, "error");
    });
});
