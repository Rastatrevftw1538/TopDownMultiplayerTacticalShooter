
namespace Stensel.Configuration {
    [System.Serializable]
    public class TimeTable : Table {

        public enum Timing {
            Universal,
            GroupRelative,
            GameRelative,
            SceneRelative,
        }
        public enum TimeFormat {
            Milliseconds,
            Seconds,
            Minutes,
            Hours,
        }

        public Timing timing;
        public bool unscaled = true;
        public TimeFormat timeFormat;
    }
}