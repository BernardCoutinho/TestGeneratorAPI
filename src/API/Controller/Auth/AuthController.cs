﻿
namespace TestGeneratorAPI.src.API.Controller.Auth
{
    using Microsoft.AspNetCore.Mvc;
    using TestGeneratorAPI.src.API.Interface.Login;
    using TestGeneratorAPI.src.API.View.Login;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public AuthController(ILoginService authService)
        {
            _loginService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine("Passou aqui");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _loginService.Authenticate(request.Username, request.Password);

            if (token == null)
                return Unauthorized(new { message = "Credenciais inválidas" });

            return Ok(new { Token = "Bearer " + token });
        }
    }
}
