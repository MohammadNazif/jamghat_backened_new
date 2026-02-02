using Azure.Core;
using static Jamghat.Models.Admin.AdminModel;

namespace Jamghat.Models.Admin
{
    public interface IAdminService
    {

        Task<bool> AddContactFormAsync(ContactForm contact);
        Task<List<ContactForm>> GetAllContactFormsAsync();
        Task InsertPdfAsync(string fileName, string filePath);
        Task<List<PdfFile>> GetAllActivePdfsAsync();

        Task<bool> InsertInternshipAsync(InternshipRequestDto request, string certificatePath);
         Task<List<InternshipResponseDto>> GetInternshipsAsync();

        int InsertMediaCentre(ItemDto item, string imagePath);
        List<ItemDto> GetMediaCentre();

        Task<int> UploadAndExtractZipAsync(IFormFile zipFile, int? itemId = null);
        List<MediaImageDto> GetMediaImages(int? itemId = null);
    }
}
