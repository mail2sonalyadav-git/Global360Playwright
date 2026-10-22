using Microsoft.Playwright;

namespace Global360Playwright.PageObjects
{
    internal sealed class DashboardPage
    {
        private readonly IPage _page;

        public DashboardPage(IPage page) => _page = page;

        private ILocator AssetsViewAllLink => _page.GetByRole(AriaRole.Link, new() { Name = "Assets view all" });

        public Task OpenAssetsAsync() => AssetsViewAllLink.ClickAsync();
    }
}