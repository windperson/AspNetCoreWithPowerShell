using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCoreWithPowerShell.Filters;
using AspNetCoreWithPowerShell.Models;
using AspNetCoreWithPowerShell.Utils;
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

        public InvokePwshDemoController(ILogger<InvokePwshDemoController> logger)
        {
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

                return Ok();
            }
            _logger.LogInformation("Not acceptable api input");
            return BadRequest(ModelState);
        }

    }
}