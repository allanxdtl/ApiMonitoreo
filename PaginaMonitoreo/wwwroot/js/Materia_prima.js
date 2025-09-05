/// <reference path="jquery-3.7.1.min.js" />
$(document).ready(function () {

    // Registrar
    $('#registrar').on('click', function () {
        let datos = {
            nombre: $('#nombre').val(),
            unidadMedida: $('#unidad').val()
        };

        if (!datos.nombre || !datos.unidadMedida) {
            Swal.fire({
                icon: 'error',
                title: 'Campos vacíos',
                text: 'Debes completar todos los campos.'
            });
            return;
        }

        $.ajax({
            url: "http://localhost:5241/api/MateriaPrima/Insert",
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(datos),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Registro exitoso',
                    text: 'Materia prima registrada correctamente'
                });
                limpiar();
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

        $.get(`http://localhost:5241/api/MateriaPrima/${id}`, function (data) {
            if (data) {
                $('#nombre').val(data.nombre);
                $('#unidad').val(data.unidadMedida);

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
            materiaPrimaId: parseInt(id),
            nombre: $('#nombre').val(),
            unidadMedida: $('#unidad').val()
        };

        if (!id || !datos.nombre || !datos.unidadMedida) {
            Swal.fire({
                icon: 'error',
                title: 'Campos incompletos',
                text: 'Debes llenar todos los campos.'
            });
            return;
        }

        $.ajax({
            url: `http://localhost:5241/api/MateriaPrima/Update?id=${id}`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(datos),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Actualizado',
                    text: 'La materia prima se actualizó correctamente.'
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
                    url: `http://localhost:5241/api/MateriaPrima/Delete/${id}`,
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
        $('#nombre').val('');
        $('#unidad').val('');
    }
});
