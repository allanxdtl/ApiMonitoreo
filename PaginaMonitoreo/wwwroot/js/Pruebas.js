import api from "./api.js";

$(document).ready(function() {
    $("#clear").click(function () {
        $("#id").val("");
        $("#descripcion").val("");
        $("#unidad").val("");
        $('#metodo').val("");   
        chargeTable(`${api}Pruebas/List`);
    });

    chargeTable(`${api}Pruebas/List`);

    function chargeTable(endpoint) {
        $.get(endpoint, function (data) {
            let tbody = $("#tablaPruebas tbody");
            tbody.empty();

            // Tomamos solo los primeros 30 registros
            let registros = data.slice(0, 30);

            registros.forEach(item => {
                let fila = $(`
                <tr data-id="${item.idprueba}" data-nombre="${item.descripcion}" data-unidad="${item.unidadMedida}">
                    <td>${item.idprueba}</td>
                    <td>${item.descripcion}</td>
                    <td>${item.unidadMedida}</td>
                    <td>${item.metodoPrueba}</td>
                </tr>
            `);

                // Evento click para rellenar los campos
                fila.on("click", function () {
                    $("#id").val($(this).data("id"));
                    $("#descripcion").val($(this).data("nombre"));
                    $("#unidad").val($(this).data("unidad"));
                    $('#metodo').val($(this).data("metodo"));
                });

                tbody.append(fila);
            });
        }).fail(function () {
            Swal.fire("Error", "Error al cargar los registros.", "error");
        });
    }

    // Registrar
    $('#registrar').on('click', function () {
        let datos = {
            descripcion: $('#descripcion').val(),
            unidadMedida: $('#unidad').val(),
            metodoPrueba: $('#metodo').val()
        };

        if (!datos.descripcion) {
            Swal.fire({
                icon: 'error',
                title: 'El campo de descripcion es obligatorio',
                text: 'Debes completar todos los campos.'
            });
            return;
        }

        $.ajax({
            url: `${api}Pruebas/Insert`,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(datos),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Registro exitoso',
                    text: 'Prueba registrada correctamente'
                });
                limpiar();
                chargeTable(`${api}Pruebas/List`);
            },
            error: function (xhr) {
                console.error(xhr);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo registrar la materia prima.'
                });
            }
        });
    });

    // Actualizar
    $('#actualizar').on('click', function () {
        let id = $('#id').val();
        let datos = {
            idprueba: parseInt(id),
            descripcion: $('#descripcion').val(),
            unidadMedida: $('#unidad').val(),
            metodoPrueba: $('#metodo').val()
        };

        if (!id || !datos.descripcion) {
            Swal.fire({
                icon: 'error',
                title: 'Campos incompletos',
                text: 'Debes llenar al menos el campo de descripcion.'
            });
            return;
        }

        $.ajax({
            url: `${api}Pruebas/Edit`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(datos),
            success:  () => {
                Swal.fire({
                    icon: 'success',
                    title: 'Actualizado',
                    text: 'La materia prima se actualizó correctamente.'
                });
                limpiar();
                chargeTable(`${api}Pruebas/List`);
            },
            error: () => {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo actualizar el registro.'
                });
            }
        });
    });
});