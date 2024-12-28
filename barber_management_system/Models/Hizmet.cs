using System.ComponentModel.DataAnnotations;

namespace barber_management_system.Models
{
    public class Hizmet
    {
        public int HizmetID { get; set; }
        [Required(ErrorMessage = "Hizmet adı gereklidir.")]
        public string HizmetAd { get; set; }
        [Required(ErrorMessage = "Fiyat gereklidir.")]
        public decimal Fiyat { get; set; }
        [Required(ErrorMessage = "Dakika gereklidir.")]
        public decimal Dakika { get; set; } // Dakika cinsinden

        // Hizmeti veren çalışanlar (Many-to-Many)
        public List<CalisanHizmet> calisanhizmetlist { get; set; }

        // Uzmanlık yapan çalışanlar (Many-to-Many)
        public List<CalisanUzmanlik> CalisanUzmanliklist { get; set; }

        // Randevular (One-to-Many)
        public List<Randevu> randevulist { get; set; }
    }

}
