using System.ComponentModel.DataAnnotations;

namespace Jory.NetCore.WebApi.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "N00001")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密碼不能為空")]
        public string Password { get; set; }
    }
}
