using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stensel.Configuration {
    [System.Serializable]
    public class Table : IMetric {
        public string name;
        public string id = Guid.NewGuid().ToString();
        
        public List<Attribute> attributes = new();
        
        public bool capped;
        public int capacity;

        public string Name => name;
    }
}