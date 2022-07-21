using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
   public  class SimpleResponse
    {
        public SimpleResponse()
        {
            ObjectToReturn = new object();
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object ObjectToReturn { get; set; }
    }
}
