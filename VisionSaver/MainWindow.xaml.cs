using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace VisionSaver
{
    public partial class MainWindow : Window
    {
        NotifyIcon ni = new NotifyIcon();
        int rest = 5, work = 30;
        DateTime restTime;
        DateTime pauseTime;
        DispatcherTimer timer;
        bool showedNotification = false;

        public MainWindow()
        {
            InitializeComponent();
            SetupNotifyIcon();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Tick;
        }

        private void SetupNotifyIcon()
        {
            ni.Icon = Properties.Resources.icon;
            ni.Visible = true;

            ni.ContextMenu = new ContextMenu();
            ni.ContextMenu.MenuItems.Add("Add 5 min", Add5Min);
            ni.ContextMenu.MenuItems.Add("Pause", Pause);
            ni.ContextMenu.MenuItems.Add("Exit", AppExit);

            RestInterval.Text = rest.ToString();
            WorkInterval.Text = work.ToString();
            restTime = DateTime.Now.AddMinutes(work);

            ni.DoubleClick += (sndr, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
            };
            ni.BalloonTipClicked += (sndr, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
            };
        }

        private void AppExit(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Pause(object sender, EventArgs e)
        {
            if (timer.IsEnabled)
            {
                pauseTime = DateTime.Now;
                timer.Stop();
                ni.ContextMenu.MenuItems[1].Text = "Start";
            }
            else
            {
                var diff = DateTime.Now - pauseTime;
                restTime = restTime.Add(diff);
                timer.Start();
                ni.ContextMenu.MenuItems[1].Text = "Stop";
            }
        }

        private void Tick(object sender, EventArgs e)
        {
            ni.Text = (restTime - DateTime.Now).Minutes + " min " + (restTime - DateTime.Now).Seconds + " sec";
            if (restTime.Subtract(DateTime.Now).TotalSeconds < 1)
            {
                restTime = DateTime.Now.AddMinutes(rest + work);
                Break();
            }
            else if (restTime.Subtract(DateTime.Now).TotalSeconds < 31 && !showedNotification)
            {
                ni.ShowBalloonTip(5000, "Напоминание", "Перерыв через 30 секунд", ToolTipIcon.Info);
                showedNotification = true;
            }
        }

        private void Break()
        {
            showedNotification = false;
            restTime = DateTime.Now.AddMinutes(rest);
            new ScreenSaver(5).ShowDialog();
        }

        private void Add5Min(object sender, EventArgs e)
        {
            restTime = restTime.AddMinutes(5);
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ni.Visible = false;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Minimized:
                    Hide();
                    break;
            }
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
                return;
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                return;
            else if (e.Key == Key.Tab || e.Key == Key.Return || e.Key == Key.Back || e.Key == Key.System)
                return;
            else
                e.Handled = true; 
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            RestInterval.Text = "5";
            WorkInterval.Text = "30";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            rest = Convert.ToInt32(RestInterval.Text);
            work = Convert.ToInt32(WorkInterval.Text);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Save_Click(null, null);
            restTime = DateTime.Now.AddMinutes(work);
            timer.Start();
            Hide();
        }
    }
}