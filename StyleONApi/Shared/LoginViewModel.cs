using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }
        [EmailAddress]
        [StringLength(50, MinimumLength =5)]
        public string PassWord { get; set; }
    }
}
// Our Simple Dto for our Login 