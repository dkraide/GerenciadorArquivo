using API.CustomToken;
using Communication.Contexts;
using Communication.DTOs;
using Communication.Models;
using Communication.Services;
using Communication.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Xml;
using static System.Net.WebRequestMethods;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthOptions.DefaultScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FileController : ControllerBase
    {
        private readonly SFile _fileService;
        private readonly SUser _userService;
        private readonly UserManager<MUser> _userManager;
        private readonly IWebHostEnvironment _hosting;
        public FileController(IWebHostEnvironment hosting, Context context, UserManager<MUser> userManager)
        {
            _fileService = new SFile(context, userManager);
            _userService = new SUser(context, userManager);
            _hosting = hosting;
        }
        
        [HttpPost("Upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return BadRequest("Invalid file");


            var userId = User.Claims.Where(c => c.Type == "id").FirstOrDefault()?.Value;
            if (userId == null)
                return Unauthorized();

            MFile mFile = new MFile()
            {
                CreatedAt = DateTime.Now,
                FileName = file.FileName,
                Type = Path.GetExtension(file.FileName),
                UserId = userId,
            };
            if (mFile.Type == ".xml")
                mFile.ContentXML = GetXMLItens(file);
            else
                mFile.ContentXML = null;
            _fileService.Create(mFile);
            var userFolder = Helpers.GetUserFolder(userId);
            var filePath = Helpers.GetFilePath(userFolder, mFile.FileName, mFile.Id);
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);   
            }
            using(FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                file.CopyTo(fs);
            }
            return Ok(mFile);
        }
        [HttpDelete("Delete")]
        public IActionResult Delete(string fileId)
        {
            var userId = User.Claims.Where(c => c.Type == "id").FirstOrDefault()?.Value;
            if (userId == null)
                return Unauthorized();

            if (
                string.IsNullOrEmpty(fileId))
                return BadRequest("Invalid object");

            var file = _fileService.Get(fileId);
            if (file == null)
                return BadRequest("Entry not found in Database");

          
            var userFolder = Helpers.GetUserFolder(file.UserId);
            var filePath = Helpers.GetFilePath(userFolder, file.FileName, file.Id);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _fileService.Remove(file);

            return Ok();
        }
        [HttpPut("Update")]
        public IActionResult Update(IFormFile file, [FromForm]string fileId)
        {
            if (file == null || file.Length <= 0)
                return BadRequest("Invalid File");


            var userId = User.Claims.Where(c => c.Type == "id").FirstOrDefault()?.Value;
            if (userId == null)
                return Unauthorized();

            MFile? mFile = _fileService.Get(fileId);

            if (mFile == null)
                return BadRequest("Entry no found in database");

            DeleteFile(mFile);

            mFile.CreatedAt = DateTime.Now;
            mFile.FileName = file.FileName;
            mFile.Type = Path.GetExtension(file.FileName);
            mFile.UserId = userId;

            if (mFile.Type == ".xml")
                mFile.ContentXML = GetXMLItens(file);
            else
                mFile.ContentXML = null;

            _fileService.Update(mFile);

            var userFolder = Helpers.GetUserFolder(userId);
            var filePath = Helpers.GetFilePath(userFolder, mFile.FileName, mFile.Id);
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                file.CopyTo(fs);
            }
            return Ok(mFile);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.Claims.Where(c => c.Type == "id").FirstOrDefault()?.Value;
            if (userId == null)
                userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Jti).FirstOrDefault()?.Value;
            if (userId == null)
                return Unauthorized();

            var files = await  _fileService.GetFiles(userId);

            files.ForEach(f =>
            {
                if (f.User != null)
                    f.User.Files = null;
            });
            var groupped = files.GroupBy(x => x.Type)
                .Select(x => new
                {
                    type = x.Key,
                    files = x.Select(file =>
                    new {
                        file.UserId,
                        file.ContentXML,
                        file.CreatedAt,
                        file.FileName,
                        file.Id,
                        file.Type,
                        user = new UserDTO
                        {
                            Email = file.User?.Email,
                            Id = file.User?.Id,
                            Name = file.User?.Name,
                            UserName = file.User?.UserName,
                        }
                    }).ToList(),
                })
                .ToList();

            return Ok(groupped);


        }
        private string GetXMLItens(IFormFile file)
        {
            try
            {
                using(var reader = new StreamReader(file.OpenReadStream()))
                {
                    var str = reader.ReadToEnd();
                    if (str == null)
                        return "INVALID XML FILE";
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(str);
                    if (doc.HasChildNodes)
                    {
                        var s = GetColumns(doc.ChildNodes);
                        return s;
                    }
                    return "Empty";
                }
            }catch(Exception ex)
            {
                return $"Error {ex.Message}";
            }
        }
        private string GetColumns(XmlNodeList nodes)
        {
            var str = "";
            foreach(XmlNode x in nodes)
            {
                if (x.Name == "#text")
                    continue;

                str += x.Name + "--";
                if(x.HasChildNodes)
                    str += GetColumns(x.ChildNodes);
            }
            return str;
        }
        private void DeleteFile(MFile mFile)
        {
            var userFolder = Helpers.GetUserFolder(mFile.UserId);
            var filePath = Helpers.GetFilePath(userFolder, mFile.FileName, mFile.Id);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }
    }
}
