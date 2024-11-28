namespace WebApp_Tickets.Models
{
    public class Afiliado
    {
        public int Id { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; } //la foto es opcional

        // Relación con Tickets
        public ICollection<Ticket> Tickets { get; set; }
    }
}
