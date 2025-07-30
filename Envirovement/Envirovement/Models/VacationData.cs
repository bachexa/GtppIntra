namespace Envirovement.Models
{
    public class VacationData
    {
        public string? EmployeeName { get; set; }
        public int DaysAvailable { get; set; }
        public int DaysTaken { get; set; }
        public int RemainingDays { get; set; }
        public List<int>? MonthlyVacationDays { get; set; } 
        public List<string>? CommentsPerDay { get; set; } 
    }
}
