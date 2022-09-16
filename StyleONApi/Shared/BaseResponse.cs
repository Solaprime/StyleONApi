using System;
using System.Text.Json.Serialization;

namespace Shared
{
    public abstract class BaseResponse
    {
        //[JsonIgnore()]
        //public bool Success { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public string ErrorCode { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public string Error { get; set; }

        
        public bool Success { get; set; }
       
        public string ErrorCode { get; set; }

        public string Error { get; set; }
    }
}
