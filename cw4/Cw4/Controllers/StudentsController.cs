using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw4.DAL;
using Cw4.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw4.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudent()
        {
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=s16703;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                List<Student> _students = new List<Student>();

                command.Connection = connection;
                command.CommandText =
                    "SELECT s.FirstName, s.LastName, s.BirthDate, ss.Name, e.Semester " +
                    " FROM Student s " +
                    " JOIN Enrollment e ON e.IdEnrollment = s.IdEnrollment " +
                    " JOIN Studies ss ON ss.IdStudy = e.IdStudy ";

                connection.Open();
                var executeReader = command.ExecuteReader();
                while (executeReader.Read())
                {
                    var student = new Student();
                    student.FirstName = executeReader["FirstName"].ToString();
                    student.LastName = executeReader["LastName"].ToString();
                    student.BirthDate = DateTime.Parse(executeReader["BirthDate"].ToString());
                    student.StudyName = executeReader["Name"].ToString();
                    student.Semester = int.Parse(executeReader["Semester"].ToString());
                    _students.Add(student);
                }

                return Ok(_students);
            }
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=s16703;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                List<Enrollment> _enrollments = new List<Enrollment>();

                command.Connection = connection;
                command.CommandText =
                    "SELECT e.* FROM Student s " +
                    " JOIN Enrollment e ON e.IdEnrollment = s.IdEnrollment " +
                    " WHERE s.IndexNumber = @indexNumber ";
                command.Parameters.AddWithValue("indexNumber", indexNumber);

                connection.Open();
                var executeReader = command.ExecuteReader();
                while (executeReader.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment.IdEnrollment = int.Parse(executeReader["IdEnrollment"].ToString());
                    enrollment.Semester = int.Parse(executeReader["Semester"].ToString());
                    enrollment.IdStudy = int.Parse(executeReader["IdStudy"].ToString());
                    enrollment.StartDate = DateTime.Parse(executeReader["StartDate"].ToString());
                    _enrollments.Add(enrollment);
                }
                if (_enrollments.Count > 0)
                {
                    return Ok(_enrollments);
                }
                else
                {
                    return NotFound("Nie znaleziono studenta");
                }
            }

        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            if (id == 1 || id == 2)
            {
                return Ok("Aktualizacja dokończona");
            }
            return NotFound("Nie znaleziono studenta");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            if (id == 1 || id == 2)
            {
                return Ok("Usuwanie dokończone");
            }
            return NotFound("Nie znaleziono studenta");
        }
    }
}