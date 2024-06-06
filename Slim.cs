using System.Net;
using System.Text;
using System.Diagnostics;


namespace TwilioWebhookListener.Infrastructure.SlimWebServer;


public class Slim(int port = 8080)
{

	private readonly int _port = port;
	//private readonly IPAddress _localAddr = IPAddress.Parse(baseUrl);
	private readonly Dictionary<string, ISlimHandler?> _routes = new ();
	private SlimRequest _slimRequest = new();
    
    	public void AddRoute(string route, ISlimHandler? controller) => _routes.Add(route, controller);
   	
    	public async Task Start() 
    	{
        	HttpListener listener = new HttpListener();
		listener.Prefixes.Add("http://localhost:"+_port+"/");
		listener.Start();
		
		Console.WriteLine("listening...");
        	
		while (true)
		{

			Console.WriteLine("Waiting for a connection");
			
			HttpListenerContext context = await listener.GetContextAsync();
			HttpListenerRequest request = context.Request;

			// 1. SECURITY VALIDATION (review) 
                	// var verified = TwillioValidator.Validate(slimRequest);
                	// if (!verified)
                	//     return StatusCodes.Status403Forbidden;	

			HttpListenerResponse response = context.Response;

			Console.WriteLine("Connected!");

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
		
			if (!string.IsNullOrEmpty(request.RawUrl) && _routes.TryGetValue(request.RawUrl, out ISlimHandler controller))
			{
				stopwatch.Restart();
                                	
                                Console.WriteLine("RawUrl: "+ request.RawUrl);

                                stopwatch.Stop();
                                Console.WriteLine("Elapsed! "+ stopwatch.ElapsedMilliseconds / 1000);
                                stopwatch.Stop();
				
                                await controller.Handle(request);
				
				response.StatusCode = 200;
				
			}
			else
			{
				response.StatusCode = 404;
			}


			byte[] buffer = Encoding.UTF8.GetBytes(" "); // make it inline (default body by twilio)
                	await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                	response.OutputStream.Close();


			stopwatch.Stop();
			Console.WriteLine("latency in secs: "+ stopwatch.ElapsedMilliseconds / 1000);

		}
        	
	}
	
	
//	private void _parseStartLine()
//	{
//
//		var startPtr = _lookahead;
//		for (var endPtr = _lookahead; ;endPtr++)
//		{
//
//			//var memSymbolSlice = _rawMemRequest[new Range(start:endPtr, end:endPtr + 1)];
//			var symbol = _rawRequest[endPtr];
//			
//			if (symbol == '\n') // break line
//		    	{
//
//				//_slimRequest.StartLine.Fill(_rawMemRequest.Slice(startPtr, endPtr - startPtr).ToString());
//				_slimRequest.StartLine.Fill(_rawRequest.Substring(startIndex: startPtr, length: endPtr - startPtr));
//				
//				_lookahead = endPtr +1; // already starts at the next beginning point	
//				
//				return;
//
//		    	}	
//		    
//		    	if (symbol == ' ' || symbol == '\r') 
//		    	{
//				
//				_slimRequest.StartLine.Fill(_rawRequest.Substring(startIndex: startPtr, length: endPtr - startPtr));
//				startPtr = endPtr + 1;    
//
//		    	}
//
//		}
//	}
	

//	private void _parseHeaders()
//	{
//		
//
//		string currKey = string.Empty;
//		int startPtr = _lookahead;
//		for (var endPtr = _lookahead; ; endPtr++)
//		{
//
//			var symbol = _rawRequest[endPtr];
//
//			Console.WriteLine("symbol: "+ symbol);
//
//			if (symbol == ':' && string.IsNullOrEmpty(currKey))
//			{ 	
//				currKey = _rawRequest.Substring(startIndex: startPtr, length: endPtr - startPtr); 	
//				startPtr = endPtr +1;
//				
//				continue;
//			}
//
//			
//			if (symbol == '\r')
//			{
//
//				_slimRequest.Headers.Add(currKey.TrimStart(), 
//                        		_rawRequest.Substring(startIndex: startPtr, length: endPtr - startPtr)
//                        		.TrimStart());
//
//				currKey = string.Empty;
//			
//				startPtr = endPtr +1;
//			}
//			
//			// means that there is a blank line ahead (jump to body)
//			var isBreakLineFollowedByCarriageReturn = (symbol == '\n' && _rawRequest[endPtr +1] == '\r');
//			if (isBreakLineFollowedByCarriageReturn) // diff between (char)32 and space (why space are 'equal' to \r or \n)
//				return;	
//		}
//		
//	}
	

//	private void _parseBody()
//	{
//		
//		string value = string.Empty;
//		
//		int startPtr = _rawRequest.Length -1; // reverse traversing
//		for (var endPtr = startPtr; ; endPtr--)
//		{
//
//			var symbol = _rawRequest[endPtr];	    
//			if (symbol == '=' && string.IsNullOrEmpty(value))
//			{
//				// Retrieve a pointer to the current key
//				value = _rawRequest.Substring(startIndex: endPtr +1, length: startPtr - endPtr);
//				startPtr = endPtr -1;
//
//				continue;
//			}
//
//		    	if (symbol == '&')
//		    	{
//				var key =  _rawRequest.Substring(startIndex: endPtr +1, length: startPtr - endPtr);	
//				_slimRequest.Body.AddContent(key, value);
//
//				value = string.Empty;
//
//				startPtr = endPtr -1;
//
//				continue;
//		    	}	
//		    
//
//		    	// Body is writen in the same line ?
//		    	if (symbol == (char)32) return;
//
//		}
//	}

}
