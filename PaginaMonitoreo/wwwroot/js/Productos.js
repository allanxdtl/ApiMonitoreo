/// <reference path="jquery-3.7.1.min.js" />
$(document).ready(function () {

    // Registrar
    $('#registrar').on('click', function () {
        let datos = {
            nombre: $('#nombre').val() + ' ' + $('#modelo').val() 
        };

        if (!datos.nombre) {
            Swal.fire({
                icon: 'error',
                title: 'Campos vacíos',
                text: 'Debes completar todos los campos.'
            });
            return;
        }

        $.ajax({
            url: "http://localhost:5241/api/ProductoTerminado/Insert",
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(datos),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Registro exitoso',
                    text: 'Producto registrado correctamente'
                });
                limpiar();
            },
            error: function (xhr) {
                console.error(xhr);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo registrar el producto.'
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

        $.get(`http://localhost:5241/api/ProductoTerminado/${id}`, function (data) {
            if (data) {
                $('#nombre').val(data.nombre);

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
            productoId: parseInt(id),
            nombre: $('#nombre').val() + ' ' + $('#modelo').val() 
        };

        if (!id || !datos.nombre) {
            Swal.fire({
                icon: 'error',
                title: 'Campos incompletos',
                text: 'Debes llenar todos los campos.'
            });
            return;
        }

        $.ajax({
            url: `http://localhost:5241/api/ProductoTerminado/Edit`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(datos),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Actualizado',
                    text: 'El producto se actualizó correctamente.'
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
                    url: `http://localhost:5241/api/ProductoTerminado/Delete/${id}`,
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
        $('#modelo').val('');
    }
});
