using System.Collections.Generic;

namespace Stensel.Configuration {
    [System.Serializable]
    public class Attribute : IAttributeWrapper {
        public enum DataType {
            Text,
            WholeNumber,
            RationalNumber,
        }

        public string name;
        public DataType type = DataType.WholeNumber;

        public Attribute Attr => this;
    }
}