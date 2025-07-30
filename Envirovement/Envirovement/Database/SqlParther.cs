namespace Envirovement.Database
{
    using Envirovement.Models;
    using System.Collections.Generic;
    using MSSqlConnect;
    using System.Data;
    using System.Data.SqlClient;
    using DocumentFormat.OpenXml.Bibliography;
    using ClosedXML.Excel;
    using DocumentFormat.OpenXml.Drawing.Charts;

    public class SqlParther
    {
        private  string _connectionStringEnvr = @"Data Source=GTPPENVIROVEMEN\SQLEXPRESS;Initial Catalog=EnvirovementReports;User ID=bachexa;Password=Zatara./12!!";
        string _connectionStringBiostar = @"Data Source = tcp:192.168.11.154,1433; Initial Catalog = BioStar; User ID = bachexa; Password=Zatara./12!";//@"Data Source=GTPPENVIROVEMEN\SQLEXPRESS;Initial Catalog=BioStar;User ID=bachexa;Password=Zatara./12!!";

        //string _externalConnection = @"Data Source = tcp:192.168.11.154,1433; Initial Catalog = BioStar; User ID = bachexa; Password=Zatara./12!";


        public bool InsertEnvirovementSentReport(List<EnvirovementSentReport> report)
        {
            using (Database db = new Database(_connectionStringEnvr))
            {
                try
                {
                    foreach (var item in report)
                    {
                        db.ExecuteNonQuery("InsertEnvirovementSentReport", CommandType.StoredProcedure,
                        new SqlParameter("@TurbinaName", SqlDbType.NVarChar) { Value = item.TurbinaName },
                        new SqlParameter("@NoxValue", SqlDbType.Float) { Value = item.NoxValue },
                        new SqlParameter("@CoValue", SqlDbType.Float) { Value = item.CoValue },
                        new SqlParameter("@RecordDate", SqlDbType.DateTime) { Value = item.RecordDate }
                        );
                    }
                    //db.CommitTransaction();
                    return true;
                }
                catch (Exception)
                {
                    //db.RollbackTransaction();
                    return false;
                }
            }
        }

        public bool InsertOfflineEnvirovementSentReport(List<EnvirovementSentReport> report)
        {
            using (Database db = new Database(_connectionStringEnvr))
            {
                try
                {
                    foreach (var item in report)
                    {
                        db.ExecuteNonQuery("InsertOfflineEnvirovementSentReport", CommandType.StoredProcedure,
                        new SqlParameter("@TurbinaName", SqlDbType.NVarChar) { Value = item.TurbinaName },
                        new SqlParameter("@NoxValue", SqlDbType.Float) { Value = item.NoxValue },
                        new SqlParameter("@CoValue", SqlDbType.Float) { Value = item.CoValue },
                        new SqlParameter("@RecordDate", SqlDbType.DateTime) { Value = item.RecordDate }
                        );
                    }
                    //db.CommitTransaction();
                    return true;
                }
                catch (Exception)
                {
                    //db.RollbackTransaction();
                    return false;
                }
            }
        }
        public bool DeleteRecordsFromEnvirovementSentReport(int records)
        {
            using (Database db = new Database(_connectionStringEnvr))
            {
                try
                {
                    db.ExecuteNonQuery("DeleteRecordsFromEnvirovementSentReport", CommandType.StoredProcedure,
                    new SqlParameter("@NumRecordsToDelete", SqlDbType.Int) { Value = records }
                    );
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool InsertIntoLanDisconect(string alert)
        {
            using (Database db = new Database(_connectionStringEnvr))
            {
                try
                {
                        db.ExecuteNonQuery("InsertIntoLanDisconect", CommandType.StoredProcedure,
                        new SqlParameter("@Alert", SqlDbType.NVarChar) { Value =alert }
                        );
                    //db.CommitTransaction();
                    return true;
                }
                catch (Exception)
                {
                    //db.RollbackTransaction();
                    return false;
                }
            }
        }


        public bool TruncateOfflineEnvirovementSentReport()
        {
            using (Database db = new Database(_connectionStringEnvr))
            {
                try
                {
                    db.ExecuteNonQuery("InsertOfflineEnvirovementSentReport");
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public  List<EnvrTHertyMinuteModel> GetAveragesBy30MinuteInterval()
        {
            string commandText = @"select Top 100 * from GetAveragesBy30MinuteInterval() order by RecordDate DESC";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var averagesBy30MinuteInterval = db.SqlQuery<EnvrTHertyMinuteModel>(commandText).ToList();
                return averagesBy30MinuteInterval;
            }
        }

        public List<EnvirovementSentReport> GetAveragesBy30MinuteIntervalByDate(string startDate,string endDate)
        {
            string commandText = @" select * from GetAveragesBy30MinuteIntervalByDate(@startDate, @endDate) order by RecordDate DESC";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var averagesBy30MinuteIntervalByDate = db.SqlQuery<EnvirovementSentReport>(commandText,new SqlParameter("@startDate", startDate), new SqlParameter("@endDate", endDate)).ToList();
                return averagesBy30MinuteIntervalByDate;
            }
        }

        public List<VacationData> ExtractVacationData()
        {
            var filePath = @"C:\Users\Administrator\Desktop\Reports\შვებულების  კონტროლის ანალიზი  2024.xlsx";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified Excel file could not be found.", filePath);
            }
            var vacationDataList = new List<VacationData>();
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Assuming it's the first worksheet

                    // Assuming your data starts from row 2, and Employee name is in column 1
                    var rows = worksheet.RangeUsed().RowsUsed();
                    foreach (var row in rows.Skip(1)) // Skip the header row
                    {
                        var employeeName = row.Cell(1).GetValue<string>().Trim(); // Trim to remove extra spaces
                        if (string.IsNullOrEmpty(employeeName))
                        {
                            break; // Exit the loop if the cell is empty or null
                        }

                        var vacationData = new VacationData
                        {
                            EmployeeName = employeeName,
                            DaysAvailable = row.Cell(6).GetValue<int>(),
                            DaysTaken = row.Cell(7).GetValue<int>(),
                            RemainingDays = row.Cell(8).GetValue<int>(),
                            MonthlyVacationDays = new List<int>(),
                            CommentsPerDay = new List<string>()
                        };

                        // Loop through the cells starting from column 9 and stop when an empty cell is encountered
                        int columnIndex = 9;
                        while (!string.IsNullOrEmpty(row.Cell(columnIndex).GetValue<string>()))
                        {
                            // Get vacation days
                            int vacationDay;
                            if (int.TryParse(row.Cell(columnIndex).GetValue<string>(), out vacationDay))
                            {
                                vacationData.MonthlyVacationDays.Add(vacationDay);
                            }

                            // Get comments if available
                            string comment = row.Cell(columnIndex).GetComment()?.Text ?? string.Empty; // Fetch comment or use empty if none
                            vacationData.CommentsPerDay.Add(comment);

                            columnIndex++;
                        }

                        vacationDataList.Add(vacationData);
                    }
                   
                }

                return vacationDataList;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
           
        }


        public List<TAIndividualReport> GetUnitReport(string team, string startDate, string endDate)
        {
            string commandText = @"select 
                                   --UC.sCardNo as CardNo,
                                   --UC.sCustomNo as CustomNo,
                                   s.Employee,
                                   s.Position,
                                   s.InTime,
                                   s.OutTime,
                                   s.Date,
                                   s.FullTime,
                                   s.fullInt 
                                   from GetTemByTeamID(@team,-1,-1,@startDate,@endDate,0) as s 
                                   --inner join TB_USER_CARD as UC on UC.nUserIdn = s.ID
                                   order by s.Employee,s.Date";
            using (Database db = new Database(_connectionStringBiostar))
            {
                var unitReport = new List<TAIndividualReport>();
                try
                {
                    db.BeginTransaction();
                    unitReport = db.SqlQuery<TAIndividualReport>(commandText, new SqlParameter("@team", team), new SqlParameter("@startDate", startDate), new SqlParameter("@endDate", endDate)).ToList();
                    db.CommitTransaction();
                    return unitReport;  
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    return unitReport;
                }
            }
        }

        public List<ShiftAttendance> GetShiftReport(string team, string startDate, string endDate)
        {
            string commandText = @"select s.ID as EmpID, s.Employee,s.InTime,s.IntimeInt,s.OutTime,s.OutTimeInt,s.Date,s.DateInt,s.Fulltime 
                                   from dbo.BiostarGetAllDateRepb(default, default, default, @team, @team, @startDate, @endDate) as s order by s.Employee,s.Date";
            using (Database db = new Database(_connectionStringBiostar))
            {
                var unitReport = new List<ShiftAttendance>();
                try
                {
                    db.BeginTransaction();
                    unitReport = db.SqlQuery<ShiftAttendance>(commandText, new SqlParameter("@team", team), new SqlParameter("@startDate", startDate), new SqlParameter("@endDate", endDate)).ToList();
                    db.CommitTransaction();
                    return unitReport;
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    return unitReport;
                }
            }
        }


        public void ImportSeq( IEnumerable<ShiftAttendance> reports)
        {
            using (Database database = new(_connectionStringBiostar))
            {
                try
                {
                    database.BeginTransaction();
                    foreach (var report in reports)
                    {
                        if (report.IntimeInt != 0 && report.OutTimeInt == 0)
                        {
                            database.ExecuteNonQuery("BachoUpdateTaResult", CommandType.StoredProcedure,
                                new SqlParameter("@startiTime", SqlDbType.Int) { Value = 1440 },
                                new SqlParameter("@datetime", SqlDbType.Int) { Value = report.DateInt },
                                new SqlParameter("@userId", SqlDbType.Int) { Value = report.EmpID }
                            );
                        }
                    }
                    database.CommitTransaction();
                }
                catch (System.Exception)
                {
                    database.RollbackTransaction();
                }
            }
        }

        public void ImportOperaciuliUnit(IEnumerable<ShiftAttendance> reports)
        {
            using (Database database = new(_connectionStringBiostar))
            {
                try
                {
                    database.BeginTransaction();
                    foreach (var report in reports)
                    {
                        if (report.IntimeInt != 0 && report.OutTimeInt == 0)
                        {
                            database.ExecuteNonQuery("BachoUpdateTaResult", CommandType.StoredProcedure,
                                new SqlParameter("@startiTime", SqlDbType.Int) { Value = 1440 },
                                new SqlParameter("@datetime", SqlDbType.Int) { Value = report.DateInt },
                                new SqlParameter("@userId", SqlDbType.Int) { Value = report.EmpID }
                            );
                        }
                        if (report.IntimeInt != 0 && report.OutTimeInt > 0 && report.OutTimeInt < 500)
                        {
                            database.ExecuteNonQuery("BachoUpdateTaResultInTime", CommandType.StoredProcedure,
                                new SqlParameter("@startiTime", SqlDbType.Int) { Value = 0 },
                                new SqlParameter("@datetime", SqlDbType.Int) { Value = report.DateInt },
                                new SqlParameter("@userId", SqlDbType.Int) { Value = report.EmpID }
                            );
                        }

                    }
                    database.CommitTransaction();
                }
                catch (System.Exception)
                {
                    database.RollbackTransaction();
                }
            }
        }


        public List<TAIndividualReport> GetCartIdes(string team, string startDate, string endDate)
        {
            string commandText = @"WITH EmployeeData AS (
                                   SELECT 
                                   UC.sCardNo AS CardNo,
                                   UC.sCustomNo AS CustomNo,
                                   s.Employee,
                                   s.Position,
                                   s.InTime,
                                   s.OutTime,
                                   s.Date,
                                   s.FullTime,
                                   s.fullInt,
                                   ROW_NUMBER() OVER (PARTITION BY s.Employee ORDER BY s.Date ASC) AS RowNum
                                   FROM 
                                   GetTemByTeamID(@team, -1, -1, @startDate, @endDate, 0) AS s
                                   INNER JOIN 
                                   TB_USER_CARD AS UC ON UC.nUserIdn = s.ID
                                   )
                                   SELECT 
                                   CardNo, CustomNo, Employee, Position, InTime, OutTime, Date, FullTime, fullInt
                                   FROM 
                                   EmployeeData
                                   WHERE 
                                   RowNum = 1
                                   ORDER BY 
                                   Employee, Date;";
            using (Database db = new Database(_connectionStringBiostar))
            {
                var unitReport = new List<TAIndividualReport>();
                try
                {
                    db.BeginTransaction();
                    unitReport = db.SqlQuery<TAIndividualReport>(commandText, new SqlParameter("@team", team), new SqlParameter("@startDate", startDate), new SqlParameter("@endDate", endDate)).ToList();
                    db.CommitTransaction();
                    return unitReport;
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    return unitReport;
                }
            }
        }

        public List<EnvirovementSentReport> GetDataBy1MinuteIntervalByDate(string startDate, string endDate)
        {
            string commandText = @" select * from GetDataBy1MinuteIntervalByDate(@startDate, @endDate) order by RecordDate DESC";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var averagesBy30MinuteIntervalByDate = db.SqlQuery<EnvirovementSentReport>(commandText, new SqlParameter("@startDate", startDate), new SqlParameter("@endDate", endDate)).ToList();
                return averagesBy30MinuteIntervalByDate;
            }
        }

        public List<EnvirovementSentReport> GetAveragesBy30MinuteIntervalByDateExcesses(string startDate, string endDate)
        {
            string commandText = @" select * from GetAveragesBy30MinuteIntervalByDateExcesses(@startDate, @endDate) order by RecordDate DESC";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var averagesBy30MinuteIntervalByDate = db.SqlQuery<EnvirovementSentReport>(commandText, new SqlParameter("@startDate", startDate), new SqlParameter("@endDate", endDate)).ToList();
                return averagesBy30MinuteIntervalByDate;
            }
        }

        public List<EnvirovementSentReport> GetDataBy1MinuteIntervalByDateExcesses(string startDate, string endDate)
        {
            string commandText = @" select * from GetAveragesBy30MinuteIntervalByDateExcesses(@startDate, @endDate) order by RecordDate DESC";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var averagesBy1MinuteIntervalByDate = db.SqlQuery<EnvirovementSentReport>(commandText, new SqlParameter("@startDate", startDate), new SqlParameter("@endDate", endDate)).ToList();
                return averagesBy1MinuteIntervalByDate;
            }
        }

        public List<EnvirovementSentReport> GetEnvirovementSentReport()
        {
            string commandText = @"select 
                                   Top 100 
                                   EnvirovementSentReport.TurbinaName
                                   ,EnvirovementSentReport.NoxValue
                                   ,EnvirovementSentReport.CoValue
                                   ,EnvirovementSentReport.RecordDate 
                                    from EnvirovementSentReport 
                                   order by EnvirovementSentReport.RecordDate desc";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var competitionID = db.SqlQuery<EnvirovementSentReport>(commandText).ToList();
                return competitionID;
            }
        }

        public List<EnvirovementSentReport> GetAllEnvirovementSentReport()
        {
            string commandText = @"select 
                                   EnvirovementSentReport.TurbinaName
                                   ,EnvirovementSentReport.NoxValue
                                   ,EnvirovementSentReport.CoValue
                                   ,EnvirovementSentReport.RecordDate 
                                    from EnvirovementSentReport 
                                   order by EnvirovementSentReport.RecordDate asc";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var competitionID = db.SqlQuery<EnvirovementSentReport>(commandText).ToList();
                return competitionID;
            }
        }

        public List<EnvirovementSentReport> GetOfflineEnvirovementSentReport()
        {
            string commandText = @"select 
                                   OfflineEnvirovementSentReport.TurbinaName
                                   ,OfflineEnvirovementSentReport.NoxValue
                                   ,OfflineEnvirovementSentReport.CoValue
                                   ,OfflineEnvirovementSentReport.RecordDate 
                                   from OfflineEnvirovementSentReport 
                                   order by OfflineEnvirovementSentReport.RecordDate asc";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var competitionID = db.SqlQuery<EnvirovementSentReport>(commandText).ToList();
                return competitionID;
            }
        }

        public List<EnvirovementSentReport> GetEnvirovementSentReportForMail()
        {
            string commandText = @"select 
                                   Top 2 
                                   EnvirovementSentReport.TurbinaName
                                   ,EnvirovementSentReport.NoxValue
                                   ,EnvirovementSentReport.CoValue
                                   ,EnvirovementSentReport.RecordDate 
                                    from EnvirovementSentReport 
                                   order by EnvirovementSentReport.RecordDate desc";
            using (Database db = new Database(_connectionStringEnvr))
            {
                var competitionID = db.SqlQuery<EnvirovementSentReport>(commandText).ToList();
                return competitionID;
            }
        }
    }
}
