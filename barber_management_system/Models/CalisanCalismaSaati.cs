namespace barber_management_system.Models
{
    public class CalisanCalismaSaati
    {
        public int CalisanId { get; set; }
        public Calisan calisan { get; set; }

        public int CalismaSaatiId { get; set; }
        public CalismaSaati calismaSaati { get; set; }
    }
}
