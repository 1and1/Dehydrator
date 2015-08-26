namespace Dehydrator.Sample.Models
{
    /// <summary>
    /// A configuration entry for a specific <see cref="Package"/>.
    /// </summary>
    public class PackageConfig : Entity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}