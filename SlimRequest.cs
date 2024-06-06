using System.Collections.Specialized;


namespace TwilioWebhookListener.Infrastructure.SlimWebServer
{

	public struct SlimRequest
	{

		internal Body Body = new();
	    
		internal StartLine StartLine = new();

		public StringDictionary Headers = new StringDictionary() {};

		public SlimRequest(){}

		public bool HasFormContent() 
		{
			var hasBody = false;
			foreach (var k in Headers.Keys)
			{
				if (string.Equals(k.ToString(), "Content-Type", StringComparison.OrdinalIgnoreCase))
					hasBody = true;

			}


			return hasBody;
		}
	}

	internal abstract class RequestSection{}
	
	
	internal class CustomHeaders : RequestSection
	{
		public Dictionary<string, string> Content = new();
                                                                                                         
	}                                                                                                                         

	//public struct StartLine : RequestSection
	internal class StartLine : RequestSection
	{
	
		private string _verb = string.Empty;
		public string Verb
		{
			get => _verb;
			set
			{
				if (string.IsNullOrEmpty(_verb))
				{
					_verb = value;
				}
				else
				{
					Resource = value;
				}
			}
		}
	
		private string _resource = string.Empty;
		public string Resource
		{
			get => _resource;
			private set
			{
				if (string.IsNullOrEmpty(_resource))
				{
					_resource = value;
				}
				else
				{
					_httpVersion = value;
				}
			}
		}

		private string _httpVersion = ""; 
	
		public StartLine() {}

		public void Fill(string value) { Verb = value; }
	}
	

	//public struct Body 
	internal class Body : RequestSection
	{
		private bool _consumed = false; 

	    	private Dictionary<string, string>? _content;
	    
	    	public Body(){}
	
	    	public void AddContent(string key, string value) 
	    	{
			_content ??= new Dictionary<string, string>();	
			_content.Add(key, value);
	    	}
	    
	    	public Dictionary<string, string>? GetContent()
	    	{
			if (_consumed)
			{
				throw new InvalidOperationException("The body has already been processed and converted to a domain entity." + 
                        		"Access through this method is not allowed after conversion.");
			}
 
			_consumed = true;
			return _content;
	    	}
	    
	}
}
