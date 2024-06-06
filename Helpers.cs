using System.Text;


namespace TwilioWebhookListener.Infrastructure.SlimWebServer.Helpers;


public static class Helper
{

	public static string FromBytes(Stream stream)
	{
		Byte[] bytes = new Byte[256];
		
		var builder = new StringBuilder();
		int i;
		
		// Number of bytes coming from NetworkStream (client.GetStream -> client http request)
		while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) 
		{
		    builder.Append(Encoding.ASCII.GetString(bytes, 0, i));
		}

		return builder.ToString();
	}

 	public static object ThownIfNull(object? obj, string? value = null)
 	{                                                                           
     		if (obj is null)
         		throw new InvalidOperationException("Value" + value + "is invalid");
     
     		return obj;
 	}

 	public static object? Parse<T>(string body)
 	{
                                                                                               
		var splittedBody = body.Split('&');
     
     		var type = typeof(T);
     		var instance = Activator.CreateInstance(type);
     
     		//ThownIfNull(instance);
     
     		var fields = type.GetFields();
     
                                                                                               
     		for (var i = 0; i < fields.Length; i++)
     		{
         		var field = fields[i];
         
         		for (var j = 0; j < splittedBody.Length; j++)
         		{
             			var bodyFieldName = splittedBody[j].Split("=")[0];
             
             			if (String.Equals(field.Name, bodyFieldName, StringComparison.OrdinalIgnoreCase))
             			{
                 
                 			var innerField = instance?.GetType().GetField(field.Name);
                 
                 			innerField?.SetValue(instance, splittedBody[j].Split("=")[1]);
                 
             			}
         		}
     		}
                                                                                               
     
     		return instance;
 	}

	// How to read straight from stream?
	public static async Task<Dictionary<string, string>> ParseBodyAsync(string rawBody)
	{
		
		var content = new Dictionary<string, string>();
		await Task.Run(() => 
		{

			string key = string.Empty;                                                                                              			
			int startPtr = 0; 
			for (var endPtr = startPtr; endPtr < rawBody.Count(); endPtr++)
			{

				var symbol = rawBody[endPtr];	    
				if (symbol == '=' && string.IsNullOrEmpty(key))
				{
					key = rawBody.Substring(startIndex: startPtr, length: endPtr - startPtr);
					startPtr = endPtr +1;
			                                                                                                         
					continue;
				}
			                                                                                                         
			    	if (symbol == '&')
			    	{
					var value = rawBody.Substring(startIndex: startPtr, length: endPtr - startPtr);	
					content.Add(key, value);
			                                                                                                         
					key = string.Empty;
			                                                                                                         
					startPtr = endPtr +1;
			                                                                                                         
					continue;
			    	}	                                                                                                         
			}
		});


		return content;
	
	}
}











