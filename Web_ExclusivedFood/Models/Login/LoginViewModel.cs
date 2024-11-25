using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_ExclusivedFood.Models.Login
{
    public class LoginViewModel
    {
        // Datos de Acceso
        public string Username { get; set; }

        [Required(ErrorMessage = "El campo Nombre Completo es requerido.")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El campo Correo Electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string CorreoElectronico { get; set; }


        [Required(ErrorMessage = "El campo Teléfono es requerido.")]
        [Phone(ErrorMessage = "Ingrese un numero de telefono valido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El campo Confirmar Contraseña es requerido.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; }

        [Required(ErrorMessage = "El campo Dirección es requerido.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El campo Ciudad es requerido.")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "El campo Código Postal es requerido.")]
        public string CodigoPostal { get; set; }

    }
}