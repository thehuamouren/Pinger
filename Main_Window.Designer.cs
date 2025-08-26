namespace Pinger
{
    partial class Main_Window
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label_IP = new System.Windows.Forms.Label();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.label_DataPackSize = new System.Windows.Forms.Label();
            this.numericUpDown_DatapackSize = new System.Windows.Forms.NumericUpDown();
            this.trackBar_DatapackSize = new System.Windows.Forms.TrackBar();
            this.label_ThreadNum = new System.Windows.Forms.Label();
            this.numericUpDown_Thread = new System.Windows.Forms.NumericUpDown();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.button_Start = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_DatapackSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_DatapackSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Thread)).BeginInit();
            this.SuspendLayout();
            // 
            // label_IP
            // 
            this.label_IP.Location = new System.Drawing.Point(12, 9);
            this.label_IP.Name = "label_IP";
            this.label_IP.Size = new System.Drawing.Size(48, 25);
            this.label_IP.TabIndex = 0;
            this.label_IP.Text = "目标IP:";
            this.label_IP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_IP
            // 
            this.textBox_IP.Location = new System.Drawing.Point(66, 10);
            this.textBox_IP.Name = "textBox_IP";
            this.textBox_IP.Size = new System.Drawing.Size(702, 23);
            this.textBox_IP.TabIndex = 1;
            this.textBox_IP.Text = "127.0.0.1";
            this.textBox_IP.TextChanged += new System.EventHandler(this.textBox_IP_TextChanged);
            // 
            // label_DataPackSize
            // 
            this.label_DataPackSize.Location = new System.Drawing.Point(12, 37);
            this.label_DataPackSize.Name = "label_DataPackSize";
            this.label_DataPackSize.Size = new System.Drawing.Size(76, 25);
            this.label_DataPackSize.TabIndex = 2;
            this.label_DataPackSize.Text = "数据包大小:";
            this.label_DataPackSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDown_DatapackSize
            // 
            this.numericUpDown_DatapackSize.Location = new System.Drawing.Point(94, 39);
            this.numericUpDown_DatapackSize.Maximum = new decimal(new int[] {
            65500,
            0,
            0,
            0});
            this.numericUpDown_DatapackSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_DatapackSize.Name = "numericUpDown_DatapackSize";
            this.numericUpDown_DatapackSize.Size = new System.Drawing.Size(68, 23);
            this.numericUpDown_DatapackSize.TabIndex = 3;
            this.numericUpDown_DatapackSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_DatapackSize.ValueChanged += new System.EventHandler(this.numericUpDown_DatapackSize_ValueChanged);
            // 
            // trackBar_DatapackSize
            // 
            this.trackBar_DatapackSize.Location = new System.Drawing.Point(168, 39);
            this.trackBar_DatapackSize.Maximum = 65500;
            this.trackBar_DatapackSize.Minimum = 1;
            this.trackBar_DatapackSize.Name = "trackBar_DatapackSize";
            this.trackBar_DatapackSize.Size = new System.Drawing.Size(600, 45);
            this.trackBar_DatapackSize.TabIndex = 5;
            this.trackBar_DatapackSize.Value = 1;
            this.trackBar_DatapackSize.Scroll += new System.EventHandler(this.trackBar_DatapackSize_Scroll);
            // 
            // label_ThreadNum
            // 
            this.label_ThreadNum.Location = new System.Drawing.Point(12, 66);
            this.label_ThreadNum.Name = "label_ThreadNum";
            this.label_ThreadNum.Size = new System.Drawing.Size(48, 25);
            this.label_ThreadNum.TabIndex = 6;
            this.label_ThreadNum.Text = "线程数:";
            this.label_ThreadNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDown_Thread
            // 
            this.numericUpDown_Thread.Location = new System.Drawing.Point(66, 68);
            this.numericUpDown_Thread.Maximum = new decimal(new int[] {
            65500,
            0,
            0,
            0});
            this.numericUpDown_Thread.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Thread.Name = "numericUpDown_Thread";
            this.numericUpDown_Thread.Size = new System.Drawing.Size(188, 23);
            this.numericUpDown_Thread.TabIndex = 7;
            this.numericUpDown_Thread.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Thread.ValueChanged += new System.EventHandler(this.numericUpDown_Thread_ValueChanged);
            // 
            // textBox_Log
            // 
            this.textBox_Log.Location = new System.Drawing.Point(15, 97);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Log.Size = new System.Drawing.Size(753, 206);
            this.textBox_Log.TabIndex = 8;
            this.textBox_Log.WordWrap = false;
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(15, 309);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(128, 31);
            this.button_Start.TabIndex = 9;
            this.button_Start.Text = "开始Ping";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.Location = new System.Drawing.Point(149, 309);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(179, 31);
            this.button_Stop.TabIndex = 10;
            this.button_Stop.Text = "强制结束所有Ping进程";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // Main_Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 352);
            this.Controls.Add(this.button_Stop);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.textBox_Log);
            this.Controls.Add(this.numericUpDown_Thread);
            this.Controls.Add(this.label_ThreadNum);
            this.Controls.Add(this.trackBar_DatapackSize);
            this.Controls.Add(this.numericUpDown_DatapackSize);
            this.Controls.Add(this.label_DataPackSize);
            this.Controls.Add(this.textBox_IP);
            this.Controls.Add(this.label_IP);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(796, 391);
            this.MinimumSize = new System.Drawing.Size(796, 391);
            this.Name = "Main_Window";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main_Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_Window_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_DatapackSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_DatapackSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Thread)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_IP;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.Label label_DataPackSize;
        private System.Windows.Forms.NumericUpDown numericUpDown_DatapackSize;
        private System.Windows.Forms.TrackBar trackBar_DatapackSize;
        private System.Windows.Forms.Label label_ThreadNum;
        private System.Windows.Forms.NumericUpDown numericUpDown_Thread;
        private System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Button button_Stop;
    }
}

