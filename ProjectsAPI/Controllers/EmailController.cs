using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsAPI.Models;
using ProjectsAPI.Services;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ProjectsAPI.Controllers
{
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [Route("email/send-email")]
        public async Task<IActionResult> SendEmailAsync([FromForm]Email email)
        {
            email.subject = "Message from Project Portfolio App!";
            await _emailService.SendEmail(email.name, email.address, email.phoneNumber, email.subject, email.message);
            return Ok();
        }
    }
}