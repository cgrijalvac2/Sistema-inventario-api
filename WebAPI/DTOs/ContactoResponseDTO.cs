namespace WebAPI.DTOs
{
    public class ClienteResponseDTO
    {
        public int ID_Cliente { get; set; }

        public int ID_Contacto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string NIT { get; set; } = string.Empty;

        public string? Direccion { get; set; }

        public string? Telefono { get; set; }

        public string? CorreoElectronico { get; set; }

        public bool Estado { get; set; }
    }

    public class ProveedorResponseDTO
    {
        public int ID_Proveedor { get; set; }

        public int ID_Contacto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string NIT { get; set; } = string.Empty;

        public string? Direccion { get; set; }

        public string? Telefono { get; set; }

        public string? CorreoElectronico { get; set; }

        public int DiasCredito { get; set; }

        public bool Estado { get; set; }
    }
}
