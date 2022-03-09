using Newtonsoft.Json;

namespace TimeTracker.Dtos.Accounts
{
    public class CreateUserRequest
    {
	    [JsonProperty("client_id")]
	    public string ClientId { get; set; }
		
	    [JsonProperty("client_secret")]
	    public string ClientSecret { get; set; }
	    
        [JsonProperty("email")]
        public string Email { get; set; }
		
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
		
        [JsonProperty("last_name")]
        public string LastName { get; set; }
		
        [JsonProperty("password")]
        public string Password { get; set; }

        public CreateUserRequest(string id, string secret, string email, string first, string last, string password)
        {
	        ClientId = id;
	        ClientSecret = secret;
	        Email = email;
	        FirstName = first;
	        LastName = last;
	        Password = password;
        }
    }
}