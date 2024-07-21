namespace JPC.Common
{
    public interface IEnvironment
    {
        string CommandLine { get; }
        void Exit(int exitCode);
        string ExpandEnvironmentVariables(string stringWithVariables);
        string[] GetCommandLineArgs();
        string MachineName { get; }
        string NewLine { get; }
        string UserName { get; }
    }
}
