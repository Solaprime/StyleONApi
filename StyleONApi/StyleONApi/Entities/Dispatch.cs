using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class Dispatch
    {
        [Required]
        [Key]
        public Guid DispatchId { get; set; }
        public int NumberofCompletedDispatch { get; set; }
        public string Email { get; set; }

        public DateTime DateRegistered { get; set; }

        
      //  public Guid ApplicationUserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser userId { get; set; }
    }
}
