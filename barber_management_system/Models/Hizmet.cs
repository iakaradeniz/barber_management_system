namespace barber_management_system.Models
{
    public class Hizmet
    {
        public int HizmetID { get; set; }
        public string HizmetAd { get; set; }
        public decimal Fiyat { get; set; }

        // Hizmeti veren çalışanlar (Many-to-Many)
        public List<CalisanHizmet> calisanhizmetlist { get; set; }

        // Uzmanlık yapan çalışanlar (Many-to-Many)
        public List<CalisanUzmanlik> CalisanUzmanliklist { get; set; }

        // Randevular (One-to-Many)
        public List<Randevu> randevulist { get; set; }
    }

}
