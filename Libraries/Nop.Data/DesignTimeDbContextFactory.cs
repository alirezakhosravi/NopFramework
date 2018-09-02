using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Nop.Core.Data;

namespace Nop.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NopObjectContext>
    {
        public NopObjectContext CreateDbContext(string[] args)
        {
            //power shell -> Add-Migration migration-name
            //consule -> dotnet ef migrations add migration-name

            var optionsBuilder = new DbContextOptionsBuilder<NopObjectContext>();
            optionsBuilder.UseSqlServer(DataSettingsManager
                .LoadSettings(filePath: "../../Presentation/Nop.Web/App_Data/dataSettings.json", reloadSettings: true)
                .DataConnectionString);
            return new NopObjectContext(optionsBuilder.Options);
        }
    }
}
