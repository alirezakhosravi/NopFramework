using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// CAPTCHA settings
    /// </summary>
    public class CaptchaSettings : ISettings
    {
        /// <summary>
        /// Is CAPTCHA enabled?
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the login page
        /// </summary>
        public bool ShowOnLoginPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the registration page
        /// </summary>
        public bool ShowOnRegistrationPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the contacts page
        /// </summary>
        public bool ShowOnContactUsPage { get; set; }
        /// <summary>
        /// reCAPTCHA public key
        /// </summary>
        public string ReCaptchaPublicKey { get; set; }
        /// <summary>
        /// reCAPTCHA private key
        /// </summary>
        public string ReCaptchaPrivateKey { get; set; }        
        /// <summary>
        /// reCAPTCHA theme
        /// </summary>
        public string ReCaptchaTheme { get; set; }
        /// <summary>
        /// reCAPTCHA default language
        /// </summary>
        public string ReCaptchaDefaultLanguage { get; set; }
        /// <summary>
        /// A value indicating whether reCAPTCHA language should be set automatically
        /// </summary>
        public bool AutomaticallyChooseLanguage { get; set; }
    }
}