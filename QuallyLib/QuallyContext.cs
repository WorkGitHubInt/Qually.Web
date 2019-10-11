using Microsoft.EntityFrameworkCore;

namespace QuallyLib
{
    public class QuallyContext : DbContext
    {
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SubModel> SubModels { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Script> Scripts { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public QuallyContext(DbContextOptions<QuallyContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
