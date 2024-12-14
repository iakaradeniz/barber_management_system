namespace barber_management_system.Models
{
    public class Calisan
    {
        public int CalisanID { get; set; }
        public string CalisanAd { get; set; }
        public string CalisanSoyad { get; set; }

        // Verdiği hizmetler (Many-to-Many)
        public List<CalisanHizmet> calisanhizmetlist { get; set; }

        // Uzmanlık alanları (Many-to-Many)
        public List<CalisanUzmanlik> calisanuzmanliklist { get; set; }

        // Çalışma saatleri (Many-to-Many)
        public List<CalisanCalismaSaati> calisanCalismaSaatiList { get; set; }

        // Randevular (One-to-Many)
        public List<Randevu> randevulist { get; set; }
    }

}
