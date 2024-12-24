namespace barber_management_system.Models
{
    public class CalisanUygunluk
    {
        public int CalisanUygunlukID { get; set; }
        public int CalisanID { get; set; }
        public DayOfWeek Gun { get; set; }  // Gun yerine DayOfWeek enum kullanılıyor
        public TimeSpan BaslangicSaati { get; set; }  // Başlangıç saati için TimeSpan
        public TimeSpan BitisSaati { get; set; }  // Bitiş saati için TimeSpan

        public Calisan Calisan { get; set; }
    }
}
