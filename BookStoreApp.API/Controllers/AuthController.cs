using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.DTOs.User;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreApp.API.Controllers {
   [Route("API/[controller]")]
   [ApiController]
   public class AuthController : ControllerBase {
      private readonly ILogger<AuthController> _logger;
      private readonly IMapper _mapper;
      private readonly UserManager<ApiUser> _userManager;
      private readonly IConfiguration _configuration;

      public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration) {
         _logger = logger;
         _mapper = mapper;
         _userManager = userManager;
         _configuration = configuration;
      }

      /// <summary>
      /// Controller action to register a User in the system.
      /// </summary>
      /// <param name="userDTO"></param>
      /// <returns></returns>
      [HttpPost("register")]
      [ProducesResponseType(StatusCodes.Status202Accepted)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> Register(UserDTO userDTO) {
         try {
            _logger.LogInformation($"Login attempt for {userDTO.Email}");

            var user = _mapper.Map<ApiUser>(userDTO);
            user.UserName = userDTO.Email;
            var result = await _userManager.CreateAsync(user, userDTO.Password);

            if (!result.Succeeded) {
               foreach (var error in result.Errors) {
                  ModelState.AddModelError(error.Code, error.Description);
               }

               return BadRequest(ModelState);
            }

            await _userManager.AddToRoleAsync(user, "User");

            return Accepted();
         } catch (Exception ex) {
            _logger.LogError(ex, $"Something went wrong in {nameof(Register)}");
            return Problem($"Something went wrong in {nameof(Register)}", statusCode: 500);
         }
      }

      /// <summary>
      /// Controller action to Login to the system
      /// </summary>
      /// <param name="userDTO"></param>
      /// <returns></returns>
      [HttpPost("Login")]
      [ProducesResponseType(StatusCodes.Status202Accepted)]
      [ProducesResponseType(StatusCodes.Status401Unauthorized)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> Login(LoginUserDTO userDTO) {
         try {
            _logger.LogInformation($"Login attempt for {userDTO.Email}");

            var user = await _userManager.FindByEmailAsync(userDTO.Email);
            var passwordValid = await _userManager.CheckPasswordAsync(user, userDTO.Password);

            if (user == null || !passwordValid) {
               return Unauthorized();
            }

            string tokenString = await GenerateToken(user);
            var response = new AuthResponse {
               UserId = user.Id,
               Token = tokenString,
               Email = user.Email
            };

            return Accepted(response);
         } catch (Exception ex) {
            _logger.LogError(ex, $"Something went wrong in {nameof(Login)}");
            return Problem($"Something went wrong in {nameof(Login)}", statusCode: 500);
         }
      }

      private async Task<string> GenerateToken(ApiUser user) {
         var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

         var roles = await _userManager.GetRolesAsync(user);
         var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

         var userClaims = await _userManager.GetClaimsAsync(user);

         var claims = new List<Claim>() {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(CustomClaimTypes.Uid, user.Id)
         }
         .Union(userClaims)
         .Union(roleClaims);

         var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToInt32(_configuration["JwtSettings:Duration"])),
            signingCredentials: credentials
         );

         return new JwtSecurityTokenHandler().WriteToken(token);
      }
   }
}
