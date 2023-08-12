using System;

namespace Stensel.Configuration {
    [System.Serializable]
    public class Datum : IMetric, IAttributeWrapper {
        public Attribute description = new();
        public string id = Guid.NewGuid().ToString();


        public string Name => description.name;
        public Attribute Attr => description;
    }
}