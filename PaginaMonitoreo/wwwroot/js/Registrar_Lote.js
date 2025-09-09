$(document).ready(function () {
    var now = new Date();

    // Formatear la fecha en yyyy-mm-dd
    var day = ("0" + now.getDate()).slice(-2);
    var month = ("0" + (now.getMonth() + 1)).slice(-2);
    var year = now.getFullYear();
    var today = `${year}-${month}-${day}`;
    $('#fecha').val(today);

    // Cargar opciones de materia prima
    $.get("http://localhost:5241/api/MateriaPrima/List", function (data) {
        const selectMateria = $("#materiaPrima");
        selectMateria.empty();
        selectMateria.append('<option value="">-- Seleccionar --</option>');
        data.forEach(item => {
            selectMateria.append(`<option value="${item.materiaPrimaId}">${item.nombre} - ${item.unidadMedida}</option>`);
        });
    });

    // Registrar lote
    $('#btnRegistrarLote').click(function () {
        var cantidad = parseFloat($('#cantidad').val());
        var materiaId = parseInt($('#materiaPrima').val());

        var loteData = {
            MateriaPrimaId: materiaId,
            FechaEntrada: $('#fecha').val(),
            CantidadInicial: cantidad,
            CantidadDisponible: cantidad
        };

        $.ajax({
            url: 'http://localhost:5241/api/LoteMateriaPrima/RegistrarLote',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(loteData),
            success: function () {
                // Obtener existencia actual después de registrar el lote
                $.get(`http://localhost:5241/api/LoteMateriaPrima/ObtenerExistencia/${materiaId}`, function (data) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Registro exitoso',
                        text: `Lote registrado exitosamente\nExistencia Actual: ${data.existenciaActual}`
                    });
                });
            },
            error: function (xhr) {
                alert('Error al registrar el lote: ' + xhr.responseText);
                console.log(xhr.responseText);
            }
        });
    });
});
