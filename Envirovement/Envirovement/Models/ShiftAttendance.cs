using System.Data.SqlClient;

namespace Envirovement.Models
{
    public class ShiftAttendance
    {

        public int EmpID { get; set; }
        public string Employee { get; set; }
        public string InTime { get; set; }
        public int IntimeInt { get; set; }
        public string OutTime { get; set; }
        public int OutTimeInt { get; set; }
        public DateTime Date { get; set; }
        public int DateInt { get; set; }
        public string Fulltime { get; set; }

        public ShiftAttendance()
        {

        }

        public ShiftAttendance(SqlDataReader reader) : this()
        {
            EmpID = (int)reader[1];
            Employee = (string)reader[3];
            InTime = (string)reader[5];
            IntimeInt = (int)reader[6];
            OutTime = (string)reader[7];
            OutTimeInt = (int)reader[8];
            Date = (DateTime)reader[9];
            DateInt = (int)reader[10];
            Fulltime = (string)reader[11];
        }
    }
}
