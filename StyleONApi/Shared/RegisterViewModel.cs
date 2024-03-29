﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength =5)]
        [DataType(DataType.Password)]

        public string PassWord { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("PassWord",
            ErrorMessage = " confirm Password and Paassword dont match")]

        public string ConfirmPassword { get; set; }
    }
}
