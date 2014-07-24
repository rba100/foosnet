using System.ComponentModel;
using System.IO;
using System.Net;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;
using FoosNet.Views;

namespace FoosNet
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;

    public partial class MainNotifyWindow : Window
    {
        private readonly Controls.ExtendedNotifyIcon m_ExtendedNotifyIcon; // global class scope for the icon as it needs to exist foer the lifetime of the window
        private readonly Storyboard m_GridFadeInStoryBoard;
        private readonly Storyboard m_GridFadeOutStoryBoard;
        private Timer m_ImageUpdateTimer;
        //private Point startPoint;

        /// <summary>
        /// Sets up the popup window and instantiates the notify icon
        /// </summary>
        public MainNotifyWindow()
        {
            // Create a manager (ExtendedNotifyIcon) for handling interaction with the notification icon and wire up events. 
            m_ExtendedNotifyIcon = new Controls.ExtendedNotifyIcon();
            m_ExtendedNotifyIcon.MouseLeave += extendedNotifyIcon_OnHideWindow;
            m_ExtendedNotifyIcon.MouseMove += extendedNotifyIcon_OnShowWindow;
            m_ExtendedNotifyIcon.targetNotifyIcon.ContextMenu = GetSystrayContextMenu();
            SetNotifyIcon("Red");

            InitializeComponent();

            // Set the startup position and the startup state to "not visible"
            SetWindowToBottomRightOfScreen();
            this.Opacity = 0;
            uiGridMain.Opacity = 0;

            // Locate these storyboards and "cache" them - we only ever want to find these once for performance reasons
            m_GridFadeOutStoryBoard = (Storyboard)this.TryFindResource("gridFadeOutStoryBoard");
            m_GridFadeOutStoryBoard.Completed += gridFadeOutStoryBoard_Completed;
            m_GridFadeInStoryBoard = (Storyboard)TryFindResource("gridFadeInStoryBoard");
            m_GridFadeInStoryBoard.Completed += gridFadeInStoryBoard_Completed;

            var vm = this.DataContext as NotifyWindowViewModel;
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private System.Windows.Forms.ContextMenu GetSystrayContextMenu()
        {
            var menu = new System.Windows.Forms.ContextMenu();

            var exit = new System.Windows.Forms.MenuItem("Exit", (sender, args) =>
            {
                m_ExtendedNotifyIcon.Dispose(); // So that the icon disappears and it looks like the app closed quickly.
                Close(); // Takes about two seconds to close.
            });

            var settings = new System.Windows.Forms.MenuItem("Settings", (sender, args) =>
            {
                var vm = (DataContext as NotifyWindowViewModel);
                if (vm != null) vm.IsShowSettings = true;
                extendedNotifyIcon_OnShowWindow();
            });

            menu.MenuItems.Add(settings);
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
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,/Images/" + iconPrefix + "Orb.ico")).Stream;
            m_ExtendedNotifyIcon.targetNotifyIcon.Icon = new System.Drawing.Icon(iconStream);
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
            m_GridFadeOutStoryBoard.Stop();
            this.Opacity = 1; // Show the window (backing)
            this.Topmost = true; // Very rarely, the window seems to get "buried" behind others, this seems to resolve the problem
            if (uiGridMain.Opacity > 0 && uiGridMain.Opacity < 1) // If its animating, just set it directly to visible (avoids flicker and keeps the UX slick)
            {
                uiGridMain.Opacity = 1;
            }
            else if (uiGridMain.Opacity == 0)
            {
                m_GridFadeInStoryBoard.Begin();  // If it is in a fully hidden state, begin the animation to show the window
            }

        }

        /// <summary>
        /// When the notification manager requests the popup to be hidden through this event, perform the below actions
        /// </summary>
        void extendedNotifyIcon_OnHideWindow()
        {
            if (PinButton.IsChecked == true) return; // Dont hide the window if its pinned open
            if (PlayerListContextMenu.IsOpen) return;
            if (SettingsGrid.Visibility == Visibility.Visible) return;

            // if (FoosTablePopup.IsOpen) return;
            if (Mouse.LeftButton.HasFlag(MouseButtonState.Pressed)) return; // Drag and drop hack

            m_GridFadeInStoryBoard.Stop(); // Stop the fade in storyboard if running.

            // Only start fading out if fully faded in, otherwise you get a flicker effect in the UX because the animation resets the opacity
            if (uiGridMain.Opacity == 1 && this.Opacity == 1)
                m_GridFadeOutStoryBoard.Begin();
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
        private void uiWindowMainNotification_MouseEnter(object sender, MouseEventArgs e)
        {
            // Cancel the mouse leave event from firing, stop the fade out storyboard from running and enusre the grid is fully visible
            m_ExtendedNotifyIcon.StopMouseLeaveEventFromFiring();
            m_GridFadeOutStoryBoard.Stop();
            uiGridMain.Opacity = 1;
        }

        /// <summary>
        /// If the mouse leaves the popup, start the process to close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiWindowMainNotification_MouseLeave(object sender, MouseEventArgs e)
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
            StopTableImageAutoUpdate();
        }

        /// <summary>
        /// Once the grid fades in, set the backing window to "visible"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gridFadeInStoryBoard_Completed(object sender, EventArgs e)
        {
            this.Opacity = 1;
            
            if (FoosTableImageBorder.Visibility == Visibility.Visible)
            {
                StartTableImageAutoUpdate();
            }
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
            m_ExtendedNotifyIcon.Dispose();
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
                ShowTableImage();
            }
            else
            {
                HideTableImage();
            }
        }

        private void ShowTableImage()
        {
            FoosTableImageBorder.Visibility = Visibility.Visible;
            UpdateFoosTableImageSource();
            StartTableImageAutoUpdate();
        }

        private void HideTableImage()
        {
            FoosTableImageBorder.Visibility = Visibility.Collapsed;
            StopTableImageAutoUpdate();
        }
        
        private void StartTableImageAutoUpdate()
        {
            if (m_ImageUpdateTimer == null) { 
                m_ImageUpdateTimer = new Timer {Interval = 5000};

                m_ImageUpdateTimer.Elapsed +=
                    (s, e) => Dispatcher.Invoke(UpdateFoosTableImageSource);
            }

            m_ImageUpdateTimer.Start();
        }

        private void StopTableImageAutoUpdate()
        {
            if (m_ImageUpdateTimer != null) { 
                m_ImageUpdateTimer.Stop();
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
