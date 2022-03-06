using System;
using Newtonsoft.Json;

namespace TimeTracker.Dtos.Authentications
{
    public class Response<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
		
        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; }
		
        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }
		
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
}