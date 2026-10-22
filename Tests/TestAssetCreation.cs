using Microsoft.Playwright;
using Global360Playwright.PageObjects;
using Global360Playwright.TestData;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright.NUnit;

namespace Global360Playwright.Tests
{
    // Inherit from BaseTest
    internal class TestAssetCreation : BaseTest
    {
        
        [Test]
        public async Task CreateAsset_VerifyDetailsAndHistory()
        {
            await NavigateToLoginPage();

            // Assert page title
            await Expect(Page).ToHaveTitleAsync("Snipe-IT Asset Management Demo");

            var username = _config.Username;
            var password = _config.Password;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Set Configuration__Username and Configuration__Password using a local secret store or the CI secret manager.");
            }

            var loginPage = new LoginPage(Page);
            await loginPage.LoginAsync(username, password);
            await Expect(Page).ToHaveTitleAsync("Dashboard :: Snipe-IT Asset Management Demo");

            var dashboardPage = new DashboardPage(Page);
            await dashboardPage.OpenAssetsAsync();

            var assetsPage = new AssetsPage(Page);
            await assetsPage.OpenCreateAssetFormAsync();

            var testData = AssetCreationTestData.Create();
            var createAssetPage = new CreateAssetPage(Page);
            await createAssetPage.FillAssetTagAsync(testData.AssetTag);
            await createAssetPage.SelectFirstCompanyAsync();
            await createAssetPage.SelectModelAsync(testData.ModelSelection);
            await createAssetPage.SelectStatusAsync(testData.Status);
            await createAssetPage.SelectFirstLocationAsync();
            await createAssetPage.SelectFirstDefaultLocationAsync();
            await createAssetPage.FillSerialNumberAsync(testData.SerialNumber);

            var selectedUserName = await createAssetPage.SelectRandomUserAsync();
            await Expect(createAssetPage.SelectedUser).Not.ToHaveTextAsync("Select a User");
            await createAssetPage.FillNotesAsync(testData.Notes);

            // Click the "Save" button to create the asset
            await createAssetPage.SaveAsync();

            // Locate the success notification and verify it confirms this asset tag.
            await Expect(assetsPage.SuccessLabel).ToBeVisibleAsync();
            await Expect(assetsPage.SuccessNotification).ToContainTextAsync(testData.SuccessMessage);

            // Capture the link to the newly created asset from the notification.
            await Expect(assetsPage.ViewAssetLink).ToBeVisibleAsync();
            await Expect(assetsPage.ViewAssetLink).ToHaveAttributeAsync("href", new Regex(@"/hardware/\d+$"));
            var assetHref = await assetsPage.ViewAssetLink.GetAttributeAsync("href") ?? string.Empty;
            var notificationAssetPath = new Uri(new Uri(Page.Url), assetHref!).AbsolutePath;

            // Search the Assets list for the generated tag and open that result.
            await Expect(assetsPage.Heading).ToBeVisibleAsync();
            await Expect(assetsPage.AssetsTable).ToBeVisibleAsync();
            await assetsPage.SearchAsync(testData.AssetTag);

            var assetTagLink = assetsPage.AssetTagLink(testData.AssetTag);
            // Wait for the filtered result and fail with the tag if it is missing.
            await Expect(assetTagLink).ToBeVisibleAsync(new() { Timeout = 10000 });
            await Expect(assetTagLink).ToHaveTextAsync(testData.AssetTag);
            await Expect(assetTagLink).ToHaveAttributeAsync("href", new Regex(Regex.Escape(notificationAssetPath) + "$"));
            await assetTagLink.ClickAsync();

            var assetDetailsPage = new AssetDetailsPage(Page);

            // Verify the detail page shows the values entered or selected during creation.
            await Expect(Page).ToHaveTitleAsync($"View Asset {testData.AssetTag} :: Snipe-IT Asset Management Demo");
            await Expect(assetDetailsPage.AssetTag(testData.AssetTag)).ToBeVisibleAsync();
            await Expect(assetDetailsPage.ModelLink(testData.ExpectedModelName)).ToBeVisibleAsync();
            await Expect(assetDetailsPage.StatusText(testData.Status)).ToBeVisibleAsync();
            await Expect(assetDetailsPage.CheckedOutUserLink(selectedUserName)).ToBeVisibleAsync();
            await Expect(assetDetailsPage.DetailText(testData.SerialNumber)).ToBeVisibleAsync();
            await Expect(assetDetailsPage.DetailText(testData.Notes)).ToBeVisibleAsync();

            // Verify asset creation and checkout history for this tag and selected user.
            await assetDetailsPage.HistoryTab.ClickAsync();
            await Expect(assetDetailsPage.HistoryHeading).ToBeVisibleAsync();
            await Expect(assetDetailsPage.HistoryTable).ToBeVisibleAsync();

            // Snipe-IT records creation as "create new" and assignment as "checkout".
            var creationEvent = assetDetailsPage.CreationEvent(testData.AssetTag);
            await Expect(creationEvent).ToBeVisibleAsync(new() { Timeout = 10000 });

            var checkoutEvent = assetDetailsPage.CheckoutEvent(testData.AssetTag);
            await Expect(checkoutEvent).ToBeVisibleAsync(new() { Timeout = 10000 });
            await Expect(checkoutEvent).ToContainTextAsync(selectedUserName);

            var accountMenuPage = new AccountMenuPage(Page);
            await accountMenuPage.LogoutAsync();
            await Expect(Page).ToHaveTitleAsync("Snipe-IT Asset Management Demo");
        }

    }

    
}