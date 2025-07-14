using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using NewCongratulator.Data;
using NewCongratulator.Models;
using NewCongratulator.Services;
using Microsoft.Extensions.Options;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.AspNetCore.Mvc.Routing;
using NewCongratulator.Dtos;
/* TO DO TASKS:
 * 1. HTML + CSS Page view *
 * 2. Create todays and nearest birthdays represent logic * Done
 * 3. Update delete logic (to delete images from wwwroot) * Done
 * 4. May be create email congratulation sending with app email * Done but I refused to create the client logic because this service required vpn to be used but i can change ma mind
 *  4.1 Create interactive page where user can create his congratulation and send it with inner site email
 * 5. To create number 4 as I read from task it needs to send email !!to user reminding him to greet people who had birthday today or near to todays date!! so i need authorization or may be microservice to do that * Do not needed
*/
namespace NewCongratulator.Controllers
{
    public class BirthdayController : Controller
    {

        private readonly HBDcontext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<BirthdayController> _logger;
        private readonly IOptions<EmailSettings> _settings;
        private readonly EmailSenderService _emailSenderService;

        private static async Task<DateOnly?> GetInternetDateAsync()
        {
            using var client = new HttpClient();
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, "https://www.google.com"));

            if (response.Headers.Date.HasValue)
            {
                var utcDateTime = response.Headers.Date.Value.UtcDateTime;
                return DateOnly.FromDateTime(utcDateTime);
            }

            return null;
        }


        public BirthdayController(HBDcontext context, ILogger<BirthdayController> logger, IWebHostEnvironment env, IOptions<EmailSettings> settings, EmailSenderService emailSenderService)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _settings = settings;
            _emailSenderService = emailSenderService;
        }

        public async Task<IActionResult> Upcoming()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var threshold = today.AddDays(7);
            var allBirthdays = await _context.HBDdatas.ToListAsync();
            var upcoming = allBirthdays.Where(data =>
            {
                var nextbirthday = new DateOnly(today.Year, data.birthdayDate.Month, data.birthdayDate.Day);
                if (nextbirthday < today) { nextbirthday = nextbirthday.AddYears(1); }
                var daysUntil = (nextbirthday.ToDateTime(TimeOnly.MinValue) - today.ToDateTime(TimeOnly.MinValue)).Days;
                return daysUntil <= 7;
            }).ToList();
            return View(upcoming);
        }

        public async Task<IActionResult> Index(bool decending = false)
        {
            var query = _context.HBDdatas.AsQueryable();
            query = decending ? query.OrderByDescending(x => x.birthdayDate) : query.OrderBy(x => x.birthdayDate);
            var item = await query.ToListAsync();
            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("fullName, birthdayDate, emailAddress")] HumanBirthdayData data, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var currentDate = await GetInternetDateAsync();
                if (currentDate.HasValue)
                {
                    int age = currentDate.Value.Year - data.birthdayDate.Year;
                    if (currentDate.Value < data.birthdayDate.AddYears(age))
                    {
                        age--;
                    }
                    data.age = age;
                    Console.WriteLine(age);
                }
                var Iservice = new ImageCreateService(_env);
                data.imageUrl = await Iservice.ImageCreate(imageFile);
                _context.HBDdatas.Add(data);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(data);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _context.HBDdatas.FirstOrDefaultAsync(x => x.Id == id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, fullName, birthdayDate, emailAddress")] HumanBirthdayData data, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var currentDate = await GetInternetDateAsync();
                if (currentDate.HasValue)
                {
                    int age = currentDate.Value.Year - data.birthdayDate.Year;
                    if (currentDate.Value < data.birthdayDate.AddYears(age))
                    {
                        age--;
                    }
                    data.age = age;
                }
                var Iservice = new ImageCreateService(_env);
                data.imageUrl = await Iservice.ImageCreate(imageFile);
                _context.HBDdatas.Update(data);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(data);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.HBDdatas.FirstOrDefaultAsync(x => x.Id == id);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var data = await _context.HBDdatas.FindAsync(id);
            if (data != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                var fileName = Path.GetFileName(data.imageUrl);
                var absolutefilePath = Path.Combine(uploadsFolder, fileName);

                if (System.IO.File.Exists(absolutefilePath))
                {
                    System.IO.File.Delete(absolutefilePath);
                }
                _context.HBDdatas.Remove(data);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromForm] EmailSendDto data)
        {
            var sender = new EmailSenderService(_settings);
            await sender.SendEmailAsync(data.Email, data.Subject, data.Body);
            return RedirectToAction("Index");
        }
    }
}
