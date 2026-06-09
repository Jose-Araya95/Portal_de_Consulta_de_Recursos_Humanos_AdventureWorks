using Microsoft.Data.SqlClient;
using Portal_de_Consulta_de_Recursos_Humanos_AdventureWorks.Models;
using System.Data;
using System.Text;

namespace Portal_de_Consulta_de_Recursos_Humanos_AdventureWorks.DAL
{
    public class EmployeeDAL
    {
        // Store the connection string securely
        private readonly string _connectionString;

        // Constructor injects the connection string so the DAL knows where to connect
        public EmployeeDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<EmployeeViewModel> GetEmployees(string name, int? departmentId, string jobTitle, int? shiftId, bool onlyActive)
        {
            var employees = new List<EmployeeViewModel>();

            // 1. Define the base query using a StringBuilder so we can dynamically append to it.
            // Notice the WHERE 1=1 trick. This is always true and makes it mathematically easy 
            // to append multiple "AND" conditions below without checking if it's the first condition.
            StringBuilder query = new StringBuilder(@"
                SELECT 
                    p.FirstName + ' ' + p.LastName AS FullName,
                    e.JobTitle,
                    d.Name AS Department,
                    s.Name AS ShiftType
                FROM HumanResources.Employee e
                INNER JOIN Person.Person p ON e.BusinessEntityID = p.BusinessEntityID
                INNER JOIN HumanResources.EmployeeDepartmentHistory edh ON e.BusinessEntityID = edh.BusinessEntityID
                INNER JOIN HumanResources.Department d ON edh.DepartmentID = d.DepartmentID
                INNER JOIN HumanResources.Shift s ON edh.ShiftID = s.ShiftID
                WHERE 1=1 ");

            // Create a list to hold our SQL parameters to prevent SQL injection
            List<SqlParameter> parameters = new List<SqlParameter>();

            // 2. Dynamically build the WHERE clause based on the provided filters

            if (!string.IsNullOrEmpty(name))
            {
                // Partial search using LIKE
                query.Append(" AND (p.FirstName + ' ' + p.LastName LIKE '%' + @Name + '%') ");
                parameters.Add(new SqlParameter("@Name", name));
            }

            if (departmentId.HasValue)
            {
                query.Append(" AND d.DepartmentID = @DepartmentId ");
                parameters.Add(new SqlParameter("@DepartmentId", departmentId.Value));
            }

            if (!string.IsNullOrEmpty(jobTitle))
            {
                query.Append(" AND e.JobTitle = @JobTitle ");
                parameters.Add(new SqlParameter("@JobTitle", jobTitle));
            }

            if (shiftId.HasValue)
            {
                query.Append(" AND s.ShiftID = @ShiftId ");
                parameters.Add(new SqlParameter("@ShiftId", shiftId.Value));
            }

            if (onlyActive)
            {
                // In AdventureWorks, an employee is active in their current department/shift 
                // if the EndDate in the history table is NULL.
                query.Append(" AND edh.EndDate IS NULL ");
            }

            // 3. Execute the ADO.NET logic
            // The 'using' blocks ensure that connections and commands are properly closed 
            // and disposed of, even if an error occurs.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query.ToString(), connection))
                {
                    // Attach all dynamically generated parameters to the command
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();

                    // ExecuteReader is used when we expect multiple rows to be returned
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Read() advances to the next row until there are no more rows
                        while (reader.Read())
                        {
                            // Map the database columns to our ViewModel properties
                            var employee = new EmployeeViewModel
                            {
                                FullName = reader["FullName"].ToString(),
                                JobTitle = reader["JobTitle"].ToString(),
                                Department = reader["Department"].ToString(),
                                ShiftType = reader["ShiftType"].ToString()
                            };

                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
        }
    }
}