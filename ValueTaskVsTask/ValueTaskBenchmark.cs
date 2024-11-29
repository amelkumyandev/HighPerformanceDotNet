using BenchmarkDotNet.Attributes;


namespace ValueTaskVsTask
{
    public class ValueTaskBenchmark
    {
        private readonly UserProfileService _service = new UserProfileService();

        [Benchmark]
        public async Task<UserProfile> UsingTask()
        {
            return await _service.GetUserProfileTaskAsync(1);
        }

        [Benchmark]
        public async ValueTask<UserProfile> UsingValueTask()
        {
            return await _service.GetUserProfileValueTaskAsync(1);
        }
    }

}
