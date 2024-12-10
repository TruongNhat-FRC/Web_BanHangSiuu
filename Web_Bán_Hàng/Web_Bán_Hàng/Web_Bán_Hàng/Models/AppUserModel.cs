using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



namespace Web_Bán_Hàng.Models

{
	public class AppUserModel: IdentityUser
	{
        public bool IsDeleted { get; set; } = false;
        // Thêm thuộc tính này để chứa danh sách quyền
        // Thêm trường Full Name
        public string FullName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

    }
}
