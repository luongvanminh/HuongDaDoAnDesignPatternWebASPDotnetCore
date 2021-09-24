using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvcblog.Models;

namespace mvcblog.Controllers
{
    public class HomeController : ControllerTemplateMethod
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            PrintInformation();
        }

        public IActionResult Index()
        {
            
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

        protected override void PrintRoutes()
        {
            _logger.LogInformation($@"{GetType().Name}
                Routes:
                GET: Index
                GET: Privacy
                GET: Error
                ");
        }

        protected override void PrintDIs()
        {
            _logger.LogInformation($@"
                Dependencies:
                ILogger<CategoryController> _logger
                ");
        }
    }
}
