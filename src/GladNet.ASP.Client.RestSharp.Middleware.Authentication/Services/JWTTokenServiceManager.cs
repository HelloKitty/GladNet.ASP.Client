using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.RestSharp.Middleware.Authentication
{
	public class JWTTokenServiceManager : ITokenRegistry, ITokenService
	{
		/// <summary>
		/// Indicates if we have a token available.
		/// </summary>
		public bool isTokenAvailable { get; private set; } = false; //we could check token constantly but GC in Unity3D should be avoided

		/// <summary>
		/// String value of token.
		/// </summary>
		public string TokenString { get; private set; }

		/// <summary>
		/// The full token value that includes Token Type and token such as: Bearer 88ysd8ysdhd...
		/// </summary>
		public string FullTokenHeaderValue { get; private set; }

		public void RegisterToken(string tokenValue)
		{
			//Check the token
			if (String.IsNullOrEmpty(tokenValue))
				throw new ArgumentException($"Provided {nameof(tokenValue)} in the service {nameof(JWTTokenServiceManager)} is invalid. Value: {tokenValue}", nameof(tokenValue));

			isTokenAvailable = true;
			TokenString = tokenValue;

			//Token header should fit the scheme Bearer {access_token}
			FullTokenHeaderValue = $"Bearer {tokenValue}";
		}
	}
}
