namespace barber_management_system.Models
{
    public class Hizmet
    {
        public int HizmetId { get; set; }
        public string HizmetAdi { get; set; }
        public decimal Fiyat { get; set; }

        // Hizmetin çalışanlarla ilişkisi
        public List<CalisanHizmet> Calisanlar { get; set; }
        public List<CalisanUzmanlik> Uzmanliklar { get; set; }

        public List<Randevu> Randevular { get; set; }
    }
}
