using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Web_Bán_Hàng.Models
{
    public class QuanLyDangNhapModel: SignInManager<AppUserModel>
    {
        public QuanLyDangNhapModel(UserManager<AppUserModel> userManager,
                               IHttpContextAccessor contextAccessor,
                               IUserClaimsPrincipalFactory<AppUserModel> claimsFactory,
                               IOptions<IdentityOptions> optionsAccessor,
                               ILogger<SignInManager<AppUserModel>> logger,
                               IAuthenticationSchemeProvider schemes,
                               IUserConfirmation<AppUserModel> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user != null && user.IsDeleted)
            {
                return SignInResult.NotAllowed;
            }

            return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }
    }
}
