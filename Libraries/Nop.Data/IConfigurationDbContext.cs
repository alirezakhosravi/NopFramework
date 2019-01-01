using Nop.Core.Configuration;

namespace Nop.Data
{
    public interface IConfigurationDbContext
    {
        /// <summary>
        /// Update the database.
        /// </summary>
        void UpdateDatabase();

        /// <summary>
        /// create temporal table
        /// </summary>
        void AddTemporal();

        /// <summary>
        /// create change traking
        /// </summary>
        void AddChangeTracking(NopConfig config);

        /// <summary>
        /// check conflict between change traking and temporal table
        /// </summary>
        void CheckConflict();
    }
}
