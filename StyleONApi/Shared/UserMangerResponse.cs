using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public  class UserMangerResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        // Our Error Message
        public IEnumerable<string> Error { get; set; }

        // THis check whe token will expire
        public DateTime? ExpiredDate { get; set; }
    }
}
