/// <reference path="jquery-3.7.1.min.js" />
import apiRoute from "./api.js";
$(document).ready(function () {
  $("#clear").click(function () {
    $("#id").val("");
    $("#nombre").val("");
    $("#unidad").val("");
    chargeTable(`${apiRoute}MateriaPrima/List`);
  });

  function chargeTable(endpoint) {
    $.get(endpoint, function (data) {
      let tbody = $("#tablaMateriaPrima tbody");
      tbody.empty();

      // Tomamos solo los primeros 30 registros
      let registros = data.slice(0, 30);

      registros.forEach((item) => {
        let fila = $(`
                <tr data-id="${item.materiaPrimaId}" data-nombre="${item.nombre}" data-unidad="${item.unidadMedida}">
                    <td>${item.materiaPrimaId}</td>
                    <td>${item.nombre}</td>
                    <td>${item.unidadMedida}</td>
                    <td>${item.existencia}</td>
                </tr>
            `);

        // Evento click para rellenar los campos
        fila.on("click", function () {
          $("#id").val($(this).data("id"));
          $("#nombre").val($(this).data("nombre"));
          $("#unidad").val($(this).data("unidad"));
        });

        tbody.append(fila);
      });
    }).fail(function () {
      Swal.fire({
        title: "Error",
        text: "No se pudieron cargar los datos.",
        icon: "error",
        draggable: true,
        position: "top"
      });
    });
  }

  chargeTable(`${apiRoute}MateriaPrima/List`);

  // Registrar
  $("#registrar").on("click", function () {
    let datos = {
      nombre: $("#nombre").val(),
      unidadMedida: $("#unidad").val(),
    };

    if (!datos.nombre || !datos.unidadMedida) {
      Swal.fire({
        icon: "error",
        title: "Campos vacíos",
        text: "Debes completar todos los campos.",
      });
      return;
    }

    $.ajax({
      url: `${apiRoute}MateriaPrima/Insert`,
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(datos),
      success: function () {
        Swal.fire({
          icon: "success",
          title: "Registro exitoso",
          text: "Materia prima registrada correctamente",
        });
        limpiar();
        chargeTable(`${apiRoute}MateriaPrima/List`);
      },
      error: function (xhr) {
        console.error(xhr);
        Swal.fire({
          icon: "error",
          title: "Error",
          text: "No se pudo registrar la materia prima.",
        });
      },
    });
  });

  // Buscar
  $("#buscar").on("click", function () {
    $("#id").val("");

    let text = $("#nombre").val();

    if (!text) {
      Swal.fire({
        icon: "error",
        title: "Sin texto",
        text: "Debes ingresar texto en algun campo para buscar.",
      });
      return;
    }

    chargeTable(`${apiRoute}MateriaPrima?text=${text}`);
  });

  // Actualizar
  $("#actualizar").on("click", function () {
    let id = $("#id").val();
    let datos = {
      materiaPrimaId: parseInt(id),
      nombre: $("#nombre").val(),
      unidadMedida: $("#unidad").val(),
    };

    if (!id || !datos.nombre || !datos.unidadMedida) {
      Swal.fire({
        icon: "error",
        title: "Campos incompletos",
        text: "Debes llenar todos los campos.",
      });
      return;
    }

    $.ajax({
      url: `${apiRoute}MateriaPrima/Update?id=${id}`,
      method: "PUT",
      contentType: "application/json",
      data: JSON.stringify(datos),
      success: function () {
        Swal.fire({
          icon: "success",
          title: "Actualizado",
          text: "La materia prima se actualizó correctamente.",
        });
        limpiar();
        chargeTable(`${apiRoute}MateriaPrima/List`);
      },
      error: function () {
        Swal.fire({
          icon: "error",
          title: "Error",
          text: "No se pudo actualizar el registro.",
        });
      },
    });
  });

  // Eliminar
  $("#eliminar").on("click", function () {
    const id = $("#id").val();

    if (!id) {
      Swal.fire({
        icon: "error",
        title: "Sin ID",
        text: "Debes ingresar un ID para eliminar.",
      });
      return;
    }

    Swal.fire({
      title: "¿Eliminar registro?",
      text: "Esta acción no se puede deshacer",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: `${apiRoute}MateriaPrima/Delete/${id}`,
          method: "DELETE",
          success: function () {
            Swal.fire({
              icon: "success",
              title: "Eliminado",
              text: "El registro se eliminó correctamente.",
            });
            limpiar();
          },
          error: function () {
            Swal.fire({
              icon: "error",
              title: "Error",
              text: "No se pudo eliminar el registro.",
            });
          },
        });
      }
    });
  });

  function limpiar() {
    $("#id").val("");
    $("#nombre").val("");
    $("#unidad").val("");
  }
});
