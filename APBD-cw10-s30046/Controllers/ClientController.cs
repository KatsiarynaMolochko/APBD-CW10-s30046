using APBD_cw10_s30046.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_cw10_s30046.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController(IDbService service) : ControllerBase
{
    
}