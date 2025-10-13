import apiRoute from "./api.js"

$(document).ready(function () {
    $("#loginForm").on("submit", function (e) {
        e.preventDefault();

        let usuario = $("#nombre").val().trim();
        let password = $("#contrasena").val().trim();

        if (usuario === "" || password === "") {
            Swal.fire("Error", "Por favor ingresa usuario y contraseña", "error");
            return;
        }

        $.ajax({
            url: `${apiRoute}Usuario/Login`, // Ajusta al puerto de tu API
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                Usuario1: usuario,
                Password: password
            }),
            success: function (response) {
                // Guardar usuario en localStorage
                localStorage.setItem("usuario", usuario);

                // Redirigir al menú principal
                window.location.href = "Menu.html";
            },
            error: function (xhr) {
                if (xhr.status === 404) {
                    Swal.fire("Login fallido :(", "Usuario o contraseña incorrectos", "error");
                } else {
                    Swal.fire("Error", "Hubo un problema con el servidor", "error");
                }
            }
        });
    });
});