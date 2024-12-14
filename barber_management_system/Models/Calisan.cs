namespace barber_management_system.Models
{
    public class Calisan
    {
        public int CalisanID { get; set; }
        public string CalisanAd { get; set; }
        public string CalisanSoyad { get; set; }
        // Verdiği hizmetler
        public List<CalisanHizmet> VerdigiHizmetler { get; set; }

        // Uzmanlık alanları
        public List<CalisanUzmanlik> UzmanlikAlanlari { get; set; }

        public List<CalismaSaati> CalismaSaatiList { get; set; }

    }
}
