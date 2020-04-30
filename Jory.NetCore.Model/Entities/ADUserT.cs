using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Jory.NetCore.Model.Entities
{
    [Table("ADUserT")]
    public class ADUserT
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string LoginName { get; set; } = default!;
        
        [NotMapped]
        public string? LoginPwd { get; set; }
        
        [JsonIgnore]
        public string? LoginPwdHash { get; set; }

        public string UserName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string? Role { get; set; }

        public static string GetRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "User";
            return role.ToLower() switch
                {
                    "admin" => "Admin",
                    "vip" => "Vip",
                    "guest" => "Guest",
                    _ => "User"
                };
        }
    }
}
