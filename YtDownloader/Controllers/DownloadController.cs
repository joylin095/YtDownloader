using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace YtDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public DownloadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public IActionResult Download([FromBody] DownloadRequest request)
        {
            if (string.IsNullOrEmpty(request.YoutubeUrl))
            {
                return BadRequest(new { success = false, message = "請提供 YouTube 影片網址" });
            }

            try
            {
                string pythonExe = @"C:\Users\USER\AppData\Local\Programs\Python\Python39\python.exe";
                string scriptPath = Path.Combine(_env.ContentRootPath, "Scripts", "download_list_ytdlp.py");
                string outputDir = Path.Combine(_env.WebRootPath, "downloads");

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"{scriptPath} {request.YoutubeUrl} {outputDir}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process
                {
                    StartInfo = psi
                };
                process.Start();

                string output = process.StandardOutput.ReadToEnd().Trim();

                process.WaitForExit();

                int jsonStartIndex = output.IndexOf('{');
                if (jsonStartIndex != -1)
                {
                    output = output.Substring(jsonStartIndex);
                }

                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    return BadRequest(new { success = false, message = "下載失敗" + error });
                }

                DownloadResult? jsonResult = JsonSerializer.Deserialize<DownloadResult>(output);

                string? title = jsonResult?.Title;
                string? fileName = Path.GetFileName(jsonResult?.Filepath);
                string downloadUrl = $"/downloads/{fileName}";

                return Ok(new { success = true, title, fileName, downloadUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class DownloadRequest
    {
        public string? YoutubeUrl { get; set; }
    }
    public class  DownloadResult
    {
        public string? Title { get; set; }
        public string? Filepath { get; set; }
    }
}
