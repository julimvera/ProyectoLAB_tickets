namespace WebApp_Tickets.Models
{

    public class TicketDetalle
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; } // Relación con TicketId
        public string DescripcionPedido { get; set; }
        public int EstadoId { get; set; }
        public Estado? Estado { get; set; } //Relacion con EstadoId
        public DateTime FechaEstado { get; set; }
    }
}
