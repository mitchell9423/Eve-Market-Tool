
using System.Collections;
using System.Threading;
using UnityEngine.Networking;
using EveMarket.Util;
using System.Threading.Tasks;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Diagnostics;

namespace EveMarket.Network.OAuth
{
	public static class LoginManager
	{
		public static async Task<bool> Login()
		{
			EveSSOAuthenticator eveSSOAuthenticator = new EveSSOAuthenticator();

			try
			{
				await eveSSOAuthenticator.RunSSOLogin("test");
			}
			catch (Exception ex)
			{
				UnityMainThreadDispatcher.LogError("Login failed: " + ex);
				return false;
			}

			return true;
		}
	}

	class EveSSOAuthenticator
	{
		private const string LOGIN_SCRIPT_FILENAME = "eve-login.sh";
		private const string LOGIN_TOKEN_FILENAME = "token.json";
		private const string OAuth_DIR = "../OAuth";

		private static string RootDir => $"{Application.dataPath}";
		private static string ScriptPath => Path.GetFullPath(Path.Combine(RootDir, $"{OAuth_DIR}/{LOGIN_SCRIPT_FILENAME}"));
		private static string TokenFilePath => Path.Combine(RootDir, $"{OAuth_DIR}/{LOGIN_TOKEN_FILENAME}");
		private string SafeTokenPreview(string token) => token.Length <= 16 ? token : $"{token.Substring(0, 8)}...{token[^8..]}";

		public async Task RunSSOLogin(string test)
		{
			UnityMainThreadDispatcher dispatcher = UnityMainThreadDispatcher.Instance;

			UnityMainThreadDispatcher.Log("🚀 Launching Login Script from Unity...");

			//UnityMainThreadDispatcher.Log($"dataPath = {RootDir}");
			//UnityMainThreadDispatcher.Log($"ScriptPath = {ScriptPath}");
			//UnityMainThreadDispatcher.Log($"TokenFilePath = {TokenFilePath}");

			if (!File.Exists(ScriptPath))
			{
				UnityMainThreadDispatcher.LogError("❌ Login script not found!");
				return;
			}

			if (!new FileInfo(ScriptPath).IsReadOnly && !File.Exists("/usr/bin/env")) // Optional enhancement
			{
				UnityMainThreadDispatcher.LogWarning("⚠️ Script file may not be executable or env is missing.");
			}

			var tcs = new TaskCompletionSource<int>();
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = "/bin/bash",
				Arguments = $"{ScriptPath} " +
                $"{LoginConfig.LogFile} " +
				$"{LoginConfig.ClientId} " +
                $"{LoginConfig.ClientSecret} " +
                $"{LoginConfig.CALLBACK_URL} " +
                $"{LoginConfig.Scope} " +
                $"{LoginConfig.AUTHORIZATION_ENDPOINT} " +
                $"{LoginConfig.TOKEN_ENDPOINT} " +
                $"{LoginConfig.CertFile} " +
                $"{LoginConfig.KeyFile} " + 
				$"{FileManager.GetFilePath<TokenResponse>()}",
				WorkingDirectory = Path.GetDirectoryName(ScriptPath),
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (Process process = new Process { StartInfo = psi })
			{
				process.OutputDataReceived += (s, e) =>
				{
					if (!e.Data.Contains("DebugStringToFile") && !e.Data.Contains("GetStacktrace"))
					{
						if (!string.IsNullOrWhiteSpace(e.Data))
							UnityMainThreadDispatcher.Log("OUT: " + e.Data);
					}
				};

				process.ErrorDataReceived += (s, e) =>
				{
					if (!e.Data.Contains("DebugStringToFile") && !e.Data.Contains("GetStacktrace"))
					{
						if (!string.IsNullOrWhiteSpace(e.Data))
							UnityMainThreadDispatcher.LogError("ERR: " + e.Data);
					}
				};

				process.EnableRaisingEvents = true;
				process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);

				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				int exitCode = await tcs.Task;


				if (exitCode != 0)
				{
					UnityMainThreadDispatcher.Enqueue(() =>
					{
						UnityMainThreadDispatcher.LogError($"❌ Script exited with code {process.ExitCode}");
					});
					return;
				}

				UnityMainThreadDispatcher.Log("✅ Login Script Completed. Reading token...");
			}

			if (!File.Exists(TokenFilePath))
			{
				UnityMainThreadDispatcher.LogError("❌ token.json not found!");
				return;
			}


			UnityMainThreadDispatcher.Log("✅ Reading Token...");
			TokenResponse token = FileManager.DeserializeFromFile<TokenResponse>();
			string json = File.ReadAllText(TokenFilePath);

			if (!int.TryParse(token.ExpiresIn, out int seconds))
			{
				UnityMainThreadDispatcher.LogError("❌ Failed to parse ExpiresIn from token response.");
				return;
			}

			// Store to app settings if needed
			AppSettings.Settings.AccessTokenExpiresAt = DateTime.Now.AddSeconds(seconds);
			AppSettings.Settings.TokenResponse = token;
			AppSettings.SaveAppSettings();
			return;
		}
	}
}
