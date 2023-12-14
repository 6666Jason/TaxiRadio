namespace RadioTaxi.Models.CompanyVM
{
	public class CompanyCRUD
	{
		public int ID { get; set; }
		public string IDCompany { get; set; }
		public string ContactPerson { get; set; }
		public string Designation { get; set; }
		public string Address { get; set; }
		public string CompanyName { get; set; }

		public string Mobile { get; set; }
		public string Telephone { get; set; }
		public string FaxNumber { get; set; }
		public string Email { get; set; }
		public string MemberShipType { get; set; }
		public int PackageId { get; set; }
		public string UserId { get; set; }

		public string PasswordHash { get; set; }
		public string ConfirmPassword { get; set; }
		public bool RememberMe { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
		public IFormFile Path { get; set; }

		public bool Payment { get; set; }


        public static implicit operator CompanyCRUD(Company _Products)
		{
			return new CompanyCRUD
			{
				ID = _Products.ID,
				IDCompany = _Products.IDCompany,
				ContactPerson = _Products.ContactPerson,
				Designation = _Products.Designation,
				Address = _Products.Address,
				Mobile = _Products.Mobile,
				Telephone = _Products.Telephone,
				FaxNumber = _Products.FaxNumber,
				Email = _Products.Email,
				MemberShipType = _Products.MemberShipType,
				PackageId = _Products.PackageId,
				UserId = _Products.UserId,
                Status = _Products.Status,
                CreateDate = _Products.CreateDate,
                Payment = _Products.Payment,
				CompanyName = _Products.CompanyName,


            };
		}

		public static implicit operator Company(CompanyCRUD vm)
		{
			return new Company
			{
				ID = vm.ID,
				IDCompany = vm.IDCompany,
				ContactPerson = vm.ContactPerson,
				Designation = vm.Designation,
				Address = vm.Address,
				Mobile = vm.Mobile,
				Telephone = vm.Telephone,
				FaxNumber = vm.FaxNumber,
				Email = vm.Email,
				MemberShipType = vm.MemberShipType,
				PackageId = vm.PackageId,
				UserId = vm.UserId,
                Status = vm.Status,
                CreateDate = vm.CreateDate,
                Payment = vm.Payment,
				CompanyName = vm.CompanyName,



            };
		}
	}
}
