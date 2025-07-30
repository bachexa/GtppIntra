namespace Envirovement.Models
{
    public class EnvironmentModel
    {
        public int EnvironmentReportsID { get; set; }
        public double NoxDisplayValue { get; set; }
        public double CoDisplayValue { get; set; }
        public DateTime ReportRecordTime { get; set; }
        public bool Checkinternet { get; set; }
        public bool CheckConnection { get; set; }
    }
}
