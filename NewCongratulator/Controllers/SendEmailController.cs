using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;
using NewCongratulator.Dtos;
using NewCongratulator.Services;

namespace NewCongratulator.Controllers
{
    public class SendEmailController : Controller
    {
        private readonly EmailSenderService _service;

        public SendEmailController(EmailSenderService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult EmailForm(string email)
        {
            return View(new EmailSendDto { Email = email });
        }
        [HttpPost]
        public async Task<IActionResult> EmailForm(EmailSendDto data)
        {
            if (!ModelState.IsValid)
            {
                return View(data);
            }
            await _service.SendEmailAsync(data.Email, data.Subject, data.Body);
            return RedirectToAction("Index", "Birthday");
        }
    }
}
