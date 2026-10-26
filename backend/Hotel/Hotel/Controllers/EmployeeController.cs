using Hotel.API.Contracts;
using Hotel.Domain.Abstractions.Services.AdministratorServices;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/admin/employees/")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class EmployeeController : ControllerBase
    {
        private IEmployeeService _employeeService;
       
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        public async Task<List<EmployeeResponseDto>> GetEmployees()
        {
            List<Employee> employees = await _employeeService.GetEmployeesAsync();
            List<User> users= await _employeeService.GetUsersEmployeesAsync();
            return employees
                .Join(users,employee=>employee.UserId,user=>user.Id,(employee,user)=>(employee,user))
                .Select(employeeWithDetails => new EmployeeResponseDto(employeeWithDetails.employee.Id,employeeWithDetails.user.UserName,
                employeeWithDetails.employee.Name.ToString(), employeeWithDetails.employee.TelephoneNum.Value))
                .ToList();
            
            
        }
        [HttpPost("add-employee")]
        public async Task<ActionResult> CreateEmployee([FromBody] EmployeeRequestDto employee)
        {
            try
            {
                Enum.TryParse(employee.Role, out Roles role);
                await _employeeService.CreateEmployee(employee.UserName,employee.Password,employee.FirstName,employee.LastName
                               ,employee.Patronymic,employee.Phone,role);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
          

        }
        [HttpPatch("update-employee/{id:guid}")]
        public async Task<ActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeRequestDto employee)
        {
            try
            {
                Enum.TryParse(employee.Role, out Roles role);
                await _employeeService.UpdateEmployee(id,employee.UserName, employee.Password, employee.FirstName, employee.LastName
                               , employee.Patronymic, employee.Phone, role);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-employee/{id:guid}")]
        public async Task<ActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                await _employeeService.DeleteEmployee(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("name-availability")]
        public async Task<ActionResult<bool>> IsNameAvailable(string userName)
        {
           return await _employeeService.IsNameAvailable(userName);
        }

    }
}
