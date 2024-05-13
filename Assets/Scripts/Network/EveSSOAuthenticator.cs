using EveMarket.Util;
using System;
using System.Net;
using System.Threading;
using UnityEngine;

namespace EveMarket.Network
{
	public class EveSSOAuthenticator : MonoBehaviour
	{
		private HttpListener listener;
		private Thread listenerThread;
		private Action<string> onCodeReceived;

		void Start()
		{
			StartListening();
		}

		void OnDestroy()
		{
			StopListening();
		}

		public void StopListening()
		{
			try
			{
				if (listener != null)
				{
					listener!.Stop();
					listener!.Close();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Listener Error.\n{ex}");
			}
			finally
			{
				if (listenerThread != null)
				{
					listenerThread.Abort();
				}
			}
		}

		public void StartListening()
		{
			listener = new HttpListener();
			listener.Prefixes.Add(NetworkSettings.CALLBACK_URL + "/"); // Add your OAuth redirect URI
			listener.Start();
			listenerThread = new Thread(new ThreadStart(HandleRequests));
			listenerThread.Start();
		}

		private void HandleRequests()
		{
			while (listener.IsListening)
			{
				try
				{
					HttpListenerContext context = listener.GetContext();
					HttpListenerRequest request = context.Request;
					HttpListenerResponse response = context.Response;

					if (request.Url.AbsolutePath.Contains("/oauth-callback"))
					{
						var code = request.QueryString["code"];
						Debug.Log($"Authorization code received: {code}");
						//response.Redirect(NetworkSettings.CALLBACK_URL + "close");

						if (onCodeReceived != null)
						{
							string responseString = "<html><body>You can close this tab/window.</body></html>";
							var buffer = global::System.Text.Encoding.UTF8.GetBytes(responseString);
							response.ContentLength64 = buffer.Length;
							response.OutputStream.Write(buffer, 0, buffer.Length);
							response.OutputStream.Close();

							UnityMainThreadDispatcher.Instance?.Enqueue(() => onCodeReceived(code));
							//StopListening();
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"Listener Error.\n{ex}");
				}

			}
		}

		public void SetCodeReceivedCallback(Action<string> callback)
		{
			onCodeReceived = callback;
		}
	}
}
