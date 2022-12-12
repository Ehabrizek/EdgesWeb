using EdgesWebAPI.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace EdgesWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EdgesController : ControllerBase
    {
        private readonly ILogger<EdgesController> _logger;

        public EdgesController(ILogger<EdgesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessExcelFile(IFormFile fileData, IFormCollection data)
        {
            try
            {
                System.IO.Directory.CreateDirectory("./files");
                string inputFile = $"./files/{fileData.FileName}";
                ExcelFileInfo excelFileInfo = new ExcelFileInfo
                {
                    sheetName = data["sheet"],
                    geneHeader = data["protein"],
                    pathwayHeader = data["pathwaydesc"],
                    keggId = data["pathwayid"]
                };

                using (Stream inputStream = fileData.OpenReadStream())
                using (FileStream inputFileStream = new FileStream(inputFile, FileMode.OpenOrCreate))
                {
                    await inputStream.CopyToAsync(inputFileStream);
                }

                CombinationProcessor processor = new CombinationProcessor();
                string outputFileName = processor.WriteCombinations(inputFile, excelFileInfo);
                
                System.IO.File.Delete(inputFile);

                FileStream outputFileStream = System.IO.File.OpenRead(outputFileName);
                string contentType = "";
                new FileExtensionContentTypeProvider().TryGetContentType(outputFileName, out contentType);
                return new FileStreamResult(outputFileStream, contentType);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        public FileStreamResult GetFile([FromBody] string fileNameGuid)
        {
            string file = $"{fileNameGuid}.csv";
            using (FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate))
            {
                return new FileStreamResult(fileStream, "application/octem-stream.csv");
            }
        }

        [HttpDelete]
        public IActionResult DeleteFile([FromBody] string fileNameGuid)
        {
            string fileName = $"{fileNameGuid}.csv";
            try
            {
                System.IO.File.Delete(fileName);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}