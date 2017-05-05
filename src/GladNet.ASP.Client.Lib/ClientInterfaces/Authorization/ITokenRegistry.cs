using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace GladNet.ASP.Client
{
	/// <summary>
	/// Registration service for a token.
	/// </summary>
	public interface ITokenRegistry
	{
		/// <summary>
		/// Register the token replacing the current active token.
		/// </summary>
		/// <param name="tokenValue">The token value to use.</param>
		void RegisterToken([NotNull] string tokenValue);
	}
}
