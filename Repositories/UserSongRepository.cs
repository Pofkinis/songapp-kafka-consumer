using MessageProcessor.Models;
using MessageProcessor.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageProcessor.Repositories;

public class UserSongRepository : IUserSongRepository
{
    private readonly DatabaseContext _databaseContext;
    
    public UserSongRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<UserSong> AddLike(UserSong userSong)
    {
        _databaseContext.Add(userSong);
        
        await _databaseContext.SaveChangesAsync();

        return userSong;
    }

    public async Task<UserSong?> GetLike(int userId, int songId)
    {
        return await _databaseContext.UserSongs.FirstOrDefaultAsync(x => x.SongId == songId && x.UserId == userId);
    }

    public async Task<int> RemoveLike(UserSong userSong)
    {
        _databaseContext.UserSongs.Remove(userSong);
         
        return await _databaseContext.SaveChangesAsync();
    }
}