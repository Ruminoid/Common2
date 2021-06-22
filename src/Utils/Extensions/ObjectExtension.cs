using Newtonsoft.Json;

namespace Ruminoid.Common2.Utils.Extensions
{
    public static class ObjectExtension
    {
        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public static T CloneUsingJson<T>(this T source) =>
            JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(source),
                SerializerSettings);
    }
}
