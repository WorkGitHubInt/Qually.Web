using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Qually.Models;

namespace Qually
{
    public class ExplorerController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExplorerController(IHostingEnvironment hostingEnvironment,
                                  IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        //
        // GET: /Explorer/

        public IActionResult Index(string path)
        {
            var folderPath = _hostingEnvironment.WebRootPath + "\\launcher\\";
            // or folderPath = "FullPath of the folder on server" 

            var realPath = folderPath + path;

            if (System.IO.File.Exists(realPath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(realPath);

                //http://stackoverflow.com/questions/1176022/unknown-file-type-mime
                return File(fileBytes, "application/force-download");
            }
            else if (System.IO.Directory.Exists(realPath))
            {
                List<DirModel> dirListModel = MapDirs(realPath);

                List<FileModel> fileListModel = MapFiles(realPath);

                ExplorerModel explorerModel = new ExplorerModel(dirListModel, fileListModel);

                //For using browser ability to correctly browsing the folders,
                //Every path needs to end with slash
                if (realPath.Last() != '/' && realPath.Last() != '\\')
                { explorerModel.URL = "/Explorer/" + path + "/"; }
                else
                { explorerModel.URL = "/Explorer/" + path; }

                var request = _httpContextAccessor.HttpContext.Request;

                UriBuilder uriBuilder = new UriBuilder
                { Path = request.Path.ToString() };

                //Gettin the current directory name using page URL.
                explorerModel.FolderName = WebUtility.UrlDecode(uriBuilder.Uri.Segments.Last());

                //Making a URL to going up one level. 
                Uri uri = new Uri(uriBuilder.Uri.AbsoluteUri.Remove
                                    (uriBuilder.Uri.AbsoluteUri.Length - uriBuilder.Uri.Segments.Last().Length));

                explorerModel.ParentFolderName = uri.AbsolutePath;

                return View(explorerModel);
            }
            else
            {
                return Content(path + " is not a valid file or directory.");
            }
        }

        private List<DirModel> MapDirs(String realPath)
        {
            List<DirModel> dirListModel = new List<DirModel>();

            IEnumerable<string> dirList = Directory.EnumerateDirectories(realPath);
            foreach (string dir in dirList)
            {
                DirectoryInfo d = new DirectoryInfo(dir);

                DirModel dirModel = new DirModel
                {
                    DirName = Path.GetFileName(dir),
                    DirAccessed = d.LastAccessTime
                };

                dirListModel.Add(dirModel);
            }

            return dirListModel;
        }

        private List<FileModel> MapFiles(String realPath)
        {
            List<FileModel> fileListModel = new List<FileModel>();

            IEnumerable<string> fileList = Directory.EnumerateFiles(realPath);
            foreach (string file in fileList)
            {
                FileInfo f = new FileInfo(file);

                FileModel fileModel = new FileModel();

                if (f.Extension.ToLower() != "php" && f.Extension.ToLower() != "aspx"
                    && f.Extension.ToLower() != "asp" && f.Extension.ToLower() != "exe")
                {
                    fileModel.FileName = Path.GetFileName(file);
                    fileModel.FileAccessed = f.LastAccessTime;
                    fileModel.FileSizeText = (f.Length < 1024) ? f.Length.ToString() + " B" : f.Length / 1024 + " KB";

                    fileListModel.Add(fileModel);
                }
            }

            return fileListModel;
        }

    }
}