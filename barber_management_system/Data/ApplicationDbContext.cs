using barber_management_system.Models;
using Microsoft.EntityFrameworkCore;

namespace barber_management_system.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Calisan> Calisanlar { get; set; }
        
        public DbSet<CalisanHizmet> CalisanHizmetler { get; set; }
        public DbSet<CalisanUzmanlik> CalisanUzmanliklar { get; set; }
        public DbSet<CalismaSaati> CalismaSaati { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
