using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.User
{

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class AuthenticationResponse
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Token { get; set; }
    }
}
