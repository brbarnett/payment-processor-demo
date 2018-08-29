using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace payment_processor
{
    [Route("/")]
    [ApiController]
    public class Controller : ControllerBase
    {
        [HttpGet("{paymentId}")]
        public ActionResult<string> GetPaymentStatus()
        {
            throw new NotImplementedException();
        }

        [HttpPost("")]
        public void CreatePaymentSync([FromBody] string payment)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("")]
        public void UpdatePaymentStatus([FromBody] string payment)
        {
            throw new NotImplementedException();
        }
    }
}
