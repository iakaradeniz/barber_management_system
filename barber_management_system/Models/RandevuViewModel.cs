using System.ComponentModel.DataAnnotations;

namespace barber_management_system.Models
{
    public class RandevuViewModel
    {
        public int? RandevuId { get; set; }
        [Required(ErrorMessage = "Müşteri ID gereklidir.")]
        public int MusteriId { get; set; }

        [Required(ErrorMessage = "Çalışan seçilmesi zorunludur.")]
        public int CalisanId { get; set; }

        [Required(ErrorMessage = "Hizmet seçilmesi zorunludur.")]
        public int HizmetId { get; set; }

        [Required(ErrorMessage = "Randevu tarihi seçilmesi zorunludur.")]

        public DateTime RandevuTarihi { get; set; }

        [Required]
        public int Dakika { get; set; }

        [Required]
        public decimal Fiyat { get; set; }

        public List<Calisan>? Calisanlar { get; set; }
        public List<Hizmet>? Hizmetler { get; set; }
    }
}
