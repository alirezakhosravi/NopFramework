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
        ParameterType UseFor { get; set; }
        bool Ignore { get; set; }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SearchableAttribute : Attribute, ISearchableAttribute
    {
        #region Properties
        public ParameterType UseFor { get; set; } = ParameterType.None;
        public bool Ignore { get; set; } = false;
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

    public enum ParameterType { None, Id, Name, Description }
}
