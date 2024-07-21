using System;

namespace JPC.Common.Internal
{
    internal class EnvironmentWrapper : IEnvironment
    {
        string IEnvironment.CommandLine => Environment.CommandLine;

        string IEnvironment.MachineName => Environment.MachineName;

        string IEnvironment.NewLine => Environment.NewLine;

        string IEnvironment.UserName => Environment.UserName;

        void IEnvironment.Exit(int exitCode) => Environment.Exit(exitCode);

        string IEnvironment.ExpandEnvironmentVariables(string stringWithVariables)
        {
            return Environment.ExpandEnvironmentVariables(stringWithVariables);
        }

        string[] IEnvironment.GetCommandLineArgs() => Environment.GetCommandLineArgs();
    }
}
