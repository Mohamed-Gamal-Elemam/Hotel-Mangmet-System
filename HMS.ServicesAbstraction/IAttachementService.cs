using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.ServicesAbstraction
{
    public interface IAttachementService
    {
        public Task<string?> UploadFileAsync(IFormFile file, string FolderName);
        public bool DeleteFileAsync(string fileName,string FolderName);
    }
}
