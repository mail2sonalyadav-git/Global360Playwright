using Microsoft.Playwright;

namespace Global360Playwright.PageObjects
{
    internal sealed class CreateAssetPage
    {
        private readonly IPage _page;

        public CreateAssetPage(IPage page) => _page = page;

        private ILocator AssetTagInput => _page.Locator("#asset_tag");
        private ILocator CompanySelect => _page.Locator("#select2-company_id_select-container");
        private ILocator ModelPlaceholder => _page.GetByText("Select a Model", new() { Exact = true });
        private ILocator ModelOption(string optionName) => _page.GetByText(optionName);
        private ILocator StatusSelect => _page.Locator("#select2-status_select_id-container");
        private ILocator LocationSelect => _page.Locator("#select2-location_id_location_select-container");
        private ILocator DefaultLocationSelect => _page.Locator("#select2-rtd_location_id_location_select-container");
        private ILocator SerialNumberInput => _page.Locator("#serial_1");
        private ILocator AssignedUserSelect => _page.Locator("#select2-assigned_user_select-container");
        public ILocator SelectedUser => AssignedUserSelect;
        private ILocator NotesInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "notes", Exact = true });
        private ILocator SaveButton => _page.Locator("#submit_button");
        private ILocator FirstOpenOption => _page.Locator(
            ".select2-container--open .select2-results__option[role='option']:not([aria-disabled='true'])").First;
        private ILocator AvailableUserOptions => _page.Locator(
            ".select2-container--open .select2-results__option[role='option']:not([aria-disabled='true']):not(.loading-results)");

        public Task FillAssetTagAsync(string assetTag) => AssetTagInput.FillAsync(assetTag);

        public Task SelectFirstCompanyAsync() => SelectFirstOptionAsync(CompanySelect);

        public async Task SelectModelAsync(string optionName)
        {
            await ModelPlaceholder.ClickAsync();
            await ModelOption(optionName).ClickAsync();
        }

        public async Task SelectStatusAsync(string status)
        {
            await StatusSelect.ClickAsync();
            await _page.GetByRole(AriaRole.Option, new() { Name = status, Exact = true }).ClickAsync();
        }

        public Task SelectFirstLocationAsync() => SelectFirstOptionAsync(LocationSelect);

        public Task SelectFirstDefaultLocationAsync() => SelectFirstOptionAsync(DefaultLocationSelect);

        public Task FillSerialNumberAsync(string serialNumber) => SerialNumberInput.FillAsync(serialNumber);

        public async Task<string> SelectRandomUserAsync()
        {
            await AssignedUserSelect.ClickAsync();
            await AvailableUserOptions.First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

            var optionCount = await AvailableUserOptions.CountAsync();
            var randomOption = AvailableUserOptions.Nth(Random.Shared.Next(optionCount));
            var selectedUserLabel = (await randomOption.InnerTextAsync()).Trim();
            await randomOption.ClickAsync();

            return selectedUserLabel.Split(" (", 2)[0].Trim();
        }

        public Task FillNotesAsync(string notes) => NotesInput.FillAsync(notes);

        public Task SaveAsync() => SaveButton.ClickAsync();

        private async Task SelectFirstOptionAsync(ILocator select)
        {
            await select.ClickAsync();
            await FirstOpenOption.ClickAsync();
        }
    }
}