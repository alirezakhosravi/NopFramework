using System;
using System.Linq;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the template model factory implementation
    /// </summary>
    public partial class TemplateModelFactory : ITemplateModelFactory
    {
        #region Fields

        #endregion

        #region Ctor

        public TemplateModelFactory()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare templates model
        /// </summary>
        /// <param name="model">Templates model</param>
        /// <returns>Templates model</returns>
        public virtual TemplatesModel PrepareTemplatesModel(TemplatesModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            
            return model;
        }

        #endregion
    }
}