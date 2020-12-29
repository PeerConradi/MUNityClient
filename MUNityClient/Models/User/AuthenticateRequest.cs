using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.User
{
    public class AuthenticateRequest
    {
        [Required]
        [MaxLength(40)]
        public string Username { get; set; }

        [Required]
        [MaxLength(300)]
        public string Password { get; set; }

        public AuthenticateRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public AuthenticateRequest()
        {

        }
    }
}
