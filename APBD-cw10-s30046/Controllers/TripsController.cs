using APBD_cw10_s30046.DTO;
using APBD_cw10_s30046.Exceptions;
using APBD_cw10_s30046.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_cw10_s30046.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController(IDbService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int? pageNum, [FromQuery] int? pageSize)
    {
        if (pageNum == null && pageSize == null)
        {
            var allTrips = await service.GetAllTripsAsync(); 
            return Ok(allTrips);
        }
        int finalPage = pageNum ?? 1;
        int finalPageSize = pageSize ?? 10;

        
        if (finalPageSize <= 0)
        {
            return BadRequest("pageSize must be greater than 0");
        }

        var result = await service.GetPagedTripsAsync(finalPage, finalPageSize);
        return Ok(result);
    }
    
    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] ClientTripRequestDto dto)
    {
        try
        {
            await service.AddClientToTripAsync(idTrip, dto);
            return Ok("Client added to trip.");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}