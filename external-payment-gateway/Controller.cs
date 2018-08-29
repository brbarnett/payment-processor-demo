using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using external_payment_gateway.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace external_payment_gateway
{
    [Route("/")]
    [ApiController]
    public class Controller : ControllerBase
    {
        [HttpPost("")]
        public ActionResult<SubmitPaymentResponse> CreatePayment([FromBody] SubmitPaymentRequest paymentRequest)
        {
            // mock processing time
            Thread.Sleep(5000);
 
            // create response with "Completed" status
            var response = new SubmitPaymentResponse(paymentRequest.PaymentId, "Completed");
            return response;
        }
    }
}
