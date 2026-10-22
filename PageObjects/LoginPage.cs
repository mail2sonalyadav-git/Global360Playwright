using Microsoft.Playwright;

namespace Global360Playwright.PageObjects
{
    internal sealed class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page) => _page = page;

        private ILocator UsernameInput => _page.Locator("#username");
        private ILocator PasswordInput => _page.Locator("#password-field");
        private ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login", Exact = true });

        public Task NavigateAsync(string url) => _page.GotoAsync(url);

        public async Task LoginAsync(string username, string password)
        {
            await UsernameInput.FillAsync(username);
            await PasswordInput.FillAsync(password);
            await LoginButton.ClickAsync();
        }
    }
}