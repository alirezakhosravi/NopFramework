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
    }
}
