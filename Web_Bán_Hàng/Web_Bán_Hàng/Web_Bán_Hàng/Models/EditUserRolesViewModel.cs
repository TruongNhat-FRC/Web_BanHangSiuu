using System.Collections.Generic;

namespace Web_Bán_Hàng.Models
{
	public class EditUserRolesViewModel
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public List<RoleViewModel> AvailableRoles { get; set; }
	}

	public class RoleViewModel
	{
		public string RoleId { get; set; }
		public string RoleName { get; set; }
		public bool IsSelected { get; set; }
	}
}
