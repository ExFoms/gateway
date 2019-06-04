using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;


namespace WindowsFormsApplication3
{

    public class clsVisualControls
    {
        // ------------- Colors
        public static Color clrText = Color.FromArgb(210, 201, 235);
        public static Color clrBorder = Color.FromArgb(130, 130, 155);
        // Color Ping
        public static Color clrPingEnable = Color.FromArgb(0, 90, 45);
        public static Color clrPingDisable = Color.Red;
        public static Color clrPingTest = Color.DarkGray;
        public static Color clrPingDeactive = Color.Gray;
        public static Color clrPingBusy = Color.FromArgb(205, 205, 142);
        // Color Alert text
        public static Color clrAlert = Color.Black;
        public static Color clrAlertInfo = Color.Yellow;
        public static Color clrPingAttantion = Color.Red;
        // Color Panel
        public static Color clrPanelDisable = SystemColors.ControlLight;
        public static Color clrPanelTest = Color.FromArgb(30, 30, 0);

        public static Color colorBrightness(Color color, float factor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;
            if(factor <0)
            {
                factor = 1 + factor;
                red *= factor;
                green *= factor;
                blue *= factor;
            }
            else 
            {
                red = (255 - red) * factor + red;
                green = (255 - green) * factor + green;
                blue = (255 - blue) * factor + blue;
            }
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }
        
    }


    public static class ControlExtentions
    {
        public static void DoubleBuffering(this Control control, bool enable)
        {
            var metod = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            metod.Invoke(control, new object[] { ControlStyles.OptimizedDoubleBuffer, enable });
        }
    }
    public class clsListBox : System.Windows.Forms.ListBox
    {
        private bool mShowScroll;
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!mShowScroll)
                    cp.Style = cp.Style & ~0x200000;
                return cp;
            }
        }
        public bool ShowScrollbar
        {
            get { return mShowScroll; }
            set
            {
                if (value != mShowScroll)
                {
                    mShowScroll = value;
                    if (IsHandleCreated)
                        RecreateHandle();
                }
            }
        }
    }

    public class clsPanelReglament
    {
        public FlowLayoutPanel pnlMain = new FlowLayoutPanel();
        FlowLayoutPanel pnlLeft = new FlowLayoutPanel();
        FlowLayoutPanel pnlMiddle = new FlowLayoutPanel();
        FlowLayoutPanel pnlRight = new FlowLayoutPanel();
        public int reglament_id;
        public int shedule_id;
        Panel pnlReglament_BottomLine = new Panel();
        LinkLabel 
            lnklblRegion = new LinkLabel(),
            lnklblActive = new LinkLabel(),
            lnklblReglament = new LinkLabel(),
            lnklblShedule = new LinkLabel(),
            lnklblPeriod = new LinkLabel(),
            lnklblTimer = new LinkLabel(),
            lnklblGate_nametask = new LinkLabel();
        bool 
            disposed = false,
            active = false;

        public clsPanelReglament(int reglament_id_, int shedule_id_,
            int parent_,
            string region_,
            string reglament_,
            string shedule_,
            int period_,
            string gate_nametask_,
            bool active_)
        {
            reglament_id = reglament_id_;
            shedule_id = shedule_id_;
            active = active_;
            int move = (parent_ == 0) ? 0:50;

            //Основная панель
            //pnlMain.Visible = true;
            pnlMain.Size = new Size(300, 50);
            pnlMain.AutoSize = true;
            //pnlMain.BackColor = Color.Blue;
            pnlMain.RightToLeft = RightToLeft.No;

            //Левая панель
            //pnlLeft.Visible = true;
            pnlLeft.Margin = new Padding(0, 0, 0, 0);
            pnlLeft.Size = new Size(50, 50);
            //pnlLeft.BackColor = Color.Green;
            pnlLeft.Parent = pnlMain;
            //Центральная панель флоу
            pnlMiddle.Margin = new Padding(0, 0, 0, 0);
            pnlMiddle.AutoSize = true;
            pnlMiddle.FlowDirection = FlowDirection.TopDown;
            //pnlMiddle.BackColor = Color.Red;
            pnlMiddle.Parent = pnlMain;
            //правая панель флоу
            //pnlRight.Visible = true;
            pnlRight.Margin = new Padding(0, 0, 0, 0);
            pnlRight.Size = new Size(215, 50);
            //pnlRight.BackColor = Color.Yellow;
            pnlRight.Parent = pnlMain;


            //Shedule
            lnklblShedule.Size = new Size(210, 15);
            lnklblShedule.Location = new System.Drawing.Point(0, 0);
            lnklblShedule.Font = (active) ? new Font("Arial", 8F, FontStyle.Bold) : new Font("Arial", 8F, FontStyle.Italic);
            //lnklblShedule.AutoSize = false;
            lnklblShedule.AutoEllipsis = true;
            lnklblShedule.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            lnklblShedule.Text = shedule_;
            lnklblShedule.BackColor = System.Drawing.Color.Transparent;
            lnklblShedule.LinkColor = (active) ? clsVisualControls.clrText : clsVisualControls.colorBrightness(clsVisualControls.clrText, (float)-0.3);
            new ToolTip().SetToolTip(lnklblShedule, shedule_);
            lnklblShedule.Parent = pnlRight;
            //Gate_nametask
            lnklblGate_nametask.Size = new Size(210, 15);
            lnklblGate_nametask.Location = new System.Drawing.Point(0, 20);
            lnklblGate_nametask.Font = (active) ? new Font("Arial", 8F) : new Font("Arial", 8F, FontStyle.Italic);
            lnklblGate_nametask.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            lnklblGate_nametask.LinkColor = clsVisualControls.clrText; 
            lnklblGate_nametask.Text = gate_nametask_;
            //lnklblGate_nametask.ForeColor = SystemColors.HighlightText;
            lnklblGate_nametask.BackColor = System.Drawing.Color.Transparent;
            lnklblGate_nametask.LinkColor = (active) ? 
                clsVisualControls.colorBrightness(clsVisualControls.clrText, (float)-0.2) : 
                clsVisualControls.colorBrightness(clsVisualControls.clrText, (float)-0.3);
            lnklblGate_nametask.Location = new System.Drawing.Point(pnlMain.Size.Width - lnklblGate_nametask.Size.Width, 0);
            new ToolTip().SetToolTip(lnklblGate_nametask, reglament_);
            lnklblGate_nametask.Parent = pnlRight;

            //Region
            lnklblRegion.Size = new Size(210, 15);
            //lnklblRegion.AutoSize = false;
            lnklblRegion.AutoEllipsis = true;
            lnklblRegion.Location = new System.Drawing.Point(0, 40);
            lnklblRegion.Font = (active) ? new Font("Arial", 8F) : new Font("Arial", 8F, FontStyle.Italic);
            lnklblRegion.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            lnklblRegion.Text = region_;
            //lnklblRegion.ForeColor = SystemColors.HighlightText;
            lnklblRegion.BackColor = System.Drawing.Color.Transparent;
            lnklblRegion.LinkColor = (active) ?
                clsVisualControls.colorBrightness(clsVisualControls.clrText, (float)-0.2) :
                clsVisualControls.colorBrightness(clsVisualControls.clrText, (float)-0.3);
            lnklblRegion.Parent = pnlRight;

            /*//Active
            lnklblActive.Text = "g";
            lnklblActive.Location = new System.Drawing.Point(0 + move, 25);
            pnlRight.Margin = new Padding(110, 116, 120, 0);
            lnklblActive.AutoSize = true;
            lnklblActive.Font = new System.Drawing.Font("Webdings", 7);
            lnklblActive.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            lnklblActive.ForeColor = SystemColors.HighlightText;
            lnklblActive.BackColor = System.Drawing.Color.Transparent;
            lnklblActive.LinkColor = (active) ? clsVisualControls.clrPingEnable : clsVisualControls.clrPingDisable;
            lnklblActive.ActiveLinkColor = Color.White;
            new ToolTip().SetToolTip(lnklblActive, "Активизировано или нет");
            lnklblActive.Parent = pnlMiddle;
            //listLabels[id].Click += new System.EventHandler(ReversEnable_Pings);
             * */
            //Period
            if (parent_ == 0)
            {
                lnklblPeriod.Text = "   ";
                lnklblPeriod.Location = new System.Drawing.Point(0, 0);
                lnklblPeriod.BackColor = System.Drawing.Color.Transparent;
                lnklblPeriod.LinkColor = clsVisualControls.colorBrightness(clsVisualControls.clrText, (float)-0.3);
                lnklblPeriod.Font = new Font("Arial", 8F);
                lnklblPeriod.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                lnklblPeriod.Text = TimeSpan.FromSeconds(period_).ToString();
                lnklblPeriod.Parent = pnlLeft;
                //listLabels[id].Click += new System.EventHandler(ReversEnable_Pings);
            }

            //Timer
            if (parent_ == 0)
            {
                lnklblTimer.Text = "   ";
                //lnklblPeriod.Location = new System.Drawing.Point(20 + move, 25);
                lnklblTimer.Size = new Size(20, 20);
                lnklblTimer.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                lnklblTimer.Image = (active) ? Pictures.clock_time_enable : Pictures.clock_time18;
                if (active)
                    new ToolTip().SetToolTip(lnklblTimer, "Работает");
                else
                    new ToolTip().SetToolTip(lnklblTimer, "Отключен");
                lnklblTimer.Text = "   ";
                lnklblTimer.Parent = pnlMiddle;
                //listLabels[id].Click += new System.EventHandler(ReversEnable_Pings);
            }

            //Добавляем разделительную полосу
            /*Panel pnl = new Panel();
            pnl.Size = new Size(pnlMain.Size.Width, 1);
            pnl.Location = new System.Drawing.Point(0 + move, 20);
            pnl.BackColor = (parent_ == 0) ? clsVisualControls.clrBorder : clsVisualControls.colorBrightness(clsVisualControls.clrBorder, (float)-0.7);
            //pnl.Dock = DockStyle.Bottom;
            pnl.Parent = pnlMain;
             * */
        }
        
        ~clsPanelReglament()
        {
            this.Dispose(false);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //Удаляем элементы
                    pnlMain.Dispose();
                    pnlReglament_BottomLine.Dispose();
                    lnklblRegion.Dispose();
                    lnklblReglament.Dispose();
                    lnklblShedule.Dispose();
                    lnklblPeriod.Dispose();
                }
            }
            disposed = true;
        }
    }


/*
        private void GateMessage_Show(int danger, DateTime time, string name, string comment)
        {
            GateDialog form = new GateDialog();
            form.Size = new Size(400, 200);
            form.Opacity = .95;
            form.FormBorderStyle = FormBorderStyle.None;
            form.pnlHeader = new Panel()
            {
                Location = new Point(0, 0),
                Height = 30,
                Width = 400,
                BackColor = log[danger].HeaderBackColor
            };
            form.pnlHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(form.MouseDown);
            form.pnlHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(form.MouseMove);
            form.Controls.Add(form.pnlHeader);
            form.pnlHeader.Controls.Add(new Label()
            {
                Text = "Gateway",
                ForeColor = Color.Black,
                Location = new Point(10, 10),
                AutoSize = true
            });
            form.pnlHeader.Controls.Add(new Label()
            {
                Text = log[danger].HeaderText,
                ForeColor = log[danger].HeaderTextColor,
                //Font = new Font(this.Font, FontStyle.Bold),
                Location = new Point(70, 10),
                AutoSize = true
            });

            form.Controls.Add(new Label() { Text = time.ToString(), AutoSize = true, ForeColor = Color.Black, Location = new Point(30, 60) });
            form.Controls.Add(new Label() { Text = name, ForeColor = Color.Black, Location = new Point(30, 90), AutoSize = true, MaximumSize = new System.Drawing.Size(300, 0) });
            form.Controls.Add(new Label() { Text = comment, ForeColor = Color.Black, Location = new Point(30, 120), AutoSize = true, MaximumSize = new System.Drawing.Size(300, 0) });

            form.llblExit = new LinkLabel()
            {
                Text = "Закрыть",
                Location = new Point(form.Width - 100, form.Height - 40),
                AutoSize = true,
                LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline,
                ForeColor = Color.Black,
                BackColor = Color.LightGray,
                LinkColor = Color.Black,
                ActiveLinkColor = Color.White,
                Padding = new Padding(10, 5, 10, 5)
            };
            form.Controls.Add(form.llblExit);
            form.llblExit.Click += new System.EventHandler(form.ExitButtonClick);
            form.llblExit.MouseEnter += new System.EventHandler(form.lnk_MouseEnter);
            form.llblExit.MouseLeave += new System.EventHandler(form.lnk_MouseLeave);
            if (log[danger].Timeout > 0)
            {
                form.timer_Dialog = new Timer() { Interval = log[danger].Timeout, Enabled = true };
                form.timer_Dialog.Tick += new System.EventHandler(form.ExitButtonClick);
            }
            form.Show();
        }

    }
 */


}
