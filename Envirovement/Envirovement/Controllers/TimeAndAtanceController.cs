using ConsoleApp1;
using Envirovement.Models;

using System.Diagnostics;
using System.Text;
using Envirovement.Clases;
using Envirovement.Database;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Bibliography;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using DocumentFormat.OpenXml.Presentation;
using System.Drawing.Drawing2D;


namespace Envirovement.Controllers
{
    public class TimeAndAtanceController : Controller
    {
        private SqlParther _dbhelper;

        public TimeAndAtanceController()
        {
            _dbhelper = new SqlParther();
        }

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetInOutReport(string[] team, string dateOne, string dateTwo) 
        {
            var allReps = new List<List<TAIndividualReport>>();

            for (int i = 0; i < team.Length; i++)
            {
                var splitedTeam = team[i].Split('|');
                var startDate = Convert.ToDateTime(dateOne);
                var endDate = Convert.ToDateTime(dateTwo);
                var rep = _dbhelper.GetUnitReport(splitedTeam[0], dateOne, dateTwo);
                var translated = TranslateNamesAndPositionsToGeorgian(rep);
                translated[0].Unit = splitedTeam[1];
                allReps.Add(translated);
            }
            ProcessColors(allReps);
            return Json(allReps);
        }

        

        public JsonResult GetTARepprt(string team, string dateOne, string dateTwo)
        {
            var vacantionData = _dbhelper.ExtractVacationData();
            var trimedVacantionData = TrimAduitionaSumbols(vacantionData);
            var dateParthersedVacantionData = ProcessComments(trimedVacantionData);

            var splitedTeam = team.Split('|');
            var startDate = Convert.ToDateTime(dateOne);
            var endDate = Convert.ToDateTime(dateTwo);

            if (splitedTeam[0] == "19")
            {
                var tmprep = _dbhelper.GetShiftReport(splitedTeam[0], dateOne, dateTwo);
                _dbhelper.ImportSeq(tmprep);
            }
            if (splitedTeam[0] == "42" || splitedTeam[0] == "44" || splitedTeam[0] == "45")
            {
                var tmprep = _dbhelper.GetShiftReport(splitedTeam[0], dateOne, dateTwo);
                AdjustShiftReport(tmprep);

                _dbhelper.ImportOperaciuliUnit(tmprep);
            }


            var rep = _dbhelper.GetUnitReport(splitedTeam[0], dateOne, dateTwo);
            if (splitedTeam[0] == "42" || splitedTeam[0] == "44" || splitedTeam[0] == "45")
            {
                AdjustTAIndividualReport(rep);
            }


            var groupAndFillReports = GroupAndFillReports(rep, startDate, endDate);
            var cHangeAdtionalSymbols = CHangeAdtionalSymbols(groupAndFillReports);

            var groupAndFillReportsWithWeekEnd = FillWeekend(cHangeAdtionalSymbols);

            

            var groupedDataWithFullHours = new List<TAReportGrouped>();

            if (splitedTeam[0] == "19")
            {
                groupedDataWithFullHours = GroupedDataWithFull24HoursShift(groupAndFillReportsWithWeekEnd);
            }
            else if (splitedTeam[0] == "42" || splitedTeam[0] == "44" || splitedTeam[0] == "45")
            {
                groupedDataWithFullHours = GroupedDataWithFull12HoursShift(groupAndFillReportsWithWeekEnd);
            }
            else
            {
                groupedDataWithFullHours = GroupedDataWithFullHours(groupAndFillReportsWithWeekEnd);
            }
            var translateNamesAndPositionsToGeorgian = TranslateNamesAndPositionsToGeorgian(groupedDataWithFullHours);
            translateNamesAndPositionsToGeorgian[0].DateTo = dateTwo.Replace('-', '.');
            translateNamesAndPositionsToGeorgian[0].Datefrom = dateOne.Replace('-', '.');
            translateNamesAndPositionsToGeorgian[0].Unit = splitedTeam[1];
            translateNamesAndPositionsToGeorgian[0].UnitPosition = splitedTeam[2];
            translateNamesAndPositionsToGeorgian[0].CompilationDate = DateTime.Now.ToString();
            var datawithCariDes = _dbhelper.GetCartIdes(splitedTeam[0], dateOne, dateTwo);

            if (translateNamesAndPositionsToGeorgian.Count == datawithCariDes.Count)
            {
                for (int i = 0; i < translateNamesAndPositionsToGeorgian.Count; i++)
                {
                    translateNamesAndPositionsToGeorgian[i].TabelNumber = datawithCariDes[i].CardNo.ToString() + "-" + datawithCariDes[i].CustomNo.ToString();
                }
            }
            CompareLists(translateNamesAndPositionsToGeorgian, dateParthersedVacantionData);
            //if (splitedTeam[0] == "62")
            //{
            //    Fillempydayes(translateNamesAndPositionsToGeorgian);
            //}
            return Json(translateNamesAndPositionsToGeorgian);
        }


        public void AdjustTAIndividualReport(List<TAIndividualReport> report)
        {
            if (report == null || report.Count < 4)
                return;

            var grouped = report.GroupBy(x => x.Employee).ToList();
            var result = new List<TAIndividualReport>();

            foreach (var group in grouped)
            {
                var records = group.OrderBy(x => x.Date).ToList();

                for (int i = 0; i < records.Count; i++)
                {
                    var current = records[i];

                    if (TimeSpan.TryParse(current.InTime, out var inTimeSpan) &&
                        inTimeSpan >= TimeSpan.FromHours(19) && inTimeSpan < TimeSpan.FromHours(24))
                    {
                        if (i + 3 < records.Count)
                        {
                            var day1 = records[i];
                            var day2 = records[i + 1];
                            var day3 = records[i + 2];
                            var day4 = records[i + 3];

                            var modifiedDay3 = new TAIndividualReport
                            {
                                Employee = day1.Employee,
                                Position = day1.Position,
                                InTime = "20:00",
                                OutTime = day1.OutTime,
                                Date = day3.Date,
                                fullInt = 240,
                                CardNo = day1.CardNo,
                                CustomNo = day1.CustomNo,
                                Unit = day1.Unit,
                                InColor = day1.InColor,
                                OutColor = day1.OutColor
                            };

                            var movedDay4 = new TAIndividualReport
                            {
                                Employee = day3.Employee,
                                Position = day3.Position,
                                InTime = day3.InTime,
                                OutTime = day3.OutTime,
                                Date = day4.Date,
                                fullInt = day3.fullInt,
                                CardNo = day3.CardNo,
                                CustomNo = day3.CustomNo,
                                Unit = day3.Unit,
                                InColor = day3.InColor,
                                OutColor = day3.OutColor
                            };

                            result.Add(day1);         // Keep day 1
                            result.Add(day2);         // Keep day 2
                            result.Add(modifiedDay3); // Modified day 3
                            result.Add(movedDay4);    // Moved day 3 to day 4

                            i += 3; // Skip the next 3 since they're already processed
                            continue;
                        }
                    }

                    // If not matched, keep the original entry
                    result.Add(current);
                }
            }

            report.Clear();
            report.AddRange(result);
        }


        public void AdjustShiftReport(List<ShiftAttendance> shiftReport)
        {
            if (shiftReport == null || shiftReport.Count < 4)
                return;

            var grouped = shiftReport.GroupBy(x => x.EmpID).ToList();
            var result = new List<ShiftAttendance>();

            foreach (var group in grouped)
            {
                var records = group.OrderBy(x => x.Date).ToList();

                for (int i = 0; i < records.Count; i++)
                {
                    var current = records[i];

                    if (TimeSpan.TryParse(current.InTime, out var inTimeSpan) &&
                        inTimeSpan >= TimeSpan.FromHours(19) && inTimeSpan < TimeSpan.FromHours(24))
                    {
                        if (i + 3 < records.Count)
                        {
                            var day1 = records[i];
                            var day2 = records[i + 1];
                            var day3 = records[i + 2];
                            var day4 = records[i + 3];

                            // Create modified day3: InTime = 20:00, OutTime = day1.OutTime
                            var modifiedDay3 = new ShiftAttendance
                            {
                                EmpID = day1.EmpID,
                                Employee = day1.Employee,
                                InTime = "20:00",
                                IntimeInt = 0,
                                OutTime = day1.OutTime,
                                OutTimeInt = day1.OutTimeInt,
                                Date = day3.Date,
                                DateInt = (int)((DateTimeOffset)day3.Date).ToUnixTimeSeconds(),
                                Fulltime = "20:00"
                            };

                            // Move original day3 to day4
                            var movedDay4 = new ShiftAttendance
                            {
                                EmpID = day3.EmpID,
                                Employee = day3.Employee,
                                InTime = day3.InTime,
                                IntimeInt = day3.IntimeInt,
                                OutTime = day3.OutTime,
                                OutTimeInt = day3.OutTimeInt,
                                Date = day4.Date,
                                DateInt = (int)((DateTimeOffset)day4.Date).ToUnixTimeSeconds(),
                                Fulltime = day3.Fulltime
                            };

                            result.Add(day1);         // Original day1
                            result.Add(day2);         // Original day2
                            result.Add(modifiedDay3); // Replaced day3
                            result.Add(movedDay4);    // Overwritten day4

                            i += 3;
                            continue;
                        }
                    }

                    result.Add(current); // No pattern matched, just keep
                }
            }

            // Replace original list contents
            shiftReport.Clear();
            shiftReport.AddRange(result);
        }


        private void Fillempydayes(List<TAReportGrouped> data) 
        {
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Data.Count; j++)
                {
                    if (data[i].Data[j].InTime == "" && (data[i].Data[j].InTime != "Saturday" || data[i].Data[j].InTime != "Saturday" || data[i].Data[j].InTime != "PublicHolliDay" || data[i].Data[j].Vacantion != "vacantion"))
                    {
                        data[i].Data[j].InTime = "10:00";
                        data[i].Data[j].OutTime = "17:00";
                    }
                }
            }
        }

        public void ProcessColors(List<List<TAIndividualReport>> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Count; j++)
                {
                    if (TimeSpan.TryParse(data[i][j].InTime, out TimeSpan inTimeSpan))
                    {
                        // Set InColor based on the InTime range
                        if (inTimeSpan >= TimeSpan.Parse("00:00") && inTimeSpan <= TimeSpan.Parse("00:00"))
                        {
                            data[i][j].InColor = "red";
                        }
                        else
                        if (inTimeSpan >= TimeSpan.Parse("07:00") && inTimeSpan <= TimeSpan.Parse("10:15"))
                        {
                            data[i][j].InColor = "green";
                        }
                        else if (inTimeSpan > TimeSpan.Parse("10:15") && inTimeSpan <= TimeSpan.Parse("16:00"))
                        {
                            data[i][j].InColor = "red";
                        }
                    }

                    if (TimeSpan.TryParse(data[i][j].OutTime, out TimeSpan outTimeSpan))
                    {
                        // Set OutColor based on the OutTime range
                        if (outTimeSpan >= TimeSpan.Parse("00:00") && outTimeSpan <= TimeSpan.Parse("00:00"))
                        {
                            data[i][j].OutColor = "red";
                        }
                        else
                        if (outTimeSpan >= TimeSpan.Parse("11:00") && outTimeSpan <= TimeSpan.Parse("16:54"))
                        {
                            data[i][j].OutColor = "red";
                        }
                        else
                        {
                            data[i][j].OutColor = "green";
                        }
                    }
                }
            }
        }

        public void CompareLists(List<TAReportGrouped> groupAndFillReportsWithWeekEnd, List<VacationData> vacantionData)
        {
            // Iterate through each report group
            foreach (var reportGroup in groupAndFillReportsWithWeekEnd)
            {
                // Clean the Employee name by removing leading digits, dots, and trimming
                var cleanedEmployee = new string(reportGroup.Employee.Where(c => !char.IsDigit(c) && c != '.').ToArray()).Trim();

                // Find the corresponding VacationData for the cleaned employee name
                var vacation = vacantionData.FirstOrDefault(v => v.EmployeeName == cleanedEmployee);

                // If no matching vacation data is found, skip this employee
                if (vacation == null) continue;

                // Iterate through each report data in the group
                foreach (var reportData in reportGroup.Data)
                {
                    // Check if there is a matching date in the CommentsPerDay for the employee
                    foreach (var comment in vacation.CommentsPerDay ?? new List<string>())
                    {
                        // Parse the string date in CommentsPerDay to DateTime for comparison
                        if (DateTime.TryParseExact(comment, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime vacationDate))
                        {
                            // Compare the vacationDate with the Date in TAIndividualReportData
                            if (reportData.Date.Date == vacationDate.Date)
                            {
                                // If both employee and date match, set Vacantion to "vacantion"
                                reportData.Vacantion = "vacantion";
                            }
                        }
                    }
                }
            }
        }

        private List<VacationData> ProcessComments(List<VacationData> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                // Ensure CommentsPerDay is not null
                if (data[i].CommentsPerDay == null)
                    continue;

                for (int j = 0; j < data[i].CommentsPerDay.Count; j++)
                {
                    string comment = data[i].CommentsPerDay[j];

                    // Normalize the separator to handle both " - " and "-"
                    comment = comment.Replace(" - ", "-").Replace("-", " - "); // Normalize both types to " - "

                    // Check if the comment contains a range (e.g., "03.09.2024 - 13.09.2024")
                    if (comment.Contains(" - "))
                    {
                        var dateRange = comment.Split(" - ");  // Now split on " - "

                        // Append .2024 if the year is missing
                        string currentYear = ".2024";
                        string startDateString = dateRange[0].Trim();  // Trim to remove any extra spaces
                        string endDateString = dateRange[1].Trim();  // Trim to remove any extra spaces

                        // Check if the dates already contain the year, if not, append the current year
                        if (!startDateString.Contains(".2024"))
                        {
                            startDateString += currentYear;
                        }
                        if (!endDateString.Contains(".2024"))
                        {
                            endDateString += currentYear;
                        }

                        // Parse the start and end dates safely
                        if (DateTime.TryParseExact(startDateString, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime startDate) &&
                            DateTime.TryParseExact(endDateString, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime endDate))
                        {
                            // Remove the original range from the list
                            data[i].CommentsPerDay.RemoveAt(j);

                            // Loop through each day in the range and insert it into the list, but skip weekends
                            while (startDate <= endDate)
                            {
                                // Check if the current date is not a weekend
                                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                                {
                                    data[i].CommentsPerDay.Insert(j, startDate.ToString("dd.MM.yyyy"));
                                    j++;  // Move to the next position for the next date
                                }
                                startDate = startDate.AddDays(1);  // Move to the next day
                            }
                            // Move back j by 1 to avoid skipping any items in the next iteration
                            j--;
                        }
                        else
                        {
                            // Handle the case where date parsing fails (optional logging)
                            Console.WriteLine($"Failed to parse date range: {comment}");
                        }
                    }
                    else
                    {
                        // For single dates, ensure it's in the right format with ".2024" appended
                        if (!comment.Contains(".2024"))
                        {
                            data[i].CommentsPerDay[j] = comment + ".2024";
                        }
                    }
                }
            }

            return data;
        }





        private List<TAReportGrouped> TrimAditionalSymbols(List<TAReportGrouped> data) 
        {
            string pattern = @"\d{2}\s*";
            for (int i = 0; i < data.Count; i++)
            {
                data[i].Employee = Regex.Replace(data[i].Employee, pattern, "");
            }
            return data;
        }

        private List<TAReportGrouped> FillVacantionData(List<TAReportGrouped> dataWithWeekkEnds, List<VacationData> dataWithVacantions) 
        {
            var trimmedData = TrimAditionalSymbols(dataWithWeekkEnds);
            for (int i = 0; i < dataWithVacantions.Count; i++)
            {
                for (int j = 0; j < trimmedData.Count; j++)
                {
                    if (dataWithVacantions[i].EmployeeName == trimmedData[i].Employee)
                    {

                    }
                }
            }
            return dataWithWeekkEnds;
        }

        private List<VacationData> TrimAduitionaSumbols(List<VacationData> data)
        {
            // Adjusted pattern to handle single dates and ranges, with or without the year
            string pattern = @"(?<date1>\d{2}\.\d{2}(?:\.\d{4})?)(?:\s*-\s*(?<date2>\d{2}\.\d{2}(?:\.\d{4})?))?";

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].CommentsPerDay.Count; j++)
                {
                    var match = Regex.Match(data[i].CommentsPerDay[j], pattern);

                    if (match.Success)
                    {
                        // Extract the first and second date (if available)
                        var date1 = match.Groups["date1"].Value;
                        var date2 = match.Groups["date2"].Value;

                        // Convert longer date format to short (remove year if present)
                        date1 = Regex.Replace(date1, @"\.\d{4}", ""); // Remove year from the first date
                        date2 = Regex.Replace(date2, @"\.\d{4}", ""); // Remove year from the second date, if present

                        // If date2 exists, concatenate the two, otherwise just use date1
                        if (!string.IsNullOrEmpty(date2))
                        {
                            data[i].CommentsPerDay[j] = $"{date1} - {date2}";
                        }
                        else
                        {
                            data[i].CommentsPerDay[j] = date1;
                        }
                    }
                }
            }

            return data;
        }



        private List<TAReportGrouped> CHangeAdtionalSymbols(List<TAReportGrouped> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Data.Count; j++)
                {
                    if (data[i].Data[j].InTime == "0:0")
                    {
                        data[i].Data[j].InTime = string.Empty;
                    }
                    if (data[i].Data[j].OutTime == "0:0")
                    {
                        data[i].Data[j].OutTime = string.Empty;
                    }
                }
            }
            return data;
        }


        private List<TAReportGrouped> TranslateNamesAndPositionsToGeorgian(List<TAReportGrouped> data) 
        {
            for (int i = 0; i < data.Count; i++)
            {
                data[i].Employee = TranslateToGeorgian(data[i].Employee);
                data[i].Position = TranslateToGeorgian(data[i].Position);
            }
            return data;
        }

        private List<TAIndividualReport> TranslateNamesAndPositionsToGeorgian(List<TAIndividualReport> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                data[i].Employee = TranslateToGeorgian(data[i].Employee);
                data[i].Position = TranslateToGeorgian(data[i].Position);
            }
            return data;
        }

        public static string TranslateToGeorgian(string input)
        {
            var translationMap = new Dictionary<string, string>
            {
                {"a", "ა"}, {"b", "ბ"}, {"c", "ც"}, {"d", "დ"}, {"e", "ე"},
                {"f", "ფ"}, {"g", "გ"}, {"h", "ჰ"}, {"i", "ი"}, {"j", "ჯ"},
                {"k", "კ"}, {"l", "ლ"}, {"m", "მ"}, {"n", "ნ"}, {"o", "ო"},
                {"p", "პ"}, {"q", "ქ"}, {"r", "რ"}, {"s", "ს"}, {"t", "ტ"},
                {"u", "უ"}, {"v", "ვ"}, {"w", "წ"}, {"x", "ხ"}, {"y", "ყ"},
                {"z", "ზ"}, {"A", "ა"}, {"B", "ბ"}, {"C", "ჩ"}, {"D", "დ"},
                {"E", "ე"}, {"F", "ფ"}, {"G", "გ"}, {"H", "ჰ"}, {"I", "ი"},
                {"J", "ჟ"}, {"K", "კ"}, {"L", "ლ"}, {"M", "მ"}, {"N", "ნ"},
                {"O", "ო"}, {"P", "პ"}, {"Q", "ქ"}, {"R", "ღ"}, {"S", "შ"},
                {"T", "თ"}, {"U", "უ"}, {"V", "ვ"}, {"W", "ჭ"}, {"X", "ხ"},
                {"Y", "ყ"}, {"Z", "ძ"}
            };

            var translated = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                // Look ahead for two-character combinations like "gh", "sh", etc.
                if (i < input.Length - 1)
                {
                    string twoCharCombo = input[i].ToString() + input[i + 1];

                    if (translationMap.ContainsKey(twoCharCombo))
                    {
                        translated.Append(translationMap[twoCharCombo]);
                        i++; // Skip the next character since it was part of a two-character translation
                        continue;
                    }
                }

                // Single character translation (no lowercase conversion)
                string singleChar = input[i].ToString(); // Keep case-sensitive check

                if (translationMap.ContainsKey(singleChar))
                {
                    translated.Append(translationMap[singleChar]);
                }
                else
                {
                    // If the character isn't found in the map, append it as-is
                    translated.Append(singleChar);
                }
            }

            return translated.ToString();
        }


        private List<TAReportGrouped> GroupedDataWithFullHours(List<TAReportGrouped> data) 
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Fulltime > 0)
                {
                    data[i].FulltimeStr = ConvertIntDataTohours(data[i].Fulltime);
                    data[i].WorkingDayes = Math.Round((Convert.ToDouble(data[i].FulltimeStr) / 8), 1).ToString().Replace(',', '.');
                    data[i].FulltimeStr = data[i].FulltimeStr?.Replace(',', ':');
                }
            }
            return data;
        }

        private List<TAReportGrouped> GroupedDataWithFull24HoursShift(List<TAReportGrouped> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Fulltime > 0) 
                {
                    for (int j = 0; j < data[i].Data.Count; j++)
                    {
                        if (data[i].Data[j].OutTime == "24:0")
                        {
                            data[i].FulltimeStr = ConvertIntDataTohours(data[i].Fulltime);
                            data[i].WorkingDayes = Math.Round((Convert.ToDouble(data[i].FulltimeStr) / 24), 1).ToString().Replace(',', '.');
                            data[i].FulltimeStr = data[i].FulltimeStr?.Replace(',', ':');
                        }
                    }
                }
            }
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Fulltime > 0 && data[i].WorkingDayes == null)
                {
                    data[i].FulltimeStr = ConvertIntDataTohours(data[i].Fulltime);
                    data[i].WorkingDayes = Math.Round((Convert.ToDouble(data[i].FulltimeStr) / 8), 1).ToString().Replace(',', '.');
                    data[i].FulltimeStr = data[i].FulltimeStr?.Replace(',', ':');
                }
            }
            return data;
        }


        private List<TAReportGrouped> GroupedDataWithFull12HoursShift(List<TAReportGrouped> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Fulltime > 0)
                {
                    for (int j = 0; j < data[i].Data.Count; j++)
                    {
                        if (data[i].Data[j].OutTime == "24:0")
                        {
                            data[i].FulltimeStr = ConvertIntDataTohours(data[i].Fulltime);
                            data[i].WorkingDayes = Math.Round((Convert.ToDouble(data[i].FulltimeStr) / 12), 1).ToString().Replace(',', '.');
                            data[i].FulltimeStr = data[i].FulltimeStr?.Replace(',', ':');
                        }
                    }
                }
            }
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Fulltime > 0 && data[i].WorkingDayes == null)
                {
                    data[i].FulltimeStr = ConvertIntDataTohours(data[i].Fulltime);
                    data[i].WorkingDayes = Math.Round((Convert.ToDouble(data[i].FulltimeStr) / 8), 1).ToString().Replace(',', '.');
                    data[i].FulltimeStr = data[i].FulltimeStr?.Replace(',', ':');
                }
            }
            return data;
        }

        private string ConvertIntDataTohours(int totalMinutes) 
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;

            return hours.ToString() + "," + minutes.ToString();
        }


        public static Dictionary<string, DateTime> GetPublicHolidays(int year)
        {
            return new Dictionary<string, DateTime>
            {
                { "New Year", new DateTime(year, 1, 1) },
                { "New Year2", new DateTime(year, 1, 2) },
                { "New Year3", new DateTime(year, 1, 3) },
                { "New Year4", new DateTime(year, 1, 6) },
                { "Christmas", new DateTime(year, 1, 7) },
                { "New Year5", new DateTime(year, 1, 8) },
                { "New Year6", new DateTime(year, 1, 9) },
                { "New Year7", new DateTime(year, 1, 10) },
                { "NatlisGeba", new DateTime(year, 1, 19) },
                { "dedisdge", new DateTime(year, 3, 3) },
                { "qalTaDge", new DateTime(year, 3, 8) },
                { "cxraaprili", new DateTime(year, 4, 9) },
                { "Independence Day", new DateTime(year, 5, 26) },
                { "Mariamoba", new DateTime(year, 8, 28) },
                { "mcxetoba", new DateTime(year, 10, 14) },
                { "giorgoba", new DateTime(year, 11, 23) },
            };
        }

        private List<TAReportGrouped> FillWeekend(List<TAReportGrouped> groupedReports)
        {
            var publicHolidays = GetPublicHolidays(2025);

            for (int i = 0; i < groupedReports.Count; i++)
            {
                for (int j = 0; j < groupedReports[i].Data.Count; j++)
                {
                    var currentDate = groupedReports[i].Data[j].Date;
                    if (groupedReports[i].Data[j].InTime == string.Empty && groupedReports[i].Data[j].OutTime == string.Empty && groupedReports[i].Data[j].Date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        groupedReports[i].Data[j].InTime = "Saturday";
                        groupedReports[i].Data[j].OutTime = "Saturday";
                    }
                    if (groupedReports[i].Data[j].InTime == string.Empty && groupedReports[i].Data[j].OutTime == string.Empty && groupedReports[i].Data[j].Date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        groupedReports[i].Data[j].InTime = "Sunday";
                        groupedReports[i].Data[j].OutTime = "Sunday";
                    }
                    if (groupedReports[i].Data[j].InTime == string.Empty && groupedReports[i].Data[j].OutTime == string.Empty && publicHolidays.Values.Contains(currentDate.Date))
                    {
                        // Find the holiday name by date
                        var holidayName = publicHolidays.First(h => h.Value.Date == currentDate.Date).Key;
                        groupedReports[i].Data[j].InTime = "PublicHolliDay";
                        groupedReports[i].Data[j].OutTime = "PublicHolliDay";
                    }


                }
            }
            return groupedReports;
        }

        private List<TAReportGrouped> GroupAndFillReports(List<TAIndividualReport> reports, DateTime startDate, DateTime endDate)
        {
            // Group reports by Employee and Position
            var groupedReports = reports.GroupBy(r => new { r.Employee, r.Position/*,r.CardNo,r.CustomNo*/})
                                        .Select(g => new TAReportGrouped
                                        {
                                            Employee = g.Key.Employee,
                                            Position = g.Key.Position,
                                            //CardNo = g.Key.CardNo,
                                           // CustomNo = g.Key.CustomNo,
                                            Fulltime = g.Sum(x=>x.fullInt),
                                            Data = g.ToList().Select(r => new TAIndividualReportData
                                            {
                                                InTime = r.InTime,
                                                OutTime = r.OutTime,
                                                Date = r.Date,
                                                FullInt = r.fullInt
                                            }).ToList()
                                        }).ToList();

            // Fill in missing dates for each grouped report
            foreach (var group in groupedReports)
            {
                var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                         .Select(d => startDate.AddDays(d));

                foreach (var date in allDates)
                {
                    if (!group.Data.Any(d => d.Date.Date == date.Date))
                    {
                        group.Data.Add(new TAIndividualReportData
                        {
                            InTime = string.Empty,
                            OutTime = string.Empty,
                            Date = date,
                            FullInt = 0
                        });
                    }
                }

                // Sort the data by date after filling the missing ones
                group.Data = group.Data.OrderBy(d => d.Date).ToList();
            }

            return groupedReports;
        }


    }
}
