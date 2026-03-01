using AuditSentinel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Data
{
    public class ApplicationDBContext : IdentityDbContext<Usuarios>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        // Agregar Modelos
        public DbSet<AuditSentinel.Models.Servidores> Servidores { get; set; }
        public DbSet<AuditSentinel.Models.Escaneos> Escaneos { get; set; }
        public DbSet<AuditSentinel.Models.EscaneosServidores> EscaneosServidores { get; set; }
        public DbSet<AuditSentinel.Models.Plantillas> Plantillas { get; set; }
        public DbSet<AuditSentinel.Models.EscaneosPlantillas> EscaneosPlantillas { get; set; }
        public DbSet<AuditSentinel.Models.Vulnerabilidades> Vulnerabilidades { get; set; }
        public DbSet<AuditSentinel.Models.PlantillasVulnerabilidades> PlantillasVulnerabilidades { get; set; }
        public DbSet<AuditSentinel.Models.Reportes> Reportes { get; set; }
        public DbSet<AuditSentinel.Models.EscaneosReportes> EscaneosReportes { get; set; }
        public DbSet<AuditSentinel.Models.EscaneosVulnerabilidades> EscaneosVulnerabilidades { get; set; }
        public DbSet<LogErroresEscaneo> LogErroresEscaneos { get; set; }
        public DbSet<AuditSentinel.Models.Correo> Correos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // servidores
            //nombre servidor debe ser unico
            modelBuilder.Entity<Servidores>()
               .HasIndex(s => s.NombreServidor)
                .IsUnique();
            // indice unico para IP

            modelBuilder.Entity<Servidores>()
                .HasIndex(s => s.IP)
                .IsUnique();

            //Enum sistemas operativos
            modelBuilder.Entity<Servidores>()
                .Property(s => s.SistemaOperativo)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Almacenar enum como string
            modelBuilder.Entity<Escaneos>()
                .Property(e => e.Estado)
                .HasConversion<string>();

            // nombre unico
            modelBuilder.Entity<Escaneos>()
               .HasIndex(e => e.NombreEscaneo)
                .IsUnique();


            modelBuilder.Entity<Vulnerabilidades>()
                .Property(v => v.NivelRiesgo)
                .HasConversion<string>();

            // nombre unico 
            modelBuilder.Entity<Vulnerabilidades>()
               .HasIndex(v => v.NombreVulnerabilidad)
                .IsUnique();


            modelBuilder.Entity<Reportes>()
                .Property(r => r.cumplimiento)
                .HasConversion<string>();

            // nombre unico 
            modelBuilder.Entity<Reportes>()
               .HasIndex(r => r.NombreReporte)
                .IsUnique();


            modelBuilder.Entity<EscaneosVulnerabilidades>()
                .Property(es => es.estado)
                .HasConversion<string>();

            //plantillas
            modelBuilder.Entity<Plantillas>()
               .HasIndex(p => p.NombrePlantilla)
                .IsUnique();

            // Configurar clave primaria en las plantillas
            modelBuilder.Entity<AuditSentinel.Models.EscaneosServidores>()
                .HasKey(es => new { es.IdEscaneo, es.IdServidor });

            modelBuilder.Entity<AuditSentinel.Models.EscaneosPlantillas>()
                .HasKey(ep => new { ep.IdEscaneo, ep.IdPlantilla });

            modelBuilder.Entity<AuditSentinel.Models.EscaneosReportes>()
                .HasKey(er => new { er.IdEscaneo, er.IdReporte });

            modelBuilder.Entity<AuditSentinel.Models.EscaneosVulnerabilidades>()
                .HasKey(ev => new { ev.IdEscaneo, ev.IdVulnerabilidad });

            modelBuilder.Entity<AuditSentinel.Models.PlantillasVulnerabilidades>()
                .HasKey(pv => new { pv.IdPlantilla, pv.IdVulnerabilidad });

            //modelBuilder.Entity<UsuariosRoles>()
            //   .HasKey(ur => new { ur.IdUsuario, ur.IdRol });

            // establecer las relacion entre las tablas
            modelBuilder.Entity<AuditSentinel.Models.EscaneosServidores>()
                .HasOne(es => es.Escaneos)
                .WithMany(e => e.EscaneosServidores)
                .HasForeignKey(es => es.IdEscaneo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditSentinel.Models.EscaneosServidores>()
                .HasOne(es => es.Servidores)
                .WithMany(s => s.EscaneosServidores)
                .HasForeignKey(es => es.IdServidor)
                .OnDelete(DeleteBehavior.Cascade);
            //EscaneosPlantillas
            modelBuilder.Entity<AuditSentinel.Models.EscaneosPlantillas>()
                .HasOne(ep => ep.Escaneos)
                .WithMany(e => e.EscaneosPlantillas)
                .HasForeignKey(ep => ep.IdEscaneo)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<AuditSentinel.Models.EscaneosPlantillas>()
                .HasOne(ep => ep.Plantillas)
                .WithMany(p => p.EscaneosPlantillas)
                .HasForeignKey(ep => ep.IdPlantilla)
                .OnDelete(DeleteBehavior.Cascade);

            //EscaneosReportes>
            modelBuilder.Entity<AuditSentinel.Models.EscaneosReportes>()
                .HasOne(er => er.Escaneos)
                .WithMany(e => e.EscaneosReportes)
                .HasForeignKey(er => er.IdEscaneo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditSentinel.Models.EscaneosReportes>()
                .HasOne(er => er.Reportes)
                .WithMany(r => r.EscaneosReportes)
                .HasForeignKey(er => er.IdReporte)
                .OnDelete(DeleteBehavior.Cascade);

            // EscaneosVulnerabilidades
            modelBuilder.Entity<AuditSentinel.Models.EscaneosVulnerabilidades>()
                .HasOne(ev => ev.Escaneos)
                .WithMany(e => e.EscaneosVulnerabilidades)
                .HasForeignKey(ev => ev.IdEscaneo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditSentinel.Models.EscaneosVulnerabilidades>()
                .HasOne(ev => ev.Vulnerabilidades)
                .WithMany(v => v.EscaneosVulnerabilidades)
                .HasForeignKey(ev => ev.IdVulnerabilidad)
                .OnDelete(DeleteBehavior.Cascade);

            // PlantillasVulnerabilidades
            modelBuilder.Entity<AuditSentinel.Models.PlantillasVulnerabilidades>()
                .HasOne(pv => pv.Plantillas)
                .WithMany(p => p.PlantillasVulnerabilidades)
                .HasForeignKey(pv => pv.IdPlantilla)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditSentinel.Models.PlantillasVulnerabilidades>()
                .HasOne(pv => pv.Vulnerabilidades)
                .WithMany(v => v.PlantillasVulnerabilidades)
                .HasForeignKey(pv => pv.IdVulnerabilidad)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogErroresEscaneo>()
                .HasOne(l => l.Escaneo)
                .WithMany(e => e.Logs) // Asegúrate de agregar ICollection<LogErroresEscaneo> en tu clase Escaneo
                .HasForeignKey(l => l.EscaneoId);
        }
    }
}
