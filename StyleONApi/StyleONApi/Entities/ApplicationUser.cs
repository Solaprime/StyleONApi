using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class ApplicationUser :IdentityUser
    {
        public String Country { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Position { get; set; }

       // shoulf your use case require a reference to the urefreshtoken table
       // no but, you can check your use case
        // Know whether to use sellerId or Seller.....
        // 4 dispatch
        // 4 seller
    }
}

// Dont forget to replace all Identitiy User references with application user
//Dont forget to change to add the genrric type as thr basd class in out context