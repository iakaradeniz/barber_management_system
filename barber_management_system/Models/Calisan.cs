namespace barber_management_system.Models
{
    public class Calisan
    {
       
        public int CalisanID { get; set; }
        public string CalisanAd { get; set; }
        public string CalisanSoyad { get; set; }
        public string IdentityUserId { get; set; } // Identity kullanıcı kimliği

        public string Email { get; set; }

        public string Sifre { get; set; }



        // Verdiği hizmetler (Many-to-Many)
        public List<CalisanHizmet> calisanhizmetlist { get; set; } = new List<CalisanHizmet>();

        // Uzmanlık alanları (Many-to-Many)
        public List<CalisanUzmanlik> calisanuzmanliklist { get; set; } = new List<CalisanUzmanlik>();

        // Haftanın her günü için çalışma saatlerini tutan ilişki
        public List<CalismaSaati> CalismaSaatleri { get; set; } = new List<CalismaSaati>();

        // Çalışanın uygunluk saatlerini tutan liste
        public List<CalisanUygunluk> CalisanUygunluklar { get; set; }



        // Randevular (One-to-Many)
        public List<Randevu> randevulist { get; set; } = new List<Randevu>();
    }

}
