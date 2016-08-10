using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.RestSharp.Middleware.Authentication
{
	/// <summary>
	/// JWT Token model.
	/// </summary>
	[JsonObject]
	public class JWTModel
	{
		[JsonProperty(PropertyName = "access_token")]
		public string AccessToken { get; private set; }
	}
}
