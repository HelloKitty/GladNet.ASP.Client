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
		/// <summary>
		/// JWT access token if authentication was successful.
		/// </summary>
		[JsonProperty(PropertyName = "access_token", Required = Required.AllowNull)] //optional because could be an error
		public string AccessToken { get; private set; }

		/// <summary>
		/// Error type if an error was encountered.
		/// </summary>
		[JsonProperty(PropertyName = "error", Required = Required.AllowNull)] //optional because could be a valid token
		public string Error { get; private set; }

		/// <summary>
		/// Humanreadable read description.
		/// </summary>
		[JsonProperty(PropertyName = "error_description", Required = Required.AllowNull)] //optional because could be a valid token
		public string ErrorDescription { get; private set; }

		/// <summary>
		/// Indicates if the model contains a valid <see cref="AccessToken"/>.
		/// </summary>
		public bool isTokenValid { get { return !String.IsNullOrEmpty(AccessToken) && String.IsNullOrEmpty(Error) && String.IsNullOrEmpty(ErrorDescription); } }
	}
}
