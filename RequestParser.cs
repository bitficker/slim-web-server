
namespace TwilioWebhookListener.Infrastructure.SlimWebServer;


public class RequestParser(string raw)
{
	private string _raw = raw;

//	public async Task ProcessInParallel()
//	{
//		var tasks = new List<Task>();
//        	
//        	tasks.Add(_parseStartLineAsync());
//        	tasks.Add(_parseHeadersAsync());
//        	tasks.Add(_parseBodyAsync());
//
//		await Task.WhenAll(tasks);
//	}

	// testando
        public void ParseRequestInParallel()
        {
                                                                                                   
        	var tasks = new List<Task<RequestSection>>();
        	
        	tasks.Add(_parseStartLineAsync());
        	tasks.Add(_parseHeadersAsync());
        	tasks.Add(_parseBodyAsync());
                                                                                                   
        	var continuation = Task.WhenAll(tasks);
        	continuation.Wait(); // it blocks the current thread? wrap it into another task.run?
                                                                                                   
        //	try
        //	{
        //		continuation.Wait();
        //	}
        //	catch (AggregateException)
        //	{}
        	
        	if (continuation.Status == TaskStatus.RanToCompletion)
        	{
        		for (var i = 0; i < continuation.Result.Length; i++)
        		{
        			Console.WriteLine("continuation.Result "+ continuation.Result[i]);
                                                                                                   
        		}
        
        	}                                                                                                   
	}                                                                                         

	private async Task<RequestSection> _parseStartLineAsync()
	{
		var startLine = new StartLine(); // how it behaves being a struct? (going to another thread)
		//var startLine = new Dictionary<string, string>;
		await Task.Run(() => 
		{
			var startPtr = 0;
			for (var endPtr = startPtr; ;endPtr++)
                	{
                                                                                                                                             
                		var symbol = _raw[endPtr];
                		
                		if (symbol == '\n')
                	    	{
                                                                                                                                             
                			//startLine.Fill(_raw.Substring(startIndex: startPtr, length: endPtr - startPtr));	

                			
                			return;
                                                                                                                                             
                	    	}	
                	    
                	    	if (symbol == ' ' || symbol == '\r') 
                	    	{
                			
                			startLine.Fill(_raw.Substring(startIndex: startPtr, length: endPtr - startPtr));
                			startPtr = endPtr + 1;    
                                                                                                                                             
                	    	}                                 
                	}

		});

		return startLine;
	}


	// keep service elastic
	private async Task<RequestSection> _parseHeadersAsync()
	{
		
		//var headers = new StringDictionary();
		var headers = new CustomHeaders();
		await Task.Run(() => 
		{

			 // improve this algorithm
			var onHeaders = false;			
			string currKey = string.Empty;	
		
			int startPtr = 0;
                	for (var endPtr = startPtr; ; endPtr++)
                	{	
				var symbol = _raw[endPtr];

				if (!onHeaders && symbol == '\n')
				{
					onHeaders = true;
				}

				if (onHeaders)
				{
					if (symbol == ':' && string.IsNullOrEmpty(currKey))
					{
						currKey = _raw.Substring(startIndex: startPtr, length: endPtr - startPtr); 	
                                        	startPtr = endPtr +1;
                                        	
                                        	continue;
					}
					
					var beforeBreakLine = (symbol == '\r');
                                	if (beforeBreakLine)
                                	{
                                                                                                                
                                		headers.Content.Add(currKey.TrimStart(), 
                                        		_raw.Substring(startIndex: startPtr, length: endPtr - startPtr)
                                        		.TrimStart());
                                                                                                                                                        
                                		currKey = string.Empty;
                                	
                                		startPtr = endPtr +1;
                                	}
                                	
                                	// means that there is a blank line ahead (jump to body)
                                	var isBreakLineFollowedByCarriageReturn = (symbol == '\n' && _raw[endPtr +1] == '\r');
                                	if (isBreakLineFollowedByCarriageReturn) 
						return;

				}		                 	
			}
		});


		return headers;

	}
	

	private async Task<RequestSection> _parseBodyAsync()
	{
		
		var body = new Body();
		await Task.Run(() => 
		{

			string value = string.Empty;                                                                                              			
			int startPtr = _raw.Length -1; // reverse traversing
			for (var endPtr = startPtr; ; endPtr--)
			{
				var symbol = _raw[endPtr];	    
				if (symbol == '=' && string.IsNullOrEmpty(value))
				{
					value = _raw.Substring(startIndex: endPtr +1, length: startPtr - endPtr);
					startPtr = endPtr -1;
			                                                                                                         
					continue;
				}
			                                                                                                         
			    	if (symbol == '&')
			    	{
					var key =  _raw.Substring(startIndex: endPtr +1, length: startPtr - endPtr);	
					body.AddContent(key, value);
			                                                                                                         
					value = string.Empty;
			                                                                                                         
					startPtr = endPtr -1;
			                                                                                                         
					continue;
			    	}	
			    
			                                                                                                         
			    	// Body is writen in the same line ?
			    	if (symbol == (char)32) return;
			}
		});


		return body;
	
	}
}




































