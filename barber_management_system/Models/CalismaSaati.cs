namespace barber_management_system.Models
{
    public class CalismaSaati
    {
        public int CalismaSaatiID { get; set; }
        public DateTime BaslangicSaati { get; set; } // Çalışma başlangıç saati
        public DateTime BitisSaati { get; set; } // Çalışma bitiş saati

        public DayOfWeek Gun { get; set; }  // Pazartesi, Salı, Çarşamba vb.

        public int CalisanId { get; set; }
        public Calisan calisan { get; set; }    
    }
}
