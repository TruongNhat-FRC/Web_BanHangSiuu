using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



namespace Web_Bán_Hàng.Models

{
	public class AppUserModel: IdentityUser
	{
        public bool IsDeleted { get; set; } = false;
        
        public string FullName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string Token {  get; set; }

    }
}
