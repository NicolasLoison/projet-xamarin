using Newtonsoft.Json;

namespace TimeTracker.Dtos.Accounts
{
	public class SetUserProfileRequest
	{
		[JsonProperty("email")]
		public string Email { get; set; }
		
		[JsonProperty("first_name")]
		public string FirstName { get; set; }
		
		[JsonProperty("last_name")]
		public string LastName { get; set; }

		public SetUserProfileRequest(string email, string first, string last)
		{
			Email = email;
			FirstName = first;
			LastName = last;
		}
	}
}