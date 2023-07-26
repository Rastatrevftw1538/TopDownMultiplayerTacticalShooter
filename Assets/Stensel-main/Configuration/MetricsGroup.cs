using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stensel.Configuration {
    public class MetricsGroup : ScriptableObject {
        public List<Datum> data = new();
        public List<Table> tables = new();
        public List<TimeTable> timeTables = new();

        public void Save() {
            Recorder.Save(this);
        }

        public void Clear() {
            Recorder.Clear(this);
        }
    }
}