using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Nop.Core
{
    public interface ISearchable
    {
        #region Properties
        [NotMapped]
        RouteModel Route { get; }
        #endregion
    }

    public interface ISearchableAttribute
    {
        #region Properties
        bool IsUseForName { get; set; }
        bool IsUseForDescription { get; set; }
        bool IsUseForId { get; set; }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SearchableAttribute : Attribute, ISearchableAttribute
    {
        #region Properties
        public bool IsUseForName { get; set; } = false;
        public bool IsUseForDescription { get; set; } = false;
        public bool IsUseForId { get; set; } = false;
        #endregion
    }

    public class RouteModel
    {
        #region ctor
        public RouteModel()
        {
            Parameters = new List<ParameterType>();
        }
        #endregion

        #region Properties
        public string RouteName { get; set; }

        public string RouteUrl { get; set; }

        public IList<ParameterType> Parameters { get; set; }
        #endregion

        #region Method
        public string GetRoute()
        {
            var route = RouteName ?? RouteUrl;
            return route;
        }
        #endregion
    }

    public enum ParameterType { Id, Name, Description }
}
