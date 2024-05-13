using System.Net;

class Program
{

	static void Main(string[] args)
	{
		StartServer();
		Console.WriteLine("Press any key to stop the server...");
		Console.ReadKey();
	}

	private static void StartServer()
	{
		//string reqURL = "http://localhost/oauth-callback/";
		string reqURL = "http://localhost:8080/oauth-callback/";
		HttpListener listener = new HttpListener();
		// Define the URLs to listen on. Make sure these are added to your system as reserved URLs if using Windows
		//listener.Prefixes.Add("http://localhost:8080/"); // Add other URIs if needed
		listener.Prefixes.Add(reqURL); // Add other URIs if needed

		try
		{
			listener.Start();
			Console.WriteLine($"Listening for connections on {reqURL}");
			Task listenTask = HandleIncomingConnections(listener);
			listenTask.GetAwaiter().GetResult();

			listener.Close();
		}
		catch (Exception e)
		{
			Console.WriteLine("An error occurred: " + e.Message);
		}
	}

	private static async Task HandleIncomingConnections(HttpListener listener)
	{
		bool runServer = true;

		while (runServer)
		{
			// Will wait here until we hear from a connection
			HttpListenerContext context = await listener.GetContextAsync();

			// Peel out the requests and response objects
			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;

			Console.WriteLine("Request Url: " + request.Url.ToString());

			if (request.Url.AbsolutePath == "/oauth-callback")
			{
				// Extract the code from the query string
				var code = request.QueryString["code"];
				Console.WriteLine("Authorization code: " + code);

				// You can now use this code to perform further OAuth steps or send it to Unity
				// Send a response back to the browser
				string responseString = "<html><body>Authorization complete. You can close this window.</body></html>";
				var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
				response.ContentLength64 = buffer.Length;
				var responseOutput = response.OutputStream;
				await responseOutput.WriteAsync(buffer, 0, buffer.Length);
				responseOutput.Close();
			}
			else
			{
				// Write the response info
				string responseString = "<html><body><h1>Hello from HttpListener</h1></body></html>";
				byte[] data = System.Text.Encoding.UTF8.GetBytes(responseString);

				// Write response data
				response.ContentType = "text/html";
				response.ContentEncoding = System.Text.Encoding.UTF8;
				response.ContentLength64 = data.LongLength;

				// Write out to the response stream (asynchronously), then close it
				await response.OutputStream.WriteAsync(data, 0, data.Length);
				response.Close();
			}

		}
	}
}
