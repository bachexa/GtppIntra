

namespace Envirovement.Models
{
    public class TAReportGrouped
    {
        public string? Employee { get; set; }
        public string? Position { get; set; }

        public int CardNo { get; set; }

        public int CustomNo { get; set; }

        public string? TabelNumber { get; set; }

        public int Fulltime { get; set; }

        public string? FulltimeStr { get; set; }

        public string? WorkingDayes { get; set; }

        public string? DateTo { get; set; }

        public string? Datefrom { get; set; }

        public string? Unit { get; set; }
        public string? UnitPosition { get; set; }

        public string? CompilationDate { get; set; }

        public List<TAIndividualReportData> Data { get; set; } = new List<TAIndividualReportData>();
    }

    public class TAIndividualReportData
    {
        public string? InTime { get; set; }
        public string? OutTime { get; set; }
        public DateTime Date { get; set; }
        public int FullInt { get; set; }
        public string? Vacantion { get; set; }
    }
}
