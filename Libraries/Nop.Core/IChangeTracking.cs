using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Core
{
    public interface IChangeTracking
    {
       [NotMapped]
        string SYS_CHANGE_VERSION { get; set; }
       [NotMapped]
        string SYS_CHANGE_CREATION_VERSION { get; set; }
       [NotMapped]
        string SYS_CHANGE_OPERATION { get; set; }
       [NotMapped]
        string SYS_CHANGE_COLUMNS { get; set; }
       [NotMapped]
        string SYS_CHANGE_CONTEXT { get; set; }
    }
}
