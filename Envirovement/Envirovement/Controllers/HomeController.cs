using Microsoft.AspNetCore.Hosting; // Use this for IWebHostEnvironment
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Envirovement.Clases;
using Envirovement.Database;
using Envirovement.Models;
using System.Diagnostics;

namespace Envirovement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly SqlParther _dbHelper;
        private readonly ILoggerFactory _loggerFactory;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, SqlParther dbHelper, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _environment = environment;
            _dbHelper = dbHelper;
            _loggerFactory = loggerFactory;
        }

        public async Task<IActionResult> Index()
        {
            var helperLogger = _loggerFactory.CreateLogger<Helper>();
            var helper = new Helper(_environment, helperLogger);
            var EnvirovementSentReport = helper.GetReportedData();
            var env = _dbHelper.GetAveragesBy30MinuteInterval();
            EnvirovementSentReport[0].thertyminutemodel = env;

            return View(EnvirovementSentReport);
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
    }
}
