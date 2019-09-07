using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectsAPI.Controllers
{
 
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string _clientId = "1u1reb06c2e5mcsh12bsl2peob";

        private readonly RegionEndpoint _region = RegionEndpoint.USEast1;

        [Route("api/register")]
        public async Task<ActionResult<string>> Register(User user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.Password,
                Username = user.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            request.UserAttributes.Add(emailAttribute);

            var response = await cognito.SignUpAsync(request);

            return Ok();
        }

        [HttpPost]
        [Route("api/signin")]
        public async Task<ActionResult<string>> SignIn(User user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = "us-east-1_h2hBTYgfp",
                ClientId = _clientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            var response = await cognito.AdminInitiateAuthAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(response.AuthenticationResult.IdToken);
            }
            else
            {
                return BadRequest(response.HttpStatusCode);
            }

            

        }
    }
}
