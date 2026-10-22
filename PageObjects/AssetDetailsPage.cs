using Microsoft.Playwright;

namespace Global360Playwright.PageObjects
{
    internal sealed class AssetDetailsPage
    {
        private readonly IPage _page;

        public AssetDetailsPage(IPage page) => _page = page;

        public ILocator AssetTag(string assetTag) => _page.GetByText(assetTag, new() { Exact = true });
        public ILocator ModelLink(string modelName) => _page.GetByRole(AriaRole.Link, new() { Name = modelName, Exact = true });
        public ILocator StatusText(string status) => _page.GetByRole(AriaRole.Main).GetByText(status).First;
        public ILocator CheckedOutUserLink(string userName) => _page.GetByRole(AriaRole.Link, new() { Name = userName, Exact = true });
        public ILocator DetailText(string text) => _page.GetByText(text, new() { Exact = true });

        public ILocator HistoryTab => _page.GetByRole(AriaRole.Link, new() { Name = "History" });
        public ILocator HistoryHeading => _page.GetByRole(AriaRole.Heading, new() { Name = "History", Exact = true });
        public ILocator HistoryTable => _page.Locator("#history table").Last;

        private ILocator HistoryRows => HistoryTable.GetByRole(AriaRole.Row);

        public ILocator CreationEvent(string assetTag) => HistoryRows
            .Filter(new() { HasText = "create new" })
            .Filter(new() { HasText = assetTag });

        public ILocator CheckoutEvent(string assetTag) => HistoryRows
            .Filter(new() { HasText = "checkout" })
            .Filter(new() { HasText = assetTag });
    }
}