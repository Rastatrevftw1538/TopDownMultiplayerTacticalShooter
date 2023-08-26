using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Attribute = Stensel.Configuration.Attribute;

namespace Stensel.Editor {
    public class AttributeElement : VisualElement {
        private Attribute _attribute;
        private TextField _name;
        private EnumField _dataType;

        public AttributeElement() {
            style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            style.flexDirection = FlexDirection.Row;

            _name = new TextField {
                style = {
                    minWidth = new StyleLength(new Length(100, LengthUnit.Pixel))
                }
            };
            _name.RegisterValueChangedCallback(ChangeName);

            _dataType = new EnumField( Attribute.DataType.Text);
            _dataType.RegisterValueChangedCallback(ChangeDataType);

            Add(_name);
            Add(_dataType);
        }

        public void Bind(Attribute attribute) {
            _attribute = attribute;
            _name.SetValueWithoutNotify(_attribute.name);
            _dataType.SetValueWithoutNotify(_attribute.type);
        }

        private void ChangeDataType(ChangeEvent<Enum> e) {
            _attribute.type = (Attribute.DataType) e.newValue;
        }

        private void ChangeName(ChangeEvent<string> e) {
            _attribute.name = e.newValue;
        }
    }
}