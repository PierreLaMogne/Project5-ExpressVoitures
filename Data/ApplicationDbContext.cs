using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Net_P5.Models;
using System.Reflection.Emit;

namespace Net_P5.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Marque> Marques => Set<Marque>();
        public DbSet<Modele> Modeles => Set<Modele>();
        public DbSet<Finition> Finitions => Set<Finition>();
        public DbSet<Voiture> Voitures => Set<Voiture>();
        public DbSet<Reparation> Reparations => Set<Reparation>();
        public DbSet<Vente> Ventes => Set<Vente>();

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
                .WithOne(mo => mo.Marque)
                .HasForeignKey(mo => mo.MarqueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Modele>()
                .HasMany(mo => mo.Finitions)
                .WithOne(f => f.Modele)
                .HasForeignKey(f => f.ModeleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Finition>()
                .HasMany(mo => mo.Voitures)
                .WithOne(v => v.Finition)
                .HasForeignKey(v => v.FinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
