using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Nop.Core
{
    public interface ISearchable
    {
        [NotMapped]
        RouteModel Route { get; }
    }

    public interface ISearchableAttribute
    {
    }

    public class RouteModel
    {
        #region ctor
        public RouteModel()
        {
            Parameters = new List<string>();
        }
        #endregion

        #region Properties
        public string RouteName { get; set; }

        public string RouteUrl { get; set; }

        public List<string> Parameters { get; set; }
        #endregion

        #region Method
        public string GetRoute()
        {
            var route = RouteName ?? RouteUrl;
            return route;
        }
        #endregion
    }
}
