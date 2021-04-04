using LibraryCoder.MainPageCommon;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ColorBars.Pages
{
    public sealed partial class About : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        public About()
        {
            InitializeComponent();
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
            LibMPC.OutputMsgNormal(TblkPageMsg, $"Application by Paul Ghilino.  Version: {mainPage.stringAppVersion}");
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);
            // Set size of buttons on About page to same size.
            List<Button> listButtonsThisPage = new List<Button>()
            {
                ButRateApp,
                ButSMPTE,
                ButLearnMore,
                ButEmail
            };
            LibMPC.SizePageButtons(listButtonsThisPage);
            LibMPC.ButtonEmailXboxDisable(ButEmail);
            // Setup scrolling for this page.
            LibMPC.ScrollViewerOn(mainPage.mainPageScrollViewer, horz: ScrollMode.Disabled, vert: ScrollMode.Auto, horzVis: ScrollBarVisibility.Disabled, vertVis: ScrollBarVisibility.Auto, zoom: ZoomMode.Disabled);
            ButRateApp.Focus(FocusState.Programmatic);      // Set focus to first button on page.
        }

        /// <summary>
        /// Invoked when user clicks a button requesting link to more information.
        /// </summary>
        /// <param name="sender">A button with a Tag that contains hyperlink string.</param>
        /// <param name="e"></param>
        private async void ButHyperlink_Click(object sender, RoutedEventArgs e)
        {
            _ = e;          // Discard unused parameter.
            await LibMPC.ButtonHyperlinkLaunchAsync((Button)sender);
        }

        /// <summary>
        /// Show User MS Store App rating popup box. Popup box will lock all access to App until closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButRateApp_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            await mainPage.RateAppInW10Store();
        }
    }
}
