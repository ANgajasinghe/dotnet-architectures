using Microsoft.AspNetCore.Mvc;

namespace ODataApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SeedController : Controller
{
    private readonly AppDbContext _appDbContext;

    public SeedController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpPost]
    public IActionResult Seed()
    {
        _appDbContext.Database.EnsureCreated();

        var users = new List<User>();

        for (int i = 0; i < 100; i++)
        {
            users.Add(
                new User()
                {
                    Name = "User " + i,
                    Email = "user" + i + "@email.com",
                    Password = "password" + i,
                    UserAddresses = new List<UserAddress>()
                    {
                        new UserAddress()
                        {
                            No = "No " + i,
                            Street = "Street " + i,
                            City = "City " + i,
                            State = "State " + i,
                            Country = "Country " + i,
                            ZipCode = "ZipCode " + i
                        },
                        new UserAddress()
                        {
                            No = "No " + i + 1,
                            Street = "Street " + i + 1,
                            City = "City " + i + 1,
                            State = "State " + i + 1,
                            Country = "Country " + i + 1,
                            ZipCode = "ZipCode " + i + 1
                        }
                    }
                }
            );
        }


        _appDbContext.Users.AddRange(users);
        _appDbContext.SaveChanges();
        return Ok(new { message = "Seeded" });

    }
}