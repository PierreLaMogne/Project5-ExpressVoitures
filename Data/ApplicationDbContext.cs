using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Net_P5.Models;

namespace Net_P5.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Marque> Marques { get; set; }
        public DbSet<Modele> Modeles { get; set; }
        public DbSet<Finition> Finitions { get; set; }
        public DbSet<Voiture> Voitures { get; set; }
        public DbSet<Reparation> Reparations { get; set; }
        public DbSet<Vente> Ventes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Configuration des entités

            builder.Entity<Voiture>()
                .Property(v => v.CodeVIN)
                .ValueGeneratedNever();

            // Configuration des relations entre les entités

            builder.Entity<Voiture>()
                .HasMany(v => v.Reparations)
                .WithOne(r => r.Voiture)
                .HasForeignKey(r => r.VoitureCodeVIN)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<Voiture>()
                .HasMany(v => v.Ventes)
                .WithOne(ve => ve.Voiture)
                .HasForeignKey(ve => ve.VoitureCodeVIN)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Marque>()
                .HasMany(m => m.Modeles)
                .WithOne(md => md.Marque)
                .HasForeignKey(md => md.MarqueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Modele>()
                .HasMany(md => md.Finitions)
                .WithOne(f => f.Modele)
                .HasForeignKey(f => f.ModeleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Finition>()
                .HasMany(f => f.Voitures)
                .WithOne(v => v.Finition)
                .HasForeignKey(v => v.FinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
