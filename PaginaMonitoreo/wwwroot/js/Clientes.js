/// <reference path="jquery-3.7.1.min.js" />
$(document).ready(function () {

    // Registrar
    $('#registrar').on('click', function () {
        let datos = {
            razonSocial: $('#razon').val(),
            cp: $('#cp').val()
        };

        if (!datos.razonSocial || !datos.cp) {
            Swal.fire({
                icon: 'error',
                title: 'Campos vacíos',
                text: 'Debes completar todos los campos.'
            });
            return;
        }

        $.ajax({
            url: "http://localhost:5241/api/Cliente/Insert",
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(datos),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Registro exitoso',
                    text: 'Cliente registrado correctamente'
                });
                limpiar();
            },
            error: function (xhr) {
                console.error(xhr);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo registrar el cliente.'
                });
            }
        });
    });

    // Buscar
    $('#buscar').on('click', function () {
        let id = $('#id').val();

        if (!id) {
            Swal.fire({
                icon: 'error',
                title: 'Sin ID',
                text: 'Debes ingresar un ID para buscar.'
            });
            return;
        }

        $.get(`http://localhost:5241/api/Cliente/${id}`, function (data) {
            if (data) {
                $('#razon').val(data.razonSocial);
                $('#cp').val(data.cp);

                Swal.fire({
                    icon: 'success',
                    title: 'Registro encontrado',
                    text: 'Los datos se cargaron en el formulario.'
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'No encontrado',
                    text: 'No existe un registro con ese ID.'
                });
            }
        }).fail(function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudo realizar la búsqueda.'
            });
        });
    });

    // Actualizar
    $('#actualizar').on('click', function () {
        let id = $('#id').val();
        let datos = {
            id: parseInt(id),
            razonSocial: $('#razon').val(),
            cp: $('#cp').val()
        };

        if (!id || !datos.razonSocial || !datos.cp) {
            Swal.fire({
                icon: 'error',
                title: 'Campos incompletos',
                text: 'Debes llenar todos los campos.'
            });
            return;
        }

        $.ajax({
            url: `http://localhost:5241/api/Cliente/Update`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(datos),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Actualizado',
                    text: 'El cliente se actualizó correctamente.'
                });
                limpiar();
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo actualizar el registro.'
                });
            }
        });
    });

    // Eliminar
    $('#eliminar').on('click', function () {
        const id = $('#id').val();

        if (!id) {
            Swal.fire({
                icon: 'error',
                title: 'Sin ID',
                text: 'Debes ingresar un ID para eliminar.'
            });
            return;
        }

        Swal.fire({
            title: '¿Eliminar registro?',
            text: 'Esta acción no se puede deshacer',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `http://localhost:5241/api/Cliente/Delete/${id}`,
                    method: 'DELETE',
                    success: function () {
                        Swal.fire({
                            icon: 'success',
                            title: 'Eliminado',
                            text: 'El registro se eliminó correctamente.'
                        });
                        limpiar();
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo eliminar el registro.'
                        });
                    }
                });
            }
        });
    });

    function limpiar() {
        $('#id').val('');
        $('#razon').val('');
        $('#cp').val('');
    }
});
