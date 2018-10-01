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

If you want to change some propertis by default parameters type, you can use SearchableAttribute above property and set UseFor parameter for this. In this case, ThreeLetterIsoCode use for Description on search structure.
```
[Searchable(UseFor = ParameterType.Description)]
public string ThreeLetterIsoCode { get; set; }
```

If you want to ignore property in search but use this property to search structure, you can set iqnore parameter as true. 
```
[Searchable(UseFor = ParameterType.Id, Ignore = true)]
public string TwoLetterIsoCode { get; set; }
```

If you use SearchableAttribute above property without set parameter, this attribute only use to query string for search.
```
[Searchable]
public string ThreeLetterIsoCode { get; set; }
```

### For create listener to get notification follow this code:
You class must implement INotificationObserver interface in Nop.Services solution
```
using Nop.Services.Notifications;
....

 public class WebNotificationObserver : INotificationObserver
 {
    private readonly INotificationHandler _notificationHandler;
    private readonly IQueuedNotificationService _queuedNotificationService;
    
    public WebNotificationObserver(
        INotificationHandler notificationHandler, 
        IQueuedNotificationService queuedNotificationService
    )
    {
         _notificationHandler = notificationHandler;
         _queuedNotificationService = queuedNotificationService;
         
         _notificationHandler.Register(this);
    }
    
    public string Identifier => "WebNotificationObserver";
    
    public INotificationHandler Handler
    {
        get => _notificationHandler;
        set => throw new NotImplementedException();
    }
     
    public void Notify(QueuedNotification message)
    {
        if (!message.IsCheckMessage(this.Identifier))
        {
            ...
            message.AddObserver(this.Identifier);
            _queuedNotificationService.UpdateQueuedNotification(message);
        }
    } 
 }
```

Identifier is very important beacause when your listener observed message,it puts it's own identifier on message so if later get this message, do not process it.
