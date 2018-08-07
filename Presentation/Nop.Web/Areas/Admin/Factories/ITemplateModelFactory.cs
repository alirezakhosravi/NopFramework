using Nop.Web.Areas.Admin.Models.Templates;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the template model factory
    /// </summary>
    public partial interface ITemplateModelFactory
    {
        /// <summary>
        /// Prepare templates model
        /// </summary>
        /// <param name="model">Templates model</param>
        /// <returns>Templates model</returns>
        TemplatesModel PrepareTemplatesModel(TemplatesModel model);
    }
}