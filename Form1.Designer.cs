namespace WindowsFormsApplication3
{
    partial class frmGateway
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGateway));
            this.timer_clearMemore = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.развернутьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.check_StatusLogining = new System.Windows.Forms.Timer(this.components);
            this.lnkStartBeats = new System.Windows.Forms.LinkLabel();
            this.timer_ping = new System.Windows.Forms.Timer(this.components);
            this.pnlPing = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblIdentificationSynch = new System.Windows.Forms.Label();
            this.lnkIdentificationSynch = new System.Windows.Forms.LinkLabel();
            this.linkLabel8 = new System.Windows.Forms.LinkLabel();
            this.lnklblReglamentRoll = new System.Windows.Forms.LinkLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCountBeats = new System.Windows.Forms.Label();
            this.lnkClear_log = new System.Windows.Forms.LinkLabel();
            this.fllpnl1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSetTest = new System.Windows.Forms.LinkLabel();
            this.lnklblRefreshReglament = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.label18 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblTimeShedule = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer_Step_TransferFiles = new System.Windows.Forms.Timer(this.components);
            this.lstbxBeats = new System.Windows.Forms.ListBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.refresh_Listboxs = new System.Windows.Forms.Timer(this.components);
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.lnkMinimizer = new System.Windows.Forms.LinkLabel();
            this.lnkRoll = new System.Windows.Forms.LinkLabel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.fpnlReglament = new System.Windows.Forms.FlowLayoutPanel();
            this.timer_RefreshReglament = new System.Windows.Forms.Timer(this.components);
            this.timer_Logining = new System.Windows.Forms.Timer(this.components);
            this.timer_showBeats = new System.Windows.Forms.Timer(this.components);
            this.timer_Finishing = new System.Windows.Forms.Timer(this.components);
            this.timer_sync_identification = new System.Windows.Forms.Timer(this.components);
            this.panel6 = new System.Windows.Forms.Panel();
            this.lstbxLog = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.pnlPing.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.fllpnl1.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer_clearMemore
            // 
            this.timer_clearMemore.Interval = 10000;
            this.timer_clearMemore.Tick += new System.EventHandler(this.Clear_Memory);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.развернутьToolStripMenuItem,
            this.toolStripMenuItem1,
            this.выходToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(136, 54);
            // 
            // развернутьToolStripMenuItem
            // 
            this.развернутьToolStripMenuItem.Name = "развернутьToolStripMenuItem";
            this.развернутьToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.развернутьToolStripMenuItem.Text = "Развернуть";
            this.развернутьToolStripMenuItem.Click += new System.EventHandler(this.развернутьToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(132, 6);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // check_StatusLogining
            // 
            this.check_StatusLogining.Interval = 500;
            this.check_StatusLogining.Tick += new System.EventHandler(this.TestStatus_Pings);
            // 
            // lnkStartBeats
            // 
            this.lnkStartBeats.ActiveLinkColor = System.Drawing.Color.White;
            this.lnkStartBeats.AutoSize = true;
            this.lnkStartBeats.BackColor = System.Drawing.Color.Transparent;
            this.lnkStartBeats.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lnkStartBeats.Font = new System.Drawing.Font("Wingdings 3", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.lnkStartBeats.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lnkStartBeats.LinkArea = new System.Windows.Forms.LinkArea(0, 1);
            this.lnkStartBeats.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkStartBeats.LinkColor = System.Drawing.Color.Red;
            this.lnkStartBeats.Location = new System.Drawing.Point(3, 0);
            this.lnkStartBeats.Name = "lnkStartBeats";
            this.lnkStartBeats.Size = new System.Drawing.Size(22, 19);
            this.lnkStartBeats.TabIndex = 28;
            this.lnkStartBeats.TabStop = true;
            this.lnkStartBeats.Text = "w";
            this.lnkStartBeats.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkStartBeats_LinkClicked);
            this.lnkStartBeats.MouseEnter += new System.EventHandler(this.lnk_MouseEnter);
            this.lnkStartBeats.MouseLeave += new System.EventHandler(this.lnk_MouseLeave);
            // 
            // timer_ping
            // 
            this.timer_ping.Interval = 5000;
            this.timer_ping.Tick += new System.EventHandler(this.Check_Pings);
            // 
            // pnlPing
            // 
            this.pnlPing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPing.AutoSize = true;
            this.pnlPing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.pnlPing.Controls.Add(this.lblHeader);
            this.pnlPing.Controls.Add(this.panel1);
            this.pnlPing.Controls.Add(this.lstbxLog);
            this.pnlPing.Controls.Add(this.linkLabel4);
            this.pnlPing.Controls.Add(this.linkLabel1);
            this.pnlPing.Controls.Add(this.fpnlReglament);
            this.pnlPing.Controls.Add(this.flowLayoutPanel1);
            this.pnlPing.Controls.Add(this.flowLayoutPanel2);
            this.pnlPing.Controls.Add(this.lnklblReglamentRoll);
            this.pnlPing.Controls.Add(this.lblCountBeats);
            this.pnlPing.Controls.Add(this.lnkClear_log);
            this.pnlPing.Controls.Add(this.label18);
            this.pnlPing.Controls.Add(this.label10);
            this.pnlPing.Controls.Add(this.lblTimeShedule);
            this.pnlPing.Controls.Add(this.button4);
            this.pnlPing.Controls.Add(this.fllpnl1);
            this.pnlPing.Controls.Add(this.button3);
            this.pnlPing.Controls.Add(this.label22);
            this.pnlPing.Controls.Add(this.label1);
            this.pnlPing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.23F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pnlPing.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pnlPing.Location = new System.Drawing.Point(0, 29);
            this.pnlPing.MinimumSize = new System.Drawing.Size(50, 25);
            this.pnlPing.Name = "pnlPing";
            this.pnlPing.Size = new System.Drawing.Size(1763, 826);
            this.pnlPing.TabIndex = 29;
            // 
            // linkLabel1
            // 
            this.linkLabel1.ActiveLinkColor = System.Drawing.Color.White;
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.linkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(0, 20);
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.linkLabel1.Location = new System.Drawing.Point(109, 86);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(80, 19);
            this.linkLabel1.TabIndex = 54;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "РЕГЛАМЕНТ";
            this.linkLabel1.UseCompatibleTextRendering = true;
            // 
            // linkLabel4
            // 
            this.linkLabel4.ActiveLinkColor = System.Drawing.Color.White;
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel4.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel4.Font = new System.Drawing.Font("Wingdings 2", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.linkLabel4.ForeColor = System.Drawing.SystemColors.Highlight;
            this.linkLabel4.Image = global::WindowsFormsApplication3.Pictures.file25_yellow;
            this.linkLabel4.LinkArea = new System.Windows.Forms.LinkArea(0, 1);
            this.linkLabel4.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel4.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.linkLabel4.Location = new System.Drawing.Point(364, 15);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(16, 24);
            this.linkLabel4.TabIndex = 50;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = " ";
            this.linkLabel4.Visible = false;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel2.Controls.Add(this.lblIdentificationSynch);
            this.flowLayoutPanel2.Controls.Add(this.lnkIdentificationSynch);
            this.flowLayoutPanel2.Controls.Add(this.linkLabel8);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(19, 39);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(146, 25);
            this.flowLayoutPanel2.TabIndex = 53;
            // 
            // lblIdentificationSynch
            // 
            this.lblIdentificationSynch.AutoSize = true;
            this.lblIdentificationSynch.Location = new System.Drawing.Point(3, 0);
            this.lblIdentificationSynch.Name = "lblIdentificationSynch";
            this.lblIdentificationSynch.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblIdentificationSynch.Size = new System.Drawing.Size(86, 18);
            this.lblIdentificationSynch.TabIndex = 53;
            this.lblIdentificationSynch.Text = "идентификация";
            // 
            // lnkIdentificationSynch
            // 
            this.lnkIdentificationSynch.ActiveLinkColor = System.Drawing.Color.White;
            this.lnkIdentificationSynch.AutoSize = true;
            this.lnkIdentificationSynch.BackColor = System.Drawing.Color.Transparent;
            this.lnkIdentificationSynch.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lnkIdentificationSynch.Font = new System.Drawing.Font("Wingdings 3", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.lnkIdentificationSynch.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lnkIdentificationSynch.LinkArea = new System.Windows.Forms.LinkArea(0, 1);
            this.lnkIdentificationSynch.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkIdentificationSynch.LinkColor = System.Drawing.Color.Red;
            this.lnkIdentificationSynch.Location = new System.Drawing.Point(95, 0);
            this.lnkIdentificationSynch.Name = "lnkIdentificationSynch";
            this.lnkIdentificationSynch.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lnkIdentificationSynch.Size = new System.Drawing.Size(22, 22);
            this.lnkIdentificationSynch.TabIndex = 54;
            this.lnkIdentificationSynch.TabStop = true;
            this.lnkIdentificationSynch.Text = "w";
            this.lnkIdentificationSynch.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel2_LinkClicked);
            // 
            // linkLabel8
            // 
            this.linkLabel8.ActiveLinkColor = System.Drawing.Color.White;
            this.linkLabel8.AutoSize = true;
            this.linkLabel8.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel8.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel8.Font = new System.Drawing.Font("Wingdings 3", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.linkLabel8.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.linkLabel8.Image = global::WindowsFormsApplication3.Pictures.reload_refresh25;
            this.linkLabel8.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel8.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.linkLabel8.Location = new System.Drawing.Point(123, 3);
            this.linkLabel8.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.linkLabel8.Name = "linkLabel8";
            this.linkLabel8.Size = new System.Drawing.Size(17, 19);
            this.linkLabel8.TabIndex = 50;
            this.linkLabel8.TabStop = true;
            this.linkLabel8.Text = " ";
            this.linkLabel8.UseCompatibleTextRendering = true;
            this.linkLabel8.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel8_LinkClicked);
            // 
            // lnklblReglamentRoll
            // 
            this.lnklblReglamentRoll.ActiveLinkColor = System.Drawing.Color.White;
            this.lnklblReglamentRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnklblReglamentRoll.AutoSize = true;
            this.lnklblReglamentRoll.BackColor = System.Drawing.Color.Transparent;
            this.lnklblReglamentRoll.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lnklblReglamentRoll.Font = new System.Drawing.Font("Webdings", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.lnklblReglamentRoll.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lnklblReglamentRoll.LinkArea = new System.Windows.Forms.LinkArea(0, 2);
            this.lnklblReglamentRoll.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnklblReglamentRoll.LinkColor = System.Drawing.SystemColors.ControlLight;
            this.lnklblReglamentRoll.Location = new System.Drawing.Point(431, 86);
            this.lnklblReglamentRoll.Name = "lnklblReglamentRoll";
            this.lnklblReglamentRoll.Size = new System.Drawing.Size(20, 19);
            this.lnklblReglamentRoll.TabIndex = 48;
            this.lnklblReglamentRoll.TabStop = true;
            this.lnklblReglamentRoll.Text = "5";
            this.lnklblReglamentRoll.UseCompatibleTextRendering = true;
            this.lnklblReglamentRoll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblReglamentRoll_LinkClicked);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.lnkStartBeats);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(537, 86);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(28, 19);
            this.flowLayoutPanel1.TabIndex = 53;
            // 
            // lblCountBeats
            // 
            this.lblCountBeats.AutoSize = true;
            this.lblCountBeats.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCountBeats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lblCountBeats.Location = new System.Drawing.Point(708, 87);
            this.lblCountBeats.Name = "lblCountBeats";
            this.lblCountBeats.Size = new System.Drawing.Size(50, 15);
            this.lblCountBeats.TabIndex = 44;
            this.lblCountBeats.Text = "(00000)";
            // 
            // lnkClear_log
            // 
            this.lnkClear_log.ActiveLinkColor = System.Drawing.Color.White;
            this.lnkClear_log.AutoSize = true;
            this.lnkClear_log.BackColor = System.Drawing.Color.Transparent;
            this.lnkClear_log.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lnkClear_log.Font = new System.Drawing.Font("Wingdings 2", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.lnkClear_log.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lnkClear_log.Image = global::WindowsFormsApplication3.Pictures.garbage_trash25;
            this.lnkClear_log.LinkArea = new System.Windows.Forms.LinkArea(0, 1);
            this.lnkClear_log.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkClear_log.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.lnkClear_log.Location = new System.Drawing.Point(841, 81);
            this.lnkClear_log.Name = "lnkClear_log";
            this.lnkClear_log.Size = new System.Drawing.Size(16, 24);
            this.lnkClear_log.TabIndex = 43;
            this.lnkClear_log.TabStop = true;
            this.lnkClear_log.Text = " ";
            this.lnkClear_log.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClear_log_LinkClicked);
            // 
            // fllpnl1
            // 
            this.fllpnl1.AutoSize = true;
            this.fllpnl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fllpnl1.BackColor = System.Drawing.Color.Transparent;
            this.fllpnl1.Controls.Add(this.lblSetTest);
            this.fllpnl1.Controls.Add(this.lnklblRefreshReglament);
            this.fllpnl1.Controls.Add(this.linkLabel3);
            this.fllpnl1.Location = new System.Drawing.Point(190, 80);
            this.fllpnl1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.fllpnl1.Name = "fllpnl1";
            this.fllpnl1.Size = new System.Drawing.Size(69, 25);
            this.fllpnl1.TabIndex = 45;
            // 
            // lblSetTest
            // 
            this.lblSetTest.ActiveLinkColor = System.Drawing.Color.White;
            this.lblSetTest.AutoSize = true;
            this.lblSetTest.BackColor = System.Drawing.Color.Transparent;
            this.lblSetTest.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSetTest.Font = new System.Drawing.Font("Wingdings 3", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.lblSetTest.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblSetTest.Image = global::WindowsFormsApplication3.Pictures.file25_yellow;
            this.lblSetTest.LinkArea = new System.Windows.Forms.LinkArea(0, 2);
            this.lblSetTest.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lblSetTest.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.lblSetTest.Location = new System.Drawing.Point(3, 3);
            this.lblSetTest.Margin = new System.Windows.Forms.Padding(3);
            this.lblSetTest.Name = "lblSetTest";
            this.lblSetTest.Size = new System.Drawing.Size(17, 19);
            this.lblSetTest.TabIndex = 52;
            this.lblSetTest.TabStop = true;
            this.lblSetTest.Text = " ";
            this.lblSetTest.UseCompatibleTextRendering = true;
            this.lblSetTest.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSetTest_LinkClicked);
            // 
            // lnklblRefreshReglament
            // 
            this.lnklblRefreshReglament.ActiveLinkColor = System.Drawing.Color.White;
            this.lnklblRefreshReglament.AutoSize = true;
            this.lnklblRefreshReglament.BackColor = System.Drawing.Color.Transparent;
            this.lnklblRefreshReglament.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lnklblRefreshReglament.Font = new System.Drawing.Font("Wingdings 3", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.lnklblRefreshReglament.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lnklblRefreshReglament.Image = global::WindowsFormsApplication3.Pictures.reload_refresh25;
            this.lnklblRefreshReglament.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnklblRefreshReglament.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.lnklblRefreshReglament.Location = new System.Drawing.Point(26, 3);
            this.lnklblRefreshReglament.Margin = new System.Windows.Forms.Padding(3);
            this.lnklblRefreshReglament.Name = "lnklblRefreshReglament";
            this.lnklblRefreshReglament.Size = new System.Drawing.Size(17, 19);
            this.lnklblRefreshReglament.TabIndex = 50;
            this.lnklblRefreshReglament.TabStop = true;
            this.lnklblRefreshReglament.Text = " ";
            this.lnklblRefreshReglament.UseCompatibleTextRendering = true;
            this.lnklblRefreshReglament.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblRefreshReglament_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.ActiveLinkColor = System.Drawing.Color.White;
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel3.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel3.Enabled = false;
            this.linkLabel3.Font = new System.Drawing.Font("Wingdings 3", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.linkLabel3.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.linkLabel3.Image = global::WindowsFormsApplication3.Pictures.invoice_document_file25;
            this.linkLabel3.LinkArea = new System.Windows.Forms.LinkArea(0, 2);
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel3.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.linkLabel3.Location = new System.Drawing.Point(49, 3);
            this.linkLabel3.Margin = new System.Windows.Forms.Padding(3);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(17, 19);
            this.linkLabel3.TabIndex = 51;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = " ";
            this.linkLabel3.UseCompatibleTextRendering = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(24, 26);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(59, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "транспорт";
            this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "соединения";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblTimeShedule
            // 
            this.lblTimeShedule.AutoSize = true;
            this.lblTimeShedule.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lblTimeShedule.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lblTimeShedule.Location = new System.Drawing.Point(574, 87);
            this.lblTimeShedule.Name = "lblTimeShedule";
            this.lblTimeShedule.Size = new System.Drawing.Size(55, 15);
            this.lblTimeShedule.TabIndex = 39;
            this.lblTimeShedule.Text = "00:00:00";
            // 
            // button4
            // 
            this.button4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(120)))));
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(70)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.button4.Location = new System.Drawing.Point(1155, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(167, 23);
            this.button4.TabIndex = 19;
            this.button4.Text = "Отчет по направлениям";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(120)))));
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(70)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.button3.Location = new System.Drawing.Point(950, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(167, 23);
            this.button3.TabIndex = 19;
            this.button3.Text = "Отчет по прикреплению";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.label22.Location = new System.Drawing.Point(804, 87);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(31, 15);
            this.label22.TabIndex = 38;
            this.label22.Text = "ЛОГ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.label1.Location = new System.Drawing.Point(476, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 36;
            this.label1.Text = "ЗАДАЧИ";
            // 
            // timer_Step_TransferFiles
            // 
            this.timer_Step_TransferFiles.Interval = 5000;
            this.timer_Step_TransferFiles.Tick += new System.EventHandler(this.Transfer_Files);
            // 
            // lstbxBeats
            // 
            this.lstbxBeats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.lstbxBeats.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstbxBeats.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstbxBeats.Enabled = false;
            this.lstbxBeats.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lstbxBeats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lstbxBeats.FormattingEnabled = true;
            this.lstbxBeats.Location = new System.Drawing.Point(479, 141);
            this.lstbxBeats.Name = "lstbxBeats";
            this.lstbxBeats.Size = new System.Drawing.Size(302, 715);
            this.lstbxBeats.TabIndex = 33;
            this.lstbxBeats.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(130)))), ((int)(((byte)(155)))));
            this.panel7.Location = new System.Drawing.Point(457, 115);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(10, 736);
            this.panel7.TabIndex = 35;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // refresh_Listboxs
            // 
            this.refresh_Listboxs.Interval = 1000;
            this.refresh_Listboxs.Tick += new System.EventHandler(this.refresh_Listboxs_Tick);
            // 
            // linkLabel5
            // 
            this.linkLabel5.ActiveLinkColor = System.Drawing.Color.White;
            this.linkLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel5.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel5.Font = new System.Drawing.Font("Webdings", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.linkLabel5.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.linkLabel5.LinkArea = new System.Windows.Forms.LinkArea(0, 2);
            this.linkLabel5.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel5.LinkColor = System.Drawing.SystemColors.ControlLight;
            this.linkLabel5.Location = new System.Drawing.Point(1448, 6);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(20, 19);
            this.linkLabel5.TabIndex = 24;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "0";
            this.linkLabel5.UseCompatibleTextRendering = true;
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel5_LinkClicked);
            this.linkLabel5.MouseEnter += new System.EventHandler(this.lnk_MouseEnter);
            this.linkLabel5.MouseLeave += new System.EventHandler(this.lnk_MouseLeave);
            // 
            // linkLabel6
            // 
            this.linkLabel6.ActiveLinkColor = System.Drawing.Color.White;
            this.linkLabel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel6.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel6.Font = new System.Drawing.Font("Webdings", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.linkLabel6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.linkLabel6.LinkArea = new System.Windows.Forms.LinkArea(0, 2);
            this.linkLabel6.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel6.LinkColor = System.Drawing.SystemColors.ControlLight;
            this.linkLabel6.Location = new System.Drawing.Point(1468, 6);
            this.linkLabel6.Margin = new System.Windows.Forms.Padding(0);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(20, 19);
            this.linkLabel6.TabIndex = 25;
            this.linkLabel6.TabStop = true;
            this.linkLabel6.Text = "r";
            this.linkLabel6.UseCompatibleTextRendering = true;
            this.linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel6_LinkClicked);
            this.linkLabel6.MouseEnter += new System.EventHandler(this.lnk_MouseEnter);
            this.linkLabel6.MouseLeave += new System.EventHandler(this.lnk_MouseLeave);
            // 
            // lnkMinimizer
            // 
            this.lnkMinimizer.ActiveLinkColor = System.Drawing.Color.White;
            this.lnkMinimizer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkMinimizer.AutoSize = true;
            this.lnkMinimizer.BackColor = System.Drawing.Color.Transparent;
            this.lnkMinimizer.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lnkMinimizer.Font = new System.Drawing.Font("Webdings", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.lnkMinimizer.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lnkMinimizer.LinkArea = new System.Windows.Forms.LinkArea(0, 2);
            this.lnkMinimizer.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkMinimizer.LinkColor = System.Drawing.SystemColors.ControlLight;
            this.lnkMinimizer.Location = new System.Drawing.Point(1428, 6);
            this.lnkMinimizer.Name = "lnkMinimizer";
            this.lnkMinimizer.Size = new System.Drawing.Size(20, 19);
            this.lnkMinimizer.TabIndex = 26;
            this.lnkMinimizer.TabStop = true;
            this.lnkMinimizer.Text = "3";
            this.lnkMinimizer.UseCompatibleTextRendering = true;
            this.lnkMinimizer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel7_LinkClicked);
            this.lnkMinimizer.MouseEnter += new System.EventHandler(this.lnk_MouseEnter);
            this.lnkMinimizer.MouseLeave += new System.EventHandler(this.lnk_MouseLeave);
            // 
            // lnkRoll
            // 
            this.lnkRoll.ActiveLinkColor = System.Drawing.Color.White;
            this.lnkRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkRoll.AutoSize = true;
            this.lnkRoll.BackColor = System.Drawing.Color.Transparent;
            this.lnkRoll.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lnkRoll.Font = new System.Drawing.Font("Webdings", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(100)));
            this.lnkRoll.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lnkRoll.LinkArea = new System.Windows.Forms.LinkArea(0, 2);
            this.lnkRoll.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkRoll.LinkColor = System.Drawing.SystemColors.ControlLight;
            this.lnkRoll.Location = new System.Drawing.Point(1408, 6);
            this.lnkRoll.Name = "lnkRoll";
            this.lnkRoll.Size = new System.Drawing.Size(20, 19);
            this.lnkRoll.TabIndex = 27;
            this.lnkRoll.TabStop = true;
            this.lnkRoll.Text = "5";
            this.lnkRoll.UseCompatibleTextRendering = true;
            this.lnkRoll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRoll_LinkClicked);
            this.lnkRoll.MouseEnter += new System.EventHandler(this.lnk_MouseEnter);
            this.lnkRoll.MouseLeave += new System.EventHandler(this.lnk_MouseLeave);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lblHeader.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblHeader.Location = new System.Drawing.Point(1344, 808);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(44, 14);
            this.lblHeader.TabIndex = 30;
            this.lblHeader.Text = "версия";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel8_MouseDown);
            this.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel8_MouseMove);
            // 
            // pnlHeader
            // 
            this.pnlHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.pnlHeader.Controls.Add(this.label2);
            this.pnlHeader.Controls.Add(this.lnkRoll);
            this.pnlHeader.Controls.Add(this.lnkMinimizer);
            this.pnlHeader.Controls.Add(this.linkLabel6);
            this.pnlHeader.Controls.Add(this.linkLabel5);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1498, 29);
            this.pnlHeader.TabIndex = 22;
            this.pnlHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel8_MouseDown);
            this.pnlHeader.MouseEnter += new System.EventHandler(this.PnlHeader_MouseEnter);
            this.pnlHeader.MouseLeave += new System.EventHandler(this.PnlHeader_MouseLeave);
            this.pnlHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel8_MouseMove);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(55)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 868);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1498, 1);
            this.panel3.TabIndex = 42;
            // 
            // fpnlReglament
            // 
            this.fpnlReglament.AutoScroll = true;
            this.fpnlReglament.BackColor = System.Drawing.Color.Transparent;
            this.fpnlReglament.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.fpnlReglament.Location = new System.Drawing.Point(109, 108);
            this.fpnlReglament.Name = "fpnlReglament";
            this.fpnlReglament.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.fpnlReglament.Size = new System.Drawing.Size(339, 714);
            this.fpnlReglament.TabIndex = 0;
            // 
            // timer_RefreshReglament
            // 
            this.timer_RefreshReglament.Interval = 1500;
            this.timer_RefreshReglament.Tick += new System.EventHandler(this.timer_RefreshReglament_Tick);
            // 
            // timer_Logining
            // 
            this.timer_Logining.Interval = 1000;
            this.timer_Logining.Tick += new System.EventHandler(this.timer_Logining_Tick);
            // 
            // timer_showBeats
            // 
            this.timer_showBeats.Interval = 1000;
            this.timer_showBeats.Tick += new System.EventHandler(this.timer_showBeats_Tick);
            // 
            // timer_Finishing
            // 
            this.timer_Finishing.Interval = 1000;
            this.timer_Finishing.Tick += new System.EventHandler(this.timer_Finishing_Tick);
            // 
            // timer_sync_identification
            // 
            this.timer_sync_identification.Interval = 3000;
            this.timer_sync_identification.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(130)))), ((int)(((byte)(155)))));
            this.panel6.Location = new System.Drawing.Point(792, 115);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(3, 736);
            this.panel6.TabIndex = 34;
            // 
            // lstbxLog
            // 
            this.lstbxLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.lstbxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstbxLog.Font = new System.Drawing.Font("Arial", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lstbxLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lstbxLog.FormattingEnabled = true;
            this.lstbxLog.Location = new System.Drawing.Point(807, 108);
            this.lstbxLog.Name = "lstbxLog";
            this.lstbxLog.Size = new System.Drawing.Size(681, 663);
            this.lstbxLog.TabIndex = 55;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.panel1.Location = new System.Drawing.Point(12, 108);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(78, 714);
            this.panel1.TabIndex = 56;
            this.panel1.MouseEnter += new System.EventHandler(this.Panel1_MouseEnter);
            this.panel1.MouseLeave += new System.EventHandler(this.Panel1_MouseLeave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Location = new System.Drawing.Point(30, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 19);
            this.label2.TabIndex = 57;
            this.label2.Text = "GATEWAY";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // frmGateway
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(1498, 869);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.lstbxBeats);
            this.Controls.Add(this.pnlPing);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmGateway";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Шлюз СРЗ";
            this.TransparencyKey = System.Drawing.Color.BlueViolet;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.pnlPing.ResumeLayout(false);
            this.pnlPing.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.fllpnl1.ResumeLayout(false);
            this.fllpnl1.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer_clearMemore;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem развернутьToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.Timer check_StatusLogining;
        private System.Windows.Forms.LinkLabel lnkStartBeats;
        private System.Windows.Forms.Timer timer_ping;
        private System.Windows.Forms.Panel pnlPing;
        private System.Windows.Forms.Timer timer_Step_TransferFiles;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox lstbxBeats;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblTimeShedule;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer refresh_Listboxs;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.LinkLabel lnkMinimizer;
        private System.Windows.Forms.LinkLabel lnkRoll;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.LinkLabel lnkClear_log;
        private System.Windows.Forms.FlowLayoutPanel fpnlReglament;
        private System.Windows.Forms.Label lblCountBeats;
        private System.Windows.Forms.FlowLayoutPanel fllpnl1;
        private System.Windows.Forms.LinkLabel lnklblRefreshReglament;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel lnklblReglamentRoll;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.LinkLabel lblSetTest;
        private System.Windows.Forms.Timer timer_RefreshReglament;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer_Logining;
        private System.Windows.Forms.Timer timer_showBeats;
        private System.Windows.Forms.Timer timer_Finishing;
        private System.Windows.Forms.Timer timer_sync_identification;
        private System.Windows.Forms.LinkLabel lnkIdentificationSynch;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lblIdentificationSynch;
        private System.Windows.Forms.LinkLabel linkLabel8;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ListBox lstbxLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
    }
}

