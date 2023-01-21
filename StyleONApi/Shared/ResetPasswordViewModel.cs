using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared
{
    public class ResetPasswordViewModel
    {

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Tested a new Data Attribute confirm Password and Paassword dont match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }


        // Reset Password could have inhrtitrd from Register view Model

    }
}
