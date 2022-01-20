using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using ViewFolders.Models;

namespace ViewFolders.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration Configuration;

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            string testPath = Configuration["ViewFolderSettings:FolderViewTestpath"];
            FolderViewer newViewer = new(testPath, Configuration["ViewFolderSettings:DocumentRootPath"]);
            return View(newViewer);
        }

        public string UpdateFolderDisplay(string rootPath, string expandedDirectories)
        {
            FolderViewer newViewer = new(rootPath, Configuration["ViewFolderSettings:DocumentRootPath"], expandedDirectories);
            return newViewer.Display();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}