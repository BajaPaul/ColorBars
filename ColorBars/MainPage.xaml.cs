using ColorBars.Pages;
using LibraryCoder.MainPageCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Following Enum is generally unique for each App so place here.
/// <summary>
/// Enum used to reset App setup value via method AppReset().
/// </summary>
public enum EnumResetApp { DoNothing, ResetApp, ResetPurchaseHistory, ResetRateHistory, ShowDataStoreValues };

namespace ColorBars
{
    public sealed partial class MainPage : Page
    {
        // TODO: Update version number in next string before publishing application to Microsoft Store.
        /// <summary>
        /// String containing version of application as set in Package.appxmanifest file.
        /// </summary>
        public readonly string stringAppVersion = "2021.4.3";

        /// <summary>
        /// Pointer to MainPage. Other pages can use this pointer to access public variables and methods in MainPage.
        /// </summary>
        public static MainPage mainPagePointer;

        /// <summary>
        /// Pointer to Showbars. Enables MainPage buttons to call public methods in ShowBars.
        /// This pointer is not initialized until ShowBars page loads.
        /// </summary>
        public ShowBars showBars;

        /// <summary>
        /// Location App uses to read or write various App settings. Save set value here for use in other pages as needed.
        /// </summary>
        public ApplicationDataContainer applicationDataContainer;

        // All data store 'ds' strings (keys) used by application declared here. These are (key, value) pairs. Each key has a matching value.

        /// <summary>
        /// Value is "BoolAppPurchased".
        /// </summary>
        public readonly string ds_BoolAppPurchased = "BoolAppPurchased";

        /// <summary>
        /// Value is "BoolAppRated".
        /// </summary>
        public readonly string ds_BoolAppRated = "BoolAppRated";

        /// <summary>
        /// Value is "IntAppRatedCounter".
        /// </summary>
        public readonly string ds_IntAppRatedCounter = "IntAppRatedCounter";

        /// <summary>
        /// True if application has been purchased, false otherwise.
        /// </summary>
        public bool boolAppPurchased = false;

        /// <summary>
        /// True if application has been rated, false otherwise.
        /// </summary>
        public bool boolAppRated = false;

        /// <summary>
        /// True if application purchase check has been competed, false otherwise.
        /// </summary>
        public bool boolPurchaseCheckCompleted = false;

        /// <summary>
        /// Save purchase check output string here for display on page Start if User comes back to page.
        /// </summary>
        public string stringPurchaseCheckOutput;

        /// <summary>
        /// True if large rectangle in page ShowBars is visible, false otherwise.
        /// </summary>
        public bool boolShowBarsLargeRect = false;

        // Make following MainPage XAML variables public via wrapper so values can be changed using methods in LibMPC from various locations.

        /// <summary>
        /// Set public value of MainPage XAML ScrollViewerMP.
        /// </summary>
        public ScrollViewer mainPageScrollViewer;

        /// <summary>
        /// Set public value of MainPage XAML ButBack.
        /// </summary>
        public Button mainPageButBack;

        /// <summary>
        /// Set public value of MainPage XAML ButAbout.
        /// </summary>
        public Button mainPageButAbout;

        /// <summary>
        /// Set public value of MainPage XAML ButLabels.
        /// </summary>
        public Button mainPageButLabels;

        /// <summary>
        /// Set public value of MainPage XAML ButReverse.
        /// </summary>
        public Button mainPageButReverse;

        /// <summary>
        /// Set public value of MainPage XAML ButRotate.
        /// </summary>
        public Button mainPageButRotate;

        /// <summary>
        /// Set public value of MainPage XAML TblkAppTitle.
        /// </summary>
        public TextBlock mainPageTblkAppTitle;

        public MainPage()
        {
            InitializeComponent();
            mainPagePointer = this;     // Set pointer to this page at this location since required by various pages, methods, and libraries.
        }

        /*** Public Methods ****************************************************************************************************/

        /// <summary>
        /// Navigate to page Start and clear back history.
        /// </summary>
        public void NavigateToPageStart()
        {
            FrameMP.Navigate(typeof(Start));
            FrameMP.BackStack.Clear();
        }

        /// <summary>
        /// Navigate to page About.
        /// </summary>
        public void NavigateToPageAbout()
        {
            FrameMP.Navigate(typeof(About));
        }

        /// <summary>
        /// Navigate to page ShowBars.
        /// </summary>
        public void NavigateToPageShowBars()
        {
            FrameMP.Navigate(typeof(ShowBars));
        }

        /// <summary>
        /// Open Windows 10 Store App so User can rate and review this App.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RateAppInW10Store()
        {
            if (await LibMPC.ShowRatingReviewDialogAsync())
            {
                boolAppRated = true;
                applicationDataContainer.Values[ds_BoolAppRated] = true;        // Write setting to data store. 
                applicationDataContainer.Values.Remove(ds_IntAppRatedCounter);  // Remove ds_IntAppRatedCounter since no longer used.
                return true;
            }
            return false;
        }

        /*** Private Methods ***************************************************************************************************/

        /// <summary>
        /// Reset App to various states using parameter enumResetApp.
        /// </summary>
        /// <param name="enumResetApp">Enum used to reset App setup values.</param>
        private void AppReset(EnumResetApp enumResetApp)
        {
            switch (enumResetApp)
            {
                case EnumResetApp.DoNothing:                // Do nothing. Most common so exit quick.
                    break;
                case EnumResetApp.ResetApp:                 // Clear all data store settings.
                    applicationDataContainer.Values.Clear();
                    break;
                case EnumResetApp.ResetPurchaseHistory:     // Clear App purchase history.
                    applicationDataContainer.Values.Remove(ds_BoolAppPurchased);
                    boolAppPurchased = false;
                    break;
                case EnumResetApp.ResetRateHistory:         // Clear App rate history.
                    applicationDataContainer.Values.Remove(ds_BoolAppRated);
                    boolAppRated = false;
                    break;
                case EnumResetApp.ShowDataStoreValues:         // Show data store values via Debug.
                    LibMPC.ListDataStoreItems(applicationDataContainer);
                    break;
                default:    // Throw exception so error can be discovered and corrected.
                    throw new NotSupportedException($"MainPage.AppReset(): enumResetApp={enumResetApp} not found in switch statement.");
            }
        }

        /// <summary>
        /// Back-a-page navigation event handler. Invoked when software or hardware back button is selected, 
        /// or Windows key + Backspace is entered, or say, "Hey Cortana, go back".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackRequestedPage(object sender, BackRequestedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            // If event has not already been handled then navigate back to previous page.
            // Next if statement required to prevent App from ending abruptly on a back event.
            if (FrameMP.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                PageGoBack();
            }
        }

        /// <summary>
        /// Navigate back a page. If large rectangle in page ShowBars is visible, then collapse large rectangle instead of going back to previous page.
        /// </summary>
        private void PageGoBack()
        {
            if (FrameMP.CanGoBack)
            {
                if (boolShowBarsLargeRect)  // True if large rectangle in page ShowBars is visible, false otherwise.
                    showBars.LargeRectCollapse();   // Collapse large rectangle instead of going back to previous page.
                else    // Go back to previous page.
                    FrameMP.GoBack();
            }
        }

        /*** Page Events *******************************************************************************************************/

        /// <summary>
        /// Initialize settings for this page and set visibility of title bar items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Set MainPage public values XAML variables so can be called from library LibMPC.
            mainPageScrollViewer = ScrollViewerMP;
            mainPageButBack = ButBack;
            mainPageButAbout = ButAbout;
            mainPageButLabels = ButLabels;
            mainPageButReverse = ButReverse;
            mainPageButRotate = ButRotate;
            mainPageTblkAppTitle = TblkAppTitle;
            // Back-a-page navigation event handler. Invoked when software or hardware back button is selected, 
            // or Windows key + Backspace is entered, or say, "Hey Cortana, go back".
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequestedPage;
            // Get App data store location used to write or read various settings to or from location.
            // https://msdn.microsoft.com/windows/uwp/app-settings/store-and-retrieve-app-data#local-app-data
            applicationDataContainer = ApplicationData.Current.LocalSettings;
            // Comment out next two lines before store publish.
            // StorageFolder storageFolderApp = ApplicationData.Current.LocalFolder;
            // Debug.WriteLine($"storageFolderApp.Path={storageFolderApp.Path}");
            LibMPC.CustomizeAppTitleBar();
            List<Button> listButtonsThisPage = new List<Button>()
            {
                ButBack,
                ButAbout,
                ButLabels,
                ButReverse,
                ButRotate
            };
            LibMPC.SizePageButtons(listButtonsThisPage);

            // TODO: set next line to EnumResetApp.DoNothing before store publish.
            AppReset(EnumResetApp.DoNothing);   // Reset App to various states using parameter enumResetApp.

            // Get data store values for next two items and set to true or false.
            boolAppPurchased = LibMPC.DataStoreStringToBool(applicationDataContainer, ds_BoolAppPurchased);
            boolAppRated = LibMPC.DataStoreStringToBool(applicationDataContainer, ds_BoolAppRated);
            // AppReset(EnumResetApp.ShowDataStoreValues);     // TODO: Comment out this line before store publish. Show data store values.
            LibMPC.ScrollViewerOff(ScrollViewerMP);     // Turn mainScroller off for now.  Individual pages will set it as required.
            NavigateToPageStart();
        }

        /// <summary>
        /// Navigate back to previous page when User clicks Back button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButBack_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            PageGoBack();
        }

        /// <summary>
        /// Navigate to page About. Button is not visible unless User is on page Start.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButAbout_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            NavigateToPageAbout();
        }

        /// <summary>
        /// Move color bar labels when User clicks Labels button. Button is not visible unless User is on page ShowBars.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLabels_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            showBars.ShowBarsLabels();
        }

        /// <summary>
        /// Reverse color bars when User clicks Reverse button. Button is not visible unless User is on page ShowBars.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButReverse_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Debug.WriteLine("MainPage.ButReverse_Click(): Reverse color bars when User clicks Reverse button");
            showBars.ShowBarsReverse();
        }

        /// <summary>
        /// Rotate color bars when User clicks Rotate button. Button is not visible unless User is on page ShowBars.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButRotate_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Debug.WriteLine("MainPage.ButRotate_Click(): Rotate color bars when User clicks Rotate button");
            showBars.ShowBarsRotate();
        }

    }
}
