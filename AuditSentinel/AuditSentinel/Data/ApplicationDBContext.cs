using AuditSentinel.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Data
{
    public class ApplicationDBContext : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Almacenar enum como string
            modelBuilder.Entity<Escaneos>()
                .Property(e => e.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Vulnerabilidades>()
                .Property(v => v.NivelRiesgo)
                .HasConversion<string>();

            modelBuilder.Entity<Reportes>()
                .Property(r => r.cumplimiento)
                .HasConversion<string>();

            modelBuilder.Entity<EscaneosVulnerabilidades>()
                .Property(es => es.estado)
                .HasConversion<string>();

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

            // establecer las relacion entre las tablas
            modelBuilder.Entity<AuditSentinel.Models.EscaneosServidores>()
                .HasOne(es => es.Escaneos)
                .WithMany(e => e.EscaneosServidores)
                .HasForeignKey(es => es.IdEscaneo);

            modelBuilder.Entity<AuditSentinel.Models.EscaneosServidores>()
                .HasOne(es => es.Servidores)
                .WithMany(s => s.EscaneosServidores)
                .HasForeignKey(es => es.IdServidor);
            //EscaneosPlantillas
            modelBuilder.Entity<AuditSentinel.Models.EscaneosPlantillas>()
                .HasOne(ep => ep.Escaneos)
                .WithMany(e => e.EscaneosPlantillas)
                .HasForeignKey(ep => ep.IdEscaneo);

            modelBuilder.Entity<AuditSentinel.Models.EscaneosPlantillas>()
                .HasOne(ep => ep.Plantillas)
                .WithMany(p => p.EscaneosPlantillas)
                .HasForeignKey(ep => ep.IdPlantilla);

            //EscaneosReportes>
            modelBuilder.Entity<AuditSentinel.Models.EscaneosReportes>()
                .HasOne(er => er.Escaneos)
                .WithMany(e => e.EscaneosReportes)
                .HasForeignKey(er => er.IdEscaneo);

            modelBuilder.Entity<AuditSentinel.Models.EscaneosReportes>()
                .HasOne(er => er.Reportes)
                .WithMany(r => r.EscaneosReportes)
                .HasForeignKey(er => er.IdReporte);

            // EscaneosVulnerabilidades
            modelBuilder.Entity<AuditSentinel.Models.EscaneosVulnerabilidades>()
                .HasOne(ev => ev.Escaneos)
                .WithMany(e => e.EscaneosVulnerabilidades)
                .HasForeignKey(ev => ev.IdEscaneo);

            modelBuilder.Entity<AuditSentinel.Models.EscaneosVulnerabilidades>()
                .HasOne(ev => ev.Vulnerabilidades)
                .WithMany(v => v.EscaneosVulnerabilidades)
                .HasForeignKey(ev => ev.IdVulnerabilidad);

            // PlantillasVulnerabilidades
            modelBuilder.Entity<AuditSentinel.Models.PlantillasVulnerabilidades>()
                .HasOne(pv => pv.Plantillas)
                .WithMany(p => p.PlantillasVulnerabilidades)
                .HasForeignKey(pv => pv.IdPlantilla);

            modelBuilder.Entity<AuditSentinel.Models.PlantillasVulnerabilidades>()
                .HasOne(pv => pv.Vulnerabilidades)
                .WithMany(v => v.PlantillasVulnerabilidades)
                .HasForeignKey(pv => pv.IdVulnerabilidad);
        }
    }
}
