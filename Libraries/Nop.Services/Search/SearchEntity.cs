using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Data.Extensions;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Services.Search
{
    public class SearchEntity : ISearchEntity
    {
        #region Fields

        private readonly ITypeFinder _typeFinder;
        private readonly IDbContext _dbContext;
        private readonly IDictionary<string, ISearchable> _searchQueries;
        private readonly IDataProvider _dataProvider;

        #endregion

        #region ctor
        public SearchEntity(
            ITypeFinder typeFinder, 
            IDbContext dbContext,
            IDataProvider dataProvider)
        {
            this._typeFinder = typeFinder;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._searchQueries = this.CreateSearchQueries();
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

        private IDictionary<string, ISearchable> CreateSearchQueries()
        {
            var types = this.GetSearchablesEntity();
            var model = new Dictionary<string, ISearchable>();
            string query = string.Empty;

            foreach (var type in types)
            {
                query = string.Empty;
                var attributes = GetAttributes(type);

                bool hasName = attributes.Any(e => e.Value.UseFor == ParameterType.Name) || type.GetProperty("Name") != null;
                bool hasDescription = attributes.Any(e => e.Value.UseFor == ParameterType.Description) || type.GetProperty("Description") != null;
                bool hasId = attributes.Any(e => e.Value.UseFor == ParameterType.Id) || type.GetProperty("Id") != null;

                if (!hasName && !hasDescription) continue;

                var entity = Activator.CreateInstance(type) as ISearchable;

                //create column
                var column = string.Empty;

                if (hasId)
                {
                    column = $"{attributes.FirstOrDefault(e => e.Value.UseFor == ParameterType.Id).Key ?? "Id" } AS Id";
                }
                else
                {
                    column = "' ' AS Id";
                }

                if (hasName)
                {
                    column += $", {attributes.FirstOrDefault(e => e.Value.UseFor == ParameterType.Name).Key ?? "Name"} AS Name";
                }
                else
                {
                    column += ", ' ' AS Name";
                }

                if (hasDescription)
                {
                    column += $", {attributes.FirstOrDefault(e => e.Value.UseFor == ParameterType.Description).Key ?? "Description"} AS Description";
                }
                else
                {
                    column += ", ' ' AS Description";
                }

                column += $", '{type.Name}' AS TableName, '{entity.Route.GetRoute()}' AS Route";

                //create whereclouse
                var where = string.Empty;

                if (!attributes.Any(e => e.Value.UseFor == ParameterType.Name) && type.GetProperty("Name") != null)
                {
                    where = $"{where} Name LIKE N'%'+@q+'%'";
                }

                if (!attributes.Any(e => e.Value.UseFor == ParameterType.Description) && type.GetProperty("Description") != null)
                {
                    if (!string.IsNullOrEmpty(where))
                    {
                        where = $"{where} OR ";
                    }

                    where = $"{where} Description LIKE N'%'+@q+'%'";
                }

                foreach (var attribute in attributes.Where(e => !e.Value.Ignore))
                {
                    if (!string.IsNullOrEmpty(where))
                    {
                        where = $"{where} OR ";
                    }

                    where = $"{where} {attribute.Key} LIKE N'%'+@q+'%' ";
                }

                query += $"SELECT {column} FROM {this._dbContext.GetTableNameByType(type)} WHERE {where}";

                model.Add(query, entity);
            }

            return model;
        }

        #endregion


        #region Method

        public IList<SearchResult> GetSearchResults(string q)
        {
            var model = new List<SearchResult>();
            var param = this._dataProvider.GetStringParameter("@q", q);
            foreach (var query in this._searchQueries)
            {
                var dbValues = this._dbContext.DynamicSqlQuery<SearchResult>(query.Key, System.Data.CommandType.Text , param).ToList() 
                                   ?? new List<SearchResult>();
                
                dbValues = dbValues.Select(e => new SearchResult
                {
                    Id = e.Id,
                    Name = e.Name,
                    TableName = e.TableName,
                    Description = e.Description,
                    Route = CreateRoute(e, query.Value.Route.Parameters)
                }).ToList();

                model.AddRange(dbValues);
            }

            return model;
        }
        #endregion
    }
}
