using System;

namespace EveMarket.Util
{
	public static class EveDelegate
	{
		public delegate void AppNotify(int typeId);
		public static AppNotify UpdateItemNotify;
		public static AppNotify UpdateUINotify;

		public delegate void AppEvent();
		public static AppEvent UpdateMarketObjectsComplete;
		public static AppEvent PresetChanged;
		public static AppEvent StaticUpdateComplete;
		public static AppEvent ItemMarketUpdateComplete;
		public static AppEvent MarketUpdateComplete;
		public static AppEvent StaticLoadComplete;
		public static AppEvent ResetAutoUpdateTimer;
		public static AppEvent AppSettingsChanged;
		public static AppEvent CreateUI;
		public static AppEvent UpdateUI;


		public static void Subscribe(ref AppNotify _delegate, Action<int> action)
		{
			// Unsubscribe the action to ensure it's not added if it already exists.
			//Unsubscribe(ref _delegate, action);

			// Subscribe the action.
			_delegate += new AppNotify(action);
		}

		public static void Unsubscribe(ref AppNotify _delegate, Action<int> action)
		{
			_delegate -= new AppNotify(action);
		}

		public static void Subscribe(ref AppEvent _delegate, Action action)
		{
			// Unsubscribe the action to ensure it's not added if it already exists.
			//Unsubscribe(ref _delegate, action);

			// Subscribe the action.
			_delegate += new AppEvent(action);
		}

		public static void Unsubscribe(ref AppEvent _delegate, Action action)
		{
			_delegate -= new AppEvent(action);
		}
	}
}
