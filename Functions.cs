using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Funcitons
{
    public static class NormalFunc
    {
        #region Functions

        #region DLL引用
        //DLL引用==================================================
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        //------------------------------------------------------------------------------------------------
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint Msg,
        UIntPtr wParam,
        string lParam,
        uint fuFlags,
        uint uTimeout,
        out UIntPtr lpdwResult);
        private const uint WM_SETTINGCHANGE = 0x001A;
        private const uint SMTO_ABORTIFHUNG = 0x0002;

        //------------------------------------------------------------------------------------------------
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLink { }
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxArgs);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("0000010b-0000-0000-C000-000000000046")]
        private interface IPersistFile
        {
            void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();
            void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, int dwMode);
            void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);
            void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
            void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
        }

        //鼠标键盘操作------------------------------------------------------------------------------------------------
        #region 鼠标键盘操作
        #region Windows API 导入和结构定义

        // 导入Windows API函数
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        // 定义POINT结构体
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        // 输入结构体
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion data;
        }

        // 输入联合体
        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
        }

        // 鼠标输入结构
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // 键盘输入结构
        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // 输入类型常量
        private const uint INPUT_MOUSE = 0;
        private const uint INPUT_KEYBOARD = 1;

        // 鼠标事件常量
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;
        private const uint MOUSEEVENTF_HWHEEL = 0x1000;
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int WHEEL_DELTA = 120;

        // 键盘事件常量
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        // 特殊键常量
        public const int VK_SHIFT = 0x10;
        public const int VK_CONTROL = 0x11;
        public const int VK_MENU = 0x12; // Alt键
        public const int VK_LWIN = 0x5B;
        public const int VK_RWIN = 0x5C;

        #endregion

        #region 鼠标操作

        // 屏幕尺寸（用于绝对坐标计算）
        private static Size _screenSize = Screen.PrimaryScreen.Bounds.Size;

        // 发送鼠标输入
        private static void SendMouseInput(uint flags, int data = 0, int dx = 0, int dy = 0)
        {
            INPUT input = new INPUT
            {
                type = INPUT_MOUSE,
                data = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = dx,
                        dy = dy,
                        mouseData = (uint)data,
                        dwFlags = flags,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// 设置鼠标绝对位置（屏幕坐标）
        /// </summary>
        public static void m_SetPositionXY(int x, int y) => SetCursorPos(x, y);

        /// <summary>
        /// 设置鼠标绝对位置（使用Point结构）
        /// </summary>
        public static void m_SetPositionPoint(Point position) => SetCursorPos(position.X, position.Y);

        /// <summary>
        /// 获取当前鼠标位置
        /// </summary>
        public static Point m_GetPosition()
        {
            GetCursorPos(out POINT point);
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// 移动鼠标相对当前位置的偏移量
        /// </summary>
        public static void MoveMouseRelative(int dx, int dy) => SendMouseInput(MOUSEEVENTF_MOVE, 0, dx, dy);

        /// <summary>
        /// 移动到屏幕上的绝对位置（使用SendInput实现）
        /// </summary>
        public static void MoveMouseAbsolute(int x, int y)
        {
            // 将坐标转换为绝对坐标系统（0-65535）
            int absoluteX = (int)((x * 65535) / _screenSize.Width);
            int absoluteY = (int)((y * 65535) / _screenSize.Height);

            SendMouseInput(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, 0, absoluteX, absoluteY);
        }

        // 左键操作
        public static void m_LeftClick() => MouseClick(MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP);
        public static void m_LeftDown() => SendMouseInput(MOUSEEVENTF_LEFTDOWN);
        public static void m_LeftUp() => SendMouseInput(MOUSEEVENTF_LEFTUP);

        // 右键操作
        public static void m_RightClick() => MouseClick(MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_RIGHTUP);
        public static void m_RightDown() => SendMouseInput(MOUSEEVENTF_RIGHTDOWN);
        public static void m_RightUp() => SendMouseInput(MOUSEEVENTF_RIGHTUP);

        // 中键操作
        public static void m_MiddleClick() => MouseClick(MOUSEEVENTF_MIDDLEDOWN, MOUSEEVENTF_MIDDLEUP);
        public static void m_MiddleDown() => SendMouseInput(MOUSEEVENTF_MIDDLEDOWN);
        public static void m_MiddleUp() => SendMouseInput(MOUSEEVENTF_MIDDLEUP);

        // 滚轮操作
        public static void m_WheelUp(int scrollCount = 1) => SendMouseInput(MOUSEEVENTF_WHEEL, WHEEL_DELTA * scrollCount);
        public static void m_WheelDown(int scrollCount = 1) => SendMouseInput(MOUSEEVENTF_WHEEL, -WHEEL_DELTA * scrollCount);

        // 通用点击方法
        private static void MouseClick(uint downFlag, uint upFlag)
        {
            SendMouseInput(downFlag);
            Thread.Sleep(50); // 添加短暂延迟模拟真实点击
            SendMouseInput(upFlag);
        }

        #endregion

        #region 键盘操作

        /// <summary>
        /// 发送键盘输入事件
        /// </summary>
        private static void SendKeyboardInput(ushort keyCode, uint flags)
        {
            INPUT input = new INPUT
            {
                type = INPUT_KEYBOARD,
                data = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = keyCode,
                        wScan = 0,
                        dwFlags = flags,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// 按下键盘按键
        /// </summary>
        public static void kbd_KeyDown(Keys key) => SendKeyboardInput((ushort)key, KEYEVENTF_KEYDOWN);

        /// <summary>
        /// 释放键盘按键
        /// </summary>
        public static void kbd_KeyUp(Keys key) => SendKeyboardInput((ushort)key, KEYEVENTF_KEYUP);

        /// <summary>
        /// 模拟按键点击（按下并释放）
        /// </summary>
        public static void kbd_KeyPress(Keys key)
        {
            kbd_KeyDown(key);
            Thread.Sleep(50); // 添加短暂延迟模拟真实按键
            kbd_KeyUp(key);
        }

        /// <summary>
        /// 模拟组合键（如Ctrl+C）
        /// </summary>
        public static void kbd_KeyCombo(params Keys[] keys)
        {
            // 按下所有修饰键
            foreach (var key in keys)
            {
                kbd_KeyDown(key);
                Thread.Sleep(10);
            }

            // 短暂延迟后释放
            Thread.Sleep(50);

            // 释放所有按键
            foreach (var key in keys)
            {
                kbd_KeyUp(key);
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 模拟输入文本
        /// </summary>
        public static void TypeText(string text)
        {
            foreach (char c in text)
            {
                // 获取字符的虚拟键码
                short key = VkKeyScan(c);

                if (key != -1)
                {
                    // 提取低字节（主键）和高字节（修饰键）
                    byte vk = (byte)(key & 0xFF);
                    byte modifiers = (byte)((key >> 8) & 0xFF);

                    // 处理修饰键（Shift、Ctrl等）
                    if ((modifiers & 1) != 0) kbd_KeyDown(Keys.ShiftKey); // Shift
                    if ((modifiers & 2) != 0) kbd_KeyDown(Keys.ControlKey); // Ctrl
                    if ((modifiers & 4) != 0) kbd_KeyDown(Keys.Menu); // Alt

                    // 发送按键事件
                    kbd_KeyPress((Keys)vk);

                    // 释放修饰键
                    if ((modifiers & 1) != 0) kbd_KeyUp(Keys.ShiftKey);
                    if ((modifiers & 2) != 0) kbd_KeyUp(Keys.ControlKey);
                    if ((modifiers & 4) != 0) kbd_KeyUp(Keys.Menu);
                }
                else
                {
                    // 特殊字符处理（如无法映射的字符）
                    // 这里可以扩展处理特殊字符
                }

                Thread.Sleep(20); // 按键间延迟
            }
        }

        /// <summary>
        /// 模拟Ctrl+[]组合键
        /// </summary>
        /// <param name="key">Ctrl+key</param>
        public static void kbd_ComboControl(Keys key)
        {
            kbd_KeyDown(Keys.ControlKey);
            kbd_KeyPress(key);
            kbd_KeyUp(Keys.ControlKey);
        }

        /// <summary>
        /// 模拟Shift+[]组合键
        /// </summary>
        /// <param name="key">Shift+key</param>
        public static void kbd_ComboShift(Keys key)
        {
            kbd_KeyDown(Keys.ShiftKey);
            kbd_KeyPress(key);
            kbd_KeyUp(Keys.ShiftKey);
        }
        #endregion



        #endregion

        //------------------------------------------------------------------------------------------------
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int x, int y);







        //DLL引用end==================================================
        #endregion

        //写注册表项
        //rootKey常用常量
        //Registry.CurrentUser (HKEY_CURRENT_USER)
        //Registry.LocalMachine (HKEY_LOCAL_MACHINE)
        //Registry.ClassesRoot (HKEY_CLASSES_ROOT)

        /*valueKind：支持的类型包括：
        String：字符串值
        DWord：32位整数
        QWord：64位整数
        Binary：二进制数据
        MultiString：字符串数组*/

        /// <summary>
        /// 写注册表项
        /// </summary>
        /// <param name="rootKey">根键，通常为Registry.CurrentUser(HKEY_CURRENT_USER)；Registry.LocalMachine(HKEY_LOCAL_MACHINE)；Registry.ClassesRoot(HKEY_CLASSES_ROOT)</param>
        /// <param name="subKeyPath">路径，前方与后方不需要加"\"</param>
        /// <param name="valueName">键名</param>
        /// <param name="value">欲写入的值</param>
        /// <param name="valueKind">键类型通常为String(字符串)；DWord(32为整数)；QWord(64位整数)；Binary(二进制数)；MultiString(字符串数组)</param>
        /// <returns>是否写入成功</returns>
        public static bool WriteRegistryValue(RegistryKey rootKey, string subKeyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            try
            {
                if (rootKey == null)
                    throw new ArgumentNullException(nameof(rootKey));

                if (string.IsNullOrEmpty(subKeyPath))
                    throw new ArgumentException("子项路径不能为空", nameof(subKeyPath));

                using (RegistryKey subKey = rootKey.CreateSubKey(subKeyPath))
                {
                    if (subKey == null) return false;

                    subKey.SetValue(valueName, value, valueKind);
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 权限不足，可能需要以管理员身份运行
                throw;
            }
            catch (Exception ex)
            {
                // 记录异常或处理其他错误
                Console.WriteLine($"写入注册表失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 读取注册表项
        /// </summary>
        /// <param name="rootKey">根键，通常为Registry.CurrentUser(HKEY_CURRENT_USER)；Registry.LocalMachine(HKEY_LOCAL_MACHINE)；Registry.ClassesRoot(HKEY_CLASSES_ROOT)</param>
        /// <param name="subKeyPath">路径，前方与后方不需要加"\"</param>
        /// <param name="valueName">键名</param>
        /// <param name="defaultValue">默认值，在读取失败时返回此值，默认为null</param>
        /// <returns>目标项的值</returns>
        public static object ReadRegistryValue(RegistryKey rootKey, string subKeyPath, string valueName, object defaultValue = null)
        {
            try
            {
                if (rootKey == null)
                    throw new ArgumentNullException(nameof(rootKey));

                if (string.IsNullOrEmpty(subKeyPath))
                    throw new ArgumentException("子项路径不能为空", nameof(subKeyPath));

                using (RegistryKey subKey = rootKey.OpenSubKey(subKeyPath, false))
                {
                    // 子项不存在时返回默认值
                    if (subKey == null) return defaultValue;

                    // 获取值（值不存在时返回默认值）
                    return subKey.GetValue(valueName, defaultValue);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 权限不足，可能需要管理员权限
                throw;
            }
            catch (Exception ex)
            {
                // 记录异常或处理其他错误
                Console.WriteLine($"读取注册表失败: {ex.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 写ini配置项
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void WriteConfig(string filePath, string section, string key, string value)
        {
            var sections = ParseConfigFile(filePath);

            // 创建或更新节
            if (!sections.ContainsKey(section))
            {
                sections[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            // 更新键值
            sections[section][key.Trim()] = value;

            // 生成配置文件内容
            var lines = new List<string>();

            // 处理默认节（空节名）
            if (sections.TryGetValue("", out var defaultSection) && defaultSection.Count > 0)
            {
                lines.AddRange(defaultSection.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }

            // 处理带节名的配置（按字母顺序排序）
            foreach (var sec in sections.Keys
                .Where(s => !string.IsNullOrEmpty(s))
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
            {
                // 添加节分隔空行
                if (lines.Count > 0) lines.Add("");

                lines.Add($"[{sec}]");
                lines.AddRange(sections[sec].Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        /// <summary>
        /// 读ini配置项
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <returns>目标值</returns>
        public static string ReadConfig(string filePath, string section, string key)
        {
            if (!File.Exists(filePath)) return null;

            var sections = ParseConfigFile(filePath);

            if (sections.TryGetValue(section, out var sectionData) &&
                sectionData.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        // 解析配置文件为节字典(读写配置)
        private static Dictionary<string, Dictionary<string, string>> ParseConfigFile(string filePath)
        {
            var sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            string currentSection = "";

            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var trimmed = line.Trim();

                    // 处理节头
                    if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        currentSection = trimmed.Substring(1, trimmed.Length - 2).Trim();
                        if (!sections.ContainsKey(currentSection))
                        {
                            sections[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        }
                        continue;
                    }

                    // 处理键值对
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                    {
                        var k = parts[0].Trim();
                        var v = parts[1].Trim();

                        if (!sections.ContainsKey(currentSection))
                        {
                            sections[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        }

                        sections[currentSection][k] = v;
                    }
                }
            }
            return sections;
        }

        /// <summary>
        /// 删除配置键
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <exception cref="ArgumentException"></exception>
        public static void DeleteConfig(string filePath, string section, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty", nameof(key));

            var sections = ParseConfigFile(filePath);
            section = section ?? "";

            if (sections.TryGetValue(section, out var sectionData) && sectionData.Remove(key))
            {
                // 如果节已空则移除整个节
                if (sectionData.Count == 0)
                {
                    sections.Remove(section);
                }

                SaveSectionsToFile(filePath, sections);

            }
        }

        // 统一保存配置的方法
        private static void SaveSectionsToFile(string filePath, Dictionary<string, Dictionary<string, string>> sections)
        {
            var lines = new List<string>();

            // 处理默认节
            if (sections.TryGetValue("", out var defaultSection) && defaultSection.Count > 0)
            {
                lines.AddRange(defaultSection.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }

            // 处理带节名的配置（按字母排序）
            foreach (var sec in sections.Keys
                .Where(s => !string.IsNullOrEmpty(s))
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
            {
                var sectionData = sections[sec];
                if (sectionData.Count == 0) continue;

                if (lines.Count > 0) lines.Add("");
                lines.Add($"[{sec}]");
                lines.AddRange(sectionData.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        /// <summary>
        /// 删除配置节
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="section">节</param>
        public static void DeleteSection(string filePath, string section)
        {
            var sections = ParseConfigFile(filePath);
            section = section ?? "";

            if (sections.Remove(section))
            {
                SaveSectionsToFile(filePath, sections);
            }
        }

        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="targetPath">源路径</param>
        /// <param name="shortcutPath">目标快捷方式路径</param>
        /// <returns>是否创建成功</returns>
        public static bool CreateShortcut(string targetPath, string shortcutPath)
        {
            if (!File.Exists(targetPath)) return false;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(shortcutPath));

                var shellLink = (IShellLinkW)new ShellLink();
                shellLink.SetPath(targetPath);
                shellLink.SetWorkingDirectory(Path.GetDirectoryName(targetPath));
                shellLink.SetIconLocation(targetPath, 0);  // 使用目标文件自身图标

                var persistFile = (IPersistFile)shellLink;
                persistFile.Save(shortcutPath, false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="source">源文件夹位置</param>
        /// <param name="dest">欲移动至的位置</param>
        /// <param name="overwrite">覆盖现有文件</param>
        /// <param name="errorMessage">Unknown</param>
        /// <returns>是否移动成功</returns>
        public static bool MoveFolder(string source, string dest, bool overwrite, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                // 检查源文件夹
                if (!Directory.Exists(source))
                {
                    errorMessage = "源文件夹不存在";
                    return false;
                }

                // 判断是否跨磁盘移动（关键逻辑）
                bool isSameDrive = Path.GetPathRoot(source)?.ToUpper()
                                == Path.GetPathRoot(dest)?.ToUpper();

                // 处理目标文件夹已存在的情况
                if (Directory.Exists(dest))
                {
                    if (overwrite) Directory.Delete(dest, true);
                    else
                    {
                        errorMessage = "目标文件夹已存在且未启用覆盖";
                        return false;
                    }
                }

                // 执行移动
                if (isSameDrive)
                {
                    Directory.Move(source, dest); // 同一磁盘直接移动
                }
                else
                {
                    // 跨磁盘：复制+删除方案
                    new Computer().FileSystem.CopyDirectory(source, dest, overwrite);
                    Directory.Delete(source, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"{(ex is IOException ? "IO错误" : ex.GetType().Name)}: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sourceFolder">源文件夹未位置</param>
        /// <param name="destFolder">欲复制到的位置</param>
        /// <param name="overwrite">覆盖原有文件</param>
        /// <returns>操作是否成功</returns>
        public static bool CopyFolder(string sourceFolder, string destFolder, bool overwrite)
        {
            try
            {
                // 检查源文件夹是否存在
                if (!Directory.Exists(sourceFolder))
                {
                    return false;
                }

                // 处理目标文件夹已存在的情况
                if (Directory.Exists(destFolder))
                {
                    if (overwrite)
                    {
                        // 递归删除目标文件夹
                        Directory.Delete(destFolder, true);
                    }
                    else
                    {
                        return false;
                    }
                }

                // 创建目标目录结构
                Directory.CreateDirectory(Path.GetDirectoryName(destFolder));

                // 执行复制操作（自动处理所有子内容和文件）
                new Computer().FileSystem.CopyDirectory(
                    sourceFolder,
                    destFolder,
                    overwrite
                );

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步HTTP下载文件带进度可取消
        /// </summary>
        /// <param name="url">目标URL</param>
        /// <param name="savePath">保存目录</param>
        /// <param name="progress">进度</param>
        /// <param name="cancellationToken">取消下载Token</param>
        /// <returns></returns>
        public static async Task DownloadFileAsync(string url, string savePath, IProgress<(int ProgressPercentage, long BytesReceived)> progress = null, CancellationToken cancellationToken = default)
        {
            using (var httpClient = new HttpClient())
            {
                // 获取响应头并验证状态
                using (var response = await httpClient.GetAsync(
                    url,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    // 创建保存目录
                    var directory = Path.GetDirectoryName(savePath);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    // 获取文件总大小
                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var receivedBytes = 0L;

                    // 创建文件流
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(
                        savePath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 8192,
                        useAsync: true))
                    {
                        var buffer = new byte[8192];
                        int bytesRead;

                        // 分段下载并更新进度
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            receivedBytes += bytesRead;

                            if (totalBytes > 0)
                            {
                                var progressPercentage = (int)((double)receivedBytes / totalBytes * 100);
                                progress?.Report((progressPercentage, receivedBytes));
                            }
                            else
                            {
                                progress?.Report((-1, receivedBytes));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 异步HTTP读文件
        /// </summary>
        /// <param name="url">目标URL</param>
        /// <returns>读取到的内容</returns>
        public static async Task<string> HttpReadFileAsync(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 设置超时时间（可选）
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // 发送GET请求
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();  // 确保响应成功
                        return await response.Content.ReadAsStringAsync();  // 读取内容为字符串
                    }
                }
            }
            catch (Exception ex)
            {
                // 这里可以记录异常或进行其他处理
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null;  // 或者根据需求返回空字符串/抛出异常
            }
        }

        //异步HTTP下载文件带进度
        public static async Task DownloadFileAsync(string url, string savePath, IProgress<int> progress)
        {
            using (WebClient webClient = new WebClient())
            {
                // 设置进度报告事件
                webClient.DownloadProgressChanged += (sender, e) =>
                {
                    progress?.Report(e.ProgressPercentage);
                };

                // 异步下载文件
                await webClient.DownloadFileTaskAsync(new Uri(url), savePath);
            }
        }

        /// <summary>
        /// 平滑移动鼠标至目标位置
        /// </summary>
        /// <param name="pos">欲移动至的位置</param>
        /// <param name="speed">移动速度，每次移动多少像素</param>
        /// <param name="delay">移动延迟(ms)，每隔多久移动一次</param>
        public static async void m_SmoothMoveToPosition(Point pos, int speed = 5, int delay = 1)
        {
            Point start = m_GetPosition();
            if (start == pos) return;  // 已在目标位置

            // 计算移动向量
            double dx = pos.X - start.X;
            double dy = pos.Y - start.Y;

            // 计算总距离
            double distance = Math.Sqrt(dx * dx + dy * dy);
            int steps = (int)Math.Ceiling(distance / speed);

            // 计算单位向量
            if (distance > 0)
            {
                dx /= distance;
                dy /= distance;
            }

            PointF current = new PointF(start.X, start.Y);
            for (int i = 0; i < steps; i++)
            {
                // 计算当前步的移动比例
                double progress = (double)(i + 1) / steps;
                // 计算新位置
                current.X = (float)(start.X + dx * distance * progress);
                current.Y = (float)(start.Y + dy * distance * progress);

                // 四舍五入取整并设置位置
                m_SetPositionPoint(new Point(
                    (int)Math.Round(current.X),
                    (int)Math.Round(current.Y)
                ));

                // 添加延迟（最后一步不延迟）
                if (i < steps - 1)
                {
                    await Task.Delay(delay);
                }
            }
        }

        /// <summary>
        /// 获取屏幕指定坐标的像素颜色
        /// </summary>
        /// <param name="x">屏幕横坐标</param>
        /// <param name="y">屏幕纵坐标</param>
        /// <returns>RGB 颜色值</returns>
        public static Color GetPixelRgb(Point pos)
        {
            IntPtr hdc = GetDC(IntPtr.Zero); // 获取屏幕设备上下文
            uint pixel = GetPixel(hdc, pos.X, pos.Y); // 获取像素值
            ReleaseDC(IntPtr.Zero, hdc);     // 释放上下文

            // 从 COLORREF (0x00BBGGRR) 提取 RGB 分量
            byte r = (byte)(pixel & 0x000000FF);
            byte g = (byte)((pixel & 0x0000FF00) >> 8);
            byte b = (byte)((pixel & 0x00FF0000) >> 16);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// 屏幕截图
        /// </summary>
        /// <param name="region">区域</param>
        /// <returns>返回位图</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Bitmap CaptureRegion(Rectangle region)
        {
            // 验证区域有效性
            if (region.Width <= 0 || region.Height <= 0)
            {
                throw new ArgumentException("Invalid capture region dimensions");
            }

            Bitmap screenshot = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(
                    sourceX: region.X,
                    sourceY: region.Y,
                    destinationX: 0,
                    destinationY: 0,
                    blockRegionSize: region.Size,
                    copyPixelOperation: CopyPixelOperation.SourceCopy
                );
            }

            return screenshot;
        }

        /// <summary>
        /// 图像识别相似度检测
        /// </summary>
        /// <param name="original">源图片</param>
        /// <param name="compare">被比较图片</param>
        /// <param name="tolerance">容限</param>
        /// <returns>相似度(%)</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static double CheckImageSimilarity_Bit(Bitmap original, Bitmap compare, int tolerance)
        {
            // 基础检查
            if (original == null || compare == null)
                throw new ArgumentNullException("Images cannot be null");

            if (original.Size != compare.Size)
                return 0.0; // 尺寸不同，相似度为0

            // 兼容旧框架的容差限制方法
            tolerance = (tolerance < 0) ? 0 : (tolerance > 255) ? 255 : tolerance;

            // 锁定位图数据
            var bd1 = original.LockBits(
                new Rectangle(Point.Empty, original.Size),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            var bd2 = compare.LockBits(
                new Rectangle(Point.Empty, compare.Size),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                int bytesPerPixel = 4; // 32bpp ARGB
                int totalPixels = original.Width * original.Height;
                int similarPixels = 0;

                // 获取扫描线宽度（可能包含填充字节）
                int stride = Math.Min(bd1.Stride, bd2.Stride);

                // 创建存储像素数据的数组
                byte[] data1 = new byte[stride * original.Height];
                byte[] data2 = new byte[stride * original.Height];

                // 复制位图数据
                System.Runtime.InteropServices.Marshal.Copy(bd1.Scan0, data1, 0, data1.Length);
                System.Runtime.InteropServices.Marshal.Copy(bd2.Scan0, data2, 0, data2.Length);

                // 逐像素比较
                for (int y = 0; y < original.Height; y++)
                {
                    int rowStart = y * stride;
                    for (int x = 0; x < original.Width; x++)
                    {
                        int idx = rowStart + x * bytesPerPixel;

                        // 确保不超出数组边界（针对最后一行）
                        if (idx + 3 >= data1.Length || idx + 3 >= data2.Length)
                            continue;

                        bool isPixelSimilar = true;

                        // 比较BGRA通道（忽略Alpha通道）
                        for (int offset = 0; offset < 3; offset++) // B, G, R
                        {
                            int diff = Math.Abs(data1[idx + offset] - data2[idx + offset]);
                            if (diff > tolerance)
                            {
                                isPixelSimilar = false;
                                break;
                            }
                        }

                        if (isPixelSimilar) similarPixels++;
                    }
                }

                // 计算相似度比例
                return totalPixels > 0 ? (double)similarPixels / totalPixels : 0.0;
            }
            finally
            {
                // 确保解锁资源
                original.UnlockBits(bd1);
                compare.UnlockBits(bd2);
            }
        }

        /// <summary>
        /// 字符串转哈希
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns>哈希值</returns>
        public static string StringToSHA256(string input)
        {
            // 检查输入是否为null或空
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                // 将字符串转换为字节数组
                byte[] bytes = Encoding.UTF8.GetBytes(input);

                // 计算哈希值
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // 将字节数组转换为十六进制字符串
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // "x2" 表示两位小写十六进制
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Http读图像异步
        /// </summary>
        /// <param name="imageUrl">URL</param>
        /// <returns></returns>
        public static async Task<Image> HttpReadImageAsync(string imageUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                byte[] imageData = await httpClient.GetByteArrayAsync(imageUrl);
                using (MemoryStream stream = new MemoryStream(imageData))
                {
                    return Image.FromStream(stream);
                }
            }
        }

        /// <summary>
        /// 从JSON文件读取并反序列化为对象
        /// </summary>
        public static T ReadJson<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 将对象序列化为JSON并写入文件
        /// </summary>
        public static void WriteJson<T>(string filePath, T data)
        {
            var serializer = new JsonSerializer
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            using (var sw = new StreamWriter(filePath))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                writer.IndentChar = ' ';
                writer.Indentation = 4;
                serializer.Serialize(writer, data);
            }
        }

        /// <summary>
        /// 异步执行控制台命令
        /// </summary>
        /// <param name="command">要执行的命令</param>
        /// <param name="showWindow">是否显示 cmd 窗口</param>
        /// <param name="closeAfter">执行完成后是否关闭窗口</param>
        /// <param name="workingDirectory">可选，指定工作目录</param>
        /// <returns>命令输出的字符串（仅在隐藏窗口时有效）</returns>
        public static async Task<string> RunCmdAsync(string command, bool showWindow = false, bool closeAfter = true, string workingDirectory = "")
        {
            return await Task.Run(() =>
            {
                try
                {
                    string argPrefix = closeAfter ? "/c " : "/k "; // /c 执行后关闭, /k 执行后保留

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = argPrefix + command,
                        RedirectStandardOutput = !showWindow,
                        RedirectStandardError = !showWindow,
                        UseShellExecute = showWindow,
                        CreateNoWindow = !showWindow
                    };

                    if (!string.IsNullOrEmpty(workingDirectory))
                    {
                        psi.WorkingDirectory = workingDirectory;
                    }

                    using (Process process = Process.Start(psi))
                    {
                        if (showWindow)
                        {
                            process.WaitForExit();
                            return "";
                        }
                        else
                        {
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                            process.WaitForExit();

                            return string.IsNullOrEmpty(error) ? output : output + Environment.NewLine + "错误: " + error;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "执行异常: " + ex.Message;
                }
            });
        }
        #endregion
    }

    public class GlobalHotkey : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const uint MOD_NONE = 0x0000;
        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;

        private const int WM_HOTKEY = 0x0312;

        private readonly IntPtr _handle;
        private readonly int _id;
        private bool _isRegistered;

        // 使用自定义委托来传递热键信息
        public delegate void HotkeyPressedHandler(GlobalHotkey hotkey);
        public event HotkeyPressedHandler HotkeyPressed;

        // 添加名称属性以便识别
        public string Name { get; set; }

        public GlobalHotkey(IntPtr handle, uint modifiers, Keys key, string name = "")
        {
            _handle = handle;
            _id = GetHashCode();
            Key = key;
            Modifiers = modifiers;
            Name = name;
        }

        public Keys Key { get; }
        public uint Modifiers { get; }

        public bool Register()
        {
            if (_isRegistered) return true;

            _isRegistered = RegisterHotKey(_handle, _id, Modifiers, (uint)Key);
            return _isRegistered;
        }

        public void Unregister()
        {
            if (!_isRegistered) return;

            UnregisterHotKey(_handle, _id);
            _isRegistered = false;
        }

        public void ProcessMessage(Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == _id)
            {
                HotkeyPressed?.Invoke(this);
            }
        }

        public void Dispose()
        {
            Unregister();
            GC.SuppressFinalize(this);
        }

        // 获取修饰键的字符串表示
        public string GetModifiersString()
        {
            string result = "";
            if ((Modifiers & MOD_CONTROL) != 0) result += "Ctrl+";
            if ((Modifiers & MOD_SHIFT) != 0) result += "Shift+";
            if ((Modifiers & MOD_ALT) != 0) result += "Alt+";
            if ((Modifiers & MOD_WIN) != 0) result += "Win+";
            return result.TrimEnd('+');
        }

        // 获取完整热键字符串
        public override string ToString()
        {
            return $"{GetModifiersString()}+{Key}";
        }
    }

}
