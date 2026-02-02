namespace Jamghat.Models.Admin
{
    public class AdminModel
    {
        public class ContactForm
        {
            public int? Id { get; set; }
            public string FullName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Phone { get; set; }
            public string Subject { get; set; } = null!;
            public string Message { get; set; } = null!;
            public DateTime? CreatedAt { get; set; }
        }

        public class PdfFile
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public Boolean Status { get; set; }
        }



public class InternshipRequestDto
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Class { get; set; }
        public string ApplyingAs { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<string> DaysPerWeek { get; set; }
        public List<string> InterestIn { get; set; }

        public string OtherInterest { get; set; }
        public string PastExperience { get; set; }
        public bool PoliceDeclaration { get; set; }

        // Certificate file upload
        public IFormFile CertificateFile { get; set; }
    }

        public class InternshipResponseDto
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public int Age { get; set; }
            public string Class { get; set; }
            public string ApplyingAs { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string DaysPerWeek { get; set; }
            public string InterestIn { get; set; }
            public string OtherInterest { get; set; }
            public string PastExperience { get; set; }
            public bool PoliceDeclaration { get; set; }
            public bool CertificateRequired { get; set; }
            public string LetterFilePath { get; set; }
        }
        public class ItemDto
        {

            public int? Id { get; set; }
            public string Title { get; set; }
            public DateTime ItemDate { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public string Link { get; set; }
            public IFormFile ImageFile { get; set; }

            public string  Image { get; set; }
        }
        public class ZipUploadDto
        {
            public IFormFile ZipFile { get; set; }
            public int? ItemId { get; set; }
        }

        public class MediaImageDto
        {
            public int Id { get; set; }
            public int? ItemId { get; set; }
            public string ImagePath { get; set; }

            public List<string> ImagePaths { get; set; } = new();
        }


    }
}

  