using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWithPowerShell.Models
{
    public class FileModel
    {
        public string FileName { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }
}
