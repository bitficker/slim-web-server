using System.Net;


namespace TwilioWebhookListener.Infrastructure.SlimWebServer;

public interface ISlimHandler
{
	//public Task<string> Handle(SlimRequest slimRequest);
	public Task Handle(HttpListenerRequest slimRequest);

}
