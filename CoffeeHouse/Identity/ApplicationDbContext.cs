using CoffeeHouse.Models;
using Microsoft.EntityFrameworkCore;
namespace CoffeeHouse.Identity
{


    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {

        }
        public DbSet<Accounts> Accounts { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Ánh xạ lớp Account với bảng Account trong SQL Server
        //    modelBuilder.Entity<Account>(entity =>
        //    {
        //        entity.ToTable("Account"); // Tên bảng trong SQL Server

        //        entity.Property(e => e.Id).HasColumnName("Id"); // Cột 'Id'

        //        entity.Property(e => e.Email)
        //            .HasMaxLength(255)
        //            .IsRequired(); // Cột 'Email'

        //        entity.Property(e => e.Password)
        //            .HasMaxLength(255)
        //            .IsRequired(); // Cột 'Password'

        //        entity.Property(e => e.Name)
        //            .HasMaxLength(100)
        //            .IsRequired(); // Cột 'Name'

        //        entity.Property(e => e.RoleId).HasColumnName("Role_Id"); // Ánh xạ 'Role_Id'

        //        // Cấu hình khóa ngoại với bảng Role
        //        entity.HasOne(d => d.Role)
        //            .WithMany(p => p.Accounts)
        //            .HasForeignKey(d => d.RoleId)
        //            .HasConstraintName("FK_Account_Role");
        //    });

        //    // Các cấu hình khác
        //}

    }
}
