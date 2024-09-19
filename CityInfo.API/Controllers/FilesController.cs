using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        //File extension content type provider used to help provide support for application header types e.g. application/pdf
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
                ?? throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }
        [HttpGet("{FileId}")]
        public ActionResult GetFile(string FileId)
        {
            //Look up the actual file, depending on fileId...

            //Demo code

            var PathToFile = "AGureCV.pdf";

            //check if file exists
            if(!System.IO.File.Exists(PathToFile)) { return NotFound(); }

            if(!_fileExtensionContentTypeProvider.TryGetContentType(PathToFile, out var contentType)) { return NotFound(); }
            contentType = "application/octet-stream";
            
            //get the bytes of the file
            var bytes = System.IO.File.ReadAllBytes(PathToFile);

            //return file if exists in plain text style, getting all its bytes information
            return File(bytes,contentType, Path.GetFileName(PathToFile));
        }

        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file) //Iform files are files sent with http requests
        {
            /**
             * 
             * We want to validate input file
             * We also want to limit the size of file inputtable
             * We also want to accept only pdf files
             * Note: normally to avoid risk we would have had the location of file uploaded be on a seperate directory with no privellages, so if the file uploaded is malicious we limit the risks on the software
             */

            if(file.Length ==0 ||file.Length > 20971520 || file.ContentType != "application/pdf")
            {
                return BadRequest("Invalid file type or size");
            }

            /**
             * Creating the path file, good practise is to not using file.FileName, as attacker could have injected malicious file. including fullpaths or relative paths
             */

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"uploaded_file_{Guid.NewGuid()}.pdf");

            using( var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("File has been uploaded successfully.");
        }
    }
}
