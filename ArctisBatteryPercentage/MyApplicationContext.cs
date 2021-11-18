using ArctisBattery;
using System.Timers;

namespace ArctisBatteryPercentage
{
    class MyApplicationContext : ApplicationContext
    {
        private NotifyIcon TrayIcon;
        private DateTime _startTime;
        private System.Timers.Timer _timer;
        private bool _balloonShown = false;

        public MyApplicationContext()
        {
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();
            TrayIcon.Visible = true;
        }

        private void InitializeComponent()
        {
            TrayIcon = new NotifyIcon
            {
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = "Battery Level Low",
                BalloonTipText = "Battery Level is at 20%. Recharge Soon.",
                Icon = CreateTextIcon(-1)
            };


            var TrayIconConextMenu = new ContextMenuStrip();
            var CloseMenuItem = new ToolStripMenuItem();
            //TrayIconConextMenu.SuspendLayout();

            TrayIconConextMenu.Items.AddRange(new ToolStripMenuItem[] { CloseMenuItem });
            TrayIconConextMenu.Name = "TrayIconContextMenu";
            TrayIconConextMenu.Size = new Size(153, 70);

            CloseMenuItem.Name = "CloseMenuItem";
            CloseMenuItem.Size = new Size(152, 22);
            CloseMenuItem.Text = "Close";
            CloseMenuItem.Click += new EventHandler(CloseMenuItem_Click);

            TrayIcon.ContextMenuStrip = TrayIconConextMenu;

            var arctis = new Arctis();
            var batteryLevel = arctis.CheckBattery();
            UpdateIcon(batteryLevel);

            _startTime = DateTime.Now;
            _timer = new System.Timers.Timer(1000 * 10);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        private void TrayIcon_MouseDown(object? sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Timer_Elapsed(object source, ElapsedEventArgs e)
        {

            var arctis = new Arctis();
            var batteryLevel = arctis.CheckBattery();
            UpdateIcon(batteryLevel);
        }

        private void UpdateIcon(int batteryLevel)
        {
            TrayIcon.Icon = CreateTextIcon(batteryLevel);

            if (batteryLevel == 0)
            {
                TrayIcon.Text = "Device not powered on.";
            }
            else
            {
                TrayIcon.Text = "Device powered on.";
            }

            if (!_balloonShown && batteryLevel != 0 && batteryLevel <= 20)
            {
                TrayIcon.ShowBalloonTip(1000);
                _balloonShown = true;
            }
        }

        private void CloseMenuItem_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnApplicationExit(object? sender, EventArgs e)
        {
            TrayIcon.Visible = false;
            _timer.Stop();
        }

        public Icon CreateTextIcon(int value)
        {
            Color color;
            if (value <= 0)
            {
                color = Color.White;
            }
            else
            {
                var red = (value > 50 ? 1 - 2 * (value - 50) / 100.0 : 1.0) * 255;
                var green = (value > 50 ? 1.0 : 2 * value / 100.0) * 255;
                var blue = 0.0;
                color = Color.FromArgb((int)red, (int)green, (int)blue);
            }

            Font fontToUse = new Font("Microsoft Sans Serif", value == 100 ? 10 : 16, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(color);
            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bitmapText);

            IntPtr hIcon;

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(value <= 0 ? "?" : value.ToString(), fontToUse, brushToUse, -4, -2);
            hIcon = bitmapText.GetHicon();
            return Icon.FromHandle(hIcon);
        }
    }
}
