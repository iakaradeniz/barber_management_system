namespace barber_management_system.Models
{
    public class Randevu
    {
        public int RandevuID { get; set; }
        public int MusteriId { get; set; }
        public Musteri musteri { get; set; }

        public int HizmetId { get; set; }
        public Hizmet hizmet { get; set; }
        
        public int CalisanId { get; set; }
        public Calisan calisan { get; set; }

        public bool OnayDurumu { get; set; } = false;  // Varsayılan olarak onaysız
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public RandevuDurumu Durum { get; set; } = RandevuDurumu.Beklemede;
    }
    public enum RandevuDurumu
    {
        Beklemede,
        Onaylandi,
        Reddedildi,
        Tamamlandi,
        Iptal
    }


}
