using ConsoleApp1;
using Envirovement.Models;

using System.Diagnostics;
using System.Text;
using Envirovement.Clases;
using Envirovement.Database;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using System.IO;

namespace Envirovement.Controllers
{
    public class ReportsController: Controller
    {
        private SqlParther _dbhelper;
        public ReportsController()
        {
            _dbhelper = new SqlParther();
        }

        public  ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEnvirovementRep(int period, string dateOne, string dateTwo)
        {
            if (period == 1)
            {
                var res = _dbhelper.GetAveragesBy30MinuteIntervalByDate(dateOne, dateTwo);
                return Json(res);
            }
            else
            {
                var res = _dbhelper.GetAveragesBy30MinuteIntervalByDateExcesses(dateOne, dateTwo);
                return Json(res);
            }
        }


        public IActionResult GetEnvirovementRepExcelExport(int period, string dateOne, string dateTwo)
        {
            List<EnvirovementSentReport> reports;

            // Retrieve data based on the selected period
            switch (period)
            {
                case 1:
                    reports = _dbhelper.GetAveragesBy30MinuteIntervalByDate(dateOne, dateTwo);
                    break;
                case 2:
                    reports = _dbhelper.GetDataBy1MinuteIntervalByDate(dateOne, dateTwo);
                    break;
                case 3:
                    reports = _dbhelper.GetAveragesBy30MinuteIntervalByDateExcesses(dateOne, dateTwo);
                    break;
                case 4:
                    reports = _dbhelper.GetDataBy1MinuteIntervalByDateExcesses(dateOne, dateTwo);
                    break;
                default:
                    return BadRequest("Invalid period.");
            }

            // Generate the CSV data as a string
            string csvData = ExportToCSV(reports);
            byte[] csvDataBytes = Encoding.UTF8.GetBytes(csvData);

            // Add a Byte Order Mark (BOM) for UTF-8 encoding
            byte[] bom = Encoding.UTF8.GetPreamble();

            // Concatenate the BOM and CSV data
            byte[] csvWithBom = bom.Concat(csvDataBytes).ToArray();

            // Return the CSV data as a downloadable file with the appropriate content type and file name
            return File(csvWithBom, "text/csv; charset=utf-8", "Report.csv");
        }


        public string ExportToCSV(List<EnvirovementSentReport> reports)
        {
            StringBuilder csvBuilder = new StringBuilder();

            // Write CSV headers
            csvBuilder.AppendLine("TurbinaName,NoxValue,CoValue,RecordDate");

            // Write CSV data
            foreach (var report in reports)
            {
                string line = $"{report.TurbinaName},{report.NoxValue},{report.CoValue},{report.RecordDate:dd.MM.yyyy HH:mm:ss}";
                csvBuilder.AppendLine(line);
            }

            // Return the CSV data as a string
            return csvBuilder.ToString();
        }

    }
}
