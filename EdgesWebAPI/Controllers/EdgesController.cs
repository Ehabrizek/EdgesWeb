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
                _logger.LogInformation($"Combinations created for excel file {fileData.FileName}");

                System.IO.File.Delete(inputFile);

                //FileStreamResult will dispose this.
                FileStream outputFileStream = System.IO.File.OpenRead(outputFileName);

                string contentType = "text/csv";
                return new FileStreamResult(outputFileStream, contentType);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, e.Message);
            }
        }
    }
}