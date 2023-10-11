namespace SupplierControlBackend.Models
{
    public class FileUpload
    {

        public string FileName { get; set; }

        public IFormFile FormFile { get; set; }
    }
}
