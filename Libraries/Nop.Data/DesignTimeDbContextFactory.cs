using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nop.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NopObjectContext>
    {
        public NopObjectContext CreateDbContext(string[] args)
        {
            //power shell -> Add-Migration migration-name
            //consule -> dotnet ef migrations add migration-name

            //consule -> dotnet ef database update

            var optionsBuilder = new DbContextOptionsBuilder<NopObjectContext>();
            optionsBuilder
                .UseSqlServer("Data Source=localhost;Initial Catalog=nopFramework;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123qweRt"
                              , sqlOptions =>
                              {
                                  sqlOptions.EnableRetryOnFailure(
                                  maxRetryCount: 10,
                                  maxRetryDelay: TimeSpan.FromSeconds(30),
                                  errorNumbersToAdd: null);
                              });

            return new NopObjectContext(optionsBuilder.Options);
        }
    }
}
