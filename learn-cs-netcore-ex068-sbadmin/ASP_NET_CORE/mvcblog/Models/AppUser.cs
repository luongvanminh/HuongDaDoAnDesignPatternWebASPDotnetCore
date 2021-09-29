using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace mvcblog.Models {

    public enum CodeAppUser {
        InvalidFullName,
        InvalidBirthday,
        Valid
    }

    interface MVCEntityBlog
    {
        Task<CodeAppUser> UpdateDatabase(UserManager<AppUser> userManager);
    }

    public class AppUser : IdentityUser, MVCEntityBlog {
        [MaxLength (100)]
        public string FullName { set; get; }

        [MaxLength (255)]
        public string Address { set; get; }

        [DataType (DataType.Date)]
        public DateTime? Birthday { set; get; }

        public async Task<CodeAppUser> UpdateDatabase(UserManager<AppUser> userManager)
        {
            await userManager.UpdateAsync(this);
            return CodeAppUser.Valid;
        }
    }

    public class ProxyAppUser : MVCEntityBlog
    {
        private AppUser _user;

        public ProxyAppUser(AppUser user)
        {
            _user = user;
        }

        public async Task<CodeAppUser> UpdateDatabase(UserManager<AppUser> userManager)
        {
            StringComparison comp = StringComparison.OrdinalIgnoreCase;
            string notAllow = "Ho Chi Minh";
            var age = 0;
            if (_user.Birthday != null)
            {
                age = DateTime.Now.Year - _user.Birthday.Value.Year;
            }

            var name = "";
            if (_user.FullName != null)
            {
                name = _user.FullName;
            }

            if (name.Contains(notAllow, comp))
            {
                return CodeAppUser.InvalidFullName;
                
            } else if (age < 18)
            {
                return CodeAppUser.InvalidBirthday;
            } else
            {
                return await _user.UpdateDatabase(userManager);
            }
        }
    }
}