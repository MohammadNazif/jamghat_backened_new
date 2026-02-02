using Jamghat.Models.Admin;
using Jamghat.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Jamghat.Models.Admin.AdminModel;

namespace Jamghat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IWebHostEnvironment _env;
        public AdminApiController(IAdminService adminService, IWebHostEnvironment env)
        {
            _adminService = adminService;
            _env = env;
        }

        [HttpPost("InserContactForm")]
        public IActionResult Login([FromBody] ContactForm contactForm)
        {
            if (contactForm == null)
                return BadRequest(new { message = "InValid Request" });

            var result = _adminService.AddContactFormAsync(contactForm);

            return Ok();
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("contactforms")]
        public async Task<IActionResult> GetAllContactForms()
        {
            var list = await _adminService.GetAllContactFormsAsync();
            return Ok(list);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("insertpdf")]
        public async Task<IActionResult> InsertPdf(IFormFile file, [FromForm] string filename)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required");

            if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                return BadRequest("Only PDF files allowed");

            // Physical folder (for saving)
            var folderPath = Path.Combine(_env.ContentRootPath, "Uploads", "pdfs");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Save file physically
            var physicalPath = Path.Combine(folderPath, file.FileName);
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ✅ Store RELATIVE path in DB
            var dbPath = Path.Combine("Uploads/pdfs", file.FileName)
                             .Replace("\\", "/");

            await _adminService.InsertPdfAsync(filename, dbPath);

            return Ok(new
            {
                message = "PDF uploaded successfully",
                pathStoredInDb = dbPath
            });
        }


        // GET PDFs
        [HttpGet("getpdf")]
        public async Task<IActionResult> GetPdfs()
        {
            var data = await _adminService.GetAllActivePdfsAsync();
            return Ok(data);
        }

        [HttpPost("apply")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ApplyInternship([FromForm] InternshipRequestDto request)
        {
            if (request.CertificateFile == null || request.CertificateFile.Length == 0)
                return BadRequest("Certificate file is required");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(request.CertificateFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only PDF, JPG, JPEG, PNG files are allowed for certificate");

            // Folder for saving certificate
            string folderPath = Path.Combine(_env.ContentRootPath, "Uploads", "certificates");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Generate unique file name to avoid collisions
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string physicalPath = Path.Combine(folderPath, uniqueFileName);

            // Save the file
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await request.CertificateFile.CopyToAsync(stream);
            }

            // Relative path to store in DB
            string dbPath = Path.Combine("Uploads/certificates", uniqueFileName)
                                .Replace("\\", "/");

            // Call service to insert the form data along with certificate path
            bool success = await _adminService.InsertInternshipAsync(request, dbPath);

            if (success)
                return Ok(new { message = "Internship application submitted successfully", certificatePath = dbPath });

            return StatusCode(500, "Error submitting internship application");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("getintershipData")]
        public async Task<ActionResult<List<InternshipResponseDto>>> GetAll()
        {
            var internships = await _adminService.GetInternshipsAsync();
            if (internships == null || internships.Count == 0)
                return NotFound("No internship applications found.");

            return Ok(internships);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("mediaCentreInsert")]
        public async Task<IActionResult> InsertMediaCentre([FromForm] ItemDto item)
        {
            if (item.ImageFile == null || item.ImageFile.Length == 0)
                return BadRequest("Image is required");

            string folderPath = Path.Combine(_env.ContentRootPath, "Uploads", "media");
            Directory.CreateDirectory(folderPath);

            string extension = Path.GetExtension(item.ImageFile.FileName);
            string fileName = $"{Guid.NewGuid()}{extension}";
            string physicalPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await item.ImageFile.CopyToAsync(stream);
            }

            string dbPath = $"/Uploads/media/{fileName}";

            int id = _adminService.InsertMediaCentre(item, dbPath);

            return Ok(new
            {
                message = "Media inserted successfully",
                id,
                imagePath = dbPath
            });
        }

        [HttpGet("mediaCentreGet")]
        public IActionResult GetMediaCentre()
        {
            var data = _adminService.GetMediaCentre();
            return Ok(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload-zip")]
        public async Task<IActionResult> UploadZip([FromForm] ZipUploadDto request)
        {
            try
            {
                var images = await _adminService.UploadAndExtractZipAsync(request.ZipFile, request.ItemId);
                return Ok(new { message = "ZIP extracted successfully", images });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("images")]
        public IActionResult GetImages([FromQuery] int? itemId = null)
        {
            var images = _adminService.GetMediaImages(itemId);
            return Ok(images);
        }
    }
}
