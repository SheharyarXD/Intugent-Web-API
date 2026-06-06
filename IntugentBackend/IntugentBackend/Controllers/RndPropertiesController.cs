using IntugentBackend;
using IntugentBackend.Controllers.Rnd;
using IntugentBackend.Services.Rnd;
using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Services.Rnd;
[ApiController]
[Route("api/[controller]")]
public class RndPropertiesController : ControllerBase
{
    private readonly ObjectsService _os;
    private readonly RndPropertiesService _svc;

    public RndPropertiesController(ObjectsService os, RndPropertiesService svc)
    {
        _os = os;
        _svc = svc;
    }

    [HttpPost("update-reaction")]
    public IActionResult UpdateReaction([FromBody] TdrvUpdateModel model)
    {
        // 1. Always initialize
        _os.RNDHome.GetDataSet(1);

        // 2. Validate inputs (Path A)
        if (!int.TryParse(model.RowId, out int irow) || !int.TryParse(model.ColId, out int icol))
        {
            return BadRequest("Invalid numeric format.");
        }

        // 3. Logic and Bounds Checking (Path B)
        if (icol <= 0 || irow > 9)
        {
            return BadRequest("Out of range.");
        }

        // ... your logic here ...

        // 4. Final Success Path (Path C)
        _os.RNDHome.UpdateFormulatiions();
        return Ok(new { success = true });
    }
    [HttpPost("update-notes")]
    public IActionResult UpdateNotes([FromBody] TdrvUpdateModel model)
    {
        // 1. INITIALIZATION: Always ensure data is loaded for the current user
        _os.RNDHome.GetDataSet(1);

        // 2. DEFENSIVE VALIDATION: Check numeric format FIRST
        if (!int.TryParse(model.RowId, out int irow))
        {
            return BadRequest("RowId must be a valid number.");
        }

        // 3. BOUNDS CHECK: Ensure the row actually exists before trying to access it
        if (irow < 0 || irow >= _os.RNDHome.dtF.Rows.Count)
        {
            return BadRequest($"Invalid row index. Rows available: {_os.RNDHome.dtF.Rows.Count}");
        }

        // 4. LOGIC: Now it is safe to proceed
        _os.RNDHome.dtF.Rows[irow]["sNote"] = model.Text;
        _os.RNDHome.UpdateFormulatiions();

        return Ok(new { success = true });
    }
}