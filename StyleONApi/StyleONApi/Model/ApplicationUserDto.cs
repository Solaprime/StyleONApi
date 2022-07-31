using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    /// <summary>
    /// Outerfacing Contract for user
    /// </summary>
    public class ApplicationUserDto
    {
        /// <summary>
        /// Id of User
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// County of User
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// City of User
        /// </summary>
        public string  City { get; set; }
        /// <summary>
        /// FullName of User
        /// </summary>
        public string  FullName { get; set; }
        /// <summary>
        /// UserName of User
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Email of User
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// PhoneNumber of User
        /// </summary>
        public string  PhoneNumber { get; set; }
    }
}

// Id, Role
