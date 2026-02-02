using System.Data;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static Jamghat.Models.Admin.AdminModel;

namespace Jamghat.Models.Admin
{
    public class AdminServices : IAdminService
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;
        public AdminServices(IConfiguration configuration,IWebHostEnvironment env)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _env = env;
        }

        public async Task<bool> AddContactFormAsync(ContactForm request)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_ContactForm", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@FullName", request.FullName);
            cmd.Parameters.AddWithValue("@Email", request.Email);
            cmd.Parameters.AddWithValue("@Phone", (object?)request.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Subject", request.Subject);
            cmd.Parameters.AddWithValue("@Message", request.Message);
            cmd.Parameters.AddWithValue("@action", "InsertContactForm");

            await con.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<List<ContactForm>> GetAllContactFormsAsync()
        {
            var list = new List<ContactForm>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_ContactForm", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@action", "GetAllContactForms");

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new ContactForm
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    FullName = reader["FullName"].ToString() ?? string.Empty,
                    Email = reader["Email"].ToString() ?? string.Empty,
                    Phone = reader["Phone"]?.ToString(),
                    Subject = reader["Subject"].ToString() ?? string.Empty,
                    Message = reader["Message"].ToString() ?? string.Empty,
                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue,
                    TotalCount = Convert.ToInt32(reader["TotalCounts"]),
              
                });
            }

            return list;
        }

        public async Task InsertPdfAsync(string fileName, string filePath)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_ManagePdf", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FileName", fileName);
            cmd.Parameters.AddWithValue("@FilePath", filePath);
            cmd.Parameters.AddWithValue("@Action", "insertpdf");

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<PdfFile>> GetAllActivePdfsAsync()
        {
            var list = new List<PdfFile>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_ManagePdf", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "getpdf");

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new PdfFile
                {
                    Id = reader.GetInt32(0),
                    FileName = reader.GetString(1),
                    FilePath = reader.GetString(2),
                    Status = reader.GetBoolean(3)
                });
            }

            return list;
        }



        public async Task<bool> InsertInternshipAsync(InternshipRequestDto request, string certificatePath)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ManageInternship", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "insert");
            cmd.Parameters.AddWithValue("@FullName", request.FullName);
            cmd.Parameters.AddWithValue("@Age", request.Age);
            cmd.Parameters.AddWithValue("@Class", request.Class);
            cmd.Parameters.AddWithValue("@ApplyingAs", request.ApplyingAs);
            cmd.Parameters.AddWithValue("@StartDate", request.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", request.EndDate);
            cmd.Parameters.AddWithValue("@DaysPerWeek", string.Join(",", request.DaysPerWeek));
            cmd.Parameters.AddWithValue("@InterestIn", string.Join(",", request.InterestIn));
            cmd.Parameters.AddWithValue("@OtherInterest", request.OtherInterest ?? "");
            cmd.Parameters.AddWithValue("@PastExperience", request.PastExperience ?? "");
            cmd.Parameters.AddWithValue("@PoliceDeclaration", request.PoliceDeclaration);
            cmd.Parameters.AddWithValue("@CertificateRequired", 1);
            cmd.Parameters.AddWithValue("@LetterFilePath", certificatePath);

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }


        public async Task<List<InternshipResponseDto>> GetInternshipsAsync()
        {
            var internships = new List<InternshipResponseDto>();

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ManageInternship", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "get");

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                internships.Add(new InternshipResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Age = reader.GetInt32(reader.GetOrdinal("Age")),
                    Class = reader.GetString(reader.GetOrdinal("Class")),
                    ApplyingAs = reader.GetString(reader.GetOrdinal("ApplyingAs")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    DaysPerWeek = reader.GetString(reader.GetOrdinal("DaysPerWeek")),
                    InterestIn = reader.GetString(reader.GetOrdinal("InterestIn")),
                    OtherInterest = reader.GetString(reader.GetOrdinal("OtherInterest")),
                    PastExperience = reader.GetString(reader.GetOrdinal("PastExperience")),
                    PoliceDeclaration = reader.GetBoolean(reader.GetOrdinal("PoliceDeclaration")),
                    CertificateRequired = reader.GetBoolean(reader.GetOrdinal("CertificateRequired")),
                    LetterFilePath = reader.GetString(reader.GetOrdinal("LetterFilePath")),
                    TotalCount = Convert.ToInt32(reader["totalCount"])
                });
            }

            return internships;
        }


        public int InsertMediaCentre(ItemDto item, string imagePath)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_Items_Insert", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Title", item.Title);
            cmd.Parameters.AddWithValue("@ItemDate", item.ItemDate);
            cmd.Parameters.AddWithValue("@Image", imagePath);
            cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Type", item.Type ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Link", item.Link ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@action", "insert");

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }


        public List<ItemDto> GetMediaCentre()
        {
            List<ItemDto> list = new();

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_Items_Insert", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "get");

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new ItemDto
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Title = dr["Title"].ToString(),
                    ItemDate = Convert.ToDateTime(dr["ItemDate"]),
                    Image = dr["Image"]?.ToString(),
                    Description = dr["Description"]?.ToString(),
                    Type = dr["Type"]?.ToString(),
                    Link = dr["Link"]?.ToString(),
                    totalCount = Convert.ToInt32(dr["totalCounts"])
                });
            }   

            return list;
        }

        public async Task<int> UploadAndExtractZipAsync(IFormFile zipFile, int? itemId = null)
        {
            if (zipFile == null || zipFile.Length == 0)
                throw new ArgumentException("ZIP file is required");

            if (Path.GetExtension(zipFile.FileName).ToLower() != ".zip")
                throw new ArgumentException("Only ZIP files are allowed");

            if (zipFile.Length > 50 * 1024 * 1024)
                throw new ArgumentException("ZIP file too large");

            string tempFolder = Path.Combine(_env.ContentRootPath, "Uploads", "temp");
            string mediaFolder = Path.Combine(_env.ContentRootPath, "Uploads", "zipmedia");
            Directory.CreateDirectory(tempFolder);
            Directory.CreateDirectory(mediaFolder);

            string tempZipPath = Path.Combine(tempFolder, $"{Guid.NewGuid()}.zip");
            List<string> extractedPaths = new();

            // Save ZIP temporarily
            using (var stream = new FileStream(tempZipPath, FileMode.Create))
            {
                await zipFile.CopyToAsync(stream);
            }

            try
            {
                using var archive = System.IO.Compression.ZipFile.OpenRead(tempZipPath);

                foreach (var entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name)) continue;

                    var extension = Path.GetExtension(entry.Name).ToLower();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    if (!allowed.Contains(extension)) continue;

                    string fileName = $"{Guid.NewGuid()}{extension}";
                    string filePath = Path.Combine(mediaFolder, fileName);
                    entry.ExtractToFile(filePath, overwrite: false);

                    // Use relative path for DB
                    string dbPath = $"/Uploads/zipmedia/{fileName}";
                    extractedPaths.Add(dbPath);
                }

                // Serialize the **whole list** into JSON array
                string jsonPaths = JsonSerializer.Serialize(extractedPaths);

                // Insert JSON into DB
                int newItemId = await InsertMediaImageAsync(jsonPaths, itemId);

                return newItemId; // Return the ItemId for this ZIP
            }
            finally
            {
                if (File.Exists(tempZipPath)) File.Delete(tempZipPath);
            }
        }


        private async Task<int> InsertMediaImageAsync(string imagePath, int? itemId)
            {
                using SqlConnection con = new(_connectionString);
                using SqlCommand cmd = new("sp_InsertMediaImage", con);
            cmd.Parameters.AddWithValue("@action", "upload");
            cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ItemId", (object)itemId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);

                await con.OpenAsync();
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            public List<MediaImageDto> GetMediaImages(int? itemId = null)
            {
                List<MediaImageDto> images = new();

                using SqlConnection con = new(_connectionString);
                using SqlCommand cmd = new("sp_InsertMediaImage", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "get");
                cmd.Parameters.AddWithValue("@ItemId", (object)itemId ?? DBNull.Value);

                con.Open(); 
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    images.Add(new MediaImageDto
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        ItemId = dr["ItemId"] != DBNull.Value ? Convert.ToInt32(dr["ItemId"]) : null,
                        ImagePaths = JsonSerializer.Deserialize<List<string>>(dr["ImagePath"].ToString())
                    });
                }

                return images;
            }
        }


    }


