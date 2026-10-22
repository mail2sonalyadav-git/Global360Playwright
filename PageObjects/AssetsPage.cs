using Microsoft.Playwright;

namespace Global360Playwright.PageObjects
{
    internal sealed class AssetsPage
    {
        private readonly IPage _page;

        public AssetsPage(IPage page) => _page = page;

        public ILocator Heading => _page.GetByRole(AriaRole.Heading, new() { Name = "Assets", Exact = true }).First;
        public ILocator AssetsTable => _page.GetByRole(AriaRole.Table, new() { Name = "Assets" });
        public ILocator SuccessNotification => _page.Locator(".alert-success");
        public ILocator SuccessLabel => SuccessNotification.GetByText("Success:", new() { Exact = true });
        public ILocator ViewAssetLink => SuccessNotification.GetByRole(AriaRole.Link, new() { Name = "Click here to view", Exact = true });

        private ILocator CreateAssetButton => _page.GetByRole(AriaRole.Button, new() { Name = "+", Exact = true });
        private ILocator SearchBox => _page.GetByRole(AriaRole.Searchbox, new() { Name = "Search", Exact = true });

        public ILocator AssetTagLink(string assetTag) => AssetsTable.GetByRole(AriaRole.Link, new() { Name = assetTag, Exact = true });

        public Task OpenCreateAssetFormAsync() => CreateAssetButton.ClickAsync();

        public async Task SearchAsync(string assetTag)
        {
            await SearchBox.FillAsync(assetTag);
            await SearchBox.PressAsync("Enter");
        }
    }
}