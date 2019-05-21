using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCoreWithPowerShell.Filters;
using AspNetCoreWithPowerShell.Models;
using AspNetCoreWithPowerShell.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreWithPowerShell.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvokePwshDemoController : ControllerBase
    {
        private readonly ILogger<InvokePwshDemoController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public InvokePwshDemoController(IHostingEnvironment hostingEnvironment, ILogger<InvokePwshDemoController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [Route("upload")]
        [HttpPost]
        [ServiceFilter(typeof(ValidateMimeMultiPartContentFilter))]
        public async Task<IActionResult> UploadFiles([FromForm]FileModel fileModel)
        {
            if (ModelState.IsValid)
            {
                var file = fileModel.File;
                if (file.Length <= 0)
                {
                    return BadRequest();
                }

                var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'));

                _logger.LogInformation("file = {0}", fileName);

                var fileContent = await file.ReadAsStringAsync();

                var scriptCode = await ReadServerFile(GetServerBackendScripts("print-content.ps1"));

                var executeResult = ExecutePowerShell(scriptCode, fileContent);

                return Ok(executeResult);
            }
            _logger.LogInformation("Not acceptable api input");
            return BadRequest(ModelState);
        }

        private string GetServerBackendScripts(string scriptName)
        {
            return _hostingEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "BackendScripts" +
                   Path.DirectorySeparatorChar + scriptName;
        }

        private static async Task<string> ReadServerFile(string filePath)
        {
            using (var reader = System.IO.File.OpenText(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static string ExecutePowerShell(string scriptCode, string inputParameter)
        {
            using (var powerShell = PowerShell.Create())
            {
                powerShell.AddScript(scriptCode);
                powerShell.AddArgument(inputParameter);
                var executeResults = powerShell.Invoke();
                
                return executeResults[0].ToString();
            }
            
        }
    }
}
