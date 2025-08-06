using FileUploadDownload.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using FileUploadDownloadAPI;
using FileUploadDownloadAPI.Controllers;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Http.Headers;

namespace FileUploadDownload.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7219/api/");
        private readonly HttpClient _client;

        public HomeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        public IActionResult Index()
        {
            //   var FileUploadDownloadAPI = new FileUploadDownloadAPI();
            // GetFiles();
            HttpResponseMessage response = _client.GetAsync(baseAddress + "File/GetFiles").Result;
            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                List<FileModel> fmodel= JsonConvert.DeserializeObject<List<FileModel>>(data) as List<FileModel> ;
                return View(fmodel);

            }
            return View();
        }
        public IActionResult UploadFile()
        {
            return View();

        }
        [HttpPost]
        public IActionResult UploadToDatabase(FileModel file)
        {
            try
            {
                //string contentType = ""; 
                //new FileExtensionContentTypeProvider().TryGetContentType(file.FormFile.FileName, out contentType);
                //string data=JsonConvert.SerializeObject(file.FormFile);
                //StringContent content = new StringContent(data,Encoding.UTF8, "text/plain");
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.FormFile.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.FormFile.ContentType);

                content.Add(fileContent, "file", file.FormFile.FileName);

                var jsonPayload = "that payload from the above sample";
                var jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);
                var jsonContent = new StreamContent(new MemoryStream(jsonBytes));
                jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                content.Add(jsonContent, "metadata", "metadata.json");
                HttpResponseMessage response = _client.PostAsync(baseAddress + "File/UploadFile/SaveFile", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "File Upload Successful";
                }
                else
                {
                    TempData["Message"] = "File Upload Failed";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Log ex
                TempData["Message"] = "File Upload Failed";
            }
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Download(int id)
        {
            //   var FileUploadDownloadAPI = new FileUploadDownloadAPI();
            // GetFiles();
            ///   string data = JsonConvert.SerializeObject(id);
            //  StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.GetAsync(baseAddress + "File/DownloadFile/" + id).Result;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    new HttpClient().GetStreamAsync(baseAddress + "File/DownloadFile/" + id).Result.CopyTo(ms);
            //}
            //if (response.IsSuccessStatusCode)
            if (response.IsSuccessStatusCode)
            {
                //string data = response.Content.ReadAsStringAsync().Result;
                //List<FileModel> fmodel = JsonConvert.DeserializeObject<List<FileModel>>(data) as List<FileModel>;
                //return View(fmodel);
                TempData["Message"] = "File download Successful";
            }
            else
            {
                TempData["Message"] = "File download Failed";
            }
            return RedirectToAction("Index");
        }
    }
}