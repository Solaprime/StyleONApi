using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StyleONApi.LogDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StyleONApi.Controllers
{
   // [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json", "application/xml")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class ProductsV2Controller : ControllerBase
    {

        private readonly ILogger _logger;
        public ProductsV2Controller(ILoggerFactory loggerFactory)
        {
            // by spedifying the string name Productversion2  means we will be able
            // to see all log entries realteed to the name we specify this will act like
            // a means to group our entry
            _logger = loggerFactory.CreateLogger("ProductVersion2");
        }
        // GET: api/<ProductsV2Controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogDebug(DataEvents.GetVTwo, "Testing some EventId on Version 2" );
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProductsV2Controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductsV2Controller>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProductsV2Controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductsV2Controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
