using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _db;
    private readonly IMapper _mapper;

    public UserRepository(DataContext db, IMapper mapper)
    {
        _mapper = mapper;
        _db = db;
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
        return await _db.Users
        .Where(u => u.UserName == username)
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        return await _db.Users
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _db.Users.Include(u => u.Photos).ToListAsync();
    }

    public async Task<AppUser> GetUsersByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUsersByUsernameAsync(string username)
    {
        return await _db.Users.Include(u => u.Photos).FirstAsync(user => user.UserName == username);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        // wtf
        _db.Entry(user).State = EntityState.Modified;
    }
}