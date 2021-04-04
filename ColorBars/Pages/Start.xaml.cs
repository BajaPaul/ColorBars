using LibraryCoder.MainPageCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ColorBars.Pages
{
    public sealed partial class Start : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        /// <summary>
        /// Show User ButRateApp button if this number of page loads since last reset.  Current value is 6.
        /// </summary>
        private readonly int intShowButRateApp = 6;

        public Start()
        {
            InitializeComponent();
        }

        /*** Private methods ***************************************************************************************************/

        /// <summary>
        /// Get purchase status of application. Method controls visibility/Enable of PBarStatus, TblkPurchaseApp, and ButPurchaseApp.
        /// </summary>
        private async Task AppPurchaseCheck()
        {
            if (mainPage.boolAppPurchased)
            {
                // App has been purchased so hide following values and return.
                PBarStatus.Visibility = Visibility.Collapsed;
                TblkPurchaseApp.Visibility = Visibility.Collapsed;
                LibMPC.ButtonVisibility(ButPurchaseApp, false);
            }
            else
            {
                if (mainPage.boolPurchaseCheckCompleted)
                {
                    // App has not been purchased but purchase check done so show previous message. This occurs if User returning from another page.
                    PBarStatus.Visibility = Visibility.Collapsed;
                    LibMPC.OutputMsgError(TblkPurchaseApp, mainPage.stringPurchaseCheckOutput);
                    TblkPurchaseApp.Visibility = Visibility.Visible;
                    LibMPC.ButtonVisibility(ButPurchaseApp, true);
                }
                else
                {
                    // App has not been purchased so do purchase check.
                    LibMPC.OutputMsgBright(TblkPurchaseApp, "Application purchase check in progress...");
                    PBarStatus.Foreground = LibMPC.colorError;          // Set color PBarStatus from default.
                    PBarStatus.Visibility = Visibility.Visible;
                    PBarStatus.IsIndeterminate = true;
                    EnablePageItems(false);
                    mainPage.boolAppPurchased = await LibMPC.AppPurchaseStatusAsync(mainPage.applicationDataContainer, mainPage.ds_BoolAppPurchased);
                    if (mainPage.boolAppPurchased)
                    {
                        LibMPC.OutputMsgSuccess(TblkPurchaseApp, LibMPC.stringAppPurchaseResult);
                        LibMPC.ButtonVisibility(ButPurchaseApp, false);
                    }
                    else
                    {
                        LibMPC.OutputMsgError(TblkPurchaseApp, LibMPC.stringAppPurchaseResult);
                        LibMPC.ButtonVisibility(ButPurchaseApp, true);
                    }
                    PBarStatus.IsIndeterminate = false;
                    PBarStatus.Visibility = Visibility.Collapsed;
                    mainPage.boolPurchaseCheckCompleted = true;
                    mainPage.stringPurchaseCheckOutput = TblkPurchaseApp.Text;
                    EnablePageItems(true);
                }
            }
        }

        /// <summary>
        /// Attempt to buy application. Method controls visibility/Enable of PBarStatus, TblkPurchaseApp, and ButPurchaseApp.
        /// </summary>
        private async Task AppPurchaseBuy()
        {
            LibMPC.OutputMsgBright(TblkPurchaseApp, "Attempting to purchase application...");
            EnablePageItems(false);
            PBarStatus.Foreground = LibMPC.colorError;          // Set color PBarStatus from default.
            PBarStatus.Visibility = Visibility.Visible;
            PBarStatus.IsIndeterminate = true;
            mainPage.boolAppPurchased = await LibMPC.AppPurchaseBuyAsync(mainPage.applicationDataContainer, mainPage.ds_BoolAppPurchased);
            if (mainPage.boolAppPurchased)
            {
                // App purchased.
                LibMPC.OutputMsgSuccess(TblkPurchaseApp, LibMPC.stringAppPurchaseResult);
                LibMPC.ButtonVisibility(ButPurchaseApp, false);
            }
            else
            {
                // App not purchased.
                LibMPC.OutputMsgError(TblkPurchaseApp, LibMPC.stringAppPurchaseResult);
                LibMPC.ButtonVisibility(ButPurchaseApp, true);
            }
            PBarStatus.IsIndeterminate = false;
            PBarStatus.Visibility = Visibility.Collapsed;
            EnablePageItems(true);
        }

        /// <summary>
        /// If application has not been rated then show ButRateApp occasionally.
        /// </summary>
        private void AppRatedCheck()
        {
            if (!mainPage.boolAppRated)
            {
                if (mainPage.applicationDataContainer.Values.ContainsKey(mainPage.ds_IntAppRatedCounter))
                {
                    int intAppRatedCounter = (int)mainPage.applicationDataContainer.Values[mainPage.ds_IntAppRatedCounter];
                    intAppRatedCounter++;
                    if (intAppRatedCounter >= intShowButRateApp)
                    {
                        // Make ButRateApp visible.
                        if (mainPage.boolAppPurchased)
                            ButRateApp.Margin = new Thickness(16, 8, 16, 16);    //ButPurchaseApp is collapsed so need to change margin from (16, 0, 16 ,16). Order is left, top, right, bottom.
                        mainPage.applicationDataContainer.Values[mainPage.ds_IntAppRatedCounter] = 0;     // Reset data store setting to 0.
                        ButRateApp.Foreground = LibMPC.colorSuccess;
                        LibMPC.ButtonVisibility(ButRateApp, true);
                    }
                    else
                        mainPage.applicationDataContainer.Values[mainPage.ds_IntAppRatedCounter] = intAppRatedCounter;     // Update data store setting to intAppRatedCounter.
                }
                else
                    mainPage.applicationDataContainer.Values[mainPage.ds_IntAppRatedCounter] = 1;     // Initialize data store setting to 1.
            }
        }

        /// <summary>
        /// Enable items on page if boolEnableItems is true, otherwise disable items on page.
        /// </summary>
        /// <param name="boolEnableItems">If true then enable page items, otherwise disable.</param>
        private void EnablePageItems(bool boolEnableItems)
        {
            LibMPC.ButtonIsEnabled(mainPage.mainPageButAbout, boolEnableItems);
            LibMPC.ButtonIsEnabled(ButColorBars, boolEnableItems);
            LibMPC.ButtonIsEnabled(ButPurchaseApp, boolEnableItems);
            LibMPC.ButtonIsEnabled(ButRateApp, boolEnableItems);
        }

        /*** Page Events *******************************************************************************************************/

        /// <summary>
        /// Initialize settings for this page and set visibility of title bar items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            LibMPC.OutputMsgNormal(TblkPageMsg, "This App is intended for multi-monitor configurations.  App displays SMPTE type color bars.  Display can be stretched across multiple monitors.  User can then adjust monitor settings via hardware buttons and/or software settings so display appears consistent across all monitors.\n\nThe display can be reversed and/or rotated allowing comparison of image on one monitor to adjacent monitor.  Double-tap a color bar and its color will expand to fill display.  Double-tap expanded color to return to previous color bar display.  Color bar label position can be toggled Left, Center, Right, or Off, as desired.");
            // Hide button not used on page.
            LibMPC.ButtonVisibility(ButPurchaseApp, false);     // This button will be turned on below when required.
            LibMPC.ButtonVisibility(ButRateApp, false);         // This button will be turned on below when required.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, true);
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButLabels, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButReverse, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButRotate, false);
            mainPage.mainPageTblkAppTitle.Visibility = Visibility.Visible;
            // Set size of buttons on Start page to same size.
            List<Button> listButtonsThisPage = new List<Button>()
            {
                ButColorBars,
                ButPurchaseApp
                // Do not include ButRateApp in this list since considerably larger and only appears intermittently.
            };
            LibMPC.SizePageButtons(listButtonsThisPage);
            await AppPurchaseCheck();   // This method controls visibility/Enable of PBarStatus, TblkPurchaseApp, and ButPurchaseApp as needed.
            AppRatedCheck();
            // Setup scrolling for this page.
            LibMPC.ScrollViewerOn(mainPage.mainPageScrollViewer, horz: ScrollMode.Disabled, vert: ScrollMode.Auto, horzVis: ScrollBarVisibility.Disabled, vertVis: ScrollBarVisibility.Auto, zoom: ZoomMode.Disabled);
            ButColorBars.Focus(FocusState.Programmatic);    // Set focus to first button on page.
        }

        /// <summary>
        /// Navigate to 'ShowBars' page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButColorBars_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            mainPage.NavigateToPageShowBars();
        }

        /// <summary>
        /// Purchase application button. Button visible if application has not been purchased, collapsed otherwise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButPurchaseApp_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            await AppPurchaseBuy();
        }

        /// <summary>
        /// Invoked when user clicks 'ButRateApp'. MS Store popup box will lock out all access to App.
        /// Goal is to get more App ratings in Microsoft Store without hassling User too much.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButRateApp_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            if (await mainPage.RateAppInW10Store())
                LibMPC.ButtonVisibility(ButRateApp, false);
        }

    }
}
