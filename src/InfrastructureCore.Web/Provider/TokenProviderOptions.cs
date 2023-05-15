using Microsoft.IdentityModel.Tokens;
using System;

namespace InfrastructureCore.Web.Provider
{
    /// <summary>
    /// Line 1: Dùng để config endpoint.
    /// Line 2: Dùng để config expired date cho token.
    /// Line 3: Dung để config "Credential" nó giống như 1 serect key để decrypt và encrypt Token.
    /// </summary>
    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/MESAccount/OnMESLogin"; //line 1

        public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(+1); //line 2

        public SigningCredentials SigningCredentials { get; set; }//line 3
    }
}
