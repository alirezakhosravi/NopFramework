namespace Nop.Core.Domain.Seo
{
    /// <summary>
    /// Represents a page title SEO adjustment
    /// </summary>
    public enum PageTitleSeoAdjustment
    {
        /// <summary>
        /// Pagename comes after storename
        /// </summary>
        PagenameAfterSitename = 0,

        /// <summary>
        /// Sitename comes after pagename
        /// </summary>
        SitenameAfterPagename = 10
    }
}
