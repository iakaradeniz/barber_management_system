using System.ComponentModel.DataAnnotations;

namespace barber_management_system.Models
{
    public class AddHizmetViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        public string Ad { get; set; }
        [Required(ErrorMessage = "Fiyat alanı zorunludur")]
        public decimal Fiyat { get; set; }
        [Required(ErrorMessage = "Dakika alanı zorunludur")]
        public decimal Dakika { get; set; }

    }
}
