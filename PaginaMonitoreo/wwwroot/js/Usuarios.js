import apiRoute from "./api.js";

$(document).ready(function () {

    const limpiar = () => {
        $('#id').val('');
        $('#nombre').val('');
        $('#apellido').val('');
        $('#usuario').val('');
        $('#pass').val('');
    }

    $(".registrar").click(function () {
        let datos = {
            id: 0,
            nombre: $('#nombre').val(),
            apellido: $('#apellido').val(),
            usuario1: $('#usuario').val(),
            password: $('#pass').val()
        };

        if (!datos.nombre || !datos.apellido || !datos.usuario1 || !datos.password) {
            Swal.fire("OJO", "Debes completar todos los campos", "warning");
            return;
        }

        $.ajax({
            url: `${apiRoute}Usuario/Crear`,
            type: "POST",
            data: JSON.stringify(datos),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (success) {
                console.log(success);
                Swal.fire("Éxito", `${success.message}`, "success");
                limpiar();
            },
            error: function (xhr) {
                if (xhr.status === 409) {
                    Swal.fire({
                        title: "Error", text: `${xhr.responseJSON.message}`, icon: "error",
                        draggable: true, position: 'top'
                    });
                } else {
                    Swal.fire("Error", "Ocurrió un error al intentar registrar el usuario", "error");
                }
            }
        });
    })
});