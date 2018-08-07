using Nop.Core.Domain.Users;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Common;
using System.Collections;
using System.Collections.Generic;
namespace Nop.Core
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current User
        /// </summary>
        User CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
