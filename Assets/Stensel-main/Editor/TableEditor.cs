using Stensel.Configuration;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Stensel.Editor {
    public class TableEditor : VisualElement {
        protected VisualElement settings;
        protected AttributeList<Attribute> attributes;

        public TableEditor(Table table, System.Action changeCallback) {
            style.marginTop = new StyleLength(new Length(2, LengthUnit.Pixel));
            style.maxWidth = new StyleLength(new Length(100, LengthUnit.Percent));
            style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            style.flexGrow = new StyleFloat(1f);
            style.flexShrink = new StyleFloat(0f);
            
            settings = new VisualElement {
                style = {
                    height = new StyleLength(new Length(Utils.lineHeight * 3, LengthUnit.Pixel))
                }
            };

            var nameField = new TextField("Name");
            nameField.SetValueWithoutNotify(table.name);
            nameField.RegisterValueChangedCallback((e) => {
                table.name = e.newValue;
                changeCallback();
            });

            var capacity = new IntegerField("Capacity");
            capacity.SetValueWithoutNotify(table.capacity);
            capacity.RegisterValueChangedCallback((e) => {
                table.capacity = e.newValue;
                changeCallback();
            });
            
            var cappedField = new Toggle("Capped");
            cappedField.SetValueWithoutNotify(table.capped);
            cappedField.RegisterValueChangedCallback((e) => {
                table.capped = e.newValue;
                capacity.isReadOnly = !e.newValue;
                changeCallback();
            });
            
            settings.Add(nameField);
            settings.Add(cappedField);
            settings.Add(capacity);

            attributes = new AttributeList<Attribute>(table.attributes) {
                style = { maxHeight = new StyleLength(new Length(100, LengthUnit.Percent))}
            };
            
            var background = EditorGUIUtility.isProSkin ? 0.15f : 0.7f;
            var divColor = new Color(background, background, background, 1f);
            var divider = new VisualElement {
                style = {
                    height = new StyleLength(new Length(8, LengthUnit.Pixel)),
                    backgroundColor = new StyleColor(divColor)
                }
            };
            
            Add(settings);
            Add(divider);
            Add(attributes);
        }
    }
}