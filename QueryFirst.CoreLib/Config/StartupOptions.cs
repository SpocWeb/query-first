namespace QueryFirst
{
    public class StartupOptions
    {
        public string SourcePath { get; set; }
        public bool Watch { get; set; }
        public string NewQueryName { get; set; }
        public bool? CreateConfig { get; set; }
        public bool? CreateRuntimeConnection { get; set; }
        public QfConfigModel StartupConfig { get; set; }
        public bool DidShowHelp { get; set; }
    }
}
