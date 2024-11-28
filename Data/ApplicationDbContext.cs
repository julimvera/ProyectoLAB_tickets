using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp_Tickets.Models;

namespace WebApp_Tickets.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

      

            public DbSet<Ticket> Tickets { get; set; }
            public DbSet<Estado> Estados { get; set; }
            public DbSet<TicketDetalle> TicketDetalles { get; set; }
            public DbSet<Afiliado> Afiliados { get; set; }

            

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // valores predeterminados para la tabla Estado
                modelBuilder.Entity<Estado>().HasData(
                    new Estado { Id = 1, Descripcion = "Pendiente" },
                    new Estado { Id = 2, Descripcion = "Resuelto" },
                    new Estado { Id = 3, Descripcion = "Rechazado" }
            );

            // Relación entre Ticket y Afiliado con eliminación en cascada
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Afiliado)
                .WithMany() 
                .HasForeignKey(t => t.AfiliadoId)
                .OnDelete(DeleteBehavior.Cascade);  // Eliminación en cascada queeelimina tickets cuando se elimina el afiliado


            // Relación entre Ticket y Estado
            modelBuilder.Entity<Ticket>()
                    .HasOne(t => t.Estado)
                    .WithMany() 
                    .HasForeignKey(t => t.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict); 

                // Relación entre TicketDetalle y Ticket
                modelBuilder.Entity<TicketDetalle>()
                    .HasOne(td => td.Ticket)
                    .WithMany(t => t.TicketDetalles) 
                    .HasForeignKey(td => td.TicketId)
                    .OnDelete(DeleteBehavior.Restrict); 

                // Relación entre TicketDetalle y Estado
                modelBuilder.Entity<TicketDetalle>()
                    .HasOne(td => td.Estado)
                    .WithMany() 
                    .HasForeignKey(td => td.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict); 
            }


        }
    }
