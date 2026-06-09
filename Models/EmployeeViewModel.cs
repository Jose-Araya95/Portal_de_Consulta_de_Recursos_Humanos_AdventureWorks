namespace Portal_de_Consulta_de_Recursos_Humanos_AdventureWorks.Models
{
    // This model holds the specific data we want to display in our view.
    // It maps to the columns we will select in our SQL query.
    public class EmployeeViewModel
    {
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string ShiftType { get; set; }
    }
}