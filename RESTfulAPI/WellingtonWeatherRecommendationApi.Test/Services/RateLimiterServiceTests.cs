using System;
using System.Threading.Tasks;
using WellingtonWeatherRecommendationApi.Services;
using Xunit;

namespace WellingtonWeatherRecommendationApi.Tests.Services
{
    public class RateLimiterServiceTests
    {
        private readonly RateLimiterService _rateLimiterService;

        public RateLimiterServiceTests()
        {
            _rateLimiterService = new RateLimiterService();
        }

        [Fact]
        public async Task IsRequestAllowedAsync_WithinLimit_ReturnsTrue()
        {
            // Arrange
            string clientIp = "192.168.1.1";

            // Act
            bool result = await _rateLimiterService.IsRequestAllowedAsync(clientIp);

            // Assert
            Assert.True(result, "First request should be allowed within limit.");
        }

        [Fact]
        public async Task IsRequestAllowedAsync_ExceedsLimit_BlocksAndReturnsFalse()
        {
            // Arrange
            string clientIp = "192.168.1.2";
            int requestLimit = 5;

            // Act
            for (int i = 0; i < requestLimit; i++)
            {
                bool allowed = await _rateLimiterService.IsRequestAllowedAsync(clientIp);
                Assert.True(allowed, $"Request {i + 1} should be allowed.");
            }
            bool result = await _rateLimiterService.IsRequestAllowedAsync(clientIp);

            // Assert
            Assert.False(result, "Sixth request should be blocked after exceeding limit.");
        }

        [Fact]
        public async Task IsRequestAllowedAsync_WindowExpired_ResetsCountAndAllowsRequest()
        {
            // Arrange
            string clientIp = "192.168.1.3";
            int requestLimit = 5;

            // Act
            for (int i = 0; i < requestLimit; i++)
            {
                await _rateLimiterService.IsRequestAllowedAsync(clientIp);
            }

            // Simulate window expiration by manipulating time (requires reflection or mocking)
            // For simplicity, we'll assume the window is 1 minute and wait
            await Task.Delay(TimeSpan.FromMinutes(1).Add(TimeSpan.FromSeconds(1)));
            bool result = await _rateLimiterService.IsRequestAllowedAsync(clientIp);

            // Assert
            Assert.True(result, "Request should be allowed after window expires.");
        }

        [Fact]
        public async Task IsRequestAllowedAsync_BlockedClient_ReturnsFalse()
        {
            // Arrange
            string clientIp = "192.168.1.4";
            int requestLimit = 5;

            // Act
            for (int i = 0; i <= requestLimit; i++)
            {
                await _rateLimiterService.IsRequestAllowedAsync(clientIp);
            }
            bool result = await _rateLimiterService.IsRequestAllowedAsync(clientIp);

            // Assert
            Assert.False(result, "Request should be blocked during block duration.");
        }

        [Fact]
        public async Task IsRequestAllowedAsync_NullClientIp_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _rateLimiterService.IsRequestAllowedAsync(null));
        }

        [Fact]
        public async Task IsRequestAllowedAsync_ConcurrentRequests_HandlesThreadSafety()
        {
            // Arrange
            string clientIp = "192.168.1.5";
            int requestLimit = 5;
            var tasks = new Task<bool>[requestLimit + 1];

            // Act
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _rateLimiterService.IsRequestAllowedAsync(clientIp);
            }
            bool[] results = await Task.WhenAll(tasks);

            // Assert
            int allowedCount = results.Count(r => r);
            Assert.Equal(requestLimit, allowedCount); // Only 5 requests should be allowed
            Assert.Contains(results, r => !r); // At least one should be blocked
        }

        [Fact]
        public async Task GetRetryAfterTime_BlockedClient_ReturnsRemainingTime()
        {
            // Arrange
            string clientIp = "192.168.1.6";
            int requestLimit = 5;

            // Act
            for (int i = 0; i <= requestLimit; i++)
            {
                await _rateLimiterService.IsRequestAllowedAsync(clientIp);
            }
            TimeSpan? retryAfter = _rateLimiterService.GetRetryAfterTime(clientIp);

            // Assert
            Assert.NotNull(retryAfter);
            Assert.True(retryAfter > TimeSpan.Zero, "Retry-after time should be positive.");
            Assert.True(retryAfter <= TimeSpan.FromMinutes(5), "Retry-after time should not exceed block duration.");
        }

        [Fact]
        public void GetRetryAfterTime_NotBlockedClient_ReturnsNull()
        {
            // Arrange
            string clientIp = "192.168.1.7";

            // Act
            TimeSpan? retryAfter = _rateLimiterService.GetRetryAfterTime(clientIp);

            // Assert
            Assert.True(retryAfter == null, "Retry-after time should be null for non-blocked client.");
        }

        [Fact]
        public async Task GetRetryAfterTime_AfterBlockExpires_ReturnsNull()
        {
            // Arrange
            string clientIp = "192.168.1.8";
            int requestLimit = 5;

            // Act
            for (int i = 0; i <= requestLimit; i++)
            {
                await _rateLimiterService.IsRequestAllowedAsync(clientIp);
            }

            // Simulate block duration expiration
            await Task.Delay(TimeSpan.FromMinutes(5).Add(TimeSpan.FromSeconds(1)));
            TimeSpan? retryAfter = _rateLimiterService.GetRetryAfterTime(clientIp);

            // Assert
            Assert.True(retryAfter == null, "Retry-after time should be null after block expires.");
        }
    }
}