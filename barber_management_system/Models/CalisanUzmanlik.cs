namespace barber_management_system.Models
{
    public class CalisanUzmanlik
    {
        public int CalisanId { get; set; }
        public Calisan Calisan { get; set; }

        public int HizmetId { get; set; }
        public Hizmet Hizmet { get; set; }
    }

}