using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Enums;
using Mnf_Portal.Core.Interfaces;

namespace Mnf_Portal.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUdController : ControllerBase
    {
        readonly IMnfContextRepo<ContactUs> _repo;
        readonly IEmailService _emailService;

        public ContactUdController(IMnfContextRepo<ContactUs> repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }


        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs([FromBody] ContactUsDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Description))
                return BadRequest("Email and Description are required.");

            string subject;
            switch (request.Type)
            {
                case MessageType.Complaint:
                    subject = "New Complaint Received";
                    break;
                case MessageType.Suggestion:
                    subject = "New Suggestion Received";
                    break;
                case MessageType.Evaluation:
                    subject = "New Evaluation Received";
                    break;
                default:
                    return BadRequest("Invalid message type.");
            }

            var adminEmail = "mazenkhtab11@gmail.com";

            var body = $"Type: {subject}\n\nFrom: {request.Email}\nMessage: {request.Description}";
            if (request.Type == MessageType.Evaluation)
            {
                body += $"\nRate: {request.Rate}";
            }

            // Send the email
            await _emailService.SendEmailAsync(adminEmail, subject, body);

            return Ok("Message sent successfully.");
        }
    }
}
