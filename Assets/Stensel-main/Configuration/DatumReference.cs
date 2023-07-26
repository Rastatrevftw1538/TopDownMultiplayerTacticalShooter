using System;
using UnityEngine;

namespace Stensel.Configuration {
    public class DatumReference : ScriptableObject {
        public MetricsGroup group;
        public string id;
        public int index = -1;


        public void Set(object datum) {
            if (index < 0) {
                Debug.LogError($"Stensel: Unable to obtain reference to datum in Metrics Group \"{@group.name}\"!");
                return;
            }
            
            Recorder.RecordDatum(group, group.data[index], datum);
        }
    }
}