using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace MainApp.Helpers
{
    sealed class IconHelper : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private string linkIcon = Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");

        public IconHelper()
        {
            _notifyIcon = new NotifyIcon
            {
                BalloonTipText = "应用程序已最小化到托盘。",
                Text = "MainApp",
                Icon = GetIconFromFile(),
                Visible = true,
            };
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("重启", null, (s, e) =>
            {
                string? exePath = Environment.ProcessPath;
                if (!string.IsNullOrWhiteSpace(exePath)) Process.Start(exePath);
                Application.Current.Shutdown();
            });
            contextMenu.Items.Add("退出", null, (s, e) => Application.Current.Shutdown());
            _notifyIcon.ContextMenuStrip = contextMenu;
        }


        public Icon? GetIconFromFile()
        {
            try
            {

                if (!File.Exists(linkIcon))
                {
                    System.Windows.Forms.MessageBox.Show($"图标文件不存在: {linkIcon}");
                    return null;
                }

                // 直接从文件路径读取图标
                using (var stream = new FileStream(linkIcon, FileMode.Open, FileAccess.Read))
                {
                    return new Icon(stream);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"加载图标时发生错误: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }
    }
}
