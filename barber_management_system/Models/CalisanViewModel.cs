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

        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola gereklidir.")]
        [StringLength(100, ErrorMessage = "Parola en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$", ErrorMessage = "Parola en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        public string Password { get; set; }

        // Çalışanın sunduğu hizmetler
        public List<Hizmet> Hizmetler { get; set; }= new List<Hizmet>();

        // Seçilen hizmetlerin ID'lerini tutacak property
        public List<int>? SelectedHizmetler { get; set; } = new List<int>();

    }

}
