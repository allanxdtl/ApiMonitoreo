import apiRoute from "./api.js";

$(document).ready(function () {
  $("#btnReporte").click(function () {
    const tipo = $("#reporte").val();
    const fecha1 = $("#fecha1").val();
    const fecha2 = $("#fecha2").val();

    if (!tipo || !fecha1 || !fecha2) {
      Swal.fire(
        "Advertencia",
        "Selecciona tipo de reporte y fechas",
        "warning"
      );
      return;
    }

    const url = `${apiRoute}Reportes/${tipo}/pdf?fecha1=${fecha1}&fecha2=${fecha2}`;

    // Aquí obtenemos el PDF
    fetch(url)
      .then((response) => {
        if (!response.ok) throw new Error("Error generando el PDF");
        return response.blob();
      })
      .then((blob) => {
        const pdfUrl = URL.createObjectURL(blob);

        // mostramos el iframe
        $("#previewContainer").show();
        $("#pdfFrame").attr("src", pdfUrl);
      })
      .catch(() => {
        Swal.fire("Error", "No se pudo generar el reporte", "error");
      });
  });
});
