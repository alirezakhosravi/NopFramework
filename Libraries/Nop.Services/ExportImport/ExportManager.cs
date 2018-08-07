using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Users;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using OfficeOpenXml;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly UserSettings _userSettings;
        private readonly ICountryService _countryService;
        private readonly IUserAttributeFormatter _userAttributeFormatter;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPictureService _pictureService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ExportManager(AddressSettings addressSettings,
            UserSettings userSettings,
            ICountryService countryService,
            IUserAttributeFormatter userAttributeFormatter,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService,
            IStateProvinceService stateProvinceService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            this._addressSettings = addressSettings;
            this._userSettings = userSettings;
            this._countryService = countryService;
            this._userAttributeFormatter = userAttributeFormatter;
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._pictureService = pictureService;
            this._stateProvinceService = stateProvinceService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>Path to the image file</returns>
        protected virtual string GetPictures(int pictureId)
        {
            var picture = _pictureService.GetPictureById(pictureId);
            return _pictureService.GetThumbLocalPath(picture);
        }

        private string GetCustomUserAttributes(User user)
        {
            var selectedUserAttributes = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CustomUserAttributes);
            return _userAttributeFormatter.FormatAttributes(selectedUserAttributes, ";");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Export user list to XML
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportUsersToXml(IList<User> users)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Users");
            xmlWriter.WriteAttributeString("Version", NopVersion.CurrentVersion);

            foreach (var user in users)
            {
                xmlWriter.WriteStartElement("User");
                xmlWriter.WriteElementString("UserId", null, user.Id.ToString());
                xmlWriter.WriteElementString("UserGuid", null, user.UserGuid.ToString());
                xmlWriter.WriteElementString("Email", null, user.Email);
                xmlWriter.WriteElementString("Username", null, user.Username);

                var userPassword = _userService.GetCurrentPassword(user.Id);
                xmlWriter.WriteElementString("Password", null, userPassword?.Password);
                xmlWriter.WriteElementString("PasswordFormatId", null, (userPassword?.PasswordFormatId ?? 0).ToString());
                xmlWriter.WriteElementString("PasswordSalt", null, userPassword?.PasswordSalt);

                xmlWriter.WriteElementString("Active", null, user.Active.ToString());

                xmlWriter.WriteElementString("IsGuest", null, user.IsGuest().ToString());
                xmlWriter.WriteElementString("IsRegistered", null, user.IsRegistered().ToString());
                xmlWriter.WriteElementString("IsAdministrator", null, user.IsAdmin().ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, user.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));

                xmlWriter.WriteElementString("FirstName", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.FirstNameAttribute));
                xmlWriter.WriteElementString("LastName", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.LastNameAttribute));
                xmlWriter.WriteElementString("Gender", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.GenderAttribute));
                xmlWriter.WriteElementString("Company", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CompanyAttribute));

                xmlWriter.WriteElementString("CountryId", null, _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.CountryIdAttribute).ToString());
                xmlWriter.WriteElementString("StreetAddress", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.StreetAddressAttribute));
                xmlWriter.WriteElementString("StreetAddress2", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.StreetAddress2Attribute));
                xmlWriter.WriteElementString("ZipPostalCode", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.ZipPostalCodeAttribute));
                xmlWriter.WriteElementString("City", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CityAttribute));
                xmlWriter.WriteElementString("County", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CountyAttribute));
                xmlWriter.WriteElementString("StateProvinceId", null, _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.StateProvinceIdAttribute).ToString());
                xmlWriter.WriteElementString("Phone", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.PhoneAttribute));
                xmlWriter.WriteElementString("Fax", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.FaxAttribute));
                xmlWriter.WriteElementString("VatNumber", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.VatNumberAttribute));
                xmlWriter.WriteElementString("VatNumberStatusId", null, _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.VatNumberStatusIdAttribute).ToString());
                xmlWriter.WriteElementString("TimeZoneId", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.TimeZoneIdAttribute));
                xmlWriter.WriteElementString("AvatarPictureId", null, _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.AvatarPictureIdAttribute).ToString());
                xmlWriter.WriteElementString("Signature", null, _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.SignatureAttribute));

                var selectedUserAttributesString = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CustomUserAttributes);

                if (!string.IsNullOrEmpty(selectedUserAttributesString))
                {
                    var selectedUserAttributes = new StringReader(selectedUserAttributesString);
                    var selectedUserAttributesXmlReader = XmlReader.Create(selectedUserAttributes);
                    xmlWriter.WriteNode(selectedUserAttributesXmlReader, false);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportNewsletterSubscribersToTxt(IList<NewsLetterSubscription> subscriptions)
        {
            if (subscriptions == null)
                throw new ArgumentNullException(nameof(subscriptions));

            const string separator = ",";
            var sb = new StringBuilder();
            foreach (var subscription in subscriptions)
            {
                sb.Append(subscription.Email);
                sb.Append(separator);
                sb.Append(subscription.Active);
                sb.Append(separator);
                sb.Append(Environment.NewLine); //new line
            }

            return sb.ToString();
        }

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportStatesToTxt(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));

            const string separator = ",";
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append(state.Country.TwoLetterIsoCode);
                sb.Append(separator);
                sb.Append(state.Name);
                sb.Append(separator);
                sb.Append(state.Abbreviation);
                sb.Append(separator);
                sb.Append(state.Published);
                sb.Append(separator);
                sb.Append(state.DisplayOrder);
                sb.Append(Environment.NewLine); //new line
            }

            return sb.ToString();
        }


        #endregion
    }
}