using System;
using Stensel.Configuration;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Stensel.Editor {
    public class TimeTableEditor : TableEditor {
        public TimeTableEditor(TimeTable table, Action changeCallback) : base(table, changeCallback) {
            settings.style.height = new StyleLength(new Length(Utils.lineHeight * 6, LengthUnit.Pixel));

            var timingField = new EnumField("Timing", table.timing);
            timingField.RegisterValueChangedCallback((e) => {
                table.timing = (TimeTable.Timing) e.newValue;
                changeCallback();
            });
            settings.Add(timingField);
            
            var timeFormat = new EnumField("Time Format", table.timeFormat);
            timeFormat.RegisterValueChangedCallback((e) => {
                table.timeFormat = (TimeTable.TimeFormat) e.newValue;
                changeCallback();
            });
            settings.Add(timeFormat);

            var unscaled = new Toggle("Unscaled Timing");
            unscaled.SetValueWithoutNotify(table.unscaled);
            unscaled.RegisterValueChangedCallback((e) => {
                table.unscaled = e.newValue;
                changeCallback();
            });
            settings.Add(unscaled);
        }
    }
}