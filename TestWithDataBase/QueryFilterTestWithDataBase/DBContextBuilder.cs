using System;
using Microsoft.EntityFrameworkCore;

namespace QueryFilterTestWithDataBase
{
    public class DBContextBuilder
    {
        string connectionString = "";
        public DBContextBuilder()
        {
            connectionString = "";
        }
        public DbContext CreateDbContext()
        {
            var opt = new DbContextOptionsBuilder();
            opt.UseSqlServer(connectionString, p => p.CommandTimeout(60).EnableRetryOnFailure() ;
            var context = new DbContext(opt.Options);
            return context;
        }

        public void DbContextAddEntiy(DbContext db)
        {
            db.
        }
    }
}
