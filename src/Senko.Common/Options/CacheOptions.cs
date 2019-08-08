namespace Senko.Framework.Options
{
    public class CacheOptions
    {
        public string Type { get; set; } = "Memory";

        public string ConnectionString { get; set; }

        public string Prefix { get; set; }
    }
}
