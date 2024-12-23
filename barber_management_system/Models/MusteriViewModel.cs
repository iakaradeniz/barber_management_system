using System.ComponentModel.DataAnnotations;

namespace barber_management_system.Models
{
    public class MusteriViewModel
    {
        public int MusteriId { get; set; }
        [Required]
        public string Ad { get; set; }
        [Required]
        public string SoyAd { get; set; }

        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola gereklidir.")]
        [StringLength(100, ErrorMessage = "Parola en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$", ErrorMessage = "Parola en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        public string Password { get; set; }

        public List<Randevu> Randevular { get; set; } = new List<Randevu>();
    }
}
