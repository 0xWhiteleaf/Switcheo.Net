using System;

namespace Switcheo.Net.Attributes
{
    public class AssetAttribute : Attribute
    {
        public string Id { get; set; }

        public AssetAttribute(string id)
        {
            this.Id = id;
        }
    }
}
