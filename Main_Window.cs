using Funcitons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Funcitons.NormalFunc;

namespace Pinger
{
    public partial class Main_Window : Form
    {
        //函数========================================================================================

        /// <summary>
        /// 执行 CMD 指令，并实时获取输出
        /// </summary>
        /// <param name="command">要执行的命令，例如 "ping 127.0.0.1 -t"</param>
        /// <param name="onOutput">每行输出回调</param>
        /// <param name="onError">每行错误回调</param>
        /// <param name="onExit">进程退出回调</param>
        public static void RunCmdGetOutput(
        string command,
        Action<string> onOutput = null,
        Action<string> onError = null,
        Action<int> onExit = null)
        {
            Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c " + command,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        // Windows CMD 默认编码 GBK，避免中文乱码
                        StandardOutputEncoding = Encoding.GetEncoding("GBK"),
                        StandardErrorEncoding = Encoding.GetEncoding("GBK")
                    };

                    using (Process process = new Process())
                    {
                        process.StartInfo = psi;

                        process.OutputDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                onOutput?.Invoke(e.Data);
                            }
                        };

                        process.ErrorDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                onError?.Invoke(e.Data);
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();

                        onExit?.Invoke(process.ExitCode);
                    }
                }
                catch (Exception ex)
                {
                    onError?.Invoke("[EXCEPTION] " + ex.Message);
                }
            });
        }

        public void Addlog(string message)
        {
            textBox_Log.AppendText($"{message}\r\n");
            textBox_Log.SelectionStart = textBox_Log.Text.Length;
            textBox_Log.ScrollToCaret();

            if (message.Length >= 2 && message.Substring(0, 2) == "来自") 
            {
                TotalDatapackSize = TotalDatapackSize + (double)GlobalConfig.PingerConfig.DatapackSize / 1048576.0;
                
                ConnectSuccess++;
                ConnectRequest++;
            }

            if (message.Length >= 4 && message.Substring(0, 4) == "请求超时")
            {
                ConnectError++;
                ConnectRequest++;
            }
            else if (message.Length >= 15 && message.Substring(0, 15) == "PING：传输失败。常见故障。")
            {
                ConnectError++;
                ConnectRequest++;
            }
            else if (message.Length >= 7 && message.Substring(0, 7) == "目标主机不可达") 
            {
                ConnectError++;
                ConnectRequest++;
            }
            else if (message.Length >= 7 && message.Substring(0, 7) == "目标网络不可达")
            {
                ConnectError++;
                ConnectRequest++;
            }
            else if (message.Length >= 4 && message.Substring(0, 4) == "传输失败")
            {
                ConnectError++;
                ConnectRequest++;
            }
            else if (message.Length >= 12 && message.Substring(0, 12) == "Ping 请求找不到主机")
            {
                ConnectError++;
                ConnectRequest++;
            }
            else if (message.Length >= 4 && message.Substring(0, 4) == "常规故障")
            {
                ConnectError++;
                ConnectRequest++;
            }


            label_TotalDatapackSize.Text = $"总发送量: {Math.Round(TotalDatapackSize, 3)}MB";
            label_Status.Text = $"请求数: {ConnectRequest}     请求成功: {ConnectSuccess}     请求失败: {ConnectError}";
        }

        //Json类=====================================================================================
        public class Config
        {
            public CfgRoot PingerConfig { get; set; }
        }

        public class CfgRoot
        {
            public string IP { get; set; }
            public int DatapackSize { get; set; }
            public int Thread { get; set; }
        }

        /*
        {
            "PingerConfig": {
                "IP": "127.0.0.1",
                "DatapackSize": 1,
                "Thread": 1
            }
        }
        */
        //变量========================================================================================
        public static string RunPath = Directory.GetCurrentDirectory();
        public static string ConfigPath = $"{RunPath}\\config.json";
        public static string Version = "Release 1.0.0.0";
        public static Config GlobalConfig;
        public static double TotalDatapackSize = 0;

        public static double ConnectSuccess = 0;
        public static double ConnectError = 0;
        public static double ConnectRequest = 0;
        //==============================================================================================
        public Main_Window()
        {
            InitializeComponent();

            try
            {
                //窗口标题
                this.Text = $"Pinger {Version}";

                //初始化json
                if (!File.Exists(ConfigPath))
                {
                    Addlog($"未检测到配置文件，正在初始化配置文件...\r\n");
                    GlobalConfig = new Config
                    {
                        PingerConfig = new CfgRoot
                        {
                            IP = "127.0.0.1",
                            DatapackSize = 1,
                            Thread = 1
                        }
                    };
                    WriteJson<Config>(ConfigPath, GlobalConfig);
                    Addlog("初始化配置文件成功!");
                }

                //读json
                GlobalConfig = ReadJson<Config>(ConfigPath);

                textBox_IP.Text = GlobalConfig.PingerConfig.IP;
                numericUpDown_DatapackSize.Value = GlobalConfig.PingerConfig.DatapackSize;
                trackBar_DatapackSize.Value = GlobalConfig.PingerConfig.DatapackSize;
                numericUpDown_Thread.Value = GlobalConfig.PingerConfig.Thread;

                //初始化完成
                Addlog("程序初始化完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"在初始化程序时发生错误！\n\n错误原因：{ex.Message}", "发生错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void textBox_IP_TextChanged(object sender, EventArgs e)
        {
            GlobalConfig.PingerConfig.IP = textBox_IP.Text;
            WriteJson<Config>(ConfigPath, GlobalConfig);
        }

        private void numericUpDown_DatapackSize_ValueChanged(object sender, EventArgs e)
        {
            trackBar_DatapackSize.Value = (int)numericUpDown_DatapackSize.Value;
            GlobalConfig.PingerConfig.DatapackSize = (int)numericUpDown_DatapackSize.Value;
            WriteJson<Config>(ConfigPath, GlobalConfig);
        }

        private void trackBar_DatapackSize_Scroll(object sender, EventArgs e)
        {
            numericUpDown_DatapackSize.Value = trackBar_DatapackSize.Value;
            GlobalConfig.PingerConfig.DatapackSize = trackBar_DatapackSize.Value;
            WriteJson<Config>(ConfigPath, GlobalConfig);
        }

        private void numericUpDown_Thread_ValueChanged(object sender, EventArgs e)
        {
            GlobalConfig.PingerConfig.Thread = (int)numericUpDown_Thread.Value;
            WriteJson<Config>(ConfigPath, GlobalConfig);
        }

        private async void button_Start_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < GlobalConfig.PingerConfig.Thread; i++)
            {
                try
                {
                    await Task.Run(() => RunProcess());

                    Addlog($"线程 {i + 1} 已启动");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"在开启进程时发生错误！\n\n错误原因：{ex.Message}", "发生错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }




            }
        }

        private void RunProcess()
        {
            RunCmdGetOutput(
                    $"ping {GlobalConfig.PingerConfig.IP} -l {GlobalConfig.PingerConfig.DatapackSize} -t",
                    onOutput: line =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            Addlog($"{line}");
                        }));
                    },
                    onError: line =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            Addlog($"[错误] {line}");
                        }));
                    },
                    onExit: code =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            Addlog($"进程退出，退出代码:{code}");
                        }));
                    }
                    );
        }

        private async void button_Stop_Click(object sender, EventArgs e)
        {
            Addlog($"{await RunCmdAsync("taskkill /f /im ping.exe")}");
            Addlog($"{await RunCmdAsync("taskkill /f /im cmd.exe")}");
            

        }

        private async void Main_Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            await RunCmdAsync("taskkill /f /im ping.exe");
            await RunCmdAsync("taskkill /f /im cmd.exe");
            
        }
    }
}
