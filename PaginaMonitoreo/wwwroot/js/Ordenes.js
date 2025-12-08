import apiRoute from "./api.js";
$(document).ready(function () {
  const apiBase = apiRoute;

  // Función para cargar los productos
  function cargarProductos() {
    $.get(`${apiBase}ProductoTerminado/List`, function (data) {
      let select = $("#producto");
      select.empty();
      select.append('<option value="">-- Seleccionar --</option>');
      data.forEach((p) => {
        select.append(`<option value="${p.productoId}" data-precio="${p.precio}">${p.nombre}</option>`);
      });
    }).fail(function () {
      Swal.fire("Error", "No se pudieron cargar los productos", "error");
    });
  }

  $("#producto, #cantidad").on("change keyup", () => {
    const selectedOption = $("#producto option:selected");
    const precio = selectedOption.data("precio") || 0;
    const cantidad = parseFloat($("#cantidad").val()) || 0;

    const total = precio * cantidad;

    $("#total").text(`Total $: ${total.toFixed(2)}`);
  });

  // Llamar a la función para cargar productos al inicio
  cargarProductos();

  // Lógica para el modal de búsqueda de clientes
  const modal = $("#modalBuscarCliente");
  const btnBuscarCliente = $("#btnBuscarCliente");
  const closeButton = $(".close-button");
  const inputBuscarCliente = $("#inputBuscarCliente");
  const resultadosBusqueda = $("#resultadosBusqueda");
  const clienteNombreDisplay = $("#clienteNombreDisplay");
  const idClienteInput = $("#idCliente");

  // Abrir el modal al hacer clic en el botón
  btnBuscarCliente.on("click", function () {
    modal.css("display", "block");
    inputBuscarCliente.val("");
    resultadosBusqueda.empty(); // Limpiar resultados anteriores
  });

  // Cerrar el modal
  closeButton.on("click", function () {
    modal.css("display", "none");
  });

  // Cerrar el modal si el usuario hace clic fuera de él
  $(window).on("click", function (event) {
    if (event.target === modal[0]) {
      modal.css("display", "none");
    }
  });

  // Búsqueda de clientes con un retardo para evitar llamadas excesivas a la API
  let debounceTimer;
  inputBuscarCliente.on("keyup", function () {
    clearTimeout(debounceTimer);
    const nombre = $(this).val();
    if (nombre.length < 3) {
      resultadosBusqueda.empty();
      return;
    }

    debounceTimer = setTimeout(() => {
      // Se asume que existe un endpoint en el backend para buscar clientes por nombre
      $.get(`${apiBase}Cliente/Buscar/${nombre}`, function (data) {
        resultadosBusqueda.empty();
        if (data.length > 0) {
          data.forEach((cliente) => {
            const clienteDiv = $(
              `<div class="resultado-cliente" data-id="${cliente.id}" data-nombre="${cliente.razonSocial}">${cliente.razonSocial}</div>`
            );
            resultadosBusqueda.append(clienteDiv);
          });
        } else {
          resultadosBusqueda.html("<p>No se encontraron resultados.</p>");
        }
      }).fail(function () {
        resultadosBusqueda.html("<p>Error al buscar clientes.</p>");
      });
    }, 300); // 300ms de retardo
  });

  // Seleccionar un cliente de los resultados
  resultadosBusqueda.on("click", ".resultado-cliente", function () {
    const id = $(this).data("id");
    const nombre = $(this).data("nombre");

    idClienteInput.val(id);
    clienteNombreDisplay.text(nombre);
    modal.css("display", "none"); // Ocultar el modal
  });

  // Manejar el envío del formulario para crear la orden
  $("#formO").on("submit", function (e) {
    e.preventDefault();

    let idCliente = idClienteInput.val();
    let idProducto = $("#producto").val();
    let cantidad = $("#cantidad").val();

    if (!idCliente || !idProducto || !cantidad) {
      Swal.fire("Atención", "Por favor, complete todos los campos", "warning");
      return;
    }

    $.ajax({
      url: `${apiBase}Orden/CrearOrden?idCliente=${idCliente}&idProducto=${idProducto}&cantidad=${cantidad}`,
      type: "POST",
      xhrFields: {
        responseType: "blob",
      },
      success: function (data) {
        // Descargar el archivo PDF retornado por la API
        let blob = new Blob([data], { type: "application/pdf" });
        let link = document.createElement("a");
        link.href = window.URL.createObjectURL(blob);
        link.download = "Orden.pdf";
        link.click();

        Swal.fire(
          "Éxito",
          "Orden creada correctamente. El PDF se ha descargado.",
          "success"
        );
      },
      error: function (xhr) {
        let errorMsg =
          xhr.responseText || "Error al crear la orden. Intente de nuevo.";
        Swal.fire("Error", errorMsg, "error");
      },
    });
  });
});
