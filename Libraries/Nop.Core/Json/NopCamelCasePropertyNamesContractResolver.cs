using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nop.Core.Json
{
    public class NopCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            ModifyProperty(member, property);

            return property;
        }

        protected virtual void ModifyProperty(MemberInfo member, JsonProperty property)
        {
            // custom rule for modify property
        }
    }
}
