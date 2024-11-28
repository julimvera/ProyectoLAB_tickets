using System.ComponentModel.DataAnnotations;

namespace WebApp_Tickets.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int AfiliadoId { get; set; }
        public Afiliado? Afiliado { get; set; } //relacion con AfiliadoId
        [DataType(DataType.Date)]  //interpretar el campo solo como fecha sin la hora
        public DateTime FechaSolicitud { get; set; }
        public string Observacion { get; set; }
        public int EstadoId { get; set; }
        public Estado? Estado { get; set; } //relacion con EstadoId

        
        public ICollection<TicketDetalle>? TicketDetalles { get; set; }
    }
}
