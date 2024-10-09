using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    //wont use authentication here since this must be accesible for unauthenticated users to create authentication
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        IConfiguration _configuration;
        public AuthenticationController(IConfiguration configuration) => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        /** 
         * We wont use this outside this class so we can scope it to this namespace
         */

        public class AuthenticationRequestBody
        {

            public string? Username { get; set; }
            public string? Password { get; set; }


        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {   
            //steep 1- validate username/password
            var user = ValidateUserCredentials(authenticationRequestBody.Username, authenticationRequestBody.Password);

            if (user == null) { return Unauthorized(); }

            //step 2 - create token
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForeignKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //claims (google what claims are - A claim in this context is identity information related to the user
            //following standards in the claim class we using
            var ClaimsForToken = new List<Claim>();
            ClaimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            ClaimsForToken.Add(new Claim("given_name", user.FirstName));
            ClaimsForToken.Add(new Claim("family_name", user.LastName));
            ClaimsForToken.Add(new Claim("city", user.City));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"], //issuer of token
                _configuration["Authentication:Audience"], //receiver eligible for token (both setup in app dev settings)
                ClaimsForToken, //claims made in token
                DateTime.UtcNow, //value indicating start time of token validity
                DateTime.UtcNow.AddHours(1), //value indicating end time of token validity
                signingCredentials); //singing credentials


            // 3- write the token we just created
            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);

        }

        private CityInfoUser ValidateUserCredentials(string? username, string? password)
        {

            /*
             * Current dont have a database so return current data
             */

            return new CityInfoUser(1, username ?? "", "Abbas", "Gure", "Xamar City");
        }
        private class CityInfoUser
        {
            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }

            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }
        }
    }
}
