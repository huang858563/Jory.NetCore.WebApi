﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Jory.NetCore.Model.Entities
{
    [DataContract]
    [Table("ADUserT")]
    public class ADUserT
    {
        [DataMember]
        [Key]
        public string UserName { get; set; } = default!;
        
        [NotMapped]
        [DataMember]
        public string? Password { get; set; }
      
        [IgnoreDataMember]
        public string? PasswordHash { get; set; }

        [DataMember]
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
