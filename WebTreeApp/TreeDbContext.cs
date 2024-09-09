
using Microsoft.EntityFrameworkCore;
using WebTreeApp.Models;

namespace WebTreeApp
{
    public class TreeDbContext : DbContext
    {
        public DbSet<TreeNode> TreeNodes { get; set; }

        public TreeDbContext(DbContextOptions<TreeDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreeNode>()
                .HasOne(t => t.Parent)
                .WithMany(t => t.Children)
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
