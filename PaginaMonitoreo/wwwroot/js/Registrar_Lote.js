$(document).ready(function () {
  let modelo = null;
  let camara = document.getElementById("camara");
  let estadoReconocimiento = document.getElementById("estadoReconocimiento");
  let camaraContainer = document.getElementById("camaraContainer");
  let btnRegistrar = document.getElementById("btnRegistrarLote");

  // Variable para guardar la clase actual a detectar
  let claseObjetivo = null;

  async function cargarModelo() {
      const URL = "https://teachablemachine.withgoogle.com/models/l9o0q_tEz/";
    modelo = await tmImage.load(`${URL}model.json`, `${URL}metadata.json`);
    console.log("Modelo cargado correctamente");
  }

  async function iniciarCamara() {
    let stream = await navigator.mediaDevices.getUserMedia({ video: true });
    camara.srcObject = stream;
    camara.play();
    reconocer();
  }

  async function reconocer() {
    if (!modelo || !claseObjetivo) return;
    const predicciones = await modelo.predict(camara);

    // Buscar la clase que corresponde a la materia seleccionada
    const predSeleccionada = predicciones.find(
      (p) => p.className === claseObjetivo
    );

    if (predSeleccionada && predSeleccionada.probability > 0.8) {
      estadoReconocimiento.innerText = "✅ Materia prima reconocida";
      estadoReconocimiento.style.color = "green";
      btnRegistrar.disabled = false;
    } else {
      estadoReconocimiento.innerText = "❌ No coincide la materia prima";
      estadoReconocimiento.style.color = "red";
      btnRegistrar.disabled = true;
    }

    requestAnimationFrame(reconocer);
  }

  // Cuando selecciones materia prima, cambia la clase objetivo y arranca cámara
  $("#materiaPrima").change(function () {
    const seleccion = $(this).val();

    if (seleccion) {
      // Mapear según el orden de tu combo
      const claseMap = {
        1: "CABLE",
        2: "BOCINA",
        3: "BOTON",
      };

      claseObjetivo = claseMap[seleccion]; // Asigna la clase correcta
      camaraContainer.style.display = "block";

      if (!modelo) {
        cargarModelo().then(() => iniciarCamara());
      } else {
        iniciarCamara();
      }
    } else {
      camaraContainer.style.display = "none";
      btnRegistrar.disabled = true;
    }
  });

  // Fecha actual automática
  let now = new Date();
  let day = ("0" + now.getDate()).slice(-2);
  let month = ("0" + (now.getMonth() + 1)).slice(-2);
  let year = now.getFullYear();
  $("#fecha").val(`${year}-${month}-${day}`);

  // Cargar opciones de materia prima desde la API
  $.get("http://localhost:5241/api/MateriaPrima/List", function (data) {
    const selectMateria = $("#materiaPrima");
    selectMateria.empty();
    selectMateria.append('<option value="">-- Seleccionar --</option>');
    data.forEach((item) => {
      selectMateria.append(
        `<option value="${item.materiaPrimaId}">${item.nombre} - ${item.unidadMedida}</option>`
      );
    });
  });

  // Registrar lote si la materia prima fue reconocida
  $("#btnRegistrarLote").click(function () {
    let cantidad = parseFloat($("#cantidad").val());
    let materiaId = parseInt($("#materiaPrima").val());

    let loteData = {
      MateriaPrimaId: materiaId,
      FechaEntrada: $("#fecha").val(),
      CantidadInicial: cantidad,
      CantidadDisponible: cantidad,
    };

    $.ajax({
      url: "http://localhost:5241/api/LoteMateriaPrima/RegistrarLote",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(loteData),
      success: function () {
        $.get(
          `http://localhost:5241/api/LoteMateriaPrima/ObtenerExistencia/${materiaId}`,
          function (data) {
            Swal.fire({
              icon: "success",
              title: "Registro exitoso",
              text: `Lote registrado exitosamente\nExistencia Actual: ${data.existenciaActual}`,
            });
          }
        );
      },
      error: function (xhr) {
        alert("Error al registrar el lote: " + xhr.responseText);
        console.log(xhr.responseText);
      },
    });
  });
});
