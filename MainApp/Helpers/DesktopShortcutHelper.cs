using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Helpers
{
    internal sealed class DesktopShortcutHelper
    {
        string linkIcon = Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");

        public void CreateDesktopShortcut(string shortcutName)
        {
            string? targetPath = Environment.ProcessPath;
            // 获取桌面路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 设置快捷方式路径
            string shortcutPath = System.IO.Path.Combine(desktopPath, shortcutName + ".lnk");

            // 创建快捷方式
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            // 设置快捷方式属性
            shortcut.TargetPath = targetPath; // 应用程序的路径
            shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(targetPath); // 设置工作目录
            shortcut.Description = "My Application Shortcut"; // 快捷方式的描述
            shortcut.IconLocation = linkIcon; // 设置图标
            shortcut.Save(); // 保存快捷方式
        }
    }
}
