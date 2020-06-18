using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using WMPLib;
using System.IO;
namespace Clock_pratice
{
    public partial class Form1 : Form
    {
        Graphics g;
        //捷徑功能表
        private Label ContextMenuStrip_Index = new Label();
        //變數宣告
        Rectangle rect;
        Font font = new Font("Arial", 12);
        SolidBrush brush = new SolidBrush(Color.Black);
        private int hh, mm, ss, millis;
        private string h, m, s, ms;
        private Button startButton;
        //StopWatch
        private bool IsOn = false;
        private Button splitTimeButton;
        private ListBox listBox1;
        private int listOrder;//listbox排序
        //Timer
        private ComboBox hourCombox;
        private ComboBox minCombox;
        private Button stopButton;
        //滑鼠是否點擊且
        //記錄當前滑鼠座標
        bool isMouseDown = false;
        int currentX, currentY;
        //鬧鐘音效
        string path = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../../ling.mp3"));
        WindowsMediaPlayer winplayer = new WMPLib.WindowsMediaPlayer();
        
        public Form1()
        {
            InitializeComponent();
            this.ContextMenuStrip = clockMenuStrip;
            winplayer.URL = path;
            winplayer.controls.stop();
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            this.clockMenuStrip.Items.Add("DigitalClock");
            this.clockMenuStrip.Items.Add("AnalogyClock");
            this.clockMenuStrip.Items.Add("StopWatch");
            this.clockMenuStrip.Items.Add("Timer");
            this.clockMenuStrip.Items.Add("關閉-左點兩下");
            this.clockMenuStrip.Items[0].Click += new System.EventHandler(CMS_DigitalClock);
            this.clockMenuStrip.Items[1].Click += new System.EventHandler(CMS_AnalogyClock);
            this.clockMenuStrip.Items[2].Click += new System.EventHandler(CMS_StopWatch);
            this.clockMenuStrip.Items[3].Click += new System.EventHandler(CMS_Timer);
            
            ContextMenuStrip_Index.TextChanged += new System.EventHandler(ContextMenuStrip_Index_TextChange);
            ContextMenuStrip_Index.Text = "1";

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //更新畫面
            Invalidate();
            
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (MouseButtons == MouseButtons.Left)
            {
                isMouseDown = true;
                //拖移時紀錄目前滑鼠座標
                currentX = MousePosition.X;
                currentY = MousePosition.Y;
            }

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                //目前位置減掉紀錄位置 等於 移動多少
                this.Left += MousePosition.X - currentX;
                this.Top += MousePosition.Y - currentY;
                currentX = MousePosition.X;
                currentY = MousePosition.Y;
            }
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //宣告GDI+繪圖介面
            g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            #region 定時繪圖的判斷迴圈
            switch (ContextMenuStrip_Index.Text)
            {

                //數位時鐘
                case "0":
                    //將時、分、秒保持擁有兩個數字
                    ss = DateTime.Now.Second;
                    mm = DateTime.Now.Minute;
                    hh = DateTime.Now.Hour;
                    Time_DoubleNumber();
                    //dynamic text
                    g.FillRectangle(Brushes.Chocolate, rect);
                    g.DrawString("當前時間" + h + ":" + m + ":" + s, font, brush, rect);
                    break;
                    
                //------------時間圖案--------------
                case "1":

                    g.DrawEllipse(new Pen(Color.Black, 2), 50, 50, 200, 200);
                    g.FillEllipse(Brushes.White, 50, 50, 200, 200);
                  
                    //重新定位坐標
                    g.TranslateTransform(150, 150);
                    g.DrawString("6"  , font, brush, -7, 72);
                    g.DrawString("3"  , font, brush, 75, -7);
                    g.DrawString("9"  , font, brush, -85, -7);
                    g.DrawString("12", font, brush, -12, -88);
                    
                    //定義常數 半徑
                    const int r = 100;


                    //繪製時鐘的小時、分鐘線
                    for (int z = 0; z < 60; z++)
                    {
                        g.ResetTransform(); //重製旋轉座標，每次都從0度
                        g.TranslateTransform(150, 150); //更改坐標原點
                        g.RotateTransform(z * 6); //旋轉，每一秒旋轉6度 不懂

                        if (z % 5 == 0) //如果是時針則
                        {
                            g.DrawLine(new Pen(Color.Black, 3.0F), 0, -r, 0, -r + 10);
                        }
                        else //分針
                        {
                            g.DrawLine(new Pen(Color.Black, 1.5F), 0, -r, 0, -r + 5);
                        }
                    }
                    
                    //繪製秒針
                    g.ResetTransform();
                    g.TranslateTransform(150, 150); //更改坐標原點
                    g.RotateTransform(DateTime.Now.Second * 6 - (135)); //旋轉
                    Pen scePen = new Pen(Color.Black, 1.0f);
                    scePen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                    scePen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    g.DrawLine(scePen, 0, 0, 60, 60);
                    //繪製分針
                    g.ResetTransform();
                    g.TranslateTransform(150, 150);
                    g.RotateTransform(DateTime.Now.Minute * 6 - (135));
                    Pen minPen = new Pen(Color.Black, 2f);
                    minPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                    minPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    g.DrawLine(minPen, 0, 0, 50, 50);
                    //繪製時針

                    g.ResetTransform();
                    g.TranslateTransform(150, 150);
                    g.RotateTransform((0 - 135) + ((DateTime.Now.Hour % 12) * 30) + (DateTime.Now.Minute / 2)); //mm/2，一個小時夾縫30度、60分鐘，除2讓時針顯示更精確
                    Pen hourPen = new Pen(Color.Black, 3f);
                    hourPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                    hourPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    g.DrawLine(hourPen, 0, 0, 35, 35);
                    break;

                case "2": 

                    //時間判斷
                    if (IsOn)
                    {
                        millis += 10;
                        if (millis == 100)
                        {
                            millis = 0;
                            ss += 1;
                        }
                        if (ss == 60)
                        {
                            ss = 0;
                            mm += 1;
                        }
                        if (mm == 60)
                        {
                            mm = 0;
                            hh += 1;
                        }
                        if (hh > 99)
                        {

                        }
                    }
                    Time_DoubleNumber();
                    g.FillRectangle(Brushes.Gray, rect);
                    g.DrawString (h + ":" + m + ":" + s, font, brush, rect);
                    
                    break;

                case "3":
                    
                    
                    g.FillRectangle(Brushes.Gray, rect);
                    if (IsOn == false)
                    {
                        g.DrawString("      時     分", font, brush, rect);

                    }
                    else if (IsOn && stopButton.Text == "暫停")
                    {
                        millis = millis - 10;

                        if (millis == 0)
                        {
                            millis = 100;
                            ss = ss - 1;
                        }
                        if (ss == -1 && mm != 0)
                        {
                            ss = 59;
                            mm -= 1;
                        }
                        if (mm == 0 && hh != 0)
                        {
                            mm = 59;
                            hh -= 1;
                        }

                        // 當mm hh ss都等於0，停止計時
                        if (hh == 0 && mm == 0 & ss == 0)
                        {
                            IsOn = false;
                            winplayer.controls.play();
                        }
                        Time_DoubleNumber();

                        g.DrawString(h + ":" + m + ":" + s, font, brush, rect);
                    }
                    else if (IsOn && stopButton.Text == "繼續")
                    {
                        g.DrawString(h + ":" + m + ":" + s, font, brush, rect);
                    }
                    break;
            }
            #endregion
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouseDown == true)  isMouseDown = false;
        }
        private void clockMenuStrip_Opening(object sender,System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
        }
        private void CMS_DigitalClock(object sender,System.EventArgs e)
        {
            ContextMenuStrip_Index.Text = "0";
        }

        private void CMS_AnalogyClock(object sender, System.EventArgs e)
        {
            ContextMenuStrip_Index.Text = "1";
        }
        private void CMS_StopWatch(object sender, System.EventArgs e)
        {
            ContextMenuStrip_Index.Text = "2";
        }

        private void CMS_Timer(object sender, System.EventArgs e)
        {
            ContextMenuStrip_Index.Text = "3";
        }
        public void Time_DoubleNumber()
        {
            s = Convert.ToString(ss);
            m = Convert.ToString(mm);
            h = Convert.ToString(hh);
            
            if (ss <= 9) s = "0" + s;
            if (mm <= 9) m = "0" + m;
            if (hh <= 9) h = "0" + h;
        }
        public void startButton_Click(object sender,EventArgs e)
        { //計時迴圈
            if (ContextMenuStrip_Index.Text == "2")
            {
                //開下開始時
                if (IsOn == false)
                {
                    startButton.Text = "停止";
                    splitTimeButton.Text = "分圈";
                    splitTimeButton.Enabled = true;
                }
                //按下停止時
                else if(IsOn == true)
                {
                    startButton.Text = "開始";
                    splitTimeButton.Text = "重設";
                }
            }
            else if (ContextMenuStrip_Index.Text == "3")
            {
                //Click Start
                if (IsOn == false)
                {
                    startButton.Text = "取消";
                    stopButton.Text = "暫停";
                    stopButton.Enabled = true;

                    hh = int.Parse(hourCombox.Text);
                    mm = int.Parse(minCombox.Text);
                    ss = 0;
                    millis = 100;
                    this.Controls.Remove(hourCombox);
                    this.Controls.Remove(minCombox);
                }
                //Click cancel
                else if(IsOn == true)
                {

                    startButton.Text = "開始";
                    stopButton.Text = "暫停";
                    stopButton.Enabled = false;

                    hh = 0;
                    mm = 0;
                    ss = 60;
                    millis = 100;
                    this.Controls.Add(hourCombox);
                    this.Controls.Add(minCombox);
                }
            }
            IsOn = !IsOn;
        }
        public void splitTimeButton_Click(object sender, EventArgs e)
        {
                if (IsOn)
                {
                    listOrder += 1;
                    listBox1.Items.Add($"{listOrder}      {hh}:{mm}:{ss}");
                }
                else if (IsOn == false)
                {
                    hh = 0;
                    mm = 0;
                    ss = 0;
                    listOrder = 0;
                    listBox1.Items.Clear();
                    splitTimeButton.Enabled = false;
                }
            }    
        
         public void stopButton_Click(object sender, EventArgs e)
         {
            if (stopButton.Text == "暫停") stopButton.Text = "繼續";
            else if (stopButton.Text == "繼續") stopButton.Text = "暫停";

        }
        public void hourComboBox_TextChange(object sender,EventArgs e)
        {
            int H;
            try
            {
                H = int.Parse(hourCombox.Text);
                if (H < 0 || H > 24)
                {
                    MessageBox.Show("輸入範圍不可超過0~24");
                    hourCombox.Text = "0";
                }

            }
            catch (FormatException)
            {
                MessageBox.Show("請輸入數字");
                hourCombox.Text = "0";
            }
            
        }
        public void minComboBox_TextChange(object sender, EventArgs e)
        {
            int M;
            
            try
            {
                M = int.Parse(minCombox.Text);
                if (M < 1 || M > 60)
                {
                    MessageBox.Show("輸入範圍不可超過1~60");
                    minCombox.Text = "1";
                }

            }
            catch (Exception)
            {
                MessageBox.Show("請輸入數字");
                minCombox.Text = "1";
            }
        }
        public void ContextMenuStrip_Index_TextChange(object sender, EventArgs e)
        {//當功能變換時，執行一次
            this.Controls.Clear();
            Invalidate();
            
            PaintEventArgs paintEA = new PaintEventArgs(this.CreateGraphics(), new Rectangle(this.Location, this.Size));
            IsOn = false;
            switch (ContextMenuStrip_Index.Text)
            {
                case "0":
                    rect = new Rectangle(65, 10, 185, 20);
                    font = new Font("Arial", 12);
                    brush = new SolidBrush(Color.White);
                    Form1_Paint(new object(), paintEA);
                    break;

                case "1":
                    font = new Font("Arial", 12);
                    brush = new SolidBrush(Color.Black);
                    Form1_Paint(new object(), paintEA);
                    break;

                case "2":
                    //變數設置
                    hh = 0;
                    mm = 0;
                    ss = 0;
                    listOrder = 0;

                    //畫面設置
                    rect = new Rectangle(0, 10, 200, 170);
                    font = new Font("標楷體", 16);
                    brush = new SolidBrush(Color.White);

                    startButton = new Button()
                    {
                        Location = new Point(5, 50),
                        Text = "開始",
                        UseVisualStyleBackColor = true
                    };
                    startButton.Click += new System.EventHandler(startButton_Click);

                    splitTimeButton = new Button
                    {
                        Location = new System.Drawing.Point(90, 50),
                        Text = "分圈",
                        Enabled = false,
                        UseVisualStyleBackColor = true
                    };
                    splitTimeButton.Click += new System.EventHandler(splitTimeButton_Click);

                    listBox1 = new ListBox
                    {
                        FormattingEnabled = true,
                        Location = new System.Drawing.Point(5, 90),
                    };
                    Form1_Paint(new object(), paintEA);
                    this.Controls.Add(startButton);
                    this.Controls.Add(splitTimeButton);
                    this.Controls.Add(listBox1);
                    break;
                case "3":
                    //變數設置

                    //畫面設置
                    rect = new Rectangle(0, 10, 200, 170);
                    font = new Font("標楷體", 16);
                    brush = new SolidBrush(Color.White);
                    
                    startButton = new Button
                    {
                        Location = new Point(5,50),
                        Text = "開始",
                        UseVisualStyleBackColor = true
                    };
                    startButton.Click += new System.EventHandler(startButton_Click);

                    stopButton = new Button
                    {
                        Location = new Point(90, 50),
                        Text = "暫停",
                        Enabled = false,
                        UseVisualStyleBackColor = true
                    };
                    stopButton.Click += new System.EventHandler(stopButton_Click);

                    
                    hourCombox = new ComboBox
                    {
                        FormattingEnabled = true,
                        Location = new Point(20, 10),
                        Size = new System.Drawing.Size(50, 15),
                        Text = "0"
            };
                    //選擇範圍控制
                    hourCombox.TextChanged += new System.EventHandler(hourComboBox_TextChange);
                    
                    for (int hour = 0; hour <= 24; hour++)
                    {
                        hourCombox.Items.Add(hour.ToString());
                    }
                    
                    minCombox = new ComboBox
                    {
                        
                        FormattingEnabled = true,
                        Location = new Point(95, 10),
                        Text = "1",
                        Size = new System.Drawing.Size(50, 15)
            };
                    minCombox.TextChanged += new System.EventHandler(minComboBox_TextChange);

                    for (int min = 1; min <= 60; min++)
                    {
                        minCombox.Items.Add(min.ToString());
                    }

                    this.Controls.Add(hourCombox);
                    this.Controls.Add(minCombox);
                    this.Controls.Add(startButton);
                    this.Controls.Add(stopButton);
                    Form1_Paint(new object(), paintEA);
                    

                    break;
            }

        }

    }
}
