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
            return "";
        }

        [HttpPost("submitSync")]
        public void CreatePaymentSync([FromBody] string payment)
        {
        }

        [HttpPost("submitAsync")]
        public void CreatePaymentAsync([FromBody] string payment)
        {
        }
    }
}
