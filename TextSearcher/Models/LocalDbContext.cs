using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSearcher.Models
{
    internal class LocalDbContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<LocalDbContext>(
                modelBuilder
            );
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        public LocalDbContext()
            : base("localdb") { } //配置使用的连接名

        public DbSet<TextFileInfo> TextFiles { get; set; }
    }
}
