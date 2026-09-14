using Microsoft.EntityFrameworkCore; 
using Kaad.Api.Models;

namespace Kaad.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<User> Users { get; set; }

    public DbSet<Card> Cards { get; set; }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<VerificationRecord> VerificationRecords { get; set; }
}

