using System;
using System.IO;
using Stensel.Configuration;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Stensel.Editor {
    
    [ScriptedImporter(5, "stensel")]
    public class MetricsGroupImporter : ScriptedImporter {

        public override void OnImportAsset(AssetImportContext ctx) {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));
            
            string json;
            try {
                json = File.ReadAllText(ctx.assetPath);
            }
            catch (Exception exception) {
                ctx.LogImportError($"Could not read file '{ctx.assetPath}' ({exception})");
                return;
            }
            
            var metrics = ScriptableObject.CreateInstance<MetricsGroup>();
            
            try {
                JsonUtility.FromJsonOverwrite(json, metrics);
            }
            catch (Exception exception) {
                ctx.LogImportError($"Could not parse metrics group in JSON format from '{ctx.assetPath}' ({exception})");
                DestroyImmediate(metrics);
                return;
            }
            
            var metricsIcon = Utils.GetIcon("Profiler.NetworkOperations@2x");
            var tableIcon = Utils.GetIcon("Mesh Icon");
            var dataIcon = Utils.GetIcon("AudioMixerController On Icon");

            metrics.name = Path.GetFileNameWithoutExtension(assetPath);
            
            ctx.AddObjectToAsset("<root>", metrics, (Texture2D)metricsIcon);
            ctx.SetMainObject(metrics);

            for (var i = 0; i < metrics.data.Count; i++) {
                var datum = metrics.data[i];
                
                var datumRef = ScriptableObject.CreateInstance<DatumReference>();
                datumRef.group = metrics;
                datumRef.name = $"{metrics.name}/{datum.Name}";
                datumRef.id = datum.id;
                datumRef.index = i;
                
                ctx.AddObjectToAsset(datum.id, datumRef, (Texture2D)dataIcon);
            }

            for (var i = 0; i < metrics.tables.Count; i++) {
                var table = metrics.tables[i];
                
                var tableRef = ScriptableObject.CreateInstance<TableReference>();
                tableRef.group = metrics;
                tableRef.name = $"{metrics.name}/{table.name}";
                tableRef.id = table.id;
                tableRef.index = i;
                tableRef.isTimeTable = false;
                
                ctx.AddObjectToAsset(table.id, tableRef, (Texture2D)tableIcon);
            }
            
            for (var i = 0; i < metrics.timeTables.Count; i++) {
                var table = metrics.timeTables[i];
                
                var tableRef = ScriptableObject.CreateInstance<TableReference>();
                tableRef.group = metrics;
                tableRef.name = $"{metrics.name}/{table.name}";
                tableRef.id = table.id;
                tableRef.index = i;
                tableRef.isTimeTable = true;
                
                ctx.AddObjectToAsset(table.id, tableRef, (Texture2D)tableIcon);
            }
        }
        
        [MenuItem("Assets/Create/Stensel/Metrics Group", false, 1)]
        private static void CreateNewAsset() {
            var m = ScriptableObject.CreateInstance<MetricsGroup>();
            
            ProjectWindowUtil.CreateAssetWithContent(
                "Metrics.stensel",
                JsonUtility.ToJson(m, true)
            );
            
            DestroyImmediate(m);
        }
    }
}