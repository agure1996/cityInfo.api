using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    /// <summary>
    /// Handles authentication operations for users.
    /// </summary>
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration; // Configuration for accessing app settings.

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when the configuration is null.</exception>
        public AuthenticationController(IConfiguration configuration) =>
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        /// <summary>
        /// Represents the request body for user authentication.
        /// </summary>
        public class AuthenticationRequestBody
        {
            public string? Username { get; set; } // The username of the user.
            public string? Password { get; set; } // The password of the user.
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <response code="200">Returns a JWT token if authentication is successful.</response>
        /// <response code="401">Returns Unauthorized if authentication fails.</response>
        /// <param name="authenticationRequestBody">The request body containing user credentials.</param>
        /// <returns>A JWT token if authentication is successful, or Unauthorized if it fails.</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<string> Authenticate([FromBody] AuthenticationRequestBody authenticationRequestBody)
        {
            // Step 1: Validate username and password
            var user = ValidateUserCredentials(authenticationRequestBody.Username, authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized(); // Return Unauthorized if credentials are invalid
            }

            // Step 2: Create token
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForeignKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define claims for the token
            var claimsForToken = new List<Claim>
            {
                new Claim("sub", user.UserId.ToString()), // Subject - user id
                new Claim("given_name", user.FirstName), // User's first name
                new Claim("family_name", user.LastName), // User's last name
                new Claim("city", user.City) // User's city
            };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"], // Issuer of the token
                audience: _configuration["Authentication:Audience"], // Audience for the token
                claims: claimsForToken, // Claims in the token
                expires: DateTime.UtcNow.AddHours(1), // Expiration time of the token
                signingCredentials: signingCredentials); // Signing credentials

            // Step 3: Write the token
            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Ok(tokenToReturn); // Return the generated token
        }

        /// <summary>
        /// Validates user credentials.
        /// Currently returns a fixed user for demonstration purposes.
        /// </summary>
        /// <param name="username">The username to validate.</param>
        /// <param name="password">The password to validate.</param>
        /// <returns>A <see cref="CityInfoUser"/> object if credentials are valid; otherwise, null.</returns>
        private CityInfoUser ValidateUserCredentials(string? username, string? password)
        {
            // For now, we return a fixed user; will implement actual validation later (e.g., against a database)
            return new CityInfoUser(1, username ?? "", "Abbas", "Gure", "Xamar City");
        }

        /// <summary>
        /// Represents a user in the CityInfo system.
        /// </summary>
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

            public int UserId { get; set; } // User's unique identifier
            public string UserName { get; set; } // User's username
            public string FirstName { get; set; } // User's first name
            public string LastName { get; set; } // User's last name
            public string City { get; set; } // User's city
        }
    }
}
