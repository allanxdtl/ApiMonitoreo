import api from "../js/api.js";

$(document).ready(function () {
  const GetProductos = () => {
    $.get(`${api}ProductoTerminado/List`, (data) => {
      $("#producto").empty();
      $("#producto").append(
        new Option("Seleccione un producto", "", true, true)
      );
      data.forEach((p) => {
        $("#producto").append(new Option(p.nombre, p.productoId));
      });
    });
  };

  const GetPruebas = () => {
    $.get(`${api}Pruebas/List`, (data) => {
      $("#prueba").empty();
      $("#prueba").append(new Option("Seleccione una prueba", "", true, true));
      data.forEach((p) => {
        $("#prueba").append(new Option(p.descripcion, p.idprueba));
      });
    });
  };

  const CargarTabla = (productoId) => {
    $.get(`${api}PruebasPorProducto/ListPorProducto/${productoId}`, (data) => {
      const tbody = $("#tablaPruebasPorProducto tbody");
      tbody.empty();
      data.forEach((pp) => {
        const row = `
                            <tr data-id="${pp.id}">
                                <td>${pp.idprueba}</td>
                                <td>${pp.comentario || ""}</td>
                                <td>${pp.valorEsperado}</td>
                                <td>${pp.tolerancia || ""}</td>
                            </tr>`;
        tbody.append(row);
      });
    });
  };

  // Inicialización
  GetProductos();
  GetPruebas();

  // Al cambiar producto
  $("#producto").change(function () {
    const productoId = $(this).val();
    if (productoId) CargarTabla(productoId);
    else $("#tablaPruebasPorProducto tbody").empty();
  });

  // Cargar datos en formulario al dar clic en una fila
  $(document).on("click", "#tablaPruebasPorProducto tbody tr", function () {
    const tds = $(this).children("td");
    $("#id").val(tds.eq(0).text());
    $("#valorEsperado").val(tds.eq(2).text());
    $("#tolerancia").val(tds.eq(3).text());
    $("#comentario").val(tds.eq(4).text());
  });

  // Registrar
  $("#registrar").click(() => {
    const data = {
      productoId: $("#producto").val(),
      pruebaId: $("#prueba").val(),
      valorEsperado: parseFloat($("#valorEsperado").val()),
      tolerancia: parseFloat($("#tolerancia").val()),
      comentario: $("#comentario").val(),
    };

    $.ajax({
      url: `${api}PruebasPorProducto/Insert`,
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(data),
      success: () => {
        Swal.fire("Éxito", "Registro agregado correctamente", "success");
        CargarTabla($("#producto").val());
      },
      error: () => Swal.fire("Error", "No se pudo registrar", "error"),
    });
  });

  // Actualizar
  $("#actualizar").click(() => {
    const id = $("#id").val();
    if (!id)
      return Swal.fire(
        "Atención",
        "Seleccione un registro para actualizar",
        "info"
      );

    const data = {
      id: parseInt(id),
      productoId: $("#producto").val(),
      pruebaId: $("#prueba").val(),
      valorEsperado: parseFloat($("#valorEsperado").val()),
      tolerancia: parseFloat($("#tolerancia").val()),
      comentario: $("#comentario").val(),
    };

    $.ajax({
      url: `${api}PruebasPorProducto/Edit`,
      type: "PUT",
      contentType: "application/json",
      data: JSON.stringify(data),
      success: () => {
        Swal.fire(
          "Actualizado",
          "Registro modificado correctamente",
          "success"
        );
        CargarTabla($("#producto").val());
      },
      error: () => Swal.fire("Error", "No se pudo actualizar", "error"),
    });
  });

  // Eliminar
  $("#eliminar").click(() => {
    const id = $("#id").val();
    if (!id)
      return Swal.fire(
        "Atención",
        "Seleccione un registro para eliminar",
        "info"
      );

    Swal.fire({
      title: "¿Seguro que desea eliminarlo?",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: `${api}PruebasPorProducto/Delete/${id}`,
          type: "DELETE",
          success: () => {
            Swal.fire("Eliminado", "Registro borrado correctamente", "success");
            CargarTabla($("#producto").val());
          },
          error: () => Swal.fire("Error", "No se pudo eliminar", "error"),
        });
      }
    });
  });

  // Limpiar formulario
  $("#clear").click(() => {
    $("#pruebasPorProductoForm")[0].reset();
    $("#id").val("");
  });

  // Buscar (por ID)
  $("#buscar").click(() => {
    const id = $("#id").val();
    if (!id) return Swal.fire("Atención", "Ingrese un ID para buscar", "info");

    $.get(`${api}PruebasPorProducto/Get/${id}`, (data) => {
      if (data) {
        $("#producto").val(data.productoId);
        $("#prueba").val(data.pruebaId);
        $("#valorEsperado").val(data.valorEsperado);
        $("#tolerancia").val(data.tolerancia);
        $("#comentario").val(data.comentario);
      } else {
        Swal.fire("Sin resultados", "No se encontró el registro", "info");
      }
    }).fail(() =>
      Swal.fire("Error", "No se pudo realizar la búsqueda", "error")
    );
  });
});
