using APBD_cw10_s30046.Exceptions;
using APBD_cw10_s30046.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_cw10_s30046.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController(IDbService service) : ControllerBase
{
    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        try
        {
            await service.DeleteClientAsync(idClient);
            return NoContent(); 
        }
        catch (ClientHasTripsException ex)
        {
            return BadRequest(ex.Message); 
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message); 
        }
    }

}