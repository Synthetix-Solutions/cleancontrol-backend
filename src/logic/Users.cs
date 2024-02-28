#region

using CleanControlDb;

#endregion

namespace CleanControlBackend.Logic;

public static class Users {
	public static IEnumerable<User> GetAllUsers(CleancontrolContext db) => db.Users;
}
