using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Users;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Code first installation service
    /// </summary>
    public partial class CodeFirstInstallationService : IInstallationService
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly INopFileProvider _fileProvider;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IRepository<UserUserRoleMapping> _userUserRoleMapping;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(IGenericAttributeService genericAttributeService,
            INopFileProvider fileProvider,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Address> addressRepository,
            IRepository<Country> countryRepository,
            IRepository<User> userRepository,
            IRepository<UserPassword> userPasswordRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<Language> languageRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,
            IRepository<SearchTerm> searchTermRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IRepository<UserUserRoleMapping> userUserRoleMapping,
            IWebHelper webHelper)
        {
            this._genericAttributeService = genericAttributeService;
            this._fileProvider = fileProvider;
            this._activityLogRepository = activityLogRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._addressRepository = addressRepository;
            this._countryRepository = countryRepository;
            this._userPasswordRepository = userPasswordRepository;
            this._userRepository = userRepository;
            this._userRoleRepository = userRoleRepository;
            this._emailAccountRepository = emailAccountRepository;
            this._languageRepository = languageRepository;
            this._messageTemplateRepository = messageTemplateRepository;
            this._scheduleTaskRepository = scheduleTaskRepository;
            this._searchTermRepository = searchTermRepository;
            this._stateProvinceRepository = stateProvinceRepository;
            this._urlRecordRepository = urlRecordRepository;
            this._userUserRoleMapping = userUserRoleMapping;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual string ValidateSeName<T>(T entity, string name) where T : BaseEntity
        {
            //simplified and very fast (no DB calls) version of "ValidateSeName" method of UrlRecordService
            //we know that there's no same names of entities in sample data

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //validation
            var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            name = name.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (var c in name.ToCharArray())
            {
                var c2 = c.ToString();
                if (okChars.Contains(c2))
                {
                    sb.Append(c2);
                }
            }

            name = sb.ToString();
            name = name.Replace(" ", "-");
            while (name.Contains("--"))
                name = name.Replace("--", "-");
            while (name.Contains("__"))
                name = name.Replace("__", "_");

            //max length
            name = CommonHelper.EnsureMaximumLength(name, NopSeoDefaults.SearchEngineNameLength);

            return name;
        }

        protected virtual string GetSamplesPath()
        {
            return _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);
        }

        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            var directoryPath = _fileProvider.MapPath(NopInstallationDefaults.LocalizationResourcesPath);
            var pattern = $"*.{NopInstallationDefaults.LocalizationResourcesFileExtension}";
            foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
            {
                var localesXml = _fileProvider.ReadAllText(filePath, Encoding.UTF8);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }
        }

        protected virtual void InstallCountriesAndStates()
        {
            var cUsa = new Country
            {
                Name = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 840,
                DisplayOrder = 1,
                Published = true
            };

            var cCanada = new Country
            {
                Name = "Canada",
                TwoLetterIsoCode = "CA",
                ThreeLetterIsoCode = "CAN",
                NumericIsoCode = 124,
                DisplayOrder = 100,
                Published = true
            };

            var countries = new List<Country>
            {
                cUsa,
                cCanada,
                //other countries
                new Country
                {
                    Name = "Argentina",
                    TwoLetterIsoCode = "AR",
                    ThreeLetterIsoCode = "ARG",
                    NumericIsoCode = 32,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Armenia",
                    TwoLetterIsoCode = "AM",
                    ThreeLetterIsoCode = "ARM",
                    NumericIsoCode = 51,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Aruba",
                    TwoLetterIsoCode = "AW",
                    ThreeLetterIsoCode = "ABW",
                    NumericIsoCode = 533,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Australia",
                    TwoLetterIsoCode = "AU",
                    ThreeLetterIsoCode = "AUS",
                    NumericIsoCode = 36,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Austria",
                    TwoLetterIsoCode = "AT",
                    ThreeLetterIsoCode = "AUT",
                    NumericIsoCode = 40,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Azerbaijan",
                    TwoLetterIsoCode = "AZ",
                    ThreeLetterIsoCode = "AZE",
                    NumericIsoCode = 31,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bahamas",
                    TwoLetterIsoCode = "BS",
                    ThreeLetterIsoCode = "BHS",
                    NumericIsoCode = 44,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bangladesh",
                    TwoLetterIsoCode = "BD",
                    ThreeLetterIsoCode = "BGD",
                    NumericIsoCode = 50,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Belarus",
                    TwoLetterIsoCode = "BY",
                    ThreeLetterIsoCode = "BLR",
                    NumericIsoCode = 112,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Belgium",
                    TwoLetterIsoCode = "BE",
                    ThreeLetterIsoCode = "BEL",
                    NumericIsoCode = 56,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Belize",
                    TwoLetterIsoCode = "BZ",
                    ThreeLetterIsoCode = "BLZ",
                    NumericIsoCode = 84,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bermuda",
                    TwoLetterIsoCode = "BM",
                    ThreeLetterIsoCode = "BMU",
                    NumericIsoCode = 60,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bolivia",
                    TwoLetterIsoCode = "BO",
                    ThreeLetterIsoCode = "BOL",
                    NumericIsoCode = 68,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bosnia and Herzegowina",
                    
                    
                    TwoLetterIsoCode = "BA",
                    ThreeLetterIsoCode = "BIH",
                    NumericIsoCode = 70,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Brazil",
                    
                    
                    TwoLetterIsoCode = "BR",
                    ThreeLetterIsoCode = "BRA",
                    NumericIsoCode = 76,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bulgaria",
                    
                    
                    TwoLetterIsoCode = "BG",
                    ThreeLetterIsoCode = "BGR",
                    NumericIsoCode = 100,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cayman Islands",
                    
                    
                    TwoLetterIsoCode = "KY",
                    ThreeLetterIsoCode = "CYM",
                    NumericIsoCode = 136,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Chile",
                    
                    
                    TwoLetterIsoCode = "CL",
                    ThreeLetterIsoCode = "CHL",
                    NumericIsoCode = 152,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "China",
                    
                    
                    TwoLetterIsoCode = "CN",
                    ThreeLetterIsoCode = "CHN",
                    NumericIsoCode = 156,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Colombia",
                    
                    
                    TwoLetterIsoCode = "CO",
                    ThreeLetterIsoCode = "COL",
                    NumericIsoCode = 170,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Costa Rica",
                    
                    
                    TwoLetterIsoCode = "CR",
                    ThreeLetterIsoCode = "CRI",
                    NumericIsoCode = 188,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Croatia",
                    
                    
                    TwoLetterIsoCode = "HR",
                    ThreeLetterIsoCode = "HRV",
                    NumericIsoCode = 191,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cuba",
                    
                    
                    TwoLetterIsoCode = "CU",
                    ThreeLetterIsoCode = "CUB",
                    NumericIsoCode = 192,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cyprus",
                    
                    
                    TwoLetterIsoCode = "CY",
                    ThreeLetterIsoCode = "CYP",
                    NumericIsoCode = 196,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Czech Republic",
                    
                    
                    TwoLetterIsoCode = "CZ",
                    ThreeLetterIsoCode = "CZE",
                    NumericIsoCode = 203,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Denmark",
                    
                    
                    TwoLetterIsoCode = "DK",
                    ThreeLetterIsoCode = "DNK",
                    NumericIsoCode = 208,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Dominican Republic",
                    
                    
                    TwoLetterIsoCode = "DO",
                    ThreeLetterIsoCode = "DOM",
                    NumericIsoCode = 214,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "East Timor",
                    
                    
                    TwoLetterIsoCode = "TL",
                    ThreeLetterIsoCode = "TLS",
                    NumericIsoCode = 626,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ecuador",
                    
                    
                    TwoLetterIsoCode = "EC",
                    ThreeLetterIsoCode = "ECU",
                    NumericIsoCode = 218,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Egypt",
                    
                    
                    TwoLetterIsoCode = "EG",
                    ThreeLetterIsoCode = "EGY",
                    NumericIsoCode = 818,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Finland",
                    
                    
                    TwoLetterIsoCode = "FI",
                    ThreeLetterIsoCode = "FIN",
                    NumericIsoCode = 246,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "France",
                    
                    
                    TwoLetterIsoCode = "FR",
                    ThreeLetterIsoCode = "FRA",
                    NumericIsoCode = 250,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Georgia",
                    
                    
                    TwoLetterIsoCode = "GE",
                    ThreeLetterIsoCode = "GEO",
                    NumericIsoCode = 268,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Germany",
                    
                    
                    TwoLetterIsoCode = "DE",
                    ThreeLetterIsoCode = "DEU",
                    NumericIsoCode = 276,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Gibraltar",
                    
                    
                    TwoLetterIsoCode = "GI",
                    ThreeLetterIsoCode = "GIB",
                    NumericIsoCode = 292,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Greece",
                    
                    
                    TwoLetterIsoCode = "GR",
                    ThreeLetterIsoCode = "GRC",
                    NumericIsoCode = 300,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guatemala",
                    
                    
                    TwoLetterIsoCode = "GT",
                    ThreeLetterIsoCode = "GTM",
                    NumericIsoCode = 320,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Hong Kong",
                    
                    
                    TwoLetterIsoCode = "HK",
                    ThreeLetterIsoCode = "HKG",
                    NumericIsoCode = 344,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Hungary",
                    
                    
                    TwoLetterIsoCode = "HU",
                    ThreeLetterIsoCode = "HUN",
                    NumericIsoCode = 348,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "India",
                    
                    
                    TwoLetterIsoCode = "IN",
                    ThreeLetterIsoCode = "IND",
                    NumericIsoCode = 356,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Indonesia",
                    
                    
                    TwoLetterIsoCode = "ID",
                    ThreeLetterIsoCode = "IDN",
                    NumericIsoCode = 360,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ireland",
                    
                    
                    TwoLetterIsoCode = "IE",
                    ThreeLetterIsoCode = "IRL",
                    NumericIsoCode = 372,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Israel",
                    
                    
                    TwoLetterIsoCode = "IL",
                    ThreeLetterIsoCode = "ISR",
                    NumericIsoCode = 376,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Italy",
                    
                    
                    TwoLetterIsoCode = "IT",
                    ThreeLetterIsoCode = "ITA",
                    NumericIsoCode = 380,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Jamaica",
                    
                    
                    TwoLetterIsoCode = "JM",
                    ThreeLetterIsoCode = "JAM",
                    NumericIsoCode = 388,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Japan",
                    
                    
                    TwoLetterIsoCode = "JP",
                    ThreeLetterIsoCode = "JPN",
                    NumericIsoCode = 392,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Jordan",
                    
                    
                    TwoLetterIsoCode = "JO",
                    ThreeLetterIsoCode = "JOR",
                    NumericIsoCode = 400,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kazakhstan",
                    
                    
                    TwoLetterIsoCode = "KZ",
                    ThreeLetterIsoCode = "KAZ",
                    NumericIsoCode = 398,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Korea, Democratic People's Republic of",
                    
                    
                    TwoLetterIsoCode = "KP",
                    ThreeLetterIsoCode = "PRK",
                    NumericIsoCode = 408,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kuwait",
                    
                    
                    TwoLetterIsoCode = "KW",
                    ThreeLetterIsoCode = "KWT",
                    NumericIsoCode = 414,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Malaysia",
                    
                    
                    TwoLetterIsoCode = "MY",
                    ThreeLetterIsoCode = "MYS",
                    NumericIsoCode = 458,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mexico",
                    
                    
                    TwoLetterIsoCode = "MX",
                    ThreeLetterIsoCode = "MEX",
                    NumericIsoCode = 484,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Netherlands",
                    
                    
                    TwoLetterIsoCode = "NL",
                    ThreeLetterIsoCode = "NLD",
                    NumericIsoCode = 528,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "New Zealand",
                    
                    
                    TwoLetterIsoCode = "NZ",
                    ThreeLetterIsoCode = "NZL",
                    NumericIsoCode = 554,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Norway",
                    
                    
                    TwoLetterIsoCode = "NO",
                    ThreeLetterIsoCode = "NOR",
                    NumericIsoCode = 578,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Pakistan",
                    
                    
                    TwoLetterIsoCode = "PK",
                    ThreeLetterIsoCode = "PAK",
                    NumericIsoCode = 586,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Palestine",
                    
                    
                    TwoLetterIsoCode = "PS",
                    ThreeLetterIsoCode = "PSE",
                    NumericIsoCode = 275,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Paraguay",
                    
                    
                    TwoLetterIsoCode = "PY",
                    ThreeLetterIsoCode = "PRY",
                    NumericIsoCode = 600,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Peru",
                    
                    
                    TwoLetterIsoCode = "PE",
                    ThreeLetterIsoCode = "PER",
                    NumericIsoCode = 604,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Philippines",
                    
                    
                    TwoLetterIsoCode = "PH",
                    ThreeLetterIsoCode = "PHL",
                    NumericIsoCode = 608,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Poland",
                    
                    
                    TwoLetterIsoCode = "PL",
                    ThreeLetterIsoCode = "POL",
                    NumericIsoCode = 616,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Portugal",
                    
                    
                    TwoLetterIsoCode = "PT",
                    ThreeLetterIsoCode = "PRT",
                    NumericIsoCode = 620,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Puerto Rico",
                    
                    
                    TwoLetterIsoCode = "PR",
                    ThreeLetterIsoCode = "PRI",
                    NumericIsoCode = 630,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Qatar",
                    
                    
                    TwoLetterIsoCode = "QA",
                    ThreeLetterIsoCode = "QAT",
                    NumericIsoCode = 634,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Romania",
                    
                    
                    TwoLetterIsoCode = "RO",
                    ThreeLetterIsoCode = "ROM",
                    NumericIsoCode = 642,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Russian Federation",
                    
                    
                    TwoLetterIsoCode = "RU",
                    ThreeLetterIsoCode = "RUS",
                    NumericIsoCode = 643,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Saudi Arabia",
                    
                    
                    TwoLetterIsoCode = "SA",
                    ThreeLetterIsoCode = "SAU",
                    NumericIsoCode = 682,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Singapore",
                    
                    
                    TwoLetterIsoCode = "SG",
                    ThreeLetterIsoCode = "SGP",
                    NumericIsoCode = 702,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Slovakia (Slovak Republic)",
                    
                    
                    TwoLetterIsoCode = "SK",
                    ThreeLetterIsoCode = "SVK",
                    NumericIsoCode = 703,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Slovenia",
                    
                    
                    TwoLetterIsoCode = "SI",
                    ThreeLetterIsoCode = "SVN",
                    NumericIsoCode = 705,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "South Africa",
                    
                    
                    TwoLetterIsoCode = "ZA",
                    ThreeLetterIsoCode = "ZAF",
                    NumericIsoCode = 710,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Spain",
                    
                    
                    TwoLetterIsoCode = "ES",
                    ThreeLetterIsoCode = "ESP",
                    NumericIsoCode = 724,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sweden",
                    
                    
                    TwoLetterIsoCode = "SE",
                    ThreeLetterIsoCode = "SWE",
                    NumericIsoCode = 752,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Switzerland",
                    
                    
                    TwoLetterIsoCode = "CH",
                    ThreeLetterIsoCode = "CHE",
                    NumericIsoCode = 756,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Taiwan",
                    
                    
                    TwoLetterIsoCode = "TW",
                    ThreeLetterIsoCode = "TWN",
                    NumericIsoCode = 158,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Thailand",
                    
                    
                    TwoLetterIsoCode = "TH",
                    ThreeLetterIsoCode = "THA",
                    NumericIsoCode = 764,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Turkey",
                    
                    
                    TwoLetterIsoCode = "TR",
                    ThreeLetterIsoCode = "TUR",
                    NumericIsoCode = 792,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ukraine",
                    
                    
                    TwoLetterIsoCode = "UA",
                    ThreeLetterIsoCode = "UKR",
                    NumericIsoCode = 804,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "United Arab Emirates",
                    
                    
                    TwoLetterIsoCode = "AE",
                    ThreeLetterIsoCode = "ARE",
                    NumericIsoCode = 784,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "United Kingdom",
                    
                    
                    TwoLetterIsoCode = "GB",
                    ThreeLetterIsoCode = "GBR",
                    NumericIsoCode = 826,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "United States minor outlying islands",
                    
                    
                    TwoLetterIsoCode = "UM",
                    ThreeLetterIsoCode = "UMI",
                    NumericIsoCode = 581,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Uruguay",
                    
                    
                    TwoLetterIsoCode = "UY",
                    ThreeLetterIsoCode = "URY",
                    NumericIsoCode = 858,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Uzbekistan",
                    
                    
                    TwoLetterIsoCode = "UZ",
                    ThreeLetterIsoCode = "UZB",
                    NumericIsoCode = 860,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Venezuela",
                    
                    
                    TwoLetterIsoCode = "VE",
                    ThreeLetterIsoCode = "VEN",
                    NumericIsoCode = 862,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Serbia",
                    
                    
                    TwoLetterIsoCode = "RS",
                    ThreeLetterIsoCode = "SRB",
                    NumericIsoCode = 688,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Afghanistan",
                    
                    
                    TwoLetterIsoCode = "AF",
                    ThreeLetterIsoCode = "AFG",
                    NumericIsoCode = 4,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Albania",
                    
                    
                    TwoLetterIsoCode = "AL",
                    ThreeLetterIsoCode = "ALB",
                    NumericIsoCode = 8,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Algeria",
                    
                    
                    TwoLetterIsoCode = "DZ",
                    ThreeLetterIsoCode = "DZA",
                    NumericIsoCode = 12,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "American Samoa",
                    
                    
                    TwoLetterIsoCode = "AS",
                    ThreeLetterIsoCode = "ASM",
                    NumericIsoCode = 16,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Andorra",
                    
                    
                    TwoLetterIsoCode = "AD",
                    ThreeLetterIsoCode = "AND",
                    NumericIsoCode = 20,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Angola",
                    
                    
                    TwoLetterIsoCode = "AO",
                    ThreeLetterIsoCode = "AGO",
                    NumericIsoCode = 24,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Anguilla",
                    
                    
                    TwoLetterIsoCode = "AI",
                    ThreeLetterIsoCode = "AIA",
                    NumericIsoCode = 660,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Antarctica",
                    
                    
                    TwoLetterIsoCode = "AQ",
                    ThreeLetterIsoCode = "ATA",
                    NumericIsoCode = 10,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Antigua and Barbuda",
                    
                    
                    TwoLetterIsoCode = "AG",
                    ThreeLetterIsoCode = "ATG",
                    NumericIsoCode = 28,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bahrain",
                    
                    
                    TwoLetterIsoCode = "BH",
                    ThreeLetterIsoCode = "BHR",
                    NumericIsoCode = 48,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Barbados",
                    
                    
                    TwoLetterIsoCode = "BB",
                    ThreeLetterIsoCode = "BRB",
                    NumericIsoCode = 52,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Benin",
                    
                    
                    TwoLetterIsoCode = "BJ",
                    ThreeLetterIsoCode = "BEN",
                    NumericIsoCode = 204,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bhutan",
                    
                    
                    TwoLetterIsoCode = "BT",
                    ThreeLetterIsoCode = "BTN",
                    NumericIsoCode = 64,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Botswana",
                    
                    
                    TwoLetterIsoCode = "BW",
                    ThreeLetterIsoCode = "BWA",
                    NumericIsoCode = 72,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bouvet Island",
                    
                    
                    TwoLetterIsoCode = "BV",
                    ThreeLetterIsoCode = "BVT",
                    NumericIsoCode = 74,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "British Indian Ocean Territory",
                    
                    
                    TwoLetterIsoCode = "IO",
                    ThreeLetterIsoCode = "IOT",
                    NumericIsoCode = 86,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Brunei Darussalam",
                    
                    
                    TwoLetterIsoCode = "BN",
                    ThreeLetterIsoCode = "BRN",
                    NumericIsoCode = 96,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Burkina Faso",
                    
                    
                    TwoLetterIsoCode = "BF",
                    ThreeLetterIsoCode = "BFA",
                    NumericIsoCode = 854,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Burundi",
                    
                    
                    TwoLetterIsoCode = "BI",
                    ThreeLetterIsoCode = "BDI",
                    NumericIsoCode = 108,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cambodia",
                    
                    
                    TwoLetterIsoCode = "KH",
                    ThreeLetterIsoCode = "KHM",
                    NumericIsoCode = 116,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cameroon",
                    
                    
                    TwoLetterIsoCode = "CM",
                    ThreeLetterIsoCode = "CMR",
                    NumericIsoCode = 120,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cape Verde",
                    
                    
                    TwoLetterIsoCode = "CV",
                    ThreeLetterIsoCode = "CPV",
                    NumericIsoCode = 132,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Central African Republic",
                    
                    
                    TwoLetterIsoCode = "CF",
                    ThreeLetterIsoCode = "CAF",
                    NumericIsoCode = 140,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Chad",
                    
                    
                    TwoLetterIsoCode = "TD",
                    ThreeLetterIsoCode = "TCD",
                    NumericIsoCode = 148,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Christmas Island",
                    
                    
                    TwoLetterIsoCode = "CX",
                    ThreeLetterIsoCode = "CXR",
                    NumericIsoCode = 162,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cocos (Keeling) Islands",
                    
                    
                    TwoLetterIsoCode = "CC",
                    ThreeLetterIsoCode = "CCK",
                    NumericIsoCode = 166,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Comoros",
                    
                    
                    TwoLetterIsoCode = "KM",
                    ThreeLetterIsoCode = "COM",
                    NumericIsoCode = 174,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Congo",
                    
                    
                    TwoLetterIsoCode = "CG",
                    ThreeLetterIsoCode = "COG",
                    NumericIsoCode = 178,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Congo (Democratic Republic of the)",
                    
                    
                    TwoLetterIsoCode = "CD",
                    ThreeLetterIsoCode = "COD",
                    NumericIsoCode = 180,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cook Islands",
                    
                    
                    TwoLetterIsoCode = "CK",
                    ThreeLetterIsoCode = "COK",
                    NumericIsoCode = 184,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cote D'Ivoire",
                    
                    
                    TwoLetterIsoCode = "CI",
                    ThreeLetterIsoCode = "CIV",
                    NumericIsoCode = 384,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Djibouti",
                    
                    
                    TwoLetterIsoCode = "DJ",
                    ThreeLetterIsoCode = "DJI",
                    NumericIsoCode = 262,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Dominica",
                    
                    
                    TwoLetterIsoCode = "DM",
                    ThreeLetterIsoCode = "DMA",
                    NumericIsoCode = 212,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "El Salvador",
                    
                    
                    TwoLetterIsoCode = "SV",
                    ThreeLetterIsoCode = "SLV",
                    NumericIsoCode = 222,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Equatorial Guinea",
                    
                    
                    TwoLetterIsoCode = "GQ",
                    ThreeLetterIsoCode = "GNQ",
                    NumericIsoCode = 226,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Eritrea",
                    
                    
                    TwoLetterIsoCode = "ER",
                    ThreeLetterIsoCode = "ERI",
                    NumericIsoCode = 232,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Estonia",
                    
                    
                    TwoLetterIsoCode = "EE",
                    ThreeLetterIsoCode = "EST",
                    NumericIsoCode = 233,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ethiopia",
                    
                    
                    TwoLetterIsoCode = "ET",
                    ThreeLetterIsoCode = "ETH",
                    NumericIsoCode = 231,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Falkland Islands (Malvinas)",
                    
                    
                    TwoLetterIsoCode = "FK",
                    ThreeLetterIsoCode = "FLK",
                    NumericIsoCode = 238,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Faroe Islands",
                    
                    
                    TwoLetterIsoCode = "FO",
                    ThreeLetterIsoCode = "FRO",
                    NumericIsoCode = 234,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Fiji",
                    
                    
                    TwoLetterIsoCode = "FJ",
                    ThreeLetterIsoCode = "FJI",
                    NumericIsoCode = 242,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "French Guiana",
                    
                    
                    TwoLetterIsoCode = "GF",
                    ThreeLetterIsoCode = "GUF",
                    NumericIsoCode = 254,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "French Polynesia",
                    
                    
                    TwoLetterIsoCode = "PF",
                    ThreeLetterIsoCode = "PYF",
                    NumericIsoCode = 258,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "French Southern Territories",
                    
                    
                    TwoLetterIsoCode = "TF",
                    ThreeLetterIsoCode = "ATF",
                    NumericIsoCode = 260,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Gabon",
                    
                    
                    TwoLetterIsoCode = "GA",
                    ThreeLetterIsoCode = "GAB",
                    NumericIsoCode = 266,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Gambia",
                    
                    
                    TwoLetterIsoCode = "GM",
                    ThreeLetterIsoCode = "GMB",
                    NumericIsoCode = 270,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ghana",
                    
                    
                    TwoLetterIsoCode = "GH",
                    ThreeLetterIsoCode = "GHA",
                    NumericIsoCode = 288,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Greenland",
                    
                    
                    TwoLetterIsoCode = "GL",
                    ThreeLetterIsoCode = "GRL",
                    NumericIsoCode = 304,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Grenada",
                    
                    
                    TwoLetterIsoCode = "GD",
                    ThreeLetterIsoCode = "GRD",
                    NumericIsoCode = 308,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guadeloupe",
                    
                    
                    TwoLetterIsoCode = "GP",
                    ThreeLetterIsoCode = "GLP",
                    NumericIsoCode = 312,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guam",
                    
                    
                    TwoLetterIsoCode = "GU",
                    ThreeLetterIsoCode = "GUM",
                    NumericIsoCode = 316,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guinea",
                    
                    
                    TwoLetterIsoCode = "GN",
                    ThreeLetterIsoCode = "GIN",
                    NumericIsoCode = 324,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guinea-bissau",
                    
                    
                    TwoLetterIsoCode = "GW",
                    ThreeLetterIsoCode = "GNB",
                    NumericIsoCode = 624,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guyana",
                    
                    
                    TwoLetterIsoCode = "GY",
                    ThreeLetterIsoCode = "GUY",
                    NumericIsoCode = 328,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Haiti",
                    
                    
                    TwoLetterIsoCode = "HT",
                    ThreeLetterIsoCode = "HTI",
                    NumericIsoCode = 332,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Heard and Mc Donald Islands",
                    
                    
                    TwoLetterIsoCode = "HM",
                    ThreeLetterIsoCode = "HMD",
                    NumericIsoCode = 334,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Honduras",
                    
                    
                    TwoLetterIsoCode = "HN",
                    ThreeLetterIsoCode = "HND",
                    NumericIsoCode = 340,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Iceland",
                    
                    
                    TwoLetterIsoCode = "IS",
                    ThreeLetterIsoCode = "ISL",
                    NumericIsoCode = 352,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Iran (Islamic Republic of)",
                    
                    
                    TwoLetterIsoCode = "IR",
                    ThreeLetterIsoCode = "IRN",
                    NumericIsoCode = 364,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Iraq",
                    
                    
                    TwoLetterIsoCode = "IQ",
                    ThreeLetterIsoCode = "IRQ",
                    NumericIsoCode = 368,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kenya",
                    
                    
                    TwoLetterIsoCode = "KE",
                    ThreeLetterIsoCode = "KEN",
                    NumericIsoCode = 404,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kiribati",
                    
                    
                    TwoLetterIsoCode = "KI",
                    ThreeLetterIsoCode = "KIR",
                    NumericIsoCode = 296,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Korea",
                    
                    
                    TwoLetterIsoCode = "KR",
                    ThreeLetterIsoCode = "KOR",
                    NumericIsoCode = 410,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kyrgyzstan",
                    
                    
                    TwoLetterIsoCode = "KG",
                    ThreeLetterIsoCode = "KGZ",
                    NumericIsoCode = 417,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Lao People's Democratic Republic",
                    
                    
                    TwoLetterIsoCode = "LA",
                    ThreeLetterIsoCode = "LAO",
                    NumericIsoCode = 418,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Latvia",
                    
                    
                    TwoLetterIsoCode = "LV",
                    ThreeLetterIsoCode = "LVA",
                    NumericIsoCode = 428,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Lebanon",
                    
                    
                    TwoLetterIsoCode = "LB",
                    ThreeLetterIsoCode = "LBN",
                    NumericIsoCode = 422,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Lesotho",
                    
                    
                    TwoLetterIsoCode = "LS",
                    ThreeLetterIsoCode = "LSO",
                    NumericIsoCode = 426,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Liberia",
                    
                    
                    TwoLetterIsoCode = "LR",
                    ThreeLetterIsoCode = "LBR",
                    NumericIsoCode = 430,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Libyan Arab Jamahiriya",
                    
                    
                    TwoLetterIsoCode = "LY",
                    ThreeLetterIsoCode = "LBY",
                    NumericIsoCode = 434,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Liechtenstein",
                    
                    
                    TwoLetterIsoCode = "LI",
                    ThreeLetterIsoCode = "LIE",
                    NumericIsoCode = 438,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Lithuania",
                    
                    
                    TwoLetterIsoCode = "LT",
                    ThreeLetterIsoCode = "LTU",
                    NumericIsoCode = 440,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Luxembourg",
                    
                    
                    TwoLetterIsoCode = "LU",
                    ThreeLetterIsoCode = "LUX",
                    NumericIsoCode = 442,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Macau",
                    
                    
                    TwoLetterIsoCode = "MO",
                    ThreeLetterIsoCode = "MAC",
                    NumericIsoCode = 446,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Macedonia",
                    
                    
                    TwoLetterIsoCode = "MK",
                    ThreeLetterIsoCode = "MKD",
                    NumericIsoCode = 807,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Madagascar",
                    
                    
                    TwoLetterIsoCode = "MG",
                    ThreeLetterIsoCode = "MDG",
                    NumericIsoCode = 450,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Malawi",
                    
                    
                    TwoLetterIsoCode = "MW",
                    ThreeLetterIsoCode = "MWI",
                    NumericIsoCode = 454,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Maldives",
                    
                    
                    TwoLetterIsoCode = "MV",
                    ThreeLetterIsoCode = "MDV",
                    NumericIsoCode = 462,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mali",
                    
                    
                    TwoLetterIsoCode = "ML",
                    ThreeLetterIsoCode = "MLI",
                    NumericIsoCode = 466,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Malta",
                    
                    
                    TwoLetterIsoCode = "MT",
                    ThreeLetterIsoCode = "MLT",
                    NumericIsoCode = 470,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Marshall Islands",
                    
                    
                    TwoLetterIsoCode = "MH",
                    ThreeLetterIsoCode = "MHL",
                    NumericIsoCode = 584,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Martinique",
                    
                    
                    TwoLetterIsoCode = "MQ",
                    ThreeLetterIsoCode = "MTQ",
                    NumericIsoCode = 474,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mauritania",
                    
                    
                    TwoLetterIsoCode = "MR",
                    ThreeLetterIsoCode = "MRT",
                    NumericIsoCode = 478,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mauritius",
                    
                    
                    TwoLetterIsoCode = "MU",
                    ThreeLetterIsoCode = "MUS",
                    NumericIsoCode = 480,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mayotte",
                    
                    
                    TwoLetterIsoCode = "YT",
                    ThreeLetterIsoCode = "MYT",
                    NumericIsoCode = 175,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Micronesia",
                    
                    
                    TwoLetterIsoCode = "FM",
                    ThreeLetterIsoCode = "FSM",
                    NumericIsoCode = 583,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Moldova",
                    
                    
                    TwoLetterIsoCode = "MD",
                    ThreeLetterIsoCode = "MDA",
                    NumericIsoCode = 498,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Monaco",
                    
                    
                    TwoLetterIsoCode = "MC",
                    ThreeLetterIsoCode = "MCO",
                    NumericIsoCode = 492,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mongolia",
                    
                    
                    TwoLetterIsoCode = "MN",
                    ThreeLetterIsoCode = "MNG",
                    NumericIsoCode = 496,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Montenegro",
                    
                    
                    TwoLetterIsoCode = "ME",
                    ThreeLetterIsoCode = "MNE",
                    NumericIsoCode = 499,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Montserrat",
                    
                    
                    TwoLetterIsoCode = "MS",
                    ThreeLetterIsoCode = "MSR",
                    NumericIsoCode = 500,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Morocco",
                    
                    
                    TwoLetterIsoCode = "MA",
                    ThreeLetterIsoCode = "MAR",
                    NumericIsoCode = 504,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mozambique",
                    
                    
                    TwoLetterIsoCode = "MZ",
                    ThreeLetterIsoCode = "MOZ",
                    NumericIsoCode = 508,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Myanmar",
                    
                    
                    TwoLetterIsoCode = "MM",
                    ThreeLetterIsoCode = "MMR",
                    NumericIsoCode = 104,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Namibia",
                    
                    
                    TwoLetterIsoCode = "NA",
                    ThreeLetterIsoCode = "NAM",
                    NumericIsoCode = 516,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Nauru",
                    
                    
                    TwoLetterIsoCode = "NR",
                    ThreeLetterIsoCode = "NRU",
                    NumericIsoCode = 520,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Nepal",
                    
                    
                    TwoLetterIsoCode = "NP",
                    ThreeLetterIsoCode = "NPL",
                    NumericIsoCode = 524,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Netherlands Antilles",
                    
                    
                    TwoLetterIsoCode = "AN",
                    ThreeLetterIsoCode = "ANT",
                    NumericIsoCode = 530,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "New Caledonia",
                    
                    
                    TwoLetterIsoCode = "NC",
                    ThreeLetterIsoCode = "NCL",
                    NumericIsoCode = 540,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Nicaragua",
                    
                    
                    TwoLetterIsoCode = "NI",
                    ThreeLetterIsoCode = "NIC",
                    NumericIsoCode = 558,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Niger",
                    
                    
                    TwoLetterIsoCode = "NE",
                    ThreeLetterIsoCode = "NER",
                    NumericIsoCode = 562,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Nigeria",
                    
                    
                    TwoLetterIsoCode = "NG",
                    ThreeLetterIsoCode = "NGA",
                    NumericIsoCode = 566,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Niue",
                    
                    
                    TwoLetterIsoCode = "NU",
                    ThreeLetterIsoCode = "NIU",
                    NumericIsoCode = 570,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Norfolk Island",
                    
                    
                    TwoLetterIsoCode = "NF",
                    ThreeLetterIsoCode = "NFK",
                    NumericIsoCode = 574,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Northern Mariana Islands",
                    
                    
                    TwoLetterIsoCode = "MP",
                    ThreeLetterIsoCode = "MNP",
                    NumericIsoCode = 580,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Oman",
                    
                    
                    TwoLetterIsoCode = "OM",
                    ThreeLetterIsoCode = "OMN",
                    NumericIsoCode = 512,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Palau",
                    
                    
                    TwoLetterIsoCode = "PW",
                    ThreeLetterIsoCode = "PLW",
                    NumericIsoCode = 585,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Panama",
                    
                    
                    TwoLetterIsoCode = "PA",
                    ThreeLetterIsoCode = "PAN",
                    NumericIsoCode = 591,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Papua New Guinea",
                    
                    
                    TwoLetterIsoCode = "PG",
                    ThreeLetterIsoCode = "PNG",
                    NumericIsoCode = 598,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Pitcairn",
                    
                    
                    TwoLetterIsoCode = "PN",
                    ThreeLetterIsoCode = "PCN",
                    NumericIsoCode = 612,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Reunion",
                    
                    
                    TwoLetterIsoCode = "RE",
                    ThreeLetterIsoCode = "REU",
                    NumericIsoCode = 638,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Rwanda",
                    
                    
                    TwoLetterIsoCode = "RW",
                    ThreeLetterIsoCode = "RWA",
                    NumericIsoCode = 646,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Saint Kitts and Nevis",
                    
                    
                    TwoLetterIsoCode = "KN",
                    ThreeLetterIsoCode = "KNA",
                    NumericIsoCode = 659,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Saint Lucia",
                    
                    
                    TwoLetterIsoCode = "LC",
                    ThreeLetterIsoCode = "LCA",
                    NumericIsoCode = 662,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Saint Vincent and the Grenadines",
                    
                    
                    TwoLetterIsoCode = "VC",
                    ThreeLetterIsoCode = "VCT",
                    NumericIsoCode = 670,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Samoa",
                    
                    
                    TwoLetterIsoCode = "WS",
                    ThreeLetterIsoCode = "WSM",
                    NumericIsoCode = 882,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "San Marino",
                    
                    
                    TwoLetterIsoCode = "SM",
                    ThreeLetterIsoCode = "SMR",
                    NumericIsoCode = 674,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sao Tome and Principe",
                    
                    
                    TwoLetterIsoCode = "ST",
                    ThreeLetterIsoCode = "STP",
                    NumericIsoCode = 678,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Senegal",
                    
                    
                    TwoLetterIsoCode = "SN",
                    ThreeLetterIsoCode = "SEN",
                    NumericIsoCode = 686,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Seychelles",
                    
                    
                    TwoLetterIsoCode = "SC",
                    ThreeLetterIsoCode = "SYC",
                    NumericIsoCode = 690,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sierra Leone",
                    
                    
                    TwoLetterIsoCode = "SL",
                    ThreeLetterIsoCode = "SLE",
                    NumericIsoCode = 694,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Solomon Islands",
                    
                    
                    TwoLetterIsoCode = "SB",
                    ThreeLetterIsoCode = "SLB",
                    NumericIsoCode = 90,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Somalia",
                    
                    
                    TwoLetterIsoCode = "SO",
                    ThreeLetterIsoCode = "SOM",
                    NumericIsoCode = 706,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "South Georgia & South Sandwich Islands",
                    
                    
                    TwoLetterIsoCode = "GS",
                    ThreeLetterIsoCode = "SGS",
                    NumericIsoCode = 239,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "South Sudan",
                    
                    
                    TwoLetterIsoCode = "SS",
                    ThreeLetterIsoCode = "SSD",
                    NumericIsoCode = 728,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sri Lanka",
                    
                    
                    TwoLetterIsoCode = "LK",
                    ThreeLetterIsoCode = "LKA",
                    NumericIsoCode = 144,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "St. Helena",
                    
                    
                    TwoLetterIsoCode = "SH",
                    ThreeLetterIsoCode = "SHN",
                    NumericIsoCode = 654,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "St. Pierre and Miquelon",
                    
                    
                    TwoLetterIsoCode = "PM",
                    ThreeLetterIsoCode = "SPM",
                    NumericIsoCode = 666,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sudan",
                    
                    
                    TwoLetterIsoCode = "SD",
                    ThreeLetterIsoCode = "SDN",
                    NumericIsoCode = 736,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Suriname",
                    
                    
                    TwoLetterIsoCode = "SR",
                    ThreeLetterIsoCode = "SUR",
                    NumericIsoCode = 740,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Svalbard and Jan Mayen Islands",
                    
                    
                    TwoLetterIsoCode = "SJ",
                    ThreeLetterIsoCode = "SJM",
                    NumericIsoCode = 744,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Swaziland",
                    
                    
                    TwoLetterIsoCode = "SZ",
                    ThreeLetterIsoCode = "SWZ",
                    NumericIsoCode = 748,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Syrian Arab Republic",
                    
                    
                    TwoLetterIsoCode = "SY",
                    ThreeLetterIsoCode = "SYR",
                    NumericIsoCode = 760,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tajikistan",
                    
                    
                    TwoLetterIsoCode = "TJ",
                    ThreeLetterIsoCode = "TJK",
                    NumericIsoCode = 762,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tanzania",
                    
                    
                    TwoLetterIsoCode = "TZ",
                    ThreeLetterIsoCode = "TZA",
                    NumericIsoCode = 834,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Togo",
                    
                    
                    TwoLetterIsoCode = "TG",
                    ThreeLetterIsoCode = "TGO",
                    NumericIsoCode = 768,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tokelau",
                    
                    
                    TwoLetterIsoCode = "TK",
                    ThreeLetterIsoCode = "TKL",
                    NumericIsoCode = 772,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tonga",
                    
                    
                    TwoLetterIsoCode = "TO",
                    ThreeLetterIsoCode = "TON",
                    NumericIsoCode = 776,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Trinidad and Tobago",
                    
                    
                    TwoLetterIsoCode = "TT",
                    ThreeLetterIsoCode = "TTO",
                    NumericIsoCode = 780,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tunisia",
                    
                    
                    TwoLetterIsoCode = "TN",
                    ThreeLetterIsoCode = "TUN",
                    NumericIsoCode = 788,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Turkmenistan",
                    
                    
                    TwoLetterIsoCode = "TM",
                    ThreeLetterIsoCode = "TKM",
                    NumericIsoCode = 795,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Turks and Caicos Islands",
                    
                    
                    TwoLetterIsoCode = "TC",
                    ThreeLetterIsoCode = "TCA",
                    NumericIsoCode = 796,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tuvalu",
                    
                    
                    TwoLetterIsoCode = "TV",
                    ThreeLetterIsoCode = "TUV",
                    NumericIsoCode = 798,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Uganda",
                    
                    
                    TwoLetterIsoCode = "UG",
                    ThreeLetterIsoCode = "UGA",
                    NumericIsoCode = 800,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Vanuatu",
                    
                    
                    TwoLetterIsoCode = "VU",
                    ThreeLetterIsoCode = "VUT",
                    NumericIsoCode = 548,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Vatican City State (Holy See)",
                    
                    
                    TwoLetterIsoCode = "VA",
                    ThreeLetterIsoCode = "VAT",
                    NumericIsoCode = 336,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Viet Nam",
                    
                    
                    TwoLetterIsoCode = "VN",
                    ThreeLetterIsoCode = "VNM",
                    NumericIsoCode = 704,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Virgin Islands (British)",
                    
                    
                    TwoLetterIsoCode = "VG",
                    ThreeLetterIsoCode = "VGB",
                    NumericIsoCode = 92,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Virgin Islands (U.S.)",
                    
                    
                    TwoLetterIsoCode = "VI",
                    ThreeLetterIsoCode = "VIR",
                    NumericIsoCode = 850,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Wallis and Futuna Islands",
                    
                    
                    TwoLetterIsoCode = "WF",
                    ThreeLetterIsoCode = "WLF",
                    NumericIsoCode = 876,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Western Sahara",
                    
                    
                    TwoLetterIsoCode = "EH",
                    ThreeLetterIsoCode = "ESH",
                    NumericIsoCode = 732,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Yemen",
                    
                    
                    TwoLetterIsoCode = "YE",
                    ThreeLetterIsoCode = "YEM",
                    NumericIsoCode = 887,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Zambia",
                    
                    
                    TwoLetterIsoCode = "ZM",
                    ThreeLetterIsoCode = "ZMB",
                    NumericIsoCode = 894,
                    
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Zimbabwe",
                    
                    
                    TwoLetterIsoCode = "ZW",
                    ThreeLetterIsoCode = "ZWE",
                    NumericIsoCode = 716,
                    
                    DisplayOrder = 100,
                    Published = true
                }
            };
            _countryRepository.Insert(countries);
        }

        protected virtual void InstallUsersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new UserRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = NopUserDefaults.AdministratorsRoleName
            };
            var crRegistered = new UserRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = NopUserDefaults.RegisteredRoleName
            };
            var crGuests = new UserRole
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = NopUserDefaults.GuestsRoleName
            };

            var userRoles = new List<UserRole>
            {
                crAdministrators,
                crRegistered,
                crGuests
            };

            _userRoleRepository.Insert(userRoles);

            //admin user
            var adminUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                
            };

            _userRepository.Insert(adminUser);
            //set default user name
            _genericAttributeService.SaveAttribute(adminUser, NopUserDefaults.FirstNameAttribute, "FirstName");
            _genericAttributeService.SaveAttribute(adminUser, NopUserDefaults.LastNameAttribute, "lastName");

            //set hashed admin password
            var userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            userRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));

            var userUserRoleMappingAdministrator = new UserUserRoleMapping
            {
                UserId = 1,
                UserRoleId = 1
            };

            var userUserRoleMappingRegistered = new UserUserRoleMapping
            {
                UserId = 1,
                UserRoleId = 2
            };

            var userUserRoleMapping = new List<UserUserRoleMapping>()
            {
                userUserRoleMappingAdministrator,
                userUserRoleMappingRegistered
            };

            _userUserRoleMapping.Insert(userUserRoleMapping);

            //search engine (crawler) built-in user
            var searchEngineUser = new User
            {
                Email = "builtin@search_engine_record.com",
                UserGuid = Guid.NewGuid(),
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = NopUserDefaults.SearchEngineUserName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                
            };

            //built-in user for background tasks
            var backgroundTaskUser = new User
            {
                Email = "builtin@background-task-record.com",
                UserGuid = Guid.NewGuid(),
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = NopUserDefaults.BackgroundTaskUserName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                
            };
        }

        protected virtual void InstallActivityLog(string defaultUserEmail)
        {
            //default user/user
            var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultUser == null)
                throw new Exception("Cannot load default user");

            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditCategory")),
                Comment = "Edited a category ('Computers')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditDiscount")),
                Comment = "Edited a discount ('Sample discount with coupon code')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditSpecAttribute")),
                Comment = "Edited a specification attribute ('CPU Type')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("AddNewProductAttribute")),
                Comment = "Added a new product attribute ('Some attribute')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("DeleteGiftCard")),
                Comment = "Deleted a gift card ('bdbbc0ef-be57')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
        }

        protected virtual void InstallSearchTerms()
        {
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 34,
                Keyword = "computer",
                
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 30,
                Keyword = "camera",
                
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 27,
                Keyword = "jewelry",
                
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 26,
                Keyword = "shoes",
                
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 19,
                Keyword = "jeans",
                
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 10,
                Keyword = "gift",
                
            });
        }

        protected virtual void InstallEmailAccounts()
        {
            var emailAccounts = new List<EmailAccount>
            {
                new EmailAccount
                {
                    Email = "test@mail.com",
                    DisplayName = "Site name",
                    Host = "smtp.mail.com",
                    Port = 25,
                    Username = "123",
                    Password = "123",
                    EnableSsl = false,
                    UseDefaultCredentials = false
                }
            };
            _emailAccountRepository.Insert(emailAccounts);
        }

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");

            var messageTemplates = new List<MessageTemplate>
            {
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserEmailValidationMessage,
                    Subject = "%Site.Name%. Email validation",
                    Body = $"<a href=\"%Site.URL%\">%Site.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To activate your account <a href=\"%User.AccountActivationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Site.Name%{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserEmailRevalidationMessage,
                    Subject = "%Site.Name%. Email validation",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Site.URL%\">%Site.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%!{Environment.NewLine}<br />{Environment.NewLine}To validate your new email address <a href=\"%User.EmailRevalidationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Site.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserPasswordRecoveryMessage,
                    Subject = "%Site.Name%. Password recovery",
                    Body = $"<a href=\"%Site.URL%\">%Site.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To change your password <a href=\"%User.PasswordRecoveryURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Site.Name%{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserWelcomeMessage,
                    Subject = "Welcome to %Site.Name%",
                    Body = $"We welcome you to <a href=\"%Site.URL%\"> %Site.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can now take part in the various services we have to offer you. Some of these services include:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.{Environment.NewLine}<br />{Environment.NewLine}Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{Environment.NewLine}<br />{Environment.NewLine}Order History - View your history of purchases that you have made with us.{Environment.NewLine}<br />{Environment.NewLine}Products Reviews - Share your opinions on products with our other users.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For help with any of our online services, please email the site-owner: <a href=\"mailto:%Site.Email%\">%Site.Email%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Site.Email%\">%Site.Email%</a>.{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserRegisteredNotification,
                    Subject = "%Site.Name%. New user registration",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Site.URL%\">%Site.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new user registered with your site. Below are the user's details:{Environment.NewLine}<br />{Environment.NewLine}Full name: %User.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %User.Email%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactUsMessage,
                    Subject = "%Site.Name%. Contact us",
                    Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                }
            };
            _messageTemplateRepository.Insert(messageTemplates);
        }

        protected virtual void InstallSettings(bool installSampleData)
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            settingService.SaveSetting(new PdfSettings
            {
                LogoPictureId = 0,
                LetterPageSizeEnabled = false,
                FontFileName = "FreeSerif.ttf",
            });

            settingService.SaveSetting(new CommonSettings
            {
                UseSystemEmailForContactUsForm = true,
                UseStoredProcedureForLoadingCategories = true,
                SitemapEnabled = true,
                SitemapPageSize = 200,
                DisplayJavaScriptDisabledWarning = false,
                UseFullTextSearch = false,
                FullTextMode = FulltextSearchMode.ExactMatch,
                Log404Errors = true,
                BreadcrumbDelimiter = "/",
                RenderXuaCompatible = false,
                XuaCompatibleValue = "IE=edge",
                BbcodeEditorOpenLinksInNewWindow = false,
                PopupForTermsOfServiceLinks = true,
                JqueryMigrateScriptLoggingActive = false,
                SupportPreviousNopcommerceVersions = true,
                UseResponseCompression = false,
                StaticFilesCacheControl = "public,max-age=604800"
            });

            settingService.SaveSetting(new SeoSettings
            {
                PageTitleSeparator = ". ",
                PageTitleSeoAdjustment = PageTitleSeoAdjustment.SitenameAfterPagename,
                DefaultTitle = "Your Site",
                DefaultMetaKeywords = string.Empty,
                DefaultMetaDescription = string.Empty,
                GenerateProductMetaDescription = true,
                ConvertNonWesternChars = false,
                AllowUnicodeCharsInUrls = true,
                CanonicalUrlsEnabled = false,
                QueryStringInCanonicalUrlsEnabled = false,
                WwwRequirement = WwwRequirement.NoMatter,
                //we disable bundling out of the box because it requires a lot of server resources
                EnableJsBundling = false,
                EnableCssBundling = false,
                TwitterMetaTags = true,
                OpenGraphMetaTags = true,
                ReservedUrlRecordSlugs = new List<string>
                {
                    "admin",
                    "install",
                    "recentlyviewedproducts",
                    "newproducts",
                    "compareproducts",
                    "clearcomparelist",
                    "setproductreviewhelpfulness",
                    "login",
                    "register",
                    "logout",
                    "cart",
                    "wishlist",
                    "emailwishlist",
                    "checkout",
                    "onepagecheckout",
                    "contactus",
                    "passwordrecovery",
                    "subscribenewsletter",
                    "blog",
                    "boards",
                    "inboxupdate",
                    "sentupdate",
                    "news",
                    "sitemap",
                    "search",
                    "config",
                    "eucookielawaccept",
                    "page-not-found",
                    //system names are not allowed (anyway they will cause a runtime error),
                    "con",
                    "lpt1",
                    "lpt2",
                    "lpt3",
                    "lpt4",
                    "lpt5",
                    "lpt6",
                    "lpt7",
                    "lpt8",
                    "lpt9",
                    "com1",
                    "com2",
                    "com3",
                    "com4",
                    "com5",
                    "com6",
                    "com7",
                    "com8",
                    "com9",
                    "null",
                    "prn",
                    "aux"
                },
                CustomHeadTags = string.Empty
            });

            settingService.SaveSetting(new AdminAreaSettings
            {
                DefaultGridPageSize = 15,
                PopupGridPageSize = 10,
                GridPageSizes = "10, 15, 20, 50, 100",
                RichEditorAdditionalSettings = null,
                RichEditorAllowJavaScript = false,
                RichEditorAllowStyleTag = false,
                UseRichEditorInMessageTemplates = false,
                UseIsoDateFormatInJsonResult = true,
                UseNestedSetting = true
            });

            settingService.SaveSetting(new LocalizationSettings
            {
                DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.Name == "English").Id,
                UseImagesForLanguageSelection = false,
                SeoFriendlyUrlsForLanguagesEnabled = false,
                AutomaticallyDetectLanguage = false,
                LoadAllLocaleRecordsOnStartup = true,
                LoadAllLocalizedPropertiesOnStartup = true,
                LoadAllUrlRecordsOnStartup = false,
                IgnoreRtlPropertyForAdminArea = false
            });

            settingService.SaveSetting(new UserSettings
            {
                UsernamesEnabled = false,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = "SHA512",
                PasswordMinLength = 6,
                UnduplicatedPasswordsNumber = 4,
                PasswordRecoveryLinkDaysValid = 7,
                PasswordLifetime = 90,
                FailedPasswordAllowedAttempts = 0,
                FailedPasswordLockoutMinutes = 30,
                UserRegistrationType = UserRegistrationType.Standard,
                AllowUsersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                ShowUsersLocation = false,
                ShowUsersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewUserRegistration = false,
                UserNameFormat = UserNameFormat.ShowFirstName,
                GenderEnabled = true,
                DateOfBirthEnabled = true,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                CompanyEnabled = true,
                StreetAddressEnabled = false,
                StreetAddress2Enabled = false,
                ZipPostalCodeEnabled = false,
                CityEnabled = false,
                CountyEnabled = false,
                CountyRequired = false,
                CountryEnabled = false,
                CountryRequired = false,
                StateProvinceEnabled = false,
                StateProvinceRequired = false,
                PhoneEnabled = false,
                FaxEnabled = false,
                AcceptPrivacyPolicyEnabled = false,
                OnlineUserMinutes = 20,
                SuffixDeletedUsers = false,
                EnteringEmailTwice = false,
                RequireRegistrationForDownloadableProducts = false,
                DeleteGuestTaskOlderThanMinutes = 1440
            });

            settingService.SaveSetting(new AddressSettings
            {
                CompanyEnabled = true,
                StreetAddressEnabled = true,
                StreetAddressRequired = true,
                StreetAddress2Enabled = true,
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true,
                CityEnabled = true,
                CityRequired = true,
                CountyEnabled = false,
                CountyRequired = false,
                CountryEnabled = true,
                StateProvinceEnabled = true,
                PhoneEnabled = true,
                PhoneRequired = true,
                FaxEnabled = true
            });

            settingService.SaveSetting(new MediaSettings
            {
                AvatarPictureSize = 120,
                ImageSquarePictureSize = 32,
                MaximumImageSize = 1980,
                DefaultImageQuality = 80,
                MultipleThumbDirectories = false,
                AzureCacheControlHeader = string.Empty
            });

            settingService.SaveSetting(new SiteInformationSettings
            {
                AllowUserToSelectTheme = false,
                DisplayMiniProfilerForAdminOnly = false,
                DisplayEuCookieLawWarning = false,
                FacebookLink = "http://www.facebook.com/nopCommerce",
                TwitterLink = "https://twitter.com/nopCommerce",
                YoutubeLink = "http://www.youtube.com/user/nopCommerce",
                GooglePlusLink = "https://plus.google.com/+nopcommerce",
            });

            settingService.SaveSetting(new ExternalAuthenticationSettings
            {
                RequireEmailValidation = false,
                AllowUsersToRemoveAssociations = true
            });

            settingService.SaveSetting(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false,
                Color1 = "#b9babe",
                Color2 = "#ebecee",
                Color3 = "#dde2e6"
            });

            settingService.SaveSetting(new SecuritySettings
            {
                ForceSslForAllPages = true,
                EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                AdminAreaAllowedIpAddresses = null,
                EnableXsrfProtectionForAdminArea = true,
                HoneypotEnabled = false,
                HoneypotInputName = "hpinput",
                AllowNonAsciiCharactersInHeaders = true
            });

            settingService.SaveSetting(new DateTimeSettings
            {
                DefaultTimeZoneId = string.Empty,
                AllowUsersToSetTimeZone = false
            });

            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            settingService.SaveSetting(new EmailAccountSettings
            {
                DefaultEmailAccountId = eaGeneral.Id
            });

            settingService.SaveSetting(new WidgetSettings
            {
                ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" }
            });

            settingService.SaveSetting(new DisplayDefaultMenuItemSettings
            {
                DisplayHomePageMenuItem = !installSampleData,
                DisplayContactUsMenuItem = !installSampleData
            });

            settingService.SaveSetting(new DisplayDefaultFooterItemSettings
            {
                DisplaySitemapFooterItem = true,
                DisplayContactUsFooterItem = true,
                DisplayUserInfoFooterItem = true,
                DisplayUserAddressesFooterItem = true,
            });

            settingService.SaveSetting(new CaptchaSettings
            {
                ReCaptchaDefaultLanguage = string.Empty,
                AutomaticallyChooseLanguage = true
            });
        }

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                //admin area activities
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttribute",
                    Enabled = true,
                    Name = "Add a new address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttributeValue",
                    Enabled = true,
                    Name = "Add a new address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAffiliate",
                    Enabled = true,
                    Name = "Add a new affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewBlogPost",
                    Enabled = true,
                    Name = "Add a new blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCampaign",
                    Enabled = true,
                    Name = "Add a new campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCategory",
                    Enabled = true,
                    Name = "Add a new category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCheckoutAttribute",
                    Enabled = true,
                    Name = "Add a new checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCountry",
                    Enabled = true,
                    Name = "Add a new country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCurrency",
                    Enabled = true,
                    Name = "Add a new currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUser",
                    Enabled = true,
                    Name = "Add a new user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserAttribute",
                    Enabled = true,
                    Name = "Add a new user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserAttributeValue",
                    Enabled = true,
                    Name = "Add a new user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserRole",
                    Enabled = true,
                    Name = "Add a new user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewDiscount",
                    Enabled = true,
                    Name = "Add a new discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewEmailAccount",
                    Enabled = true,
                    Name = "Add a new email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewGiftCard",
                    Enabled = true,
                    Name = "Add a new gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewLanguage",
                    Enabled = true,
                    Name = "Add a new language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewManufacturer",
                    Enabled = true,
                    Name = "Add a new manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureDimension",
                    Enabled = true,
                    Name = "Add a new measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureWeight",
                    Enabled = true,
                    Name = "Add a new measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewNews",
                    Enabled = true,
                    Name = "Add a new news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProduct",
                    Enabled = true,
                    Name = "Add a new product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProductAttribute",
                    Enabled = true,
                    Name = "Add a new product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSetting",
                    Enabled = true,
                    Name = "Add a new setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttribute",
                    Enabled = true,
                    Name = "Add a new specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStateProvince",
                    Enabled = true,
                    Name = "Add a new state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSite",
                    Enabled = true,
                    Name = "Add a new site"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTopic",
                    Enabled = true,
                    Name = "Add a new topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewReviewType",
                    Enabled = true,
                    Name = "Add a new review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendor",
                    Enabled = true,
                    Name = "Add a new vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendorAttribute",
                    Enabled = true,
                    Name = "Add a new vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendorAttributeValue",
                    Enabled = true,
                    Name = "Add a new vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWarehouse",
                    Enabled = true,
                    Name = "Add a new warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWidget",
                    Enabled = true,
                    Name = "Add a new widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttribute",
                    Enabled = true,
                    Name = "Delete an address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttributeValue",
                    Enabled = true,
                    Name = "Delete an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAffiliate",
                    Enabled = true,
                    Name = "Delete an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPost",
                    Enabled = true,
                    Name = "Delete a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPostComment",
                    Enabled = true,
                    Name = "Delete a blog post comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCampaign",
                    Enabled = true,
                    Name = "Delete a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCategory",
                    Enabled = true,
                    Name = "Delete category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCheckoutAttribute",
                    Enabled = true,
                    Name = "Delete a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCountry",
                    Enabled = true,
                    Name = "Delete a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCurrency",
                    Enabled = true,
                    Name = "Delete a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUser",
                    Enabled = true,
                    Name = "Delete a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserAttribute",
                    Enabled = true,
                    Name = "Delete a user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserAttributeValue",
                    Enabled = true,
                    Name = "Delete a user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserRole",
                    Enabled = true,
                    Name = "Delete a user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteDiscount",
                    Enabled = true,
                    Name = "Delete a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteEmailAccount",
                    Enabled = true,
                    Name = "Delete an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteGiftCard",
                    Enabled = true,
                    Name = "Delete a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteLanguage",
                    Enabled = true,
                    Name = "Delete a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteManufacturer",
                    Enabled = true,
                    Name = "Delete a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureDimension",
                    Enabled = true,
                    Name = "Delete a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureWeight",
                    Enabled = true,
                    Name = "Delete a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMessageTemplate",
                    Enabled = true,
                    Name = "Delete a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteNews",
                    Enabled = true,
                    Name = "Delete a news"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "DeleteNewsComment",
                    Enabled = true,
                    Name = "Delete a news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteOrder",
                    Enabled = true,
                    Name = "Delete an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeletePlugin",
                    Enabled = true,
                    Name = "Delete a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProduct",
                    Enabled = true,
                    Name = "Delete a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductAttribute",
                    Enabled = true,
                    Name = "Delete a product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductReview",
                    Enabled = true,
                    Name = "Delete a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReturnRequest",
                    Enabled = true,
                    Name = "Delete a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReviewType",
                    Enabled = true,
                    Name = "Delete a review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSetting",
                    Enabled = true,
                    Name = "Delete a setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttribute",
                    Enabled = true,
                    Name = "Delete a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStateProvince",
                    Enabled = true,
                    Name = "Delete a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSite",
                    Enabled = true,
                    Name = "Delete a site"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSystemLog",
                    Enabled = true,
                    Name = "Delete system log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTopic",
                    Enabled = true,
                    Name = "Delete a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendor",
                    Enabled = true,
                    Name = "Delete a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendorAttribute",
                    Enabled = true,
                    Name = "Delete a vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendorAttributeValue",
                    Enabled = true,
                    Name = "Delete a vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWarehouse",
                    Enabled = true,
                    Name = "Delete a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWidget",
                    Enabled = true,
                    Name = "Delete a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttribute",
                    Enabled = true,
                    Name = "Edit an address attribute"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttributeValue",
                    Enabled = true,
                    Name = "Edit an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAffiliate",
                    Enabled = true,
                    Name = "Edit an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditBlogPost",
                    Enabled = true,
                    Name = "Edit a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCampaign",
                    Enabled = true,
                    Name = "Edit a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCategory",
                    Enabled = true,
                    Name = "Edit category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCheckoutAttribute",
                    Enabled = true,
                    Name = "Edit a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCountry",
                    Enabled = true,
                    Name = "Edit a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCurrency",
                    Enabled = true,
                    Name = "Edit a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUser",
                    Enabled = true,
                    Name = "Edit a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserAttribute",
                    Enabled = true,
                    Name = "Edit a user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserAttributeValue",
                    Enabled = true,
                    Name = "Edit a user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserRole",
                    Enabled = true,
                    Name = "Edit a user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditDiscount",
                    Enabled = true,
                    Name = "Edit a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditEmailAccount",
                    Enabled = true,
                    Name = "Edit an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditGiftCard",
                    Enabled = true,
                    Name = "Edit a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditLanguage",
                    Enabled = true,
                    Name = "Edit a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditManufacturer",
                    Enabled = true,
                    Name = "Edit a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureDimension",
                    Enabled = true,
                    Name = "Edit a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureWeight",
                    Enabled = true,
                    Name = "Edit a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMessageTemplate",
                    Enabled = true,
                    Name = "Edit a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditNews",
                    Enabled = true,
                    Name = "Edit a news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditOrder",
                    Enabled = true,
                    Name = "Edit an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPlugin",
                    Enabled = true,
                    Name = "Edit a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProduct",
                    Enabled = true,
                    Name = "Edit a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductAttribute",
                    Enabled = true,
                    Name = "Edit a product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductReview",
                    Enabled = true,
                    Name = "Edit a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPromotionProviders",
                    Enabled = true,
                    Name = "Edit promotion providers"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReturnRequest",
                    Enabled = true,
                    Name = "Edit a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReviewType",
                    Enabled = true,
                    Name = "Edit a review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStateProvince",
                    Enabled = true,
                    Name = "Edit a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSite",
                    Enabled = true,
                    Name = "Edit a site"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTask",
                    Enabled = true,
                    Name = "Edit a task"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttribute",
                    Enabled = true,
                    Name = "Edit a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendor",
                    Enabled = true,
                    Name = "Edit a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendorAttribute",
                    Enabled = true,
                    Name = "Edit a vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendorAttributeValue",
                    Enabled = true,
                    Name = "Edit a vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWarehouse",
                    Enabled = true,
                    Name = "Edit a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTopic",
                    Enabled = true,
                    Name = "Edit a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWidget",
                    Enabled = true,
                    Name = "Edit a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Started",
                    Enabled = true,
                    Name = "User impersonation session. Started"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Finished",
                    Enabled = true,
                    Name = "User impersonation session. Finished"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportCategories",
                    Enabled = true,
                    Name = "Categories were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportManufacturers",
                    Enabled = true,
                    Name = "Manufacturers were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportProducts",
                    Enabled = true,
                    Name = "Products were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportStates",
                    Enabled = true,
                    Name = "States were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UninstallPlugin",
                    Enabled = true,
                    Name = "Uninstall a plugin"
                },
                //public site activities
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.ViewCategory",
                    Enabled = false,
                    Name = "Public site. View a category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.ViewManufacturer",
                    Enabled = false,
                    Name = "Public site. View a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.ViewProduct",
                    Enabled = false,
                    Name = "Public site. View a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.PlaceOrder",
                    Enabled = false,
                    Name = "Public site. Place an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.SendPM",
                    Enabled = false,
                    Name = "Public site. Send PM"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.ContactUs",
                    Enabled = false,
                    Name = "Public site. Use contact us form"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.AddToCompareList",
                    Enabled = false,
                    Name = "Public site. Add to compare list"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.AddToShoppingCart",
                    Enabled = false,
                    Name = "Public site. Add to shopping cart"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.AddToWishlist",
                    Enabled = false,
                    Name = "Public site. Add to wishlist"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.Login",
                    Enabled = false,
                    Name = "Public site. Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.Logout",
                    Enabled = false,
                    Name = "Public site. Logout"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.AddProductReview",
                    Enabled = false,
                    Name = "Public site. Add product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.AddNewsComment",
                    Enabled = false,
                    Name = "Public site. Add news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.AddBlogComment",
                    Enabled = false,
                    Name = "Public site. Add blog comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.AddForumTopic",
                    Enabled = false,
                    Name = "Public site. Add forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.EditForumTopic",
                    Enabled = false,
                    Name = "Public site. Edit forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.DeleteForumTopic",
                    Enabled = false,
                    Name = "Public site. Delete forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.AddForumPost",
                    Enabled = false,
                    Name = "Public site. Add forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.EditForumPost",
                    Enabled = false,
                    Name = "Public site. Edit forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicSite.DeleteForumPost",
                    Enabled = false,
                    Name = "Public site. Delete forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UploadNewPlugin",
                    Enabled = true,
                    Name = "Upload a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UploadNewTheme",
                    Enabled = true,
                    Name = "Upload a theme"
                }
            };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }

        protected virtual void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Nop.Services.Messages.QueuedMessagesSendTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "Nop.Services.Common.KeepAliveTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "Nop.Services.Users.DeleteGuestsTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "Nop.Services.Caching.ClearCacheTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Nop.Services.Logging.ClearLogTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Update currency exchange rates",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Nop.Services.Directory.UpdateExchangeRateTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false
                }
            };

            _scheduleTaskRepository.Insert(tasks);
        }

        protected virtual void InstallWarehouses()
        {
            var warehouse1address = new Address
            {
                Address1 = "21 West 52nd Street",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow
            };
            _addressRepository.Insert(warehouse1address);
            var warehouse2address = new Address
            {
                Address1 = "300 South Spring Stree",
                City = "Los Angeles",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90013",
                CreatedOnUtc = DateTime.UtcNow
            };
            _addressRepository.Insert(warehouse2address);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <param name="defaultUserPassword">Default user password</param>
        /// <param name="installSampleData">A value indicating whether to install sample data</param>
        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            InstallLanguages();
            InstallCountriesAndStates();
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallSettings(installSampleData);
            InstallUsersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallLocaleResources();
            InstallActivityLogTypes();
            InstallScheduleTasks();

            if (!installSampleData) 
                return;
            
            InstallWarehouses();
            InstallActivityLog(defaultUserEmail);
            InstallSearchTerms();
        }

        #endregion
    }
}