using Newtonsoft.Json;

namespace TimeTracker.Dtos.Authentications.Credentials
{
	public class LoginWithCredentialsRequest
	{
		[JsonProperty("login")]
		public string Login { get; set; }
		
		[JsonProperty("password")]
		public string Password { get; set; }
		
		[JsonProperty("client_id")]
		public string ClientId { get; set; }
		
		[JsonProperty("client_secret")]
		public string ClientSecret { get; set; }

		public LoginWithCredentialsRequest(string login, string password, string id, string secret)
		{
			Login = login;
			Password = password;
			ClientId = id;
			ClientSecret = secret;
		}
	}
}