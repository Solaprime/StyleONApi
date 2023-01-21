using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class DispatchForUpdateDto
    {
        [Required]
        public string Email { get; set; }
    }
}
