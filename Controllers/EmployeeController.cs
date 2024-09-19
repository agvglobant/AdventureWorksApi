using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.DTOs;
using Newtonsoft.Json;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly string connectionString = "Server=myserver;Database=mydb;User Id=admin;Password=password;";


        public EmployeeController(ApiDbContext context)
        {
            _context = context;
        }
        
       [HttpGet]
        public async Task<IActionResult> GetEmployeeBlocking()
        {
            var employee = _context.Employees.FindAsync(1).Result; // Blocking async call
            return Ok(employee);
        }

        [HttpGet]
        public async Task<IActionResult> SlowGetEmployee()
        {
            Thread.Sleep(5000); // Blocking thread in async code
            var employee = await _context.Employees.FindAsync(1);
            return Ok(employee);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeNoDispose()
        {
            var context = new ApiDbContext(); // Manual context creation without disposal
            var employee = await context.Employees.FindAsync(1);
            return Ok(employee);
        }

        [HttpGet]
        public async Task<IActionResult> DoTheThing() // Bad practice: Unclear method name
        {
            var x = await _context.Employees.FindAsync(1); // Bad practice: Unclear variable name
            return Ok(x);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesWithSql()
        {
            var employees = await _context.Employees
                .FromSqlRaw("SELECT * FROM Employees WHERE CurrentFlag = 1") // Raw SQL in the controller
                .ToListAsync();
            return Ok(employees);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployeeWithoutValidation(Employee employee)
        {
            // No validation of incoming data
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetEmployee", new { id = employee.BusinessEntityID }, employee);
        }

        [HttpGet]
        public async Task<IActionResult> AsyncMethodWithoutAwait()
        {
            // async keyword without any await usage
            var employee = _context.Employees.Find(1);
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> InefficientDatabaseCalls()
        {
            var ids = new List<int> { 1, 2, 3, 4, 5 };
            List<Employee> employees = new List<Employee>();
        
            foreach (var id in ids)
            {
                var employee = await _context.Employees.FindAsync(id); // Bad practice: Multiple DB calls inside loop
                if (employee != null)
                {
                    employees.Add(employee);
                }
            }
        
            return Ok(employees);
        }

        [HttpGet]
        public async Task<IActionResult> ManualServiceCreation()
        {
            var service = new EmployeeService(); // Bad practice: Manual instantiation instead of DI
            var employees = service.GetEmployees();
            return Ok(employees);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeByName(string name)
        {
            var employees = await _context.Employees
                .FromSqlRaw($"SELECT * FROM Employees WHERE FirstName = '{name}'") // SQL injection risk
                .ToListAsync();
            return Ok(employees);
        }


        // GET: api/Employee
        [HttpGet]
        public async Task<ActionResult<string>> GetEmployees()
        {
            //return await _context.Employees.ToListAsync();

            using(_context)
            {
                var query = from p in _context.People 
                            join e in _context.Employees on p.BusinessEntityID equals e.BusinessEntityID
                            select new dtoEmployee
                            {
                                BusinessEntityID = e.BusinessEntityID,
                                NationalIDNumber = e.NationalIDNumber,
                                LoginID = e.LoginID,
                                OrganizationNode = e.OrganizationNode.ToString(),
                                OrganizationLevel = e.OrganizationLevel.ToString(),
                                JobTitle = e.JobTitle,
                                BirthDate = e.BirthDate,
                                MaritalStatus = e.MaritalStatus,
                                Gender = e.Gender,
                                HireDate = e.HireDate,
                                SalariedFlag = e.SalariedFlag,
                                VacationHours = e.VacationHours,
                                SickLeaveHours = e.SickLeaveHours,
                                CurrentFlag = e.CurrentFlag,
                                ModifiedDateEmployee = e.ModifiedDate,
                                Person = new dtoPerson{
                                    PersonType = p.PersonType,
                                    NameStyle = p.NameStyle,
                                    Title = p.Title,
                                    FirstName = p.FirstName, 
                                    MiddleName = p.MiddleName,
                                    LastName = p.LastName,
                                    Suffix = p.Suffix,
                                    EmailPromotion = p.EmailPromotion,
                                    AdditionalContactInfo = p.AdditionalContactInfo,
                                    Demographics = p.Demographics,
                                    ModifiedDatePerson = p.ModifiedDate
                                }
                            };
                var employee = await query.ToListAsync();

                if (employee == null)
                {
                    return null;
                }

                return JsonConvert.SerializeObject(employee);

            }
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetEmployee(int id)
        {
            using(_context)
            {
                var query = from p in _context.People 
                            join e in _context.Employees on p.BusinessEntityID equals e.BusinessEntityID
                            where p.BusinessEntityID == id
                            select new dtoEmployee
                            {
                                BusinessEntityID = e.BusinessEntityID,
                                NationalIDNumber = e.NationalIDNumber,
                                LoginID = e.LoginID,
                                OrganizationNode = e.OrganizationNode.ToString(),
                                OrganizationLevel = e.OrganizationLevel.ToString(),
                                JobTitle = e.JobTitle,
                                BirthDate = e.BirthDate,
                                MaritalStatus = e.MaritalStatus,
                                Gender = e.Gender,
                                HireDate = e.HireDate,
                                SalariedFlag = e.SalariedFlag,
                                VacationHours = e.VacationHours,
                                SickLeaveHours = e.SickLeaveHours,
                                CurrentFlag = e.CurrentFlag,
                                ModifiedDateEmployee = e.ModifiedDate,
                                Person = new dtoPerson{
                                    PersonType = p.PersonType,
                                    NameStyle = p.NameStyle,
                                    Title = p.Title,
                                    FirstName = p.FirstName, 
                                    MiddleName = p.MiddleName,
                                    LastName = p.LastName,
                                    Suffix = p.Suffix,
                                    EmailPromotion = p.EmailPromotion,
                                    AdditionalContactInfo = p.AdditionalContactInfo,
                                    Demographics = p.Demographics,
                                    ModifiedDatePerson = p.ModifiedDate
                                }
                            };
                var employee = await query.SingleOrDefaultAsync();

                if (employee == null)
                {
                    return null;
                }

                return JsonConvert.SerializeObject(employee);
            }
        }

        // PUT: api/Employee/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.BusinessEntityID)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employee
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeExists(employee.BusinessEntityID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployee", new { id = employee.BusinessEntityID }, employee);
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            try
            {
                return _context.Employees.Any(e => e.BusinessEntityID == id);
            }
            catch
            {
                return false;
            }
        }
    }
}
