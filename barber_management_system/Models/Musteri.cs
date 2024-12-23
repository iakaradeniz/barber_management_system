﻿namespace barber_management_system.Models
{
    public class Musteri
    {
        public string IdentityUserId { get; set; }
        public int MusteriID { get; set; }
        public string MusteriAd { get; set; }
        public string MusteriSoyAd {  get; set; } 
        public List<Randevu> randevulist {  get; set; } = new List<Randevu>();

        public string Email { get; set; }
        public string Sifre { get; set; }
    }
}
