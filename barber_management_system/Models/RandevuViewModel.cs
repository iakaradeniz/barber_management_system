using System.ComponentModel.DataAnnotations;

namespace barber_management_system.Models
{
    public class RandevuViewModel
    {
        public int MusteriId { get; set; }
        public int CalisanId { get; set; }

        [Required(ErrorMessage = "Hizmet seçilmesi zorunludur.")]
        public int HizmetId { get; set; }
        [Required(ErrorMessage = "Randevu tarihi seçilmesi zorunludur.")]
        public DateTime RandevuTarihi { get; set; }
        public int Dakika { get; set; }
        public decimal Fiyat { get; set; }

        // Eklenen alanlar: Çalışanlar ve Hizmetler listeleri
        public List<Calisan>? Calisanlar { get; set; }  
        public List<Hizmet>? Hizmetler { get; set; }
    }
}
