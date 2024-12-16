using barber_management_system.Models;
using Microsoft.EntityFrameworkCore;

namespace barber_management_system.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Calisan> Calisanlar { get; set; }

        public DbSet<CalisanHizmet> CalisanHizmetler { get; set; }
        public DbSet<CalisanUzmanlik> CalisanUzmanliklar { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

        public DbSet<CalismaSaati> CalismaSaatleri { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //Randevu-Musteri
        //    modelBuilder.Entity<Randevu>()
        //        .HasOne(r => r.musteri)
        //        .WithMany(m => m.randevular)
        //        .HasForeignKey(r => r.MusteriId);

        //    //Randevu-Hizmet
        //    modelBuilder.Entity<Randevu>()
        //        .HasOne(r => r.hizmet)
        //        .WithMany(h => h.randevular)
        //        .HasForeignKey(r => r.HizmetId);


        //    //Randevu-Calisan
        //    modelBuilder.Entity<Randevu>()
        //        .HasOne(r => r.calisan)
        //        .WithMany(c => c.randevular)
        //        .HasForeignKey(r => r.CalisanId);

        //    //Randevu-CalismaSaati
        //    modelBuilder.Entity<Randevu>()
        //        .HasOne(r => r.calismasaati)
        //        .WithMany(cs => cs.randevularList)
        //        .HasForeignKey(r => r.CalismaSaatiId);

        //    //Calisan-CalisanCalismaSaati
        //    modelBuilder.Entity<Calisan>()
        //        .HasMany(c => c.calisanCalismaSaatiList)
        //        .WithOne(ccs => ccs.calisan)
        //        .HasForeignKey(ccs => ccs.CalisanId);

        //    // CalismaSaati-CalisanCalismaSaati 
        //    modelBuilder.Entity<CalismaSaati>()
        //        .HasMany(cs => cs.calisanCalismaSaatiList)
        //        .WithOne(ccs => ccs.calismaSaati)
        //        .HasForeignKey(ccs => ccs.CalismaSaatiId);

        //    // Composite Key (CalisanId ve CalismaSaatiId birlikte birincil anahtar)
        //    modelBuilder.Entity<CalisanCalismaSaati>()
        //        .HasKey(ccs => new { ccs.CalisanId, ccs.CalismaSaatiId });

        //    ////Calisan-CalisanUzmanlık
        //    //modelBuilder.Entity<Calisan>()
        //    //    .HasMany(c => c.uzmanlikAlanlari)
        //    //    .WithOne(ua => ua.calisan)
        //    //    .HasForeignKey(ua => ua.CalisanId);

        //    //Calisan-CalisanHizmet
        //    modelBuilder.Entity<Calisan>()
        //        .HasMany(c => c.verdigiHizmetler)
        //        .WithOne(ch => ch.calisan)
        //        .HasForeignKey(ch => ch.CalisanId);

        //    //Hizmet-CalisanHizmet
        //    modelBuilder.Entity<Hizmet>()
        //        .HasMany(h=>h.uzmanliklar)
        //        .WithOne(ch => ch.hizmet)
        //        .HasForeignKey(ch =>ch.HizmetId);



        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            //// Randevu - CalismaSaati (One-to-Many)
            //modelBuilder.Entity<Randevu>()
            //    .HasOne(r => r.calismasaati)
            //    .WithMany(cs => cs.randevulist)
            //    .HasForeignKey(r => r.CalismaSaatiId);

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

            // Çalışan ile ÇalışmaSaatleri arasında bire çok ilişki
            //modelBuilder.Entity<Calisan>()
            //    .HasMany(c => c.CalismaSaatleri) // Bir çalışanın birden fazla çalışma saati olabilir
            //    .WithOne(cs => cs.Calisan) // Her çalışma saati bir çalışana bağlıdır
            //    .HasForeignKey(cs => cs.CalisanId) // Yabancı anahtar CalismaSaati üzerinde
            //    .OnDelete(DeleteBehavior.Cascade); // Çalışan silinirse çalışma saatleri de silinsin

        //    modelBuilder.Entity<Calisan>()
        //.HasMany(c => c)
        //.WithOne(cs => cs.Calisan)
        //.HasForeignKey(cs => cs.CalisanId)
        //.OnDelete(DeleteBehavior.Cascade); // Çalışan silindiğinde çalışma saatleri de silinir
        }









    }
}
