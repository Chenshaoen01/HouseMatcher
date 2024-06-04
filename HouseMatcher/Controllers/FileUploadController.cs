using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseMatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        public readonly IWebHostEnvironment _env;

        public FileUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // POST api/<FileUploadController>
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            try
            {
                Guid NewGuid = Guid.NewGuid();
                string FileExtention = System.IO.Path.GetExtension(file.FileName);
                string filePath = _env.ContentRootPath + @"\wwwroot\" + NewGuid.ToString() + FileExtention;

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { id = NewGuid.ToString(), fileType = FileExtention });
            }
            catch
            {
                return BadRequest("圖片上傳失敗");
            }
        }

        [HttpPost("CKEditorImgUpload")]
        public async Task<IActionResult> CKEditorImgUpload(IFormFile file)
        {
            Guid NewGuid = Guid.NewGuid();
            string FileExtention = System.IO.Path.GetExtension(file.FileName);
            string filePath = _env.ContentRootPath + @"\wwwroot\" + NewGuid.ToString() + FileExtention;

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { url = NewGuid.ToString() + FileExtention });
        }
    }
}
