using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class RefreshToken
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevorked { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }


        // whenenever the refreshtoken has been revoked always 
        //always set the isrevoked property to false
        //the token in this case is it the JWT TOKen or Refreshtoken 
        // if it is refresh tpken it os best to hash it oh

    }
}
