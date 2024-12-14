namespace barber_management_system.Models
{
    public class Musteri
    {
        public int MusteriID { get; set; }
        public string MusteriAd { get; set; }
        public string MusteriSoyAd {  get; set; } 
        public List<Randevu> randevulist {  get; set; }
    }
}
