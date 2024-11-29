namespace ValueTaskVsTask
{
    public class UserProfileService
    {
        private readonly Dictionary<int, UserProfile> _cache = new();
        private readonly Random _random = new();

        public async Task<UserProfile> GetUserProfileTaskAsync(int userId)
        {
            if (_cache.TryGetValue(userId, out UserProfile profile))
            {
                return profile;
            }

            // Simulate cache miss 10% of the time
            if (_random.Next(10) == 0)
            {
                profile = await FetchFromDatabaseAsync(userId);
                _cache[userId] = profile;
            }
            else
            {
                profile = new UserProfile { UserId = userId, Name = "Cached User" };
            }

            return profile;
        }

        public async ValueTask<UserProfile> GetUserProfileValueTaskAsync(int userId)
        {
            if (_cache.TryGetValue(userId, out UserProfile profile))
            {
                return profile;
            }

            // Simulate cache miss 10% of the time
            if (_random.Next(10) == 0)
            {
                profile = await FetchFromDatabaseAsync(userId);
                _cache[userId] = profile;
            }
            else
            {
                profile = new UserProfile { UserId = userId, Name = "Cached User" };
            }

            return profile;
        }

        private Task<UserProfile> FetchFromDatabaseAsync(int userId)
        {
            // Simulate async database call
            return Task.FromResult(new UserProfile { UserId = userId, Name = "Database User" });
        }
    }

}
