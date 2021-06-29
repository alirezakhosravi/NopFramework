using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Nop.Core.Data;
using Nop.Core.Infrastructure;

namespace Nop.Data
{
    /// <summary>
    /// Design time db context factory.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NopObjectContext>
    {
        /// <summary>
        /// Creates the db context.
        /// </summary>
        /// <returns>The db context.</returns>
        public NopObjectContext CreateDbContext(string[] args)
        {
            //power shell -> Add-Migration migration-name
            //consule -> dotnet ef migrations add migration-name

            //consule -> dotnet ef database update

            var nopFileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            string connection = DataSettingsManager.LoadSettings(nopFileProvider.MapPath("~/App_Data/dataSettings.json"), true).DataConnectionString;

            if (string.IsNullOrEmpty(connection))
            {
                //this connection string used when programmer need forec start migration as design time
                connection = "Data Source=10.1.11.102,1433;Initial Catalog=NopFramework;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123qweRt";
            }

            var optionsBuilder = new DbContextOptionsBuilder<NopObjectContext>();
            try
            {
                //test connection string
                //var _ = new SqlConnectionStringBuilder(connection);

                optionsBuilder
                    .UseSqlServer(connection
                                  , sqlOptions =>
                                  {
                                      sqlOptions.EnableRetryOnFailure(
                                      maxRetryCount: 10,
                                      maxRetryDelay: TimeSpan.FromSeconds(30),
                                      errorNumbersToAdd: null);
                                  });
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
                // do noting
            }
            return new NopObjectContext(optionsBuilder.Options);
        }
    }
}
