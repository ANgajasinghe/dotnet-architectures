using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddControllers(options => options.Filters.Add<ActionLogFilter>())
    .AddOData(option =>
        option
            .AddRouteComponents("api", EdmModelBuilder.GetEdmModel())
            .SetMaxTop(100)
            .Select()
            .Filter()
            .Count()
            .OrderBy()
            .Expand()
    ).AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });


builder.Services.AddSqlServer<AppDbContext>(
    "Data Source=DESKTOP-TERE1H0\\SQLEXPRESS;Initial Catalog=odata-test;User Id=sa;Password=#compaq123;",
    opt => { });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserAddress> UserAddresses { get; set; }

    
    

    public async Task<int> ExecuteSqAsync<T>(string spName, T parameters)
    {
        var data = GetParams(parameters);
        return await Database.ExecuteSqlRawAsync("EXEC " + spName.Trim() +" "+data.Item2, data.Item1);
    }

    private Tuple<List<SqlParameter>, string> GetParams(object parameters)
    {
        var parms = new List<SqlParameter>();
        var paramString = new List<string>();

        var props = parameters.GetType().GetProperties();

        foreach (var prop in props)
        {
            if(prop.Name == "Id")
                continue;
            
            parms.Add(new()
            {
                ParameterName = $"@{prop.Name}",
                Value = prop.GetValue(parameters)
            });

            paramString.Add($"@{prop.Name}");
        }

        return new Tuple<List<SqlParameter>, string>(parms, string.Join(",", paramString));
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public ICollection<UserAddress> UserAddresses { get; set; }
}

public class UserAddress
{
    public int Id { get; set; }
    public string No { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    
    public int UserId { get; set; }
}

public class EdmModelBuilder
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<User>("user");
        builder.EntitySet<UserAddress>("user-address");


        builder.EnableLowerCamelCase();
        return builder.GetEdmModel();
    }
}