using Azure;
using Communication.Constants;
using Communication.Contexts;
using Communication.Models;
using Communication.Models.NFeModel;
using Communication.Services;
using Communication.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace WEB.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly SFile _fileService;
        public FileController(Context context, UserManager<MUser> userManager)
        {
            _fileService = new SFile(context, userManager);
        }
        public async Task<IActionResult> Index()
        {
         
            return View();
        }
        [HttpGet]
        public IActionResult Delete(string id)
        {
            MFile? file = _fileService.Get(id);
            if (file == null)
                return RedirectToAction("Index");
            return View(file);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(MFile file)
        {
            if (file == null)
            {
                return View(file);
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", $"{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
                string message = JsonConvert.SerializeObject(file, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                string url = $"{Consts.URLAPI}/File/Delete?fileId={file.Id}";

                var res = await client.DeleteAsync(url);

                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");

                }
                return View(file);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Upload(IFormFile file, string fileId = null)
        {
            if(file == null)
            {
                return BadRequest("Invalid File");
            }
            using (var stream = new MemoryStream())
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", $"{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
                file.CopyTo(stream);
                var requestContent = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(stream.ToArray());
                requestContent.Add(fileContent, file.Name, file.FileName);

                HttpResponseMessage response;
                if(fileId == null)
                {
                    string url = $"{Consts.URLAPI}/File/Upload";
                    response = await client.PostAsync(url, requestContent);
                }
                else
                {
                    var stringContenet = new StringContent(fileId);
                    requestContent.Add(stringContenet, "fileId");
                    string url = $"{Consts.URLAPI}/File/Update";
                    response = await client.PutAsync(url, requestContent);
                }


                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var savedFile = JsonConvert.DeserializeObject<MFile>(result);
                    return Ok(savedFile);

                }
                var err = await response.Content.ReadAsStringAsync();
                return BadRequest(err);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var files = await _fileService.GetFiles(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(files);
        }

        [HttpGet]
        public async Task<ActionResult> GetXMLObject(string fileId)
        {
            if (fileId == null)
            {
                return BadRequest("Invalid File");
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", $"{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
                string url = $"{Consts.URLAPI}/File/GetXMLObject?fileId={fileId}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject(result);
                    return Ok(obj);
                }
                return BadRequest(response.StatusCode);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(string fileId)
        {
            if (fileId == null)
            {
                return BadRequest("Invalid File");
            }
            MFile? file = _fileService.Get(fileId);
            if (file == null)
                return RedirectToAction("Index");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", $"{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
                string url = $"{Consts.URLAPI}/File/DownloadFile?fileId={fileId}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsByteArrayAsync();
                    return File(result, "application/octet-stream", file.FileName);
                }
                var err = await response.Content.ReadAsStringAsync();
                return BadRequest(err);
            }
        }

    }
}
