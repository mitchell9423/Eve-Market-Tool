using EveMarket.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace EveMarket
{
	public struct BuyPreset
	{
		public Region buyRegion;
		public System buySystem;
		public string buyRange;
		public int profitMargin;

		public BuyPreset(Region buyRegion, System buySystem, string buyRange, int profitMargin)
		{
			this.buyRegion = buyRegion;
			this.buySystem = buySystem;
			this.buyRange = buyRange;
			this.profitMargin = profitMargin;
		}
	}

	public class Settings
	{
		public bool EnableTimedUpdate { get; set; } = true;
		public float BaseYield { get; set; } = .54f;
		public float ReprocessTax { get; set; } = .1f;
		public int MarginPercentage { get; set; } = 15;
		public string BuyRange { get; set; } = "4";
		public System BuyOrderSystem { get; set; } = System.Tunttaras;
		public Region BuyRegion { get; set; } = Region.Lonetrek;
		public Region SellRegion { get; set; } = Region.The_Forge;
		public System ActivePreset { get; set; } = System.None;
		public double IskAmount { get; set; }
		public double UnitPrice { get; set; }
		public int CorpId { get; set; } = 98422411;
		public DateTime RefreshExpiration { get; set; }
		public TokenResponse TokenResponse { get; set; } = new TokenResponse();
	}

	public static class AppSettings
	{
		private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
		private static Task lastQueuedTask = Task.CompletedTask;
		private static bool isTaskQueued = false;

		private enum Prefs
		{
			MarginPercentage,
			BuyRange,
			BuyOrderSystem,
			BuyRegion,
			SellRegion
		}

		public static Settings Settings { get; set; }

		public static Dictionary<System, BuyPreset> Presets = new Dictionary<System, BuyPreset>()
		{
			{ System.Tunttaras, new BuyPreset(Region.Lonetrek, System.Tunttaras, "4", 10) },
			{ System.Umokka, new BuyPreset(Region.Lonetrek, System.Umokka, "3", 10) },
			{ System.Ylandoki, new BuyPreset(Region.Lonetrek, System.Ylandoki, "1", 10) },
			{ System.Inaro, new BuyPreset(Region.The_Citadel, System.Inaro, "3", 10) }
		};

		public static void SetProfitMargin(int val)
		{
			if (Settings.MarginPercentage == val) return;

			Settings.MarginPercentage = val;

			Debug.Log($"Profit Margin set to {val}");

			QueueSave();
		}

		public static void SetBasePercent(float val)
		{
			if (Settings.BaseYield == val) return;

			Settings.BaseYield = val;

			Debug.Log($"Base Percent set to {val}");

			QueueSave();
		}

		public static void SetReprocessTax(float val)
		{
			if (Settings.ReprocessTax == val) return;

			Settings.ReprocessTax = val;

			Debug.Log($"Reprocess Tax set to {val}");

			QueueSave();
		}

		public static void UpdatePreset(System preset)
		{
			if (Settings.ActivePreset == preset) return;

			Settings.ActivePreset = preset;
			Settings.BuyRegion = Presets[preset].buyRegion;
			Settings.BuyOrderSystem = Presets[preset].buySystem;
			Settings.BuyRange = Presets[preset].buyRange;

			Debug.Log($"Active Preset set to {preset}");

			QueueSave();

		}

		public static bool LoadAppSettings()
		{
			Debug.Log($"Loading App Settings.");
			Settings = FileManager.DeserializeFromFile<Settings>();

			if (Settings == null)
			{
				Debug.LogWarning($"Creating new settings object.");
				Settings = new Settings();
			}

			EveDelegate.AppSettingsChanged?.Invoke();
			return true;
		}

		public static void SaveAppSettings()
		{
			FileManager.SerializeObject(Settings);
			Debug.Log($"App settings saved.");
		}

		public static void QueueSave()
		{
			_ = RunOrQueueTask(SaveAppSettings);
		}

		public static async Task RunOrQueueTask(Action taskAction, Action<AppState> onCompletion = null)
		{
			if (isTaskQueued)
			{
				// If there's already a queued task, ignore any new requests.
				return;
			}

			// Wait asynchronously for the semaphore
			await semaphore.WaitAsync();

			try
			{
				if (!lastQueuedTask.IsCompleted)
				{
					// If the last task hasn't completed, set the flag and prepare to queue
					isTaskQueued = true;

					// Run the task after the last one completes
					lastQueuedTask = lastQueuedTask.ContinueWith(t => taskAction(), TaskScheduler.Default)
												   .ContinueWith(t =>
												   {
													   // Execute the onCompletion action if provided
													   //onCompletion?.Invoke(AppState.None);

													   // Reset the queued task flag and release semaphore when done
													   isTaskQueued = false;
													   semaphore.Release();
												   }, TaskScheduler.Default);
				}
				else
				{
					// No need to queue, run immediately
					lastQueuedTask = Task.Run(taskAction)
										 .ContinueWith(t =>
										 {
											 // Execute the onCompletion action if provided
											 //onCompletion?.Invoke(AppState.None);

											 semaphore.Release();
										 }, TaskScheduler.Default);
				}

				await lastQueuedTask;
			}
			catch
			{
				// Ensure semaphore is released in case of an exception
				semaphore.Release();
				throw;
			}
		}
	}
}
