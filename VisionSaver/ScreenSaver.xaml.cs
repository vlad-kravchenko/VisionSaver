using System;
using System.Windows;
using System.Windows.Threading;

namespace VisionSaver
{
    public partial class ScreenSaver : Window
    {
        DispatcherTimer timer;
        int restInterval = 300;
        int ticks = 0;

        public ScreenSaver(int restInterval)
        {
            InitializeComponent();
            this.restInterval = restInterval;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Tick;
            timer.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            ticks++;
            if (ticks >= restInterval) Close();
        }
    }
}