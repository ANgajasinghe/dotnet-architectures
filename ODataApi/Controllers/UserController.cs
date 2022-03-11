using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

namespace ODataApi.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ODataController
{
    private readonly AppDbContext _appDbContext;

    public UserController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }   
    
    
    
    [EnableQuery(PageSize = 20)]
    [EnableCount]
    [HttpGet]
    public ActionResult GetOdata()
    {
        return Ok(_appDbContext.Users);
    }
    
}