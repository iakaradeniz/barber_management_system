using System.ComponentModel.DataAnnotations;

namespace barber_management_system.Models
{
    public class CalisanViewModel
    {
        public int CalisanId { get; set; }
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string CalisanAd { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string CalisanSoyad { get; set; }


      
        
    }

}
