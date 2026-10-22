using Allure.Net.Commons;
using Allure.NUnit;
using Global360Playwright.PageObjects;
using Global360Playwright.Utils;
using Microsoft.Playwright.NUnit;

namespace Global360Playwright.Tests
{
    [AllureNUnit]
    internal abstract class BaseTest : PageTest
    {
        protected Settings _config = null!;

        [SetUp]
        public virtual void Setup()
        {
            _config = ConfigUtils.Load<Settings>("settings.json", "Configuration");
        }

        [TearDown]
        public async Task CaptureFailureAndLogoutAsync()
        {
            var testFailed = string.Equals(
                TestContext.CurrentContext.Result.Outcome.Status.ToString(),
                "Failed",
                StringComparison.Ordinal);

            if (testFailed)
            {
                await AttachFailureScreenshotAsync();
            }

            try
            {
                var accountMenuPage = new AccountMenuPage(Page);
                if (await accountMenuPage.IsProfileMenuVisibleAsync())
                {
                    await accountMenuPage.LogoutAsync();
                }
            }
            catch (Exception exception)
            {
                TestContext.Progress.WriteLine($"Logout during teardown failed: {exception.Message}");
                if (!testFailed)
                {
                    await AttachFailureScreenshotAsync();
                    throw new InvalidOperationException("Logout failed during teardown.", exception);
                }
            }
        }

        public async Task NavigateToLoginPage()
        {
            var baseUrl = _config.BaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("BaseUrl must be configured.");
            }

            var loginPage = new LoginPage(Page);
            await loginPage.NavigateAsync(baseUrl);
        }

        private async Task AttachFailureScreenshotAsync()
        {
            try
            {
                var screenshotPath = await ScreenshotUtils.CaptureAsync(
                    Page,
                    TestContext.CurrentContext.Test.Name);
                AllureApi.AddAttachment("Failure screenshot", "image/png", screenshotPath);
            }
            catch (Exception exception)
            {
                TestContext.Progress.WriteLine($"Could not attach failure screenshot to Allure: {exception.Message}");
            }
        }
    }
}