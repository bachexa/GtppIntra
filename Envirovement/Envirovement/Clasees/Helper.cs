using ConsoleApp1;
using Envirovement.Models;
using Envirovement.Database;
using FluentAssertions.Common;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Hosting; // Use this for IWebHostEnvironment
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Envirovement.Clases
{
    public class Helper
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<Helper> _logger;
        private readonly SqlParther _dbHelper;
        private readonly Mutex _fileMutex = new Mutex();

        public Helper(IWebHostEnvironment environment, ILogger<Helper> logger)
        {
            _environment = environment;
            _logger = logger;
            _dbHelper = new SqlParther();
        }

        private void SendToEmail(string messageText, string[] adresses)
        {
            // Office 365 SMTP server settings
            string smtpAddress = "smtp.office365.com";
            int portNumber = 587;
            bool enableSSL = true;

            // Office 365 account credentials
            string emailFrom = "envirovementreports@gardabanitpp.ge";
            string password = "6uq53SE*0*Lj";

            for (int i = 0; i < adresses.Length; i++)
            {
                var fromAddress = new MailAddress(emailFrom, "From Office 365");
                var toAddress = new MailAddress(adresses[i], "To Name");
                const string subject = "გაფრქვევების მონიტორინგის სისტემა";
                string body = messageText;

                var smtp = new SmtpClient
                {
                    Host = smtpAddress,
                    Port = portNumber,
                    EnableSsl = enableSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(emailFrom, password),
                    Timeout = 20000
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
        }


        public void GetInformationFromFactory(object state)
        {
            string sourceDirectory = @"\\192.168.8.91\imed";
            string destinationDirectory = Path.Combine(_environment.ContentRootPath, "HelperFiles", "imed");

            _logger.LogInformation($"Copying from {sourceDirectory} to {destinationDirectory}");

            try
            {
                CopyDirectory(sourceDirectory, destinationDirectory);
                _logger.LogInformation("Copying completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while copying the directory.");
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            var sourceDirectory = new DirectoryInfo(sourceDir);
            if (!sourceDirectory.Exists)
            {
                _logger.LogError($"Source directory '{sourceDir}' not found.");
                return;
            }

            Directory.CreateDirectory(destinationDir);

            foreach (var file in sourceDirectory.GetFiles())
            {
                string destinationFilePath = Path.Combine(destinationDir, file.Name);
                bool copySuccess = false;
                while (!copySuccess)
                {
                    try
                    {
                        file.CopyTo(destinationFilePath, true);
                        copySuccess = true;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }

            foreach (var subDir in sourceDirectory.GetDirectories())
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }





        

        private List<EnvironmentModel> GetAllFromText()
        {
            var environment = new List<EnvironmentModel>();
            var reportRecordTime = DateTime.Now;
            //string destPath = @"C:\inetpub\wwwroot\";

            //// Build paths using the environment content root path
            //string filePath2014 = Path.Combine(_environment.ContentRootPath, "HelperFiles", "imed", "IMED2014.txt");
            //string filePath2015 = Path.Combine(_environment.ContentRootPath, "HelperFiles", "imed", "IMED2015.txt");

            string sourrth = @"C:\Users\Administrator\Desktop\Reports\imed";
            // Build paths using the environment content root path
            string filePath2014 = Path.Combine(sourrth, "IMED2014.txt");
            string filePath2015 = Path.Combine(sourrth, "IMED2015.txt");

            var res2014 = GetAllFromTextFile(filePath2014, "2014", 15, 16);
            var res2015 = GetAllFromTextFile(filePath2015, "2015", 16, 17);

            foreach (var item in res2014)
            {
                environment.Add(new EnvironmentModel
                {
                    NoxDisplayValue = item.Key,
                    CoDisplayValue = item.Value,
                    ReportRecordTime = reportRecordTime
                });
            }

            foreach (var item in res2015)
            {
                environment.Add(new EnvironmentModel
                {
                    NoxDisplayValue = item.Key,
                    CoDisplayValue = item.Value,
                    ReportRecordTime = reportRecordTime
                });
            }
            return environment;
        }
        private Dictionary<double, double> GetAllFromTextFile(string path, string yearName, int noxParam, int coParam)
        {
            Dictionary<double, double> numberNames = new Dictionary<double, double>();
            var reader = new StreamReader(path);
            try
            {
                string[] stringSeparators = new string[] { yearName };
                string[] stringSeparators1 = new string[] { "1;" };
                string address2014 = path;
                reader.BaseStream.Position = 0;
                string[] text1 = reader.ReadToEnd().Split(stringSeparators, StringSplitOptions.None);
                var nox2014Arr = text1[noxParam].Split(stringSeparators1, StringSplitOptions.None)[0].Split(';');
                var co2014Arr = text1[coParam].Split(stringSeparators1, StringSplitOptions.None)[0].Split(';');
                var noxName2014Display = nox2014Arr[6].Trim(' ');
                var coName2014Display = co2014Arr[6].Trim(' ');
                double result1 = Convert.ToDouble(noxName2014Display, CultureInfo.InvariantCulture);
                double result2 = Convert.ToDouble(coName2014Display, CultureInfo.InvariantCulture);
                numberNames.Add(result1, result2);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reader.Close();
            }
            return numberNames;
        }

        bool connectionRestored = true;
        bool emailSent = false;

        public async Task<List<Envirovement.Models.GetDataByDate>> GetSentData(object state)
        {
            var adresses = new string[] { "b.nozadze@gardabanitpp.ge" };
            swaggerClient swagger = new swaggerClient(new HttpClient());
            Guid CompanyGUID = new Guid("00471ea0-3f5e-44c8-b998-c82260971a9b");
            Guid BuildingGUID = new Guid("353e6580-a1ba-4ad5-ab3e-2c5ec47203e9");
            Guid MonitoringPointGUID = new Guid("fb940c8e-7649-41a3-9205-0f42d1cc5981");
            Guid SensorGUID = new Guid("60953271-5307-4179-b798-b74806707120");

            List<Envirovement.Models.GetDataByDate> getDataList = new List<Envirovement.Models.GetDataByDate>();
            var sentData = await swagger.GetDataByDateAsync(CompanyGUID, BuildingGUID, MonitoringPointGUID, SensorGUID, DateTime.Now);
                    // Convert the data and add it to the list
                    foreach (var item in sentData)
                    {
                        getDataList.Add(new Envirovement.Models.GetDataByDate
                        {
                            AddGUID = item.AddGUID,
                            BuildingGUID = item.BuildingGUID,
                            CompanyGUID = item.CompanyGUID,
                            Data = item.Data,
                            DataDateTime = item.DataDateTime.ToString(),
                            WsLogID = item.WsLogID,
                            StatusId = item.StatusId
                        });
                    }
            return getDataList;
        }


        // Timer callback to periodically check connection status
        
        public async Task InsertDataexternalDatabaseAsync(object state,EnvironmentModel Turbina1, EnvironmentModel Turbina2)
        {
            swaggerClient swagger = new swaggerClient(new HttpClient());
                var requestData = new List<Request>();
               
                    requestData.Add(new Request
                    {
                        CompanyGUID = new Guid("00471ea0-3f5e-44c8-b998-c82260971a9b"),
                        BuildingGUID = new Guid("353e6580-a1ba-4ad5-ab3e-2c5ec47203e9"),
                        MonitoringPointGUID = new Guid("fb940c8e-7649-41a3-9205-0f42d1cc5981"),
                        SensorGUID = new Guid("60953271-5307-4179-b798-b74806707120"),
                        Data = Turbina1.NoxDisplayValue,
                        DataDateTime = Turbina1.ReportRecordTime
                    });
                    
                    requestData.Add(new Request
                    {
                        CompanyGUID = new Guid("00471ea0-3f5e-44c8-b998-c82260971a9b"),
                        BuildingGUID = new Guid("353e6580-a1ba-4ad5-ab3e-2c5ec47203e9"),
                        MonitoringPointGUID = new Guid("fb940c8e-7649-41a3-9205-0f42d1cc5981"),
                        SensorGUID = new Guid("4c447a67-7e4b-42eb-8f91-fa55f863a189"),
                        Data = Turbina1.CoDisplayValue,
                        DataDateTime = Turbina1.ReportRecordTime
                    });

                    requestData.Add(new Request
                    {
                        CompanyGUID = new Guid("00471ea0-3f5e-44c8-b998-c82260971a9b"),
                        BuildingGUID = new Guid("353e6580-a1ba-4ad5-ab3e-2c5ec47203e9"),
                        MonitoringPointGUID = new Guid("d942a9c4-2667-424f-88f9-9e58ea5a1c48"),
                        SensorGUID = new Guid("f09e849f-ed28-43a3-b8ca-2c7bfa93d11f"),
                        Data = Turbina2.NoxDisplayValue,
                        DataDateTime = Turbina2.ReportRecordTime
                    });
                    
                    requestData.Add(new Request
                    {
                        CompanyGUID = new Guid("00471ea0-3f5e-44c8-b998-c82260971a9b"),
                        BuildingGUID = new Guid("353e6580-a1ba-4ad5-ab3e-2c5ec47203e9"),
                        MonitoringPointGUID = new Guid("d942a9c4-2667-424f-88f9-9e58ea5a1c48"),
                        SensorGUID = new Guid("e6e65f32-a0b5-41db-bc1c-bd93297f5ec0"),
                        Data = Turbina2.CoDisplayValue,
                        DataDateTime = Turbina2.ReportRecordTime
                    });

                var b = await swagger.AddBulkDataAsync(requestData);
        }

        public void InsertDataexternalDatabaseAsyncCallback(object state)
        {
            var wan = PingHost("142.250.184.142");
           
                _fileMutex.WaitOne();
                try
                {
                    var report = GetAllFromText();
                    InsertSentReportTodatabase(report,true);

                    if (wan)
                    {
                         var k = InsertDataexternalDatabaseAsync(state, report[0], report[1]);
                    }

                    else if (!wan)
                    {
                     InsertSentReportTodatabase(report,false);
                    }
                }
                finally
                {
                    // Release the mutex after the operation is complete
                    _fileMutex.ReleaseMutex();
                }
        }


        private void InsertSentReportTodatabase(List<EnvironmentModel>report,bool wan )
        {
            var dbInsert = new List<EnvirovementSentReport>();
            for (int i = 0; i < report.Count; i++)
            {
                if (i < 1)
                {
                    dbInsert.Add(new EnvirovementSentReport()
                    {
                        TurbinaName = "ტურბინა N11",
                        NoxValue = report[i].NoxDisplayValue,
                        CoValue = report[i].CoDisplayValue,
                        RecordDate = report[i].ReportRecordTime
                    });
                }
                else
                {
                    dbInsert.Add(new EnvirovementSentReport()
                    {
                        TurbinaName = "ტურბინა N12",
                        NoxValue = report[i].NoxDisplayValue,
                        CoValue = report[i].CoDisplayValue,
                        RecordDate = report[i].ReportRecordTime
                    });
                }
            }
            if (wan)
            {
                var res = _dbHelper.InsertEnvirovementSentReport(dbInsert);
            }
            else 
            {
                var res = _dbHelper.InsertOfflineEnvirovementSentReport(dbInsert);
            }  
        }

        

        private bool problemDetected = false;
        private bool emailSentForProblem = false;
        private bool emailSentForProblemFixed = false;

        public void CheckReportAndSendMail(object state)
        {
            var adresses = new string[] { "b.nozadze@gardabanitpp.ge" };

            // Get the report from the database or wherever it is fetched from
            var report = _dbHelper.GetEnvirovementSentReportForMail();

            // Check if any problem exists in the report
            bool anyProblem = report.Any(entry => entry.NoxValue > 25 || entry.CoValue > 20);

            if (anyProblem && !problemDetected && !emailSentForProblem)
            {
                // If a problem is detected for the first time, send an email
                SendToEmail("გაფრქვევების მონაცემები ცდება დასაშვებ ძრვარს...", adresses);
                problemDetected = true;
                emailSentForProblem = true;
            }
            else if (!anyProblem && problemDetected && !emailSentForProblemFixed)
            {
                // If the problem is fixed for the first time, send an email
                SendToEmail("გაფრქვევების მონაცემები გამოსწორებულია", adresses);
                problemDetected = false;
                emailSentForProblemFixed = true;
                emailSentForProblem = false; // Reset emailSentForProblem flag
            }
        }



        public List<EnvirovementSentReport> GetReportedData()
        {
           var reportData = new List<EnvirovementSentReport>();
            var wan = PingHost("142.250.184.142");
            var lan = PingHost("192.168.8.91");
            if (wan && lan)
            {
                var report = _dbHelper.GetEnvirovementSentReport();
                report[0].CheckConnection = true;
                reportData = report;
            }
            else
            {
                var report = _dbHelper.GetEnvirovementSentReport();
                report[0].CheckConnection = false;
                reportData = report;
            }
            return reportData;


        }


        bool previousLanStatus = true; // Assuming LAN connection is initially true
        bool emailSentForCurrentStatusChange = false; // Flag to track whether an email has been sent for the current status change

        public void CheckLanConnection(object state)
        {
            var adresses = new string[] { "b.nozadze@gardabanitpp.ge" };
            var currentLanStatus = PingHost("192.168.8.91");

            if (currentLanStatus != previousLanStatus)
            {
                if (!emailSentForCurrentStatusChange) // Check if an email has not been sent for the current status change
                {
                    if (currentLanStatus)
                    {
                        // LAN connection is restored
                        SendToEmail("გაფრქვევების კომპიუტერთან კავშირი აღდგენილია", adresses);
                    }
                    else
                    {
                        // LAN connection is lost
                        SendToEmail("გაფრქვევების კომპიუტერთან კავშირი დაკარგულია", adresses);
                    }
                    // Update emailSentForCurrentStatusChange flag
                    emailSentForCurrentStatusChange = true;
                }
                // Update previous LAN status
                previousLanStatus = currentLanStatus;
            }
            else
            {
                // Reset the emailSentForCurrentStatusChange flag if the LAN status remains the same
                emailSentForCurrentStatusChange = false;
            }
        }

        public void CheckLanAndInsertDatabase(object state)
        {
            var currentLanStatus = PingHost("192.168.8.91");
            if (!currentLanStatus)
            {
                _dbHelper.InsertIntoLanDisconect("ReportCompWasDisconected");
            }
        }


        private bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;
            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            return pingable;
        }


        public void DeleOldRecords(object state)
        {
            var records = _dbHelper.GetAllEnvirovementSentReport();
            if (records.Count > 2064000)
            {
                _dbHelper.DeleteRecordsFromEnvirovementSentReport(1064000);
            }
        }
    }
}
