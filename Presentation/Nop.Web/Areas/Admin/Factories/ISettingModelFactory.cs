using Nop.Web.Areas.Admin.Models.Settings;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory
    /// </summary>
    public partial interface ISettingModelFactory
    {
        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option list model</returns>
        SortOptionListModel PrepareSortOptionListModel(SortOptionSearchModel searchModel);

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <returns>Media settings model</returns>
        MediaSettingsModel PrepareMediaSettingsModel();

        /// <summary>
        /// Prepare user user settings model
        /// </summary>
        /// <returns>User user settings model</returns>
        UserUserSettingsModel PrepareUserUserSettingsModel();

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <returns>General and common settings model</returns>
        GeneralCommonSettingsModel PrepareGeneralCommonSettingsModel();

        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting search model</returns>
        SettingSearchModel PrepareSettingSearchModel(SettingSearchModel searchModel);

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting list model</returns>
        SettingListModel PrepareSettingListModel(SettingSearchModel searchModel);

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>Setting mode model</returns>
        SettingModeModel PrepareSettingModeModel(string modeName);
    }
}