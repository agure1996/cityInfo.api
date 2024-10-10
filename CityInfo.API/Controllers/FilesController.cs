using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace CityInfo.API.Controllers
{
    /// <summary>
    /// Manages file operations including uploading and retrieving files.
    /// </summary>
    [Route("api/v{version:apiVersion}/files")]
    [ApiVersion("1.0")]
    [Authorize] // Requires authorization to access this controller
    [ApiController]
    public class FilesController : ControllerBase
    {
        // File extension content type provider to determine content types for files
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesController"/> class.
        /// </summary>
        /// <param name="fileExtensionContentTypeProvider">Provider for file extension content types.</param>
        /// <exception cref="ArgumentNullException">Thrown when the file extension content type provider is null.</exception>
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
                ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }

        /// <summary>
        /// Retrieves a file based on its ID.
        /// </summary>
        /// <param name="FileId">The ID of the file to retrieve.</param>
        /// <returns>
        /// The requested file as a byte array, or a NotFound result if the file does not exist.
        /// </returns>
        [HttpGet("{FileId}")]
        [ApiVersion(0.1, Deprecated = true)]
        public ActionResult GetFile(string FileId)
        {
            // Look up the actual file, depending on FileId...

            // Demo code: This should be replaced with actual file lookup logic
            var pathToFile = "AGureCV.pdf";

            // Check if the file exists
            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound(); // Return NotFound if the file does not exist
            }

            // Determine the content type of the file
            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
            {
                contentType = "application/octet-stream"; // Default content type
            }

            // Read the bytes of the file
            var bytes = System.IO.File.ReadAllBytes(pathToFile);

            // Return the file as a response
            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }

        /// <summary>
        /// Creates a new file from the uploaded file.
        /// </summary>
        /// <param name="file">The file to be uploaded.</param>
        /// <returns>
        /// An ActionResult indicating the success or failure of the upload operation.
        /// Returns a success message if the file is uploaded successfully,
        /// or a BadRequest response if the file is invalid.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            // Validate the input file
            if (file.Length == 0 || file.Length > 20971520 || file.ContentType != "application/pdf")
            {
                return BadRequest("Invalid file type or size"); // Return BadRequest for invalid file
            }

            // Create a safe file path for the uploaded file
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"uploaded_file_{Guid.NewGuid()}.pdf");

            // Write the file to the specified path
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream); // Copy the file stream to the newly created file
            }

            return Ok("File has been uploaded successfully."); // Return success response
        }
    }
}
