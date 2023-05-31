using MessageProcessor.Models;

namespace MessageProcessor.Repositories.Interfaces;

public interface IUserSongRepository
{
    Task<UserSong> AddLike(UserSong userSong);

    Task<UserSong?> GetLike(int userId, int songId);
    
    Task<int> RemoveLike(UserSong userSong);
}