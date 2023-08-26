using Stensel;
using Stensel.Configuration;
using UnityEngine;

public class MetricsGroupTest : MonoBehaviour {
    public MetricsGroup metrics;
    public DatumReference[] data;
    public TableReference[] tables;
    [Range(1, 5)]
    public int number = 1;
    [Range(1, 64)]
    public int rows = 1;

    private int _r = 0;
    private int _n = 0;
        
    public void Update() {
        foreach (var table in tables) {
            table.Append(Random.value, Random.value, Random.value, Random.value);
        }

        _r++;

        if (_r < rows) return;

        _r = 0;
        foreach (var datum in data) {
            datum.Set(Random.value);
        }
            
        Recorder.Save(metrics);

        _n++;

        if (_n >= number) enabled = false;
    }
}