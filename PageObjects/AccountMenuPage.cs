using Microsoft.Playwright;

namespace Global360Playwright.PageObjects
{
    internal sealed class AccountMenuPage
    {
        private readonly IPage _page;

        public AccountMenuPage(IPage page) => _page = page;

        private ILocator ProfileMenuButton => _page.Locator("nav a.dropdown-toggle:has(img.user-image)");
        private ILocator LogoutLink => _page.GetByRole(AriaRole.Link, new() { Name = "Logout", Exact = true });
        private ILocator LoginUsernameInput => _page.Locator("#username");

        public Task<bool> IsProfileMenuVisibleAsync() => ProfileMenuButton.IsVisibleAsync();

        public async Task LogoutAsync()
        {
            await ProfileMenuButton.ClickAsync();
            await LogoutLink.ClickAsync();
            await LoginUsernameInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }
    }
}