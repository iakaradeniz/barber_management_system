namespace barber_management_system.Models
{
    public class RandevuViewModel
    {
        public int MusteriId { get; set; }
        public int CalisanId { get; set; }
        public int HizmetId { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public int Dakika { get; set; }

        // Eklenen alanlar: Çalışanlar ve Hizmetler listeleri
        public List<Calisan> Calisanlar { get; set; }
        public List<Hizmet> Hizmetler { get; set; }
    }
}
