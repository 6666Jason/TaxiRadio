using RadioTaxi.Models.CompanyVM;

namespace RadioTaxi.Models.DriversVM
{
    public class DriversCRUD
    {
        public Drivers DriversMain { get; set; }
        public Company CompanyMain { get; set; }
        public Advertise AdvertiseMain { get; set; }
        public bool CheckRemove { get; set; }
		public int ID { get; set; }
		public string IDDriver { get; set; }
		public string ContactPerson { get; set; }
		public string Address { get; set; }
		public string City { get; set; }

		public string Mobile { get; set; }
		public string Telephone { get; set; }

		public string? Experience { get; set; }
		public string Email { get; set; }
		public string? Description { get; set; }
		public int PackageId { get; set; }
		public string UserId { get; set; }
		public DateTime CreateDate { get; set; }
		public string PasswordHash { get; set; }
		public string ConfirmPassword { get; set; }
		public bool RememberMe { get; set; }
        public bool Status { get; set; }
        public bool Payment { get; set; }
        public bool CheckAds { get; set; }

        public IFormFile Path { get; set; }


        public static implicit operator DriversCRUD(Drivers _Products)
		{
			return new DriversCRUD
			{
				ID = _Products.ID,
				IDDriver = _Products.IDDriver,
				ContactPerson = _Products.ContactPerson,
				Address = _Products.Address,
				Mobile = _Products.Mobile,
                City = _Products.City,
				Telephone = _Products.Telephone,
				Experience = _Products.Experience,
				Email = _Products.Email,
				Description = _Products.Description,
				PackageId = _Products.PackageId,
				UserId = _Products.UserId,
                Status = _Products.Status,
                Payment = _Products.Payment,
                CreateDate = _Products.CreateDate,




			};
		}

		public static implicit operator Drivers(DriversCRUD vm)
		{
			return new Drivers
			{
				ID = vm.ID,
				IDDriver = vm.IDDriver,
				ContactPerson = vm.ContactPerson,
				Address = vm.Address,
				Mobile = vm.Mobile,
                City = vm.City,

                Telephone = vm.Telephone,
				Experience = vm.Experience,
				Email = vm.Email,
				Description = vm.Description,
				PackageId = vm.PackageId,
				UserId = vm.UserId,
                Status = vm.Status,
                Payment = vm.Payment,
                CreateDate = vm.CreateDate

            };
		}
	}
}
