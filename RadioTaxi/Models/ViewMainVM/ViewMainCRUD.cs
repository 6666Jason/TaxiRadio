namespace RadioTaxi.Models.ViewMainVM
{
    public class ViewMainCRUD
    {
        public Drivers DriversMain { get; set; }
        public Participants ParticipantsMain { get; set; }
        public List<Drivers> DriversList { get; set; }
        public List<Drivers> DriverOfCompany { get; set; }

        public List<Company> CompanyList { get; set; }
        public List<FeedBack> FeedBackList { get; set; }
        public Company CompanyMain { get; set; }
        public List<Package> PackageMain { get; set; }
        public List<CategoryPackage> CategoryPackageMain { get; set; }
        public List<Participants> ParticipantsList { get; set; }
        public List<Renter> RenterList { get; set; }
        public ApplicationUser ApplicationUserMain { get; set; }

        public bool IsDriver { get; set; }


    }
}
