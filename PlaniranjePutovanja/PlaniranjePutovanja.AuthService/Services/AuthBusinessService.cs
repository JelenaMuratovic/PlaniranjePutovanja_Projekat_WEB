using FluentValidation;
using PlaniranjePutovanja.AuthService.Helpers.Passwords;
using PlaniranjePutovanja.AuthService.Helpers.Tokens;
using PlaniranjePutovanja.AuthService.Mappers;
using PlaniranjePutovanja.AuthService.Models;
using PlaniranjePutovanja.AuthService.Repositories;
using PlaniranjePutovanja.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Services
{
    public class AuthBusinessService : IAuthBusinessService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthMapper _authMapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;

        public AuthBusinessService(
        IUserRepository userRepository,
        IAuthMapper authMapper,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator)
        {
            _userRepository = userRepository;
            _authMapper = authMapper;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        //public async Task<bool> CanRegisterAsync(string email, CancellationToken cancellationToken = default)
        //{
        //    return !await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        //}

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (emailExists)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"User with email '{request.Email}' already exists."
                };
            }

            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = _authMapper.MapToUser(request, passwordHash);

            await _userRepository.AddAsync(user, cancellationToken);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Registration successful.",
                User = _authMapper.MapToUserDto(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid credentials."
                };
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successful.",
                User = _authMapper.MapToUserDto(user),
                AccessToken = accessToken
            };
        }

        //public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        //{
        //    var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        //    return user == null ? null : _authMapper.MapToUserDto(user);
        //}
    }
}
