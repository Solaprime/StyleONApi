using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class SellerForUpdateDto
    {
        [Required]
        public string Email { get; set; }
        public string StoreName { get; set; }
        public DateTime DateRegistered { get; set; }
        //Store Adress
        // Account Number 
    }
}
