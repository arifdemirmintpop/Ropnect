using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace tiplay
{
    public static class Terminal
    {
        public delegate void TerminalCallback(TerminalResponse response);

        public static TerminalResponse RunCommand(string workingDirectory, string command)
        {
            var startInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDirectory,
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = "/bin/bash",
                Arguments = "-c \"" + command + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();

            return new TerminalResponse(command, process);

            //Debug.Log($"{command}; " + process.StandardOutput.ReadToEnd());
            //Debug.LogError($"{command}; " + process.StandardError.ReadToEnd());
        }
    }

    public struct TerminalResponse
    {
        private string result;
        private string error;
        private string command;

        public bool HasError => !string.IsNullOrEmpty(error);
        public string Result => result;
        public string Error => error;
        public string Command => command;

        public TerminalResponse(string command, Process process)
        {
            result = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            this.command = command;
        }
    }
}