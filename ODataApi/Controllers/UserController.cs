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


    [HttpPost("address")]
    public async Task<ActionResult> PostAddress()
    {
        await _appDbContext.ExecuteSqAsync("UserAddress_Add", new UserAddress()
        {
            No = "100",
            City = "张三",
            Country = "中国",
            Street = "河南路",
            ZipCode = "100000",
            State = "河南省",
            UserId = 101
        });
        
        return Ok();
    }

    [EnableQuery(PageSize = 20)]
    [EnableCount]
    [HttpGet]
    public ActionResult GetOdata()
    {
        return Ok(_appDbContext.Users);
    }
    
}