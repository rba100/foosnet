//-------------------------------------------------------------------------------------
// Author:   Murray Foxcroft - April 2009
// Comments: code behind for the main WPF popup window 
//-------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using FoosNet.Controls;
using FoosNet.Network;
using FoosNet.Views;

namespace FoosNet
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;

    public partial class MainNotifyWindow : Window
    {
        private Controls.ExtendedNotifyIcon extendedNotifyIcon; // global class scope for the icon as it needs to exist foer the lifetime of the window
        private Storyboard gridFadeInStoryBoard;
        private Storyboard gridFadeOutStoryBoard;
        //private Point startPoint;

        /// <summary>
        /// Sets up the popup window and instantiates the notify icon
        /// </summary>
        public MainNotifyWindow()
        {
            // Create a manager (ExtendedNotifyIcon) for handling interaction with the notification icon and wire up events. 
            extendedNotifyIcon = new Controls.ExtendedNotifyIcon();
            extendedNotifyIcon.MouseLeave += extendedNotifyIcon_OnHideWindow;
            extendedNotifyIcon.MouseMove += extendedNotifyIcon_OnShowWindow;
            extendedNotifyIcon.targetNotifyIcon.ContextMenu = GetSystrayContextMenu();
            SetNotifyIcon("Red");

            InitializeComponent();

            // Set the startup position and the startup state to "not visible"
            SetWindowToBottomRightOfScreen();
            this.Opacity = 0;
            uiGridMain.Opacity = 0;

            // Locate these storyboards and "cache" them - we only ever want to find these once for performance reasons
            gridFadeOutStoryBoard = (Storyboard)this.TryFindResource("gridFadeOutStoryBoard");
            gridFadeOutStoryBoard.Completed += gridFadeOutStoryBoard_Completed;
            gridFadeInStoryBoard = (Storyboard)TryFindResource("gridFadeInStoryBoard");
            gridFadeInStoryBoard.Completed += gridFadeInStoryBoard_Completed;

            var vm = this.DataContext as NotifyWindowViewModel;
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private System.Windows.Forms.ContextMenu GetSystrayContextMenu()
        {
            var menu = new System.Windows.Forms.ContextMenu();
            var exit = new System.Windows.Forms.MenuItem("Exit", (sender, args) =>
            {
                extendedNotifyIcon.Dispose(); // So that the icon disappears and it looks like the app closed quickly.
                Close(); // Takes about two seconds to close.
            });
            menu.MenuItems.Add(exit);
            return menu;
        }

        private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as NotifyWindowViewModel;
            if (e.PropertyName == "IsTableFree")
            {
                SetNotifyIcon(vm.IsTableFree ? "Green" : "Red");
            }
        }

        /// <summary>
        /// Pulls an icon from the packed resource and applies it to the NotifyIcon control
        /// </summary>
        /// <param name="iconPrefix"></param>
        private void SetNotifyIcon(string iconPrefix)
        {
            System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,/Images/" + iconPrefix + "Orb.ico")).Stream;
            extendedNotifyIcon.targetNotifyIcon.Icon = new System.Drawing.Icon(iconStream);
        }

        /// <summary>
        /// Does what it says on the tin - ensures the popup window appears at the bottom right of the screen, just above the task bar
        /// </summary>
        private void SetWindowToBottomRightOfScreen()
        {
            Left = SystemParameters.WorkArea.Width - Width - 10;
            Top = SystemParameters.WorkArea.Height - Height;
        }

        /// <summary>
        /// When the notification manager requests the popup to be displayed through this event, perform the below actions
        /// </summary>
        void extendedNotifyIcon_OnShowWindow()
        {
            gridFadeOutStoryBoard.Stop();
            this.Opacity = 1; // Show the window (backing)
            this.Topmost = true; // Very rarely, the window seems to get "buried" behind others, this seems to resolve the problem
            if (uiGridMain.Opacity > 0 && uiGridMain.Opacity < 1) // If its animating, just set it directly to visible (avoids flicker and keeps the UX slick)
            {
                uiGridMain.Opacity = 1;
            }
            else if (uiGridMain.Opacity == 0)
            {
                gridFadeInStoryBoard.Begin();  // If it is in a fully hidden state, begin the animation to show the window
            }
        }

        /// <summary>
        /// When the notification manager requests the popup to be hidden through this event, perform the below actions
        /// </summary>
        void extendedNotifyIcon_OnHideWindow()
        {
            if (PinButton.IsChecked == true) return; // Dont hide the window if its pinned open
            if (PlayerListContextMenu.IsOpen) return;
            // if (FoosTablePopup.IsOpen) return;
            if (Mouse.LeftButton.HasFlag(MouseButtonState.Pressed)) return; // Drag and drop hack

            gridFadeInStoryBoard.Stop(); // Stop the fade in storyboard if running.

            // Only start fading out if fully faded in, otherwise you get a flicker effect in the UX because the animation resets the opacity
            if (uiGridMain.Opacity == 1 && this.Opacity == 1)
                gridFadeOutStoryBoard.Begin();
            else // Just hide the window and grid
            {
                uiGridMain.Opacity = 0;
                this.Opacity = 0;
            }
        }

        /// <summary>
        /// When the mouse enters the popup window's bounds, cancel any pending closing actions and immediately show the popup. 
        /// This is primarily to handle the case where the mouse termporarily leaves the popup window and returns again - 
        /// a UX / usability enhancement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiWindowMainNotification_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Cancel the mouse leave event from firing, stop the fade out storyboard from running and enusre the grid is fully visible
            extendedNotifyIcon.StopMouseLeaveEventFromFiring();
            gridFadeOutStoryBoard.Stop();
            uiGridMain.Opacity = 1;
        }

        /// <summary>
        /// If the mouse leaves the popup, start the process to close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiWindowMainNotification_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            extendedNotifyIcon_OnHideWindow();
        }

        /// <summary>
        /// Once the grid fades out, set the backing window to "not visible"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gridFadeOutStoryBoard_Completed(object sender, EventArgs e)
        {
            this.Opacity = 0;
        }

        /// <summary>
        /// Once the grid fades out, set the backing window to "visible"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gridFadeInStoryBoard_Completed(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        /// <summary>
        /// When the pin button is pressed/unpressed, switch the icon appropriately. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            if (PinButton.IsChecked == true)
                PinImage.Source = new BitmapImage(new Uri("pack://application:,,/Images/Pinned.png"));
            else
                PinImage.Source = new BitmapImage(new Uri("pack://application:,,/Images/Un-Pinned.png"));
        }

        /// <summary>
        /// Shut down the popup window and dispose the notify icon (otherwise it hangs around in the task bar until you mouse over) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            extendedNotifyIcon.Dispose();
            this.Close();
        }

        private void TitleLabel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var aboutView = new About();
            aboutView.ShowDialog();
        }

        private void TableStatusImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleTableImageVisibility();
        }

        private void ToggleTableImageVisibility()
        {
            if (FoosTableImageBorder.Visibility != Visibility.Visible)
            {
                FoosTableImageBorder.Visibility = Visibility.Visible;
                UpdateFoosTableImageSource();
            }
            else
            {
                FoosTableImageBorder.Visibility = Visibility.Collapsed;
            }
        }

        private async void UpdateFoosTableImageSource()
        {
            var original = FoosTableImage.Source as BitmapImage;
            if (original != null) original.StreamSource.Dispose();

            FoosTableImage.Visibility = Visibility.Collapsed;
            var url = new Uri(@"http://10.120.115.224/snapshot.cgi?user=viewer&amp;pwd=&amp;");
            var webRequest = WebRequest.CreateDefault(url);
            webRequest.ContentType = "image/jpeg";
            var response = await webRequest.GetResponseAsync();
            var dataStream = response.GetResponseStream();
            var memStream = new MemoryStream();
            await dataStream.CopyToAsync(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = memStream;
            image.EndInit();
            image.Freeze();

            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                FoosTableImage.Source = image;
                FoosTableImage.Visibility = Visibility.Visible;
            }));
        }

        private void TestButtonClick_OpenPlayersJoined(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as NotifyWindowViewModel;
            vm.IsTableFree = true;
        }

        private void FoosTableImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateFoosTableImageSource();
        }
    }
}