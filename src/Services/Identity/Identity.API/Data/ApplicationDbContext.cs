using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversRoom.Services.Identity.API.Models;

namespace UniversRoom.Services.Identity.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly string _connectionString;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("IdentityDB") ?? throw new NullReferenceException("Connection string");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}