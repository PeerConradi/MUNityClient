using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation.Schema
{

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class JoinAuthenticate
    {
        [MaxLength(100)]
        public string Password { get; set; } = "";

        [MaxLength(50)]
        [Required]
        public string DisplayName { get; set; }
    }
}
