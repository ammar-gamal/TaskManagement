using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using TaskManagement.IntegrationTests.Utilities.Constants;

namespace TaskManagement.IntegrationTests.Utilities;

internal class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
     ILoggerFactory logger, UrlEncoder encoder)
     : base(options, logger, encoder)
    {
    }


    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, TestAuthData.UserId.ToString()) };
        var identity = new ClaimsIdentity(claims, TestAuthData.Type);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestAuthData.Scheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
