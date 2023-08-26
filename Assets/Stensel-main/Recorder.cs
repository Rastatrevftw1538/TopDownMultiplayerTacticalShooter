using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Stensel.Configuration;
using Stensel.Data;
using UnityEngine;
using Attribute = Stensel.Configuration.Attribute;

namespace Stensel {
    public class Recorder : MonoBehaviour {
        public class RuntimeMetricsGroup {
            public MetricsGroup config;
            public Dictionary<Datum, object> data = new();
            public Dictionary<Table, LinkedList<IEnumerable<object>>> tables = new();
            public Dictionary<TimeTable, LinkedList<IEnumerable<object>>> timeTables = new();
            public double creationTime;
            public double creationTimeUnscaled;
        }

        public static LinkedList<RuntimeMetricsGroup> cache = new();
        public static Dictionary<MetricsGroup, RuntimeMetricsGroup> groups = new();

        private RuntimeMetricsGroup GetRuntimeMetricsGroup(MetricsGroup group) {
            if (groups.TryGetValue(group, out var runtimeGroup)) return runtimeGroup;

            runtimeGroup = new RuntimeMetricsGroup {
                config = group,
                creationTime = Time.timeAsDouble,
                creationTimeUnscaled = Time.unscaledTimeAsDouble
            };

            for (var i = 0; i < group.data.Count; i++) {
                switch (group.data[i].description.type) {
                    case Attribute.DataType.Text:
                        runtimeGroup.data.Add(group.data[i], "");
                        break;
                    case Attribute.DataType.WholeNumber:
                        runtimeGroup.data.Add(group.data[i], (long)0);
                        break;
                    case Attribute.DataType.RationalNumber:
                        runtimeGroup.data.Add(group.data[i], 0d);
                        break;
                    default:
                        Debug.LogError("Stensel: Unknown Data Type! Defaulting to whole number.");
                        runtimeGroup.data.Add(group.data[i], (long)0);
                        break;
                }
            }

            groups.Add(group, runtimeGroup);

            return runtimeGroup;
        }

        private static bool Compatible(string prefix, Attribute attr, object data, bool allowFiller = true) {
            if (allowFiller && (data is EmptyValue)) return true;
            
            switch (attr.type) {
                case Attribute.DataType.Text:
                    if (data is string) return true;
                    break;
                case Attribute.DataType.WholeNumber:
                    switch (data) {
                        case int or long:
                            return true;
                        case uint or ulong:
                            Debug.LogError("Stensel: Unsigned integers unsupported! Cast data to an int or long before attempting to record it.");
                            return false;
                    }

                    break;
                case Attribute.DataType.RationalNumber:
                    if (data is float or double) return true;
                    break;
                default:
                    Debug.LogError("Stensel: Unknown Data Type! Not recording...");
                    return false;
            }
            
            Debug.LogError($"Stensel: {prefix}/{attr.name} is a {attr.type}! You cannot record it using data with the type \"{data.GetType()}\"");
            return false;
        }

        public static void RecordDatum(MetricsGroup group, Datum datum, object data) {
            var runtimeGroup = Instance.GetRuntimeMetricsGroup(group);
            
            if (runtimeGroup.data.ContainsKey(datum)) {
                if (Compatible(group.name, datum.Attr, data)) runtimeGroup.data[datum] = data;
                return;
            }
            
            Debug.LogError($"Stensel: Metrics Group \"{group.name}\" does not contain datum \"{datum.Name}\"");
        }

        public static void RecordTableRow(MetricsGroup group, Table table, object[] data) {
            var runtimeGroup = Instance.GetRuntimeMetricsGroup(group);
            if (!group.tables.Contains(table)) {
                Debug.LogError($"Stensel: Metrics Group \"{group.name}\" does not contain table \"{table.Name}\"");
                return;
            }

            if (table.attributes.Count > data.Length) {
                Debug.LogError($"Stensel: {group.name}/{table.name} has {table.attributes.Count} but you only provided {data.Length} objects. If there are values you wish to not record, use {typeof(EmptyValue)} as padding.");
                return;
            }
            
            if (table.attributes.Count < data.Length) {
                Debug.LogError($"Stensel: {group.name}/{table.name} has {table.attributes.Count} but you provided {data.Length} objects.");
                return;
            }

            for (var i = 0; i < data.Length; i++) {
                var attr = table.attributes[i];
                if (!Compatible($"{group.name}/{attr.name}", attr, data[i])) return;
            }

            if (runtimeGroup.tables.TryGetValue(table, out var rows)) rows.AddLast(data);
            else {
                rows = new LinkedList<IEnumerable<object>>();
                rows.AddLast(data);
                runtimeGroup.tables.Add(table, rows);
            }

            while (table.capped && rows.Count > table.capacity) rows.RemoveFirst();
        }
        
        public static void RecordTimeTableRow(MetricsGroup group, TimeTable table, object[] data) {
            var runtimeGroup = Instance.GetRuntimeMetricsGroup(group);
            if (!group.timeTables.Contains(table)) {
                Debug.LogError($"Stensel: Metrics Group \"{group.name}\" does not contain table \"{table.Name}\"");
                return;
            }

            if (table.attributes.Count > data.Length) {
                Debug.LogError($"Stensel: {group.name}/{table.name} has {table.attributes.Count} attributes but you only provided {data.Length} objects. If there are values you wish to not record, use {typeof(EmptyValue)} as padding.");
                return;
            }
            
            if (table.attributes.Count < data.Length) {
                Debug.LogError($"Stensel: {group.name}/{table.name} has {table.attributes.Count} attributes but you provided {data.Length} objects.");
                return;
            }

            TimeSpan t;

            switch (table.timing) {
                case TimeTable.Timing.Universal:
                    t = DateTime.UtcNow - DateTime.UnixEpoch;
                    break;
                case TimeTable.Timing.GroupRelative:
                    var seconds = table.unscaled ? (Time.unscaledTimeAsDouble - runtimeGroup.creationTimeUnscaled) : (Time.timeAsDouble - runtimeGroup.creationTime);
                    t = TimeSpan.FromSeconds(seconds);
                    break;
                case TimeTable.Timing.GameRelative:
                    t = TimeSpan.FromSeconds(table.unscaled ? Time.unscaledTimeAsDouble : Time.timeAsDouble);
                    break;
                case TimeTable.Timing.SceneRelative:
                    t = TimeSpan.FromSeconds(Time.timeSinceLevelLoadAsDouble);
                    break;
                default:
                    Debug.LogError("Stensel: Invalid Timing! Recorded time value will be 0...");
                    t = TimeSpan.FromSeconds(0);
                    break;
            }

            double time;
            switch (table.timeFormat) {
                case TimeTable.TimeFormat.Milliseconds:
                    time = Math.Round(t.TotalMilliseconds);
                    break;
                case TimeTable.TimeFormat.Seconds:
                    time = t.TotalSeconds;
                    break;
                case TimeTable.TimeFormat.Minutes:
                    time = t.TotalMinutes;
                    break;
                case TimeTable.TimeFormat.Hours:
                    time = t.TotalHours;
                    break;
                default:
                    Debug.LogError("Stensel: Invalid Timing! Recorded time value will be in seconds");
                    time = t.TotalSeconds;
                    break;
            }

            var appended = new LinkedList<object>();
            appended.AddLast(Math.Round(time, 2));
            
            for (var i = 0; i < data.Length; i++) {
                var attr = table.attributes[i];
                if (!Compatible($"{group.name}/{attr.name}", attr, data[i])) return;
                appended.AddLast(data[i]);
            }

            if (runtimeGroup.timeTables.TryGetValue(table, out var rows)) rows.AddLast(appended);
            else {
                rows = new LinkedList<IEnumerable<object>>();
                rows.AddLast(appended);
                runtimeGroup.timeTables.Add(table, rows);
            }

            while (table.capped && rows.Count > table.capacity) rows.RemoveFirst();
        }

        public static void Save(MetricsGroup group) {
            if (!groups.TryGetValue(group, out var runtimeGroup)) {
                Debug.LogWarning($"Stensel: Metrics from {group.name} have either already been save or haven't been recorded yet. Ignoring call...");
                return;
            }

            cache.AddLast(runtimeGroup);
            groups.Remove(group);
        }
        
        public static void Clear(MetricsGroup group) {
            if (!groups.ContainsKey(group)) {
                Debug.LogWarning($"Stensel: Metrics from {group.name} have either already been reset or haven't been recorded. Ignoring call...");
                return;
            }
            
            groups.Remove(group);
        }

        private static Recorder _i;
        private static readonly object Lock = new object();
        protected static bool appIsQuitting;

        protected static Recorder Instance {
            get {
                if (appIsQuitting) {
                    Debug.LogWarning("Metrics instance cannot be made after application starts quitting.");
                    return null;
                }

                lock (Lock) {
                    if (_i != null) return _i;
                    _i = (Recorder)FindObjectOfType(typeof(Recorder));

                    if (_i != null) return _i;
                    
                    var singleton = new GameObject();
                    _i = singleton.AddComponent<Recorder>();
                    singleton.name = "Metrics Recorder";
                    Debug.Log("Metrics recording started...");

                    DontDestroyOnLoad(singleton);

                    return _i;
                }
            }
        }

        private class SerializedMetricsGroup {
            public string data;
            public List<(string, string)> tables = new();
        }

        private static string Sanatize(string input) {
            return Regex.Replace(input, @"[^\w ]", " ").Trim();
        }

        public void OnApplicationQuit() {
            appIsQuitting = true;

            foreach (var runtimeGroup in groups.Values) cache.AddLast(runtimeGroup);

            var metrics = new Dictionary<MetricsGroup, List<SerializedMetricsGroup>>();

            // serialize metrics
            foreach (var cached in cache) {
                var group = cached.config;
                var serialized = new SerializedMetricsGroup();

                serialized.data = SerializeData(cached);

                foreach (var table in cached.tables) {
                    serialized.tables.Add((table.Key.name, SerializeTable(table.Key, table.Value, false)));
                }
                
                foreach (var table in cached.timeTables) {
                    serialized.tables.Add((Sanatize(table.Key.name), SerializeTable(table.Key, table.Value, true)));
                }

                if (metrics.TryGetValue(group, out var list)) list.Add(serialized);
                else {
                    list = new List<SerializedMetricsGroup>();
                    list.Add(serialized);
                    metrics.Add(group, list);
                }
            }
            
            // save serialized data in documents folder
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var location =  Sanatize(Application.productName) + " Metrics";
            var root = Path.Combine(path, location);

            Directory.CreateDirectory(root);

            var timestamp = DateTime.UtcNow.ToString("yyyy-M-d H_mm_ss");
            var metricsFolder = Path.Combine(root, timestamp);

            Directory.CreateDirectory(metricsFolder);

            foreach (var group in metrics) {
                var metricsName = Sanatize(group.Key.name);
                var serialized = group.Value;
                
                switch (serialized.Count) {
                    case 0:
                        continue;
                    case 1:
                        SaveMetricsGroup(metricsFolder, metricsName, serialized[0]);
                        break;
                    default:
                        var groupPath = Path.Combine(metricsFolder, metricsName);
                        Directory.CreateDirectory(groupPath);

                        for (var i = 0; i < serialized.Count; i++) {
                            var elementPath = Path.Combine(groupPath, (i + 1).ToString());
                            Directory.CreateDirectory(elementPath);
                            
                            SaveMetricsGroup(elementPath, metricsName, serialized[i]);
                        }
                        break;
                }
            }
        }

        private static string AsString(object data) {
            if (data is not EmptyValue e) return data.ToString();

            return e switch {
                EmptyValue.None => "NONE",
                EmptyValue.Empty => string.Empty,
                EmptyValue.Null => "NULL",
                EmptyValue.NotApplicable => "NA",
                _ => string.Empty
            };
        }

        private static string SerializeData(RuntimeMetricsGroup metrics) {
            if (metrics.data.Count <= 0) return string.Empty;
            
            var dataLabels = new StringBuilder();
            var dataFormatted = new StringBuilder();
            var columnsLeft = metrics.data.Count;
            
            foreach (var datum in metrics.config.data) {
                dataLabels.Append(datum.Name);
                dataFormatted.Append(AsString(metrics.data[datum]));

                columnsLeft--;
                if (columnsLeft <= 0) continue;
                        
                dataLabels.Append("\t");
                dataFormatted.Append("\t");
            }

            return dataLabels.Append("\n").Append(dataFormatted).ToString();
        }

        private static string SerializeTable(Table table, LinkedList<IEnumerable<object>> data, bool addTimeColumn) {
            if (table.attributes.Count <= 0) return string.Empty;
            
            var serialized = new StringBuilder();

            if (addTimeColumn) {
                serialized.Append("Time");
                if (table.attributes.Count > 0) serialized.Append("\t");
            }
            for (var i = 0; i < table.attributes.Count; i++) {
                serialized.Append(table.attributes[i].name);
                
                if (i >= table.attributes.Count - 1) continue;

                serialized.Append("\t");
            }

            serialized.Append("\n");

            var rowsLeft = data.Count;
            foreach (var row in data) {
                IEnumerator<object> r = row.GetEnumerator();
                
                for (var hasNext = r.MoveNext(); hasNext;) {
                    var datum = r.Current;
                    hasNext = r.MoveNext();

                    serialized.Append(AsString(datum));

                    if (hasNext) serialized.Append("\t");
                }
                
                r.Dispose();
                rowsLeft--;
                
                if (rowsLeft <= 0) continue;

                serialized.Append("\n");
            }

            return serialized.ToString();
        }

        private static void SaveMetricsGroup(string path, string name, SerializedMetricsGroup group) {
            File.WriteAllText(Path.Combine(path, $"{name}.tsv"), group.data);

            foreach (var table in group.tables) {
                File.WriteAllText(Path.Combine(path, $"[{name}] {table.Item1}.tsv"), table.Item2);
            }
        }
    }
}