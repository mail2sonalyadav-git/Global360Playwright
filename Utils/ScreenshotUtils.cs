using Microsoft.Playwright;

namespace Global360Playwright.Utils
{
    internal static class ScreenshotUtils
    {
        public static async Task<string> CaptureAsync(IPage page, string name = "")
        {
            var directory = PathUtils.GetScreenshotPath();
            Directory.CreateDirectory(directory);

            var defaultName = String.IsNullOrWhiteSpace(name) ? "Screenshot" : name;
            var fileName = $"{defaultName}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss-fff}_{Guid.NewGuid():N}.png";
            var filePath = Path.Combine(directory, fileName);

            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = filePath,
                FullPage = true
            });

            return filePath;
        }
    }
}