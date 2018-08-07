using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using OfficeOpenXml;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Constants

        //it's quite fast hash (to cheaply distinguish between objects)
        private const string IMAGE_HASH_ALGORITHM = "SHA1";

        private const string UPLOADS_TEMP_PATH = "~/App_Data/TempUploads";

        #endregion

        #region Fields

        private readonly ICountryService _countryService;
        private readonly IDataProvider _dataProvider;
        private readonly IEncryptionService _encryptionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly INopFileProvider _fileProvider;
        private readonly IPictureService _pictureService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public ImportManager(ICountryService countryService,
            IDataProvider dataProvider,
            IEncryptionService encryptionService,
            ILocalizationService localizationService,
            ILogger logger,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INopFileProvider fileProvider,
            IPictureService pictureService,
            IServiceScopeFactory serviceScopeFactory,
            IStateProvinceService stateProvinceService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            this._countryService = countryService;
            this._dataProvider = dataProvider;
            this._encryptionService = encryptionService;
            this._fileProvider = fileProvider;
            this._localizationService = localizationService;
            this._logger = logger;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._pictureService = pictureService;
            this._serviceScopeFactory = serviceScopeFactory;
            this._stateProvinceService = stateProvinceService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        private static ExportedAttributeType GetTypeOfExportedAttribute(ExcelWorksheet worksheet, PropertyManager<ExportSpecificationAttribute> specificationAttributeManager, int iRow)
        {
            specificationAttributeManager.ReadFromXlsx(worksheet, iRow);

            if (specificationAttributeManager.IsCaption)
            {
                return ExportedAttributeType.SpecificationAttribute;
            }

            return ExportedAttributeType.NotSpecified;
        }

        private static void SetOutLineForSpecificationAttributeRow(object cellValue, ExcelWorksheet worksheet, int endRow)
        {
            var attributeType = (cellValue ?? string.Empty).ToString();

            if (attributeType.Equals("AttributeType", StringComparison.InvariantCultureIgnoreCase))
            {
                worksheet.Row(endRow).OutlineLevel = 1;
            }
        }

        private static void CopyDataToNewFile(ExcelWorksheet worksheet, string filePath, int startRow, int endRow, int endCell)
        {
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handles to the worksheets
                    var outRow = 2;
                    for (var row = startRow; row <= endRow; row++)
                    {
                        outRow += 1;
                    }

                    xlPackage.Save();
                }
            }
        }

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            for (var i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);
            
            //set to jpeg in case mime type cannot be found
            return mimeType ?? MimeTypes.ImageJpeg;
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>The image or null if the image has not changed</returns>
        protected virtual Picture LoadPicture(string picturePath, string name, int? picId = null)
        {
            if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath))
                return null;

            var mimeType = GetMimeTypeFromFilePath(picturePath);
            var newPictureBinary = _fileProvider.ReadAllBytes(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing product pictures
                var existingPicture = _pictureService.GetPictureById(picId.Value);
                if (existingPicture != null)
                {
                    var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                    //picture binary after validation (like in database)
                    var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                    if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                        existingBinary.SequenceEqual(newPictureBinary))
                    {
                        pictureAlreadyExists = true;
                    }
                }
            }

            if (pictureAlreadyExists) return null;

            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(name));
            return newPicture;
        }

        private void LogPictureInsertError(string picturePath, Exception ex)
        {
            var extension = _fileProvider.GetFileExtension(picturePath);
            var name = _fileProvider.GetFileNameWithoutExtension(picturePath);

            var point = string.IsNullOrEmpty(extension) ? string.Empty : ".";
            var fileName = _fileProvider.FileExists(picturePath) ? $"{name}{point}{extension}" : string.Empty;
            _logger.Error($"Insert picture failed (file name: {fileName})", ex);
        }


        private string DownloadFile(string urlString, IList<string> downloadedFiles)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;

            if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
                return urlString;
            
            //ensure that temp directory is created
            var tempDirectory = _fileProvider.MapPath(UPLOADS_TEMP_PATH);
            _fileProvider.CreateDirectory(tempDirectory);

            var fileName = _fileProvider.GetFileName(urlString);
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var filePath = _fileProvider.Combine(tempDirectory, fileName);
            try
            {
                WebRequest.Create(urlString);
            }
            catch
            {
                return string.Empty;
            }

            try
            {
                byte[] fileData;
                using (var client = new WebClient())
                {
                    fileData = client.DownloadData(urlString);
                }

                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }

                downloadedFiles?.Add(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Error("Download image failed", ex);
            }

            return string.Empty;
        }

        #endregion

        #region Methods

        #endregion

        #region Nested classes


        #endregion
    }
}