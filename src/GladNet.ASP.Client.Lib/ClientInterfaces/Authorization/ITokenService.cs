using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client
{
	/// <summary>
	/// Service that provides token information for consuming HTTP clients.
	/// </summary>
	public interface ITokenService
	{
		/// <summary>
		/// Indicates if we have a token available.
		/// </summary>
		bool isTokenAvailable { get; }

		/// <summary>
		/// String value of token.
		/// </summary>
		string TokenString { get; }

		/// <summary>
		/// The full token value that includes Token Type and token such as: Bearer 88ysd8ysdhd...
		/// </summary>
		string FullTokenHeaderValue { get; }
	}
}
