using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Services.Search
{
    public class SearchEntity : ISearchEntity
    {
        #region Fields

        private readonly ITypeFinder _typeFinder;

        #endregion

        #region ctor
        public SearchEntity(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }
        #endregion


        #region Utilities

        private IList<Type> GetSearchablesEntity()
        {
            var types = _typeFinder.FindClassesOfType<ISearchable>(_typeFinder.GetAssemblies().Where(e => e.GetName().ToString().ToLower().Contains("nop.core"))).ToList();

            return types;
        }

        public IDictionary<string, ISearchableAttribute> GetAttributes(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            var attributes = new Dictionary<string, ISearchableAttribute>();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    SearchableAttribute searchableAttribute = attr as SearchableAttribute;
                    if (searchableAttribute != null)
                    {
                        attributes.Add(prop.Name, searchableAttribute);
                    }
                }

            }

            return attributes;
        }

        private string CreateRoute(SearchResult result, IList<ParameterType> parameters)
        {
            var route = result.Route;
            if (parameters.Any())
            {
                var param = new List<string>();
                foreach (var item in parameters)
                {
                    switch (item)
                    {
                        case ParameterType.Id: param.Add($"{result.Id}"); break;
                        case ParameterType.Name: param.Add($"{result.Name}"); break;
                        case ParameterType.Description: param.Add($"{result.Description}"); break;
                        default: param.Add(""); break;
                    }
                }
                route = string.Format(route, param.ToArray());
            }
            return route;
        }

        #endregion


        #region Method

        public IList<SearchResult> GetSearchResults(IDbContext dbContext, string q)
        {
            var types = this.GetSearchablesEntity();
            var model = new List<SearchResult>();
            string query = string.Empty;

            foreach (var type in types)
            {
                query = string.Empty;
                var attributes = GetAttributes(type);

                bool hasName = attributes.Any(e => e.Value.IsUseForName) || type.GetProperty("Name") != null;
                bool hasDescription = attributes.Any(e => e.Value.IsUseForDescription) || type.GetProperty("Description") != null;
                bool hasId = attributes.Any(e => e.Value.IsUseForId) || type.GetProperty("Id") != null;

                if (!hasName && !hasDescription) continue;

                var entity = Activator.CreateInstance(type) as ISearchable;

                //create column
                var column = (hasId ? $"{attributes.FirstOrDefault(e => e.Value.IsUseForId).Key ?? "Id" } AS Id" : "'' AS Name")
                    + "," + (hasName ? $"{attributes.FirstOrDefault(e => e.Value.IsUseForName).Key ?? "Name"} AS Name" : "'' AS Name")
                    + "," + (hasDescription ? $"{attributes.FirstOrDefault(e => e.Value.IsUseForDescription).Key ?? "Description"} AS Description" : "'' AS Description")
                    + $",'{entity.Route.GetRoute()}' AS Route";

                //create whereclouse
                var where = string.Empty;

                foreach (var attribute in attributes)
                {
                    if (!string.IsNullOrEmpty(where))
                    {
                        where = $"{where} OR ";
                    }

                    where = $"{where} {attribute.Key} LIKE N'%{q}%' ";
                }

                query += $"SELECT Id, '{type.Name}' AS TableName, {column} FROM {dbContext.GetTableNameByType(type)} WHERE {where}";
                var dbValues = dbContext.DynamicSqlQuery<SearchResult>(query).ToList() ?? new List<SearchResult>();

                dbValues = dbValues.Select(e => new SearchResult
                {
                    Id = e.Id,
                    Name = e.Name,
                    TableName = e.TableName,
                    Description = e.Description,
                    Route = CreateRoute(e, entity.Route.Parameters)
                }).ToList();

                model.AddRange(dbValues);
            }

            return model;
        }
        #endregion
    }
}
