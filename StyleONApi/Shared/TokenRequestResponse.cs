using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
   public   class TokenRequestResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool IsSucess { get; set; }
    }

}
