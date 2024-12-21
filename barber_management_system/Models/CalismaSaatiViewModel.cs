using System.ComponentModel.DataAnnotations;

namespace barber_management_system.Models
{

    public class CalismaSaatiViewModel
    {
        public int CalismaSaatiId { get; set; }
        //[Required(ErrorMessage = "Gün bilgisi gereklidir.")]
        public DayOfWeek Gun { get; set; }

        //[Required(ErrorMessage = "Başlangıç saati gereklidir.")]
        public DateTime BaslangicSaati { get; set; }

        //[Required(ErrorMessage = "Bitiş saati gereklidir.")]
        public DateTime BitisSaati { get; set; }
    }
}
