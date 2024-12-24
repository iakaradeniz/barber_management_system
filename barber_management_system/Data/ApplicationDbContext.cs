using barber_management_system.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace barber_management_system.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<CalisanUygunluk> CalisanUygunluklar { get; set; }

        public DbSet<CalisanHizmet> CalisanHizmetler { get; set; }
        public DbSet<CalisanUzmanlik> CalisanUzmanliklar { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

        public DbSet<CalismaSaati> CalismaSaatleri { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CalisanUygunluk yapılandırması
            modelBuilder.Entity<CalisanUygunluk>(entity =>
            {
                entity.HasKey(cu => cu.CalisanUygunlukID);
                entity.Property(cu => cu.Gun).IsRequired();
                entity.Property(cu => cu.BaslangicSaati).IsRequired();
                entity.Property(cu => cu.BitisSaati).IsRequired();
                entity.HasOne(cu => cu.Calisan)
                      .WithMany(c => c.CalisanUygunluklar)
                      .HasForeignKey(cu => cu.CalisanID);
            });

            // Randevu yapılandırması
            modelBuilder.Entity<Randevu>(entity =>
            {
                entity.HasKey(r => r.RandevuID);
                entity.Property(r => r.RandevuTarihi).IsRequired();
                entity.Property(r => r.Dakika).IsRequired();
                entity.Property(r => r.Ucret).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(r => r.OnayDurumu).IsRequired();

                // Randevu - Musteri (One-to-Many)
                modelBuilder.Entity<Randevu>()
                    .HasOne(r => r.musteri)
                    .WithMany(m => m.randevulist)
                    .HasForeignKey(r => r.MusteriId);

                // Randevu - Calisan (One-to-Many)
                modelBuilder.Entity<Randevu>()
                    .HasOne(r => r.calisan)
                    .WithMany(c => c.randevulist)
                    .HasForeignKey(r => r.CalisanId);

                // Randevu - Hizmet (One-to-Many)
                modelBuilder.Entity<Randevu>()
                    .HasOne(r => r.hizmet)
                    .WithMany(h => h.randevulist)
                    .HasForeignKey(r => r.HizmetId);


            });


            // Calisan - UzmanlikAlanlari (Many-to-Many with Hizmet)
            modelBuilder.Entity<CalisanUzmanlik>()
                .HasKey(cu => new { cu.CalisanId, cu.HizmetId }); // Composite Key

            modelBuilder.Entity<CalisanUzmanlik>()
                .HasOne(cu => cu.Calisan)
                .WithMany(c => c.calisanuzmanliklist)
                .HasForeignKey(cu => cu.CalisanId);

            modelBuilder.Entity<CalisanUzmanlik>()
                .HasOne(cu => cu.Hizmet)
                .WithMany(h => h.CalisanUzmanliklist)
                .HasForeignKey(cu => cu.HizmetId);

            // Calisan - Hizmet (Many-to-Many for Verdiği Hizmetler)
            modelBuilder.Entity<CalisanHizmet>()
                .HasKey(ch => new { ch.CalisanId, ch.HizmetId }); // Composite Key

            modelBuilder.Entity<CalisanHizmet>()
                .HasOne(ch => ch.Calisan)
                .WithMany(c => c.calisanhizmetlist)
                .HasForeignKey(ch => ch.CalisanId);

            modelBuilder.Entity<CalisanHizmet>()
                .HasOne(ch => ch.Hizmet)
                .WithMany(h => h.calisanhizmetlist)
                .HasForeignKey(ch => ch.HizmetId);

            modelBuilder.Entity<CalismaSaati>()
       .HasOne(cs => cs.Calisan)
       .WithMany(c => c.CalismaSaatleri)
       .HasForeignKey(cs => cs.CalisanId)
       .OnDelete(DeleteBehavior.Cascade);  // Cascade silme etkinleştiriliyor

           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Uyarıyı bastırmak için
            optionsBuilder.ConfigureWarnings(warnings => warnings
                .Log(RelationalEventId.PendingModelChangesWarning)); // Uyarıyı loglayın
        }

    }
}
