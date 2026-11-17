using Aplicacion.Menu;
using Dominio.Modelos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Presentacion.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]   
    public class MenuController : ControllerBase
    {
        IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ByRol",Name = "/GetByRol")]
        public async Task<IActionResult> GetByRol([FromQuery] int rol)
        {
            var resultado = await _mediator.Send(new ObtenerMenuPorRolQuery(rol));
           return Ok(resultado);
        }

        [HttpGet(Name = "/GetAll")]
        public async Task<IActionResult> Get()
        {
            var resultado = await _mediator.Send(new ObtenerMenuQuery());
            return Ok(resultado);
        }
    }
}
