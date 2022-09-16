using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }



        // some use case might be needed for u to collect the userId along with the parmeters stated above
        // some use casw might only need the UserId and the refreshtoken, while some will need the userId, Token, RefreshToken e.t.c
    }
}
