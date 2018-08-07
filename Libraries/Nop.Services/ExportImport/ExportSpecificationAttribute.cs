namespace Nop.Services.ExportImport
{
    public class ExportSpecificationAttribute
    {
        public int AttributeTypeId { get; set; }
        public bool AllowFiltering { get; set; }
        public int SpecificationAttributeOptionId { get; set; }
        public int SpecificationAttributeId { get; set; }
    }
}