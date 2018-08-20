using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Services.Search
{
    /// <summary>
    /// search service
    /// </summary>
    public partial class SearchService : ISearchService
    {
        #region Fields

        private readonly ISearchEntity _searchEntity;
        private readonly IDbContext _dbContext;

        #endregion

        #region Ctor

        public SearchService(ISearchEntity searchEntity,
                             IDbContext dbContext)
        {
            this._searchEntity = searchEntity;
            this._dbContext = dbContext;
        }

        #endregion

        #region Utilities

        private string CreateRoute(SearchResult result, List<string> parameters)
        {
            var route = result.Route;
            if (parameters.Any())
            {
                var param = new List<string>();
                foreach (var item in parameters)
                {
                    switch (item.ToLower())
                    {
                        case "id": param.Add($"{result.Id}"); break;
                        case "name": param.Add($"{result.Name}"); break;
                        case "description": param.Add($"{result.Description}"); break;
                        default: param.Add(""); break;
                    }
                }
                route = string.Format(route, param.ToArray());
            }
            return route;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets the seatch result.
        /// </summary>
        /// <returns>The seatch result.</returns>
        /// <param name="q">The search value.</param>
        public IList<SearchResult> GetSeatchResult(string q)
        {
            var types = _searchEntity.GetSearchablesEntity();
            var model = new List<SearchResult>();
            string query = string.Empty;

            foreach (var type in types)
            {
                query = string.Empty;
                bool hasName = type.GetProperty("Name") != null;
                bool hasDescription = type.GetProperty("Description") != null;

                if (!hasName && !hasDescription) continue;

                var entity = Activator.CreateInstance(type) as ISearchable;

                var column = (hasName ? "Name" : "'' AS Name") 
                    + "," + (hasDescription ? "Description" : "'' AS Description") 
                    + $",'{entity.Route.GetRoute()}' AS Route";
                
                var where = hasName ? $"Name LIKE N'%{q}%'" : string.Empty;
                if (hasDescription)
                {
                    if (!string.IsNullOrEmpty(where))
                    {
                        where = $"{where} OR ";
                    }

                    where = $"{where} Description LIKE N'%{q}%'";
                }

                query += $"SELECT Id, '{type.Name}' AS TableName, {column} FROM {_dbContext.GetTableNameByType(type)} WHERE {where}";
                var dbValues = _dbContext.DynamicSqlQuery<SearchResult>(query).ToList() ?? new List<SearchResult>();

                if(entity.Route.Parameters.Any())
                {
                    dbValues = dbValues.Select(e => new SearchResult
                    {
                        Id = e.Id,
                        Name = e.Name,
                        TableName = e.TableName,
                        Description = e.Description,
                        Route = CreateRoute(e, entity.Route.Parameters)
                    }).ToList();
                }

                model.AddRange(dbValues);
            }

            return model;
        }
        #endregion
    }
}