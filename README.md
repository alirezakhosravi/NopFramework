# ASP.net Core Development Framework
###### Derived from NopCommerce

By fast growing of NopCommerce usage and numerous developers who are familier with this e-commerce solution and based on this fact that the design and architecture of this platform is well formed and very easy to understand and develop, I've derived this fundumental developement framework from [V4.1 of NopCommerce](https://github.com/nopSolutions/nopCommerce).
This solution is a boilerplate for creating any .net Core web application.
I'll adding some other capabilities and components to this premitive solution in future versions.


Fill free to ask me any questions:alirezakhosravi [at] live.com


(beta version)

### For create searchable entity follow this code:
Your entity must implement ISearchable interface in Nop.Core solution.
```
public partial class Country : BaseEntity, ILocalizedEntity, ISearchable
{
    public Country()
    {
       Route = new RouteModel()
       {
           RouteUrl = "your route url when you want show detail of entity",
           RouteName = "if you predefine route, enter route name",
           Parameters = new List<ParameterType> { ParameterType.Id, ParameterType.Description, ParameterType.Name}
       };
    }
    
    [NotMapped]
    public RouteModel Route { get; }
        ...
        
     [Searchable(UseFor = ParameterType.Description)]
     public string ThreeLetterIsoCode { get; set; }
     
     [Searchable(UseFor = ParameterType.Id, Ignore = true)]
      public string TwoLetterIsoCode { get; set; }
}
```
You should use one of RoutUrl and RouteName at each time.
Parameter field must be used in route parameter, when you want pass parameter to route.

```
Route = new RouteModel()
{
   RouteUrl = "/Country/Detail?name={0}&isocode={1}",
   Parameters = new List<ParameterType> { ParameterType.Name, ParameterType.Description}
};
```
On above code 'Name value' replace with {0} and 'Description value' replace with {1}.

If you want to change some attribites by default parameter type, you can use SearchableAttribute above property and set UseFor parameter for this.

If you want to ignore property in search but use this property to search structure, you can set iqnore parameter as true. 

If you use SearchableAttribute above property without set parameter, this attribute only use to query string for search.
```
[Searchable]
public string ThreeLetterIsoCode { get; set; }
```
