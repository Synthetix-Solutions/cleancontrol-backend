#region

using CleanControlDb;
using Microsoft.EntityFrameworkCore;

#endregion

namespace CleanControlBackend.Logic;

public static class Users {
	public static IEnumerable<User> GetAllUsers(CleancontrolContext db) => db.Users;
}
