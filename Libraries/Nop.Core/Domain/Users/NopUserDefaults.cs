
namespace Nop.Core.Domain.Users
{
    /// <summary>
    /// Represents default values related to Users data
    /// </summary>
    public static partial class NopUserDefaults
    {
        #region System User roles

        /// <summary>
        /// Gets a system name of 'administrators' User role
        /// </summary>
        public static string AdministratorsRoleName => "Administrators";

        /// <summary>
        /// Gets a system name of 'registered' User role
        /// </summary>
        public static string RegisteredRoleName => "Registered";

        /// <summary>
        /// Gets a system name of 'guests' User role
        /// </summary>
        public static string GuestsRoleName => "Guests";

        #endregion

        #region System Users

        /// <summary>
        /// Gets a system name of 'search engine' User object
        /// </summary>
        public static string SearchEngineUserName => "SearchEngine";

        /// <summary>
        /// Gets a system name of 'background task' User object
        /// </summary>
        public static string BackgroundTaskUserName => "BackgroundTask";

        #endregion

        #region User attributes

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'FirstName'
        /// </summary>
        public static string FirstNameAttribute => "FirstName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastName'
        /// </summary>
        public static string LastNameAttribute => "LastName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Gender'
        /// </summary>
        public static string GenderAttribute => "Gender";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'DateOfBirth'
        /// </summary>
        public static string DateOfBirthAttribute => "DateOfBirth";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Company'
        /// </summary>
        public static string CompanyAttribute => "Company";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'StreetAddress'
        /// </summary>
        public static string StreetAddressAttribute => "StreetAddress";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'StreetAddress2'
        /// </summary>
        public static string StreetAddress2Attribute => "StreetAddress2";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ZipPostalCode'
        /// </summary>
        public static string ZipPostalCodeAttribute => "ZipPostalCode";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'City'
        /// </summary>
        public static string CityAttribute => "City";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'County'
        /// </summary>
        public static string CountyAttribute => "County";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CountryId'
        /// </summary>
        public static string CountryIdAttribute => "CountryId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'StateProvinceId'
        /// </summary>
        public static string StateProvinceIdAttribute => "StateProvinceId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Phone'
        /// </summary>
        public static string PhoneAttribute => "Phone";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Fax'
        /// </summary>
        public static string FaxAttribute => "Fax";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'VatNumber'
        /// </summary>
        public static string VatNumberAttribute => "VatNumber";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'VatNumberStatusId'
        /// </summary>
        public static string VatNumberStatusIdAttribute => "VatNumberStatusId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'TimeZoneId'
        /// </summary>
        public static string TimeZoneIdAttribute => "TimeZoneId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CustomUserAttributes'
        /// </summary>
        public static string CustomUserAttributes => "CustomUserAttributes";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AvatarPictureId'
        /// </summary>
        public static string AvatarPictureIdAttribute => "AvatarPictureId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Signature'
        /// </summary>
        public static string SignatureAttribute => "Signature";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryToken'
        /// </summary>
        public static string PasswordRecoveryTokenAttribute => "PasswordRecoveryToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryTokenDateGenerated'
        /// </summary>
        public static string PasswordRecoveryTokenDateGeneratedAttribute => "PasswordRecoveryTokenDateGenerated";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AccountActivationToken'
        /// </summary>
        public static string AccountActivationTokenAttribute => "AccountActivationToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'EmailRevalidationToken'
        /// </summary>
        public static string EmailRevalidationTokenAttribute => "EmailRevalidationToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastVisitedPage'
        /// </summary>
        public static string LastVisitedPageAttribute => "LastVisitedPage";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ImpersonatedUserId'
        /// </summary>
        public static string ImpersonatedUserIdAttribute => "ImpersonatedUserId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LanguageId'
        /// </summary>
        public static string LanguageIdAttribute => "LanguageId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LanguageAutomaticallyDetected'
        /// </summary>
        public static string LanguageAutomaticallyDetectedAttribute => "LanguageAutomaticallyDetected";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'NotifiedAboutNewPrivateMessages'
        /// </summary>
        public static string NotifiedAboutNewPrivateMessagesAttribute => "NotifiedAboutNewPrivateMessages";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'WorkingThemeName'
        /// </summary>
        public static string WorkingThemeNameAttribute => "WorkingThemeName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'EuCookieLawAccepted'
        /// </summary>
        public static string EuCookieLawAcceptedAttribute => "EuCookieLaw.Accepted";

        #endregion
    }
}