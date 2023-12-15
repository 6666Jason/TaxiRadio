namespace RadioTaxi.Models.ProfileVM
{
    public class ProfileCRUD
    {
        public Drivers DriversMain { get; set; }
        public Company CompanyMain { get; set; }
        public Advertise AdvertiseMain { get; set; }
        public Participants ParticipantsMain { get; set; }
        public List<Package> PackageList { get; set; }

        public int PackageID { get; set; }
        public bool CheckAds { get; set; }
        public bool DriverCompany { get; set; }

        public IFormFile? PrPath { get; set; }

       




    }
}
