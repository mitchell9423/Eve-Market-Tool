using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace EveMarket.Util
{
    public static class ProxyLauncher
    {
        private static Process proxyProcess;

        public static void StartLocalSslProxy()
        {
            if (proxyProcess != null && !proxyProcess.HasExited)
            {
                UnityEngine.Debug.LogWarning("🔄 Proxy already running.");
                return;
            }

            string certPath = GetCertPath();
            string keyPath = GetKeyPath();

            string proxyCommand = $"local-ssl-proxy --source 8080 --target 5555 --cert \"{certPath}\" --key \"{keyPath}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = GetShell(),
                Arguments = GetShellArguments(proxyCommand),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            proxyProcess = new Process { StartInfo = startInfo };
            proxyProcess.OutputDataReceived += (s, e) => UnityEngine.Debug.LogWarning($"Proxy: {e.Data}");
            proxyProcess.ErrorDataReceived += (s, e) => UnityEngine.Debug.LogError($"Proxy Error: {e.Data}");

            proxyProcess.Start();
            proxyProcess.BeginOutputReadLine();
            proxyProcess.BeginErrorReadLine();

            UnityEngine.Debug.LogWarning("🚀 Proxy started: https://localhost:8080 → http://localhost:5555");
        }

        public static void StopLocalSslProxy()
        {
            if (proxyProcess != null && !proxyProcess.HasExited)
            {
                proxyProcess.Kill();
                proxyProcess.WaitForExit();
                UnityEngine.Debug.LogWarning("🛑 Proxy stopped.");
            }
        }

        private static string GetShell()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return "cmd.exe";
#else
            return "/bin/bash";
#endif
        }

        private static string GetShellArguments(string command)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return $"/C {command}";
#else
            return $"-c \"{command}\"";
#endif
        }

        private static string GetCertPath()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return "C:\\path\\to\\localhost.pem"; // 📝 Update with your Windows cert path
#else
            return "localhost.pem"; // In Unity working directory or provide full path
#endif
        }

        private static string GetKeyPath()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return "C:\\path\\to\\localhost-key.pem"; // 📝 Update this
#else
            return "localhost-key.pem";
#endif
        }
    }
}
