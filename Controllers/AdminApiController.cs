using jamghat.Models.ResponseFormat;
using Jamghat.Models.Admin;
using Jamghat.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Jamghat.Models.Admin.AdminModel;

namespace Jamghat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : GenericApiController
    {
        private readonly IAdminService _adminService;
        private readonly IWebHostEnvironment _env;
        private readonly MailSender _mailsender;

        public AdminApiController(IAdminService adminService, IWebHostEnvironment env, MailSender mailSender)
        {
            _adminService = adminService;
            _env = env;
            _mailsender = mailSender;


        }

        // ---------------- CONTACT FORM ----------------
        [HttpPost("InserContactForm")]
        public async Task<IActionResult> InsertContactForm([FromBody] ContactForm contactForm)
        {
            if (contactForm == null)
                return Fail("Invalid request");

            return await RespondAsync(async () =>
            {
                var result = await _adminService.AddContactFormAsync(contactForm);
                if (result)
                {
                    await _mailsender.SendAsync(
               contactForm.Email,
          "Welcome to Jamghat 🎉",
       $"<h3>Hello {contactForm.FullName}</h3><p>Your account is created successfully.</p>"
    );
                }
                return true;

            }, "Contact form submitted successfully");


        }

        [HttpGet("contactforms")]
        public async Task<IActionResult> GetAllContactForms()
        {
            return await RespondAsync(
                () => _adminService.GetAllContactFormsAsync(),
                "Contact forms fetched successfully");
        }

        // ---------------- PDF ----------------
        [HttpPost("insertpdf")]
        public async Task<IActionResult> InsertPdf(IFormFile file, [FromForm] string filename)
        {
            if (file == null || file.Length == 0)
                return Fail("File is required");

            if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                return Fail("Only PDF files allowed");

            return await RespondAsync(async () =>
            {
                var folderPath = Path.Combine(_env.ContentRootPath, "Uploads", "pdfs");
                Directory.CreateDirectory(folderPath);

                var physicalPath = Path.Combine(folderPath, file.FileName);
                await using var stream = new FileStream(physicalPath, FileMode.Create);
                await file.CopyToAsync(stream);

                var dbPath = Path.Combine("Uploads/pdfs", file.FileName)
                    .Replace("\\", "/");

                await _adminService.InsertPdfAsync(filename, dbPath);

                return new { PathStoredInDb = dbPath };
            }, "PDF uploaded successfully");
        }

        [HttpGet("getpdf")]
        public async Task<IActionResult> GetPdfs()
        {
            return await RespondAsync(
                () => _adminService.GetAllActivePdfsAsync(),
                "PDFs fetched successfully");
        }

        // ---------------- INTERNSHIP ----------------
        [HttpPost("apply")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ApplyInternship([FromForm] InternshipRequestDto request)
        {
            if (request.CertificateFile == null || request.CertificateFile.Length == 0)
                return Fail("Certificate file is required");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(request.CertificateFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return Fail("Only PDF, JPG, JPEG, PNG files are allowed");

            return await RespondAsync(async () =>
            {
                var folderPath = Path.Combine(_env.ContentRootPath, "Uploads", "certificates");
                Directory.CreateDirectory(folderPath);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var physicalPath = Path.Combine(folderPath, uniqueFileName);

                await using var stream = new FileStream(physicalPath, FileMode.Create);
                await request.CertificateFile.CopyToAsync(stream);

                var dbPath = Path.Combine("Uploads/certificates", uniqueFileName)
                    .Replace("\\", "/");

                await _adminService.InsertInternshipAsync(request, dbPath);

                return new { CertificatePath = dbPath };
            }, "Internship application submitted successfully");
        }

        [HttpGet("getintershipData")]
        public async Task<IActionResult> GetAllInternships()
        {
            return await RespondAsync(
                () => _adminService.GetInternshipsAsync(),
                "Internship applications fetched successfully");
        }

        // ---------------- MEDIA CENTRE ----------------
        [HttpPost("mediaCentreInsert")]
        public IActionResult InsertMediaCentre([FromForm] ItemDto item)
        {
            if (item.ImageFile == null || item.ImageFile.Length == 0)
                return Fail("Image is required");

            var folderPath = Path.Combine(_env.ContentRootPath, "Uploads", "media");
            Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(item.ImageFile.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var physicalPath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(physicalPath, FileMode.Create);
            item.ImageFile.CopyTo(stream);

            var dbPath = $"/Uploads/media/{fileName}";
            var id = _adminService.InsertMediaCentre(item, dbPath);

            return Respond(new { Id = id, ImagePath = dbPath },
                "Media inserted successfully");
        }

        [HttpGet("mediaCentreGet")]
        public IActionResult GetMediaCentre()
        {
            return Respond(
                _adminService.GetMediaCentre(),
                "Media fetched successfully");
        }

        // ---------------- ZIP ----------------
        [HttpPost("upload-zip")]
        public async Task<IActionResult> UploadZip([FromForm] ZipUploadDto request)
        {
            return await RespondAsync(
                () => _adminService.UploadAndExtractZipAsync(
                    request.ZipFile, request.ItemId),
                "ZIP extracted successfully");
        }

        [HttpGet("images")]
        public IActionResult GetImages([FromQuery] int? itemId = null)
        {
            return Respond(
                _adminService.GetMediaImages(itemId),
                "Images fetched successfully");
        }
    }
}
