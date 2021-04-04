using LibraryCoder.MainPageCommon;
using System;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace ColorBars.Pages
{
    /// <summary>
    /// Position of labels shown in horizontal color bars.
    /// </summary>
    enum EnumPositionLabels { center, right, off, left };

    public sealed partial class ShowBars : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        /// <summary>
        /// Setup SMPTE colors ARGB bytes.  Format is (A, R, G, B). Also set coresponding text color to use within 
        /// defined color so text shows clearly through background. This is a 2 dimensional array.
        /// Number of colors defined here has to match XAML code and XAML resource "numColors"
        /// </summary>
        private readonly SolidColorBrush[,] arraySolidColorBrushBarColors = new SolidColorBrush[,]
        {
            { new SolidColorBrush(Color.FromArgb(255, 255, 000, 000)),  new SolidColorBrush(Colors.White) },  // Red
            { new SolidColorBrush(Color.FromArgb(255, 000, 255, 000)),  new SolidColorBrush(Colors.Black) },  // Green
            { new SolidColorBrush(Color.FromArgb(255, 000, 000, 255)),  new SolidColorBrush(Colors.White) },  // Blue
            { new SolidColorBrush(Color.FromArgb(255, 255, 255, 000)),  new SolidColorBrush(Colors.Black) },  // Yellow
            { new SolidColorBrush(Color.FromArgb(255, 255, 000, 255)),  new SolidColorBrush(Colors.Black) },  // Magenta
            { new SolidColorBrush(Color.FromArgb(255, 000, 255, 255)),  new SolidColorBrush(Colors.Black) },  // Cyan
            { new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),  new SolidColorBrush(Colors.Black) },  // White
            { new SolidColorBrush(Color.FromArgb(255, 128, 000, 000)),  new SolidColorBrush(Colors.White) },  // Dim Red
            { new SolidColorBrush(Color.FromArgb(255, 000, 128, 000)),  new SolidColorBrush(Colors.Black) },  // Dim Green
            { new SolidColorBrush(Color.FromArgb(255, 000, 000, 128)),  new SolidColorBrush(Colors.White) },  // Dim Blue
            { new SolidColorBrush(Color.FromArgb(255, 128, 128, 000)),  new SolidColorBrush(Colors.Black) },  // Dim Yellow
            { new SolidColorBrush(Color.FromArgb(255, 128, 000, 128)),  new SolidColorBrush(Colors.Black) },  // Dim Magenta
            { new SolidColorBrush(Color.FromArgb(255, 000, 128, 128)),  new SolidColorBrush(Colors.Black) },  // Dim Cyan
            { new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)),  new SolidColorBrush(Colors.Black) },  // Dim White = Gray
            { new SolidColorBrush(Color.FromArgb(255, 000, 000, 000)),  new SolidColorBrush(Colors.White) },  // Black
            { new SolidColorBrush(Color.FromArgb(255, 008, 008, 008)),  new SolidColorBrush(Colors.White) },  // Dark Black Shade
            { new SolidColorBrush(Color.FromArgb(255, 016, 016, 016)),  new SolidColorBrush(Colors.White) },  // Medium Black Shade
            { new SolidColorBrush(Color.FromArgb(255, 032, 032, 032)),  new SolidColorBrush(Colors.White) },  // Light Black Shade
            { new SolidColorBrush(Color.FromArgb(255, 064, 064, 064)),  new SolidColorBrush(Colors.White) },  // Dark Gray
            { new SolidColorBrush(Color.FromArgb(255, 096, 096, 096)),  new SolidColorBrush(Colors.White) }   // Light Gray
        };

        /// <summary>
        /// Number of bar colors defined in arraySolidColorBrushBarColors.
        /// </summary>
        private int intBarColors;

        /// <summary>
        /// Array of strings containing bar color definition labels.
        /// </summary>
        private string[] stringArrayBarColorLabels;

        /// <summary>
        /// True if color bar labels are visible, false otherwise.
        /// </summary>
        private bool boolColorBarLabels = true;

        /// <summary>
        /// True if color bars are reversed, false otherwise.
        /// </summary>
        private bool boolColorBarsReverse = false;

        /// <summary>
        /// True if color bars are vertical, false otherwise.
        /// </summary>
        private bool boolColorBarsVertical = false;

        /// <summary>
        /// ToolTip string. "Double-tap to show this color".
        /// </summary>
        private const string stringToolTipThisColor = "Double-tap to show this color";

        /// <summary>
        /// ToolTip string. "Double-tap to go back to color bars".
        /// </summary>
        private const string stringToolTipColorBars = "Double-tap to go back to color bars";

        /// <summary>
        /// Enum of current label position. Options are center, right, off, left.
        /// </summary>
        private EnumPositionLabels enumPositionLabels = EnumPositionLabels.center;

        public ShowBars()
        {
            InitializeComponent();
        }

        /*** Public methods follow *********************************************************************************************/

        /// <summary>
        /// Method called from mainPage.ButLabels_Click(). Toggle position of color bar labels when User clicks on Labels button.
        /// </summary>
        public void ShowBarsLabels()
        {
            switch (enumPositionLabels)
            {
                case EnumPositionLabels.center:
                    enumPositionLabels = EnumPositionLabels.right;
                    ColorBarsLabelsPosition(enumPositionLabels);        // Change label position.
                    break;
                case EnumPositionLabels.right:
                    enumPositionLabels = EnumPositionLabels.off;
                    ColorBarsLabelsCollapse();                          // Labels were on, so turn them off.
                    break;
                case EnumPositionLabels.off:
                    enumPositionLabels = EnumPositionLabels.left;
                    ColorBarsLabelsVisible();                           // Labels were off, so turn them on.
                    ColorBarsLabelsPosition(enumPositionLabels);        // Change label position.
                    break;
                case EnumPositionLabels.left:
                    enumPositionLabels = EnumPositionLabels.center;
                    ColorBarsLabelsPosition(enumPositionLabels);        // Change label position.
                    break;
                default:    // Throw exception so error can be discovered and corrected.
                    throw new NotSupportedException($"ShowBars.ShowBarsLabels(): Match for enumPositionLabels={enumPositionLabels} not found in switch statement.");
            }
        }

        /// <summary>
        /// Method called from mainPage.ButReverse_Click(). Reverse order of color bars when User clicks Reverse button.
        /// </summary>
        public void ShowBarsReverse()
        {
            if (boolColorBarsReverse)
                ColorBarsNormal();
            else
                ColorBarsReverse();
        }

        /// <summary>
        /// Method called from mainPage.ButRotate_Click(). Rotate order of color bars when User clicks Rotate button.
        /// </summary>
        public void ShowBarsRotate()
        {
            if (boolColorBarsVertical)
                ColorBarsHorizontal();
            else
                ColorBarsVertical();
        }

        // Following method is public since also called from MainPage.PageGoBack().
        /// <summary>
        /// Collapse large rectangle and show previous color bar layout.
        /// </summary>
        public void LargeRectCollapse()
        {
            LargeRect.Visibility = Visibility.Collapsed;
            LargeRectText.Visibility = Visibility.Collapsed;
            if (boolColorBarsVertical)
            {
                foreach (UIElement uIElement in GridVR.Children)        // Make all items on GridVR visible.
                    uIElement.Visibility = Visibility.Visible;
            }
            else
            {
                foreach (UIElement uIElement in GridHR.Children)        // Make all items on GridHR visible.
                    uIElement.Visibility = Visibility.Visible;
                if (boolColorBarLabels)
                {
                    foreach (UIElement uIElement in GridHT.Children)    // Make all items on GridHT visible.
                        uIElement.Visibility = Visibility.Visible;
                }
            }
            mainPage.boolShowBarsLargeRect = false;
        }

        /*** Private methods follow ********************************************************************************************/

        /// <summary>
        /// Convert SolidColorBrush to a string to show User.
        /// </summary>
        /// <param name="solidColorBrush">SolidColorBrush to convert.</param>
        /// <param name="boolBasicString">Return basic string if true, otherwise return detailed string.</param>
        /// <returns></returns>
        private string SolidColorBrushToString(SolidColorBrush solidColorBrush, bool boolBasicString)
        {
            int intRed = solidColorBrush.Color.R;
            int intGreen = solidColorBrush.Color.G;
            int intBlue = solidColorBrush.Color.B;
            if (boolBasicString)
            {
                // This is horizontal rectangle default.
                return $"R={intRed:D3}, G={intGreen:D3}, B={intBlue:D3}";
            }
            else
            {
                // This is large rectangle default. Since have space, show Alpha and Hex values too. 
                int intAlpha = solidColorBrush.Color.A;
                return $"R={intRed:D3}, G={intGreen:D3}, B={intBlue:D3}, A={intAlpha}\nHex={solidColorBrush.Color}";
            }
        }

        /// <summary>
        /// Add ToolTips to horizontal, vertical, and large rectangles.
        /// </summary>
        private void AddToolTips()
        {
            Rectangle rectangle;
            foreach (UIElement uIElement in GridHR.Children)     // Set ToolTip for horizontal rectangles in GridHR.
            {
                rectangle = uIElement as Rectangle;
                ToolTipService.SetToolTip(rectangle, stringToolTipThisColor);
            }
            foreach (UIElement uIElement in GridVR.Children)     // Set ToolTip for vertical rectangles in GridVR.
            {
                rectangle = uIElement as Rectangle;
                ToolTipService.SetToolTip(rectangle, stringToolTipThisColor);
            }
            ToolTipService.SetToolTip(LargeRect, stringToolTipColorBars);   // Set ToolTip for large rectangle in GridLR.
        }

        /// <summary>
        /// Set normal colors.
        /// </summary>
        private void ColorBarsNormal()
        {
            int intCounter = 0;
            Rectangle rectangle;
            foreach (UIElement uIElement in GridHR.Children)     // Set horizontal rectangle colors in normal order.
            {
                rectangle = uIElement as Rectangle;
                rectangle.Fill = arraySolidColorBrushBarColors[intCounter, 0];
                intCounter++;
            }
            intCounter = 0;
            TextBlock textBlock;
            foreach (UIElement uIElement in GridHT.Children)     // Set text strings and text colors in normal order.
            {
                textBlock = uIElement as TextBlock;
                textBlock.Text = stringArrayBarColorLabels[intCounter];
                textBlock.Foreground = arraySolidColorBrushBarColors[intCounter, 1];
                intCounter++;
            }
            intCounter = 0;
            foreach (UIElement uIElement in GridVR.Children)     // Set vertical rectangle colors in normal order.
            {
                rectangle = uIElement as Rectangle;
                rectangle.Fill = arraySolidColorBrushBarColors[intCounter, 0];
                intCounter++;
            }
            boolColorBarsReverse = false;  // False since colors normal (not reversed).
        }

        /// <summary>
        /// Set reverse colors.
        /// </summary>
        private void ColorBarsReverse()
        {
            int intCounter = intBarColors - 1;
            Rectangle rectangle;
            TextBlock textBlock;
            foreach (UIElement uIElement in GridHR.Children)     // Set horizontal rectangle colors in reverse order.
            {
                rectangle = uIElement as Rectangle;
                rectangle.Fill = arraySolidColorBrushBarColors[intCounter, 0];
                intCounter--;
            }
            intCounter = intBarColors - 1;
            foreach (UIElement uIElement in GridHT.Children)     // Set text strings and text colors in reverse order.
            {
                textBlock = uIElement as TextBlock;
                textBlock.Text = stringArrayBarColorLabels[intCounter];
                textBlock.Foreground = arraySolidColorBrushBarColors[intCounter, 1];
                intCounter--;
            }
            intCounter = intBarColors - 1;
            foreach (UIElement uIElement in GridVR.Children)     // Set vertical rectangle colors in reverse order.
            {
                rectangle = uIElement as Rectangle;
                rectangle.Fill = arraySolidColorBrushBarColors[intCounter, 0];
                intCounter--;
            }
            boolColorBarsReverse = true;   // True since colors reversed.
        }

        /// <summary>
        /// Collapse vertical bars and make horizontal bars visible. Also make labels visible if boolLabelsVisible is true.
        /// </summary>
        private void ColorBarsHorizontal()
        {
            foreach (UIElement uIElement in GridVR.Children)        // Collapse all items in GridVR.
                uIElement.Visibility = Visibility.Collapsed;
            foreach (UIElement uIElement in GridHR.Children)        // Make all items in GridHR visible.
                uIElement.Visibility = Visibility.Visible;
            if (boolColorBarLabels)                                 // If true, make labels visible.
            {
                foreach (UIElement uIElement in GridHT.Children)    // Make all items on GridHT visible.
                    uIElement.Visibility = Visibility.Visible;
            }
            boolColorBarsVertical = false;                          // False since bars are now horizontal.
        }

        /// <summary>
        /// Collapse horizontal bars and labels and make vertical bars visible.
        /// </summary>
        private void ColorBarsVertical()
        {
            foreach (UIElement uIElement in GridHR.Children)    // Collapse all items on GridHR.
                uIElement.Visibility = Visibility.Collapsed;
            foreach (UIElement uIElement in GridHT.Children)    // Collapse all items on GridHT.
                uIElement.Visibility = Visibility.Collapsed;
            foreach (UIElement uIElement in GridVR.Children)    // Make all items on GridVR visible.
                uIElement.Visibility = Visibility.Visible;
            boolColorBarsVertical = true;                       // True since bars are now vertical.
        }

        /// <summary>
        /// Make all TextBlocks in GridHT and GridLT visible.
        /// </summary>
        private void ColorBarsLabelsVisible()
        {
            if (!boolColorBarsVertical && !mainPage.boolShowBarsLargeRect)  // Only make TextBlocks visible if currently in grid that should show them.
            {
                foreach (UIElement uIElement in GridHT.Children)
                    uIElement.Visibility = Visibility.Visible;      // Make all TextBlocks in GridHT visible.
            }
            else if (mainPage.boolShowBarsLargeRect)
                LargeRectText.Visibility = Visibility.Visible;      // Make TextBlock in GridLT visible.
            boolColorBarLabels = true;                              // True since labels now visible.
        }

        /// <summary>
        /// Collapse all TextBlocks in GridHT and GridLT.
        /// </summary>
        private void ColorBarsLabelsCollapse()
        {
            foreach (UIElement uIElement in GridHT.Children)    // Collapse all TextBlocks in GridHT.
                uIElement.Visibility = Visibility.Collapsed;
            LargeRectText.Visibility = Visibility.Collapsed;    // Collapse TextBlock in GridLT.
            boolColorBarLabels = false;                         // False since labels now collapsed.
        }

        /// <summary>
        /// Change alignment of all TextBlocks in GridHT and GridLT.
        /// </summary>
        /// <param name="enumPositionLabelsNew">New label position.</param>
        private void ColorBarsLabelsPosition(EnumPositionLabels enumPositionLabelsNew)
        {
            HorizontalAlignment horizontalAlignment;
            switch (enumPositionLabelsNew)
            {
                case EnumPositionLabels.center:
                    horizontalAlignment = HorizontalAlignment.Center;
                    break;
                case EnumPositionLabels.right:
                    horizontalAlignment = HorizontalAlignment.Right;
                    break;
                case EnumPositionLabels.left:
                    horizontalAlignment = HorizontalAlignment.Left;
                    break;
                case EnumPositionLabels.off:
                    return;     // Return if labels are off.
                default:        // Throw exception so error can be discovered and corrected.
                    throw new NotSupportedException($"ShowBars.ColorBarsLabelsPosition(): Match for enumPositionLabelsNew={enumPositionLabelsNew} not found in switch statement.");
            }
            TextBlock textBlock;
            // Change alignment of all TextBlocks in GridHT.
            foreach (UIElement uIElement in GridHT.Children)
            {
                textBlock = uIElement as TextBlock;
                textBlock.HorizontalAlignment = horizontalAlignment;
            }
            // Change alignment of TextBlock in GridLT.
            LargeRectText.HorizontalAlignment = horizontalAlignment;
        }

        /// <summary>
        /// Collpase color bar layout and show show large rectangle.
        /// </summary>
        private void LargeRectVisible()
        {
            foreach (UIElement uIElement in GridHR.Children)     // Collapse all items on GridHR.
                uIElement.Visibility = Visibility.Collapsed;
            foreach (UIElement uIElement in GridHT.Children)     // Collapse all items on GridHT.
                uIElement.Visibility = Visibility.Collapsed;
            foreach (UIElement uIElement in GridVR.Children)     // Collapse all items on GridVR.
                uIElement.Visibility = Visibility.Collapsed;
            LargeRect.Visibility = Visibility.Visible;
            LargeRectText.Visibility = Visibility.Visible;
            mainPage.boolShowBarsLargeRect = true;
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
            mainPage.showBars = this;       // Set ShowBars pointer defined in MainPage to point to this page.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);
            LibMPC.ButtonVisibility(mainPage.mainPageButLabels, true);
            LibMPC.ButtonVisibility(mainPage.mainPageButReverse, true);
            LibMPC.ButtonVisibility(mainPage.mainPageButRotate, true);
            mainPage.mainPageTblkAppTitle.Visibility = Visibility.Collapsed;
            intBarColors = arraySolidColorBrushBarColors.GetLength(0);  // Get number of colors defined in arraySolidColorBrushBarColors.
            // Create array of strings to use as labels in horizontal color bars.
            stringArrayBarColorLabels = new string[intBarColors];
            for (int i = 0; i < intBarColors; i++)
                stringArrayBarColorLabels[i] = SolidColorBrushToString(arraySolidColorBrushBarColors[i, 0], true);  // Get first element from 2 dimension array.
            ColorBarsNormal();
            AddToolTips();
        }

        /// <summary>
        /// Invoked when User double-taps on a color bar or label in color bar.
        /// Collapse color bar layout and show large rectangle filled with double-tapped color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Color_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            SolidColorBrush rectangleFillColor = new SolidColorBrush();
            object objectDoubleTapped = e.OriginalSource;     // This gets object that was double-tapped. Should be Rectangle or TextBlock.
            if (objectDoubleTapped is Rectangle rectangleDoubleTapped)
            {
                rectangleFillColor = (SolidColorBrush)rectangleDoubleTapped.Fill;
            }
            else if (objectDoubleTapped is TextBlock textBlockDoubleTapped)     // User double-tapped color bar label instead of rectangle behind it.
            {
                int intCounter = 0;
                TextBlock textBlock;
                // Search GridHT for matching string. Need index so can look up corresponding rectangle fill color.
                foreach (UIElement uIElement in GridHT.Children)
                {
                    textBlock = uIElement as TextBlock;
                    if (textBlockDoubleTapped.Text.Equals(textBlock.Text))  // Found match.
                        break;
                    intCounter++;
                }
                // Debug.WriteLine($"ShowBars.Color_DoubleTapped(): textBlockDoubleTapped.Text={textBlockDoubleTapped.Text}, found match at intCounter={intCounter}");
                int intIndex = 0;
                Rectangle rectangle;
                // Search GridHR for matching index and then get rectangle fill color.
                foreach (UIElement uIElement in GridHR.Children)     // Set horizontal rectangle colors in normal order.
                {
                    if (intIndex == intCounter)     // Match found.
                    {
                        rectangle = uIElement as Rectangle;
                        rectangleFillColor = (SolidColorBrush)rectangle.Fill;
                        break;
                    }
                    intIndex++;
                }
            }
            SolidColorBrush rectangleTextForeground = new SolidColorBrush();
            // Search array for matching fill color and then get corresponding text color.
            for (int i = 0; i < intBarColors; i++)
            {
                if (rectangleFillColor == arraySolidColorBrushBarColors[i, 0])  // Match found
                {
                    rectangleTextForeground = arraySolidColorBrushBarColors[i, 1];   // Use index to get text foreground color.
                    break;
                }
            }
            LargeRect.Fill = rectangleFillColor;
            // Show detailed SolidColorBrush string since large rectangle has space to show it.
            LargeRectText.Text = SolidColorBrushToString(rectangleFillColor, false);
            LargeRectText.Foreground = rectangleTextForeground;
            LargeRectVisible();
        }

        /// <summary>
        /// Invoked when User double-taps on a large rectangle or label in large rectangle.
        /// Collapse large rectangle and show previous color bar layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            LargeRectCollapse();
        }

    }
}
