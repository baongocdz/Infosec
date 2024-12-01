using DragonAcc.Service.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly DatabaseBackupService _backupService;

    public DatabaseController(DatabaseBackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpPost("backup")]
    public IActionResult BackupDatabase()
    {
        try
        {
            _backupService.BackupDatabase();
            return Ok("Database backup completed successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error during backup: {ex.Message}");
        }
    }
}
