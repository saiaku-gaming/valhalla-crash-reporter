namespace ValhallaCrashReporter
{
    class BuildInfo
    {
        // Jenkins will replace this after checkout with a real one, there are probably better ways to do this but meh. 
        public const string CrashUrl = "http://localhost:8080/public/crash/add";
    }
}
