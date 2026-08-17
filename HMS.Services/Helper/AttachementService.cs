using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Services.Helper
{
    public class AttachementService : IAttachementService
    {

        private readonly long _maxSize=5*1024*1024;
        private readonly string[] _allawedExtensions = [".jpg", ".jpeg", ".png", ".svg"];
        private readonly ILogger _logger;
        public AttachementService(ILogger<AttachementService> logger)
        {
            _logger = logger;
        }

        public async Task<string?> UploadFileAsync(IFormFile file, string FolderName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;
                if(file.Length > _maxSize) return null;
                var extention = Path.GetExtension(file.FileName).ToLower();
                if (!_allawedExtensions.Contains(extention)) return null;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", FolderName);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var UnqueFileName = Guid.NewGuid().ToString() + extention;
                var fullPath = Path.Combine(filePath, UnqueFileName);
                using var streamFile = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(streamFile);
                return UnqueFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return null;
            }
        }
        public bool DeleteFileAsync(string fileName, string FolderName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(FolderName))
                    return false;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", FolderName, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return false;
            }
        }

        
    }
}
