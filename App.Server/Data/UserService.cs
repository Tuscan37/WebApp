using Microsoft.EntityFrameworkCore;

namespace App.Server.Data;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> GetUserByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<bool> RegisterUser(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas rejestracji użytkownika: {ex.Message}");
            return false;
        }
    }
    public bool IsUsernameTaken(string username)
    {
        return _context.Users.Any(u => u.Username == username);
    }
}
