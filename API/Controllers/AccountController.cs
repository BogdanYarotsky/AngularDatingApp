using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _db;
    private readonly KeyedHashAlgorithm _hash;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext db, KeyedHashAlgorithm hash, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _hash = hash;
        _db = db;
    }

    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username is taken");
        }

        var passwordBytes = Encoding.UTF8.GetBytes(registerDto.Password);
        var passwordHash = _hash.ComputeHash(passwordBytes);
        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = passwordHash,
            PasswordSalt = _hash.Key
        };
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return new UserDTO(user.UserName, _tokenService.CreateToken(user));
    }

    [HttpPost("login")] // api/account/login
    public async Task<ActionResult<UserDTO>> Login(LoginDto loginDto)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username);
        if (user is null) return Unauthorized("User not found");
        _hash.Key = user.PasswordSalt;
        var loginPassword = Encoding.UTF8.GetBytes(loginDto.Password);
        var computedHash = _hash.ComputeHash(loginPassword);
        if (ByteArraysEqual(computedHash, user.PasswordHash))
        {
            return new UserDTO(user.UserName, _tokenService.CreateToken(user));
        }
        else
        {
            return Unauthorized("Invalid password");
        }
    }




    private bool ByteArraysEqual(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
    => a1.SequenceEqual(a2);
    private async Task<bool> UserExists(string username)
    => await _db.Users.AnyAsync(u => u.UserName == username.ToLower());
}
