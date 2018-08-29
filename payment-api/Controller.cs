using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace payment_api
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

        [HttpPost("submitSync")]
        public void CreatePaymentSync([FromBody] string payment)
        {
            throw new NotImplementedException();
        }

        [HttpPost("submitAsync")]
        public void CreatePaymentAsync([FromBody] string payment)
        {
            throw new NotImplementedException();
        }
    }
}
