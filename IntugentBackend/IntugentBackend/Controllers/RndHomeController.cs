using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Services.Core;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndHomeController : ControllerBase
    {
        private readonly ObjectsService _objectsService;
        public RndHomeController(ObjectsService objectsService) => _objectsService = objectsService;

        [HttpPost("search")]
        public IActionResult SearchDataSets([FromBody] SearchCriteria criteria)
        {
            // 1. Initialize schema
            _objectsService.CLists.InitializeEmployeeTableSchema();

            // 2. Initialize row safely
            if (_objectsService.CLists.drEmployee == null || _objectsService.CLists.drEmployee.Table == null)
            {
                _objectsService.CLists.drEmployee = _objectsService.CLists.dtEmployees.NewRow();
                _objectsService.CLists.dtEmployees.Rows.Add(_objectsService.CLists.drEmployee);
            }

            // 3. Assign values now that columns are guaranteed to exist
            _objectsService.CLists.drEmployee["RndDate1"] = (object)criteria.Date1 ?? DBNull.Value;
            _objectsService.CLists.drEmployee["RNDNameSearch"] = (object)criteria.Name ?? DBNull.Value;

            return Ok(new { message = "Search executed successfully" });
        }
    }

    public class SearchCriteria
    {
        public DateTime? Date1 { get; set; }
        public string? Name { get; set; }
    }
}