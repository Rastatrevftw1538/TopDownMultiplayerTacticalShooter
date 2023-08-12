using System;
using UnityEngine;

namespace Stensel.Configuration {
    public class TableReference : ScriptableObject {
        public MetricsGroup group;
        public string id;
        public bool isTimeTable;
        public int index = -1;

        public void Append(params object[] row) {
            if (index < 0) {
                Debug.LogError($"Stensel: Unable to obtain reference to table in Metrics Group \"{@group.name}\"!");
                return;
            }
            
            if (isTimeTable) Recorder.RecordTimeTableRow(group, group.timeTables[index], row);
            else Recorder.RecordTableRow(group, group.tables[index], row);
        }
    }
}