using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization; //временно для теста
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using System.Drawing.Imaging;
//using System.Reflection;
using Npgsql;
//using System.Net.Security;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication3
{

    public partial class frmGateway : Form
    {
       
        #region Объявления переменных 
        public string ver = "0.1.5.0";
        // Form
        public int window_height_min = 112;//29
        public int window_height_max;
        public int window_width_min = 320;
        public int window_width_max;
        // Visual Controls
        clsListBox lstbxLog1;
        // Mouse 
        private int mouse_x = 0; private int mouse_y = 0;
        // Список LinkLabel, метки контроля пинга
        public List<LinkLabel> listLabels = new List<LinkLabel>();
        public List<clsPanelReglament> listPanelReglament = new List<clsPanelReglament>();
        public Queue<clsLog> logQueue = new Queue<clsLog>();
        public clsLogining Gate_Logining = new clsLogining("uid=sa;pwd=Wedfzx8!;server=SERVER-SHRK\\SQLEXPRESS;database=gate;");
        // контрольные точки
        public List<clsConnections> Gate_Connections = new List<clsConnections>();
        public List<clsTransport_files> Gate_Transport_files = new List<clsTransport_files>();
        public List<clsTransport_files_inpersonalfolder> Gate_Transport_files_inpersonalfolder = new List<clsTransport_files_inpersonalfolder>();
        public List<clstransport_files_email> Gate_Transport_files_email = new List<clstransport_files_email>();
        public List<clsConfigPrototype>[] connections_list = new List<clsConfigPrototype>[3];
        // Список расписания с задачами
        // инициализируются в frmGateway()
        public clsShedules shedules;
        //Передаем метод работы с Логом в класс
        //Делегаты Metod_Log объявлен в классе
        //
        public bool ready_finishing = false; //состояние готовности к завершению работы
        //Потоки
        public Thread thread_identification_synch;


        public delegate void Metod();
        #endregion

        public bool InitializeElements()
        {
            bool result = true;

            loadConfig();
            // Запускаем логирование
            timer_Logining.Enabled = true;
            connections_list = new List<clsConfigPrototype>[4]
            {
                new List<clsConfigPrototype>(Gate_Connections),
                new List<clsConfigPrototype>(Gate_Transport_files),
                new List<clsConfigPrototype>(Gate_Transport_files_inpersonalfolder),
                new List<clsConfigPrototype>(Gate_Transport_files_email)
            };
            shedules = new clsShedules(logQueue.Enqueue, ref Gate_Connections);
            lblSetTest_LinkClicked(null, null); //обновляем визуальные элементы по состоянию тест или раб

            timer_Step_TransferFiles.Enabled = true;
            check_StatusLogining.Enabled = true;
            refresh_Listboxs.Enabled = true;
            timer_showBeats.Enabled = true;
            return result;
        }

        public frmGateway()
        {
            InitializeComponent();
        }

        private void loadConfig()
        {
            int connectionType;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(Directory.GetCurrentDirectory() + @"\config.xml");
                if (!Gate_Logining.Server_connecting())
                    MessageBox.Show("Нет соединения с базой Лога!");
                logQueue.Enqueue(new clsLog(DateTime.Now, 9, "connections", 0, 0, DateTime.Now, DateTime.Now, "Получение файла настроек - connections"));
                //------------------ Connections
                int document = 0;
                foreach (XmlNode node in doc.DocumentElement["connections"])
                {
                    connectionType = String.IsNullOrEmpty(node["connectiontype"].InnerText) ? 0 : int.Parse(node["connectiontype"].InnerText);
                    Gate_Connections.Add(new clsConnections()
                    {
                        nrecord = String.IsNullOrEmpty(node["nrecord"].InnerText) ? 0 : int.Parse(node["nrecord"].InnerText),
                        name = node["name"].InnerText,
                        comment = node["comments"].InnerText
                    });
                    Gate_Connections[document].ping.connectionType = String.IsNullOrEmpty(node["connectiontype"].InnerText) ? 0 : int.Parse(node["connectiontype"].InnerText);
                    switch (connectionType)
                    {
                        case 1:
                            Gate_Connections[document].connectionString = node["connectionString"].InnerText.Replace(@"\\", @"\");
                            Gate_Connections[document].ping.ping_resource = node["serverIp"].InnerText;
                            break;
                        case 2:
                            List<string> folders_ = new List<string>();
                            foreach (XmlNode node_folder in node["connectionFolders"]) folders_.Add(@node_folder.InnerText);
                            if(folders_.Count > 0) Gate_Connections[document].ping.ping_resource = folders_.ToArray();
                            Gate_Connections[document].connectionString = node["connectionString"].InnerText;
                            break;
                        case 4:
                            Gate_Connections[document].ping.ping_resource = node["program"].InnerText;
                            Gate_Connections[document].restartInterval = String.IsNullOrEmpty(node["restartInterval"].InnerText) ? 0 : int.Parse(node["restartInterval"].InnerText);
                            if (Gate_Connections[document].restartInterval != 0) Gate_Connections[document].startRestart();
                            break;
                    }       
                   document++;
                }
                //------------------ Processes
                logQueue.Enqueue(new clsLog(DateTime.Now, 9, "transport_files", 0, 0, DateTime.Now, DateTime.Now, "Получение файла настроек"));
                List<string> masks_ = new List<string>();
                //string comment_;    
                document = 0;
                foreach (XmlNode node in doc.DocumentElement["processes"]["transport_files"])
                {
                    masks_.Clear(); foreach (XmlNode masks in node["masks"]) masks_.Add(@masks.InnerText);
                    Gate_Transport_files.Add(new clsTransport_files()
                    {
                        name = node["name"].InnerText,
                        tick = int.Parse(node["tick"].InnerText),
                        nrecord = int.Parse(node["nrecord"].InnerText),
                        comment = node["comment"].InnerText,
                        prefix = node["prefix"].InnerText,
                        masks = masks_.ToArray(),
                        rewrite = (node["rewrite"].InnerText == "true") ? true : false,
                        //metod_log = logQueue.Enqueue //передача ссылки на метод публикации Лога
                    });
                    Gate_Transport_files[document].ping.connectionType = 2;
                    Gate_Transport_files[document].ping.ping_resource = new string[] { @node["source"].InnerText, @node["destination"].InnerText };
                    logQueue.Enqueue(new clsLog(DateTime.Now, 9, "Gate_Transport_files", 0, 0, DateTime.Now, DateTime.Now, 
                        Gate_Transport_files[document].comment + " - " +
                        ((string[])Gate_Transport_files[document].ping.ping_resource)[0] + " | " + ((string[])Gate_Transport_files[document].ping.ping_resource)[1]));
                    document++;
                }

                logQueue.Enqueue(new clsLog(DateTime.Now, 9, "transport_files_inpersonalfolder", 0, 0, DateTime.Now, DateTime.Now, "Получение файла настроек"));
                List<string> recipients_ = new List<string>();
                document = 0;
                foreach (XmlNode node in doc.DocumentElement["processes"]["transport_files_inpersonalfolder"])
                {
                    masks_.Clear(); foreach (XmlNode masks in node["masks"]) masks_.Add(@masks.InnerText);
                    recipients_.Clear(); foreach (XmlNode recipients in node["recipients"]) recipients_.Add(recipients.InnerText);
                    Gate_Transport_files_inpersonalfolder.Add(new clsTransport_files_inpersonalfolder()
                    {
                        name = node["name"].InnerText,
                        tick = int.Parse(node["tick"].InnerText),
                        nrecord = int.Parse(node["nrecord"].InnerText),
                        comment = node["comment"].InnerText,
                        recipients = recipients_.ToArray(),
                        prefix = node["prefix"].InnerText,
                        masks = masks_.ToArray(),
                        rewrite = (node["rewrite"].InnerText == "true") ? true : false,
                        //connection_name = node["connection_name"].InnerText,
                        //metod_log = logQueue.Enqueue //передача ссылки на метод публикации Лога
                    });
                    Gate_Transport_files_inpersonalfolder[document].ping.connectionType = 2;
                    Gate_Transport_files_inpersonalfolder[document].ping.ping_resource = new string[] { @node["source"].InnerText, @node["destination"].InnerText };
                    logQueue.Enqueue(new clsLog(DateTime.Now, 9, "Gate_Transport_files", 0, 0, DateTime.Now, DateTime.Now,
                        Gate_Transport_files_inpersonalfolder[document].comment + " - " +
                        ((string[])Gate_Transport_files_inpersonalfolder[document].ping.ping_resource)[0] + " | " + ((string[])Gate_Transport_files_inpersonalfolder[document].ping.ping_resource)[1]));
                    document++;

                }

                logQueue.Enqueue(new clsLog(DateTime.Now, 9, "transport_files_email", 0, 0, DateTime.Now, DateTime.Now, "Получение файла настроек"));
                document = 0;
                foreach (XmlNode node in doc.DocumentElement["processes"]["transport_files_email"])
                {
                    Gate_Transport_files_email.Add(new clstransport_files_email()
                    {
                        name = node["name"].InnerText,
                        tick = int.Parse(node["tick"].InnerText),
                        nrecord = int.Parse(node["nrecord"].InnerText),
                        folder = node["source"].InnerText,
                        email = node["email"].InnerText,
                        caption = node["caption"].InnerText,
                        comment = node["comment"].InnerText,
                        //connection_name = node["connection_name"].InnerText,
                        //metod_log = logQueue.Enqueue //передача ссылки на метод публикации Лога
                    });
                    Gate_Transport_files_email[document].ping.connectionType = 3;
                    Gate_Transport_files_email[document].ping.ping_resource = new string[] { @node["source"].InnerText };
                    document++;
                }

                createLabel_Pings(); //
                timer_ping.Enabled = true;                
            }
            catch
            {
                logQueue.Enqueue(new clsLog(DateTime.Now, 2, "Gate", 0, 0, DateTime.Now, DateTime.Now, "Получение файла настроек - Ошибка!"));
            }
        }

        private void createLabel_Pings()
        {
            int id = 0; 
            int i = 0;
            foreach (var connection in Gate_Connections)
            {
                listLabels.Add(new System.Windows.Forms.LinkLabel());
                listLabels[id].Name = "lnkPing" + id.ToString();
                listLabels[id].Text = "g";
                listLabels[id].Location = new System.Drawing.Point(85 + i * 14, 10);
                listLabels[id].Visible = true;
                listLabels[id].AutoSize = true;
                listLabels[id].Font = new System.Drawing.Font("Webdings", 7);
                listLabels[id].LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                listLabels[id].ForeColor = SystemColors.HighlightText;
                listLabels[id].BackColor = System.Drawing.Color.Transparent;
                listLabels[id].LinkColor = clsVisualControls.clrPingDisable;
                listLabels[id].ActiveLinkColor = Color.White;
                new ToolTip().SetToolTip(listLabels[id], connection.comment);
                listLabels[id].Parent = pnlPing;
                listLabels[id].Click += new System.EventHandler(ReversEnable_Pings);
                pnlPing.Controls.Add(listLabels[id]);
                ++id;
                ++i;
            }
            //MessageBox.Show(id.ToString());
            i = 0;
            foreach (var connection in Gate_Transport_files)
            {
                listLabels.Add(new System.Windows.Forms.LinkLabel());
                listLabels[id].Name = "lnkPing" + id.ToString();
                listLabels[id].Text = "g";
                listLabels[id].Location = new System.Drawing.Point(85 + i * 14, 25);
                listLabels[id].Visible = true;
                listLabels[id].AutoSize = true;
                listLabels[id].Font = new System.Drawing.Font("Webdings", 7);
                listLabels[id].LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                listLabels[id].ForeColor = SystemColors.HighlightText;
                listLabels[id].BackColor = System.Drawing.Color.Transparent;
                listLabels[id].LinkColor = clsVisualControls.clrPingDisable;
                listLabels[id].ActiveLinkColor = Color.White;
                new ToolTip().SetToolTip(listLabels[id], connection.comment);
                listLabels[id].Parent = pnlPing;
                // отключено событие (не проработано) listLabels[id].Click += new System.EventHandler(ReversEnable_Pings);
                pnlPing.Controls.Add(listLabels[id]);
                ++id;
                ++i;
            }
            foreach (var connection in Gate_Transport_files_inpersonalfolder)
            {
                listLabels.Add(new System.Windows.Forms.LinkLabel());
                listLabels[id].Name = "lnkPing" + id.ToString();
                listLabels[id].Text = "g";
                listLabels[id].Location = new System.Drawing.Point(85 + i * 14, 25);
                listLabels[id].Visible = true;
                listLabels[id].AutoSize = true;
                listLabels[id].Font = new System.Drawing.Font("Webdings", 7);
                listLabels[id].LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                listLabels[id].ForeColor = SystemColors.HighlightText;
                listLabels[id].BackColor = System.Drawing.Color.Transparent;
                listLabels[id].LinkColor = clsVisualControls.clrPingDisable;
                listLabels[id].ActiveLinkColor = Color.White;
                new ToolTip().SetToolTip(listLabels[id], connection.comment);
                listLabels[id].Parent = pnlPing;
                // отключено событие (не проработано) listLabels[id].Click += new System.EventHandler(ReversEnable_Pings);
                pnlPing.Controls.Add(listLabels[id]);
                ++id;
                ++i;
            }
            foreach (var connection in Gate_Transport_files_email)
            {
                listLabels.Add(new System.Windows.Forms.LinkLabel());
                listLabels[id].Name = "lnkPing" + id.ToString();
                listLabels[id].Text = "g";
                listLabels[id].Location = new System.Drawing.Point(85 + i * 14, 25);
                listLabels[id].Visible = true;
                listLabels[id].AutoSize = true;
                listLabels[id].Font = new System.Drawing.Font("Webdings", 7);
                listLabels[id].LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                listLabels[id].ForeColor = SystemColors.HighlightText;
                listLabels[id].BackColor = System.Drawing.Color.Transparent;
                listLabels[id].LinkColor = clsVisualControls.clrPingDisable;
                listLabels[id].ActiveLinkColor = Color.White;
                new ToolTip().SetToolTip(listLabels[id], connection.comment);
                listLabels[id].Parent = pnlPing;
                // отключено событие (не проработано) listLabels[id].Click += new System.EventHandler(ReversEnable_Pings);
                pnlPing.Controls.Add(listLabels[id]);
                ++id;
                ++i;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Проверям на повтор запуска
            string procName = Process.GetCurrentProcess().ProcessName; 
            Process[] processes = Process.GetProcessesByName(procName);
            if (processes.Length > 1) выходToolStripMenuItem_Click(sender, e);
            // Сворачиваем в систрей
            //notifyIcon1.Icon = new System.Drawing.Icon(@"Globe-off.ico");
            notifyIcon1.Text = "Шлюз СРЗ - нет соединения";
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            // Устанавливаем стартовые значения и переменным
            //this.TopMost = true;
            lblHeader.Text += " " + ver;
            //linkLabel7_LinkClicked(sender, null);            

            window_height_max = this.Height;
            window_width_max = this.Width;
            lstbxLog.Visible = false;
            lstbxLog1 = new clsListBox()
            {
                Location = lstbxLog.Location,
                Size = lstbxLog.Size,
                BackColor = lstbxLog.BackColor,
                ForeColor = Color.FromArgb(50, clsVisualControls.clrText),
                BorderStyle = lstbxLog.BorderStyle,
                Font = new System.Drawing.Font("Arial", 7),
                Parent = lstbxLog.Parent,
                Visible = true
            };
            if (!InitializeElements())
            {
                MessageBox.Show("Приложение не может стартовать, причины указаны в Логах!");
                linkLabel6_LinkClicked(null, null);
            }
            // Запускаем таймеры
            timer_RefreshReglament.Enabled = true;

            logQueue.Enqueue(new clsLog(DateTime.Now, 1, "Gate", 0, 0, DateTime.Now, DateTime.Now, "Диспетчер запущен!"));
        }

        private void Clear_Memory(object sender, EventArgs e) //Задачи по расписанию
        {
            long totalMemory = GC.GetTotalMemory(false);
            GC.Collect(0);
            GC.WaitForPendingFinalizers();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Dispose(); System.Windows.Forms.Application.Exit();
        }

        private void развернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1_DoubleClick(sender, e);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) notifyIcon1_DoubleClick(sender, e);
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            //if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "Шлюз СРЗ";
                notifyIcon1.BalloonTipText = "Обратите внимание что программа была спрятана в трей и продолжит свою работу.";
                notifyIcon1.ShowBalloonTip(5000);
            }

        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(MessageBox.Show("Вы уверены что Диспетчер следует остановить, оставив без исполнения срочные и текущие задачи!?", "Диспетчер", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //останавливаем потоки
                shedules.finishing = true;
                Thread thread = new Thread(threadFinishing);
                thread.IsBackground = true;
                thread.Start();
                timer_Finishing.Enabled = true;
                this.Enabled = false;
                MessageBox.Show("Ожидаем завершения работавших процессов...");
            }
        }

        private void panel8_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_x = e.X; mouse_y = e.Y;
        }

        private void threadFinishing() //Оценка возможности завершения работы
        {
            bool finishing;
            do
            {
                Thread.Sleep(1000);
                finishing = true;
                foreach (clsBeat beat in shedules.beats)
                    if (beat.state == Thread_state.starting || beat.state == Thread_state.process)
                    {
                        finishing = false;
                        break;
                    }
            } while (!finishing);
            ready_finishing = finishing;
        }

        private void panel8_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this.Location = new System.Drawing.Point(this.Location.X + (e.X - mouse_x), this.Location.Y + (e.Y - mouse_y));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Width = 300;
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lnkMinimizer.Text == "3")
            {
                this.Width = window_width_min;
                pnlHeader.Width = window_width_min;
                lnkMinimizer.Text = "4";
            }
            else
            {
                this.Width = window_width_max;
                pnlHeader.Width = window_width_max;
                lnkMinimizer.Text = "3";
            }
        }

        private void lnkRoll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkMinimizer.Enabled = (lnkRoll.Text != "5");
            if (lnkRoll.Text == "5")
            {
                this.Height = window_height_min;
                lnkRoll.Text = "6";
                this.Width = window_width_min;
                pnlHeader.Width = window_width_min;
                //check_StatusLogining.Enabled = false;
            }
            else
            {
                this.Height = window_height_max;
                lnkRoll.Text = "5";
                //check_StatusLogining.Enabled = true;
                if (lnkMinimizer.Text == "3")
                {
                    this.Width = window_width_max;
                    pnlHeader.Width = window_width_max;
                }
            }

        }

        private void lnk_MouseEnter(object sender, EventArgs e)
        {
            LinkLabel lnk = (LinkLabel) sender;
            lnk.BackColor = Color.Black;
        }

        private void lnk_MouseLeave(object sender, EventArgs e)
        {
            LinkLabel lnk = (LinkLabel)sender;
            lnk.BackColor = System.Drawing.Color.Transparent;
        }

        private void Check_Pings(object sender, EventArgs e)
        {
            // пингуем контрольные точки Шлюза
            foreach (var connections in connections_list)
                foreach (clsConfigPrototype connection in connections)
                {
                    string destination = @"C:\temp\";
                    System.IO.Directory.CreateDirectory(destination);
                    FileStream fileStream = new FileStream(Path.Combine(destination, String.Format("GATE_{0}.log", DateTime.Now.ToString("yyyyMMdd"))), FileMode.Append);
                    StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                    streamWriter.Write(String.Format("{0} | ping | {1}\r\n", DateTime.Now.ToString(), connection.name));
                    streamWriter.Close();
                    connection.check();
                }
        }
        public void ReversEnable_Pings(object sender, EventArgs e)
        {
            for (int i = 0; i < listLabels.Count; i++)
                if (sender.GetHashCode() == listLabels[i].GetHashCode())
                {
                    if (Gate_Connections[i].enable)
                    {
                        if (Gate_Connections[i].ping.connectionType == 4) Gate_Connections[i].ping.stop_program();
                        Gate_Connections[i].enable = false;
                        logQueue.Enqueue(new clsLog(DateTime.Now, 0, "Gate", 0, 0, DateTime.Now, DateTime.Now, "снято с контроля - " + Gate_Connections[i].comment));
                    }
                    else
                    {
                        if (Gate_Connections[i].ping.connectionType == 4) Gate_Connections[i].ping.exec_program();
                        Gate_Connections[i].enable = true;
                        logQueue.Enqueue(new clsLog(DateTime.Now, 1, "Gate", 0, 0, DateTime.Now, DateTime.Now, "поставлено на контроль - " + Gate_Connections[i].comment));
                    }
                    //if (Gate_Connections[i].active) Gate_Connections[i].change_status = true;
                }
        }
        private void TestStatus_Pings(object sender, EventArgs e)
        //Проверяем состояние пингов, логиним и меняем цвет индикаторов 
        {
            int id = 0;
            foreach (var connections in connections_list)
            {
                if (connections.Count > 0)
                    foreach (clsConfigPrototype connection in connections)
                    {
                        if (connection.busy)
                        {
                            new ToolTip().SetToolTip(listLabels[id], connection.comment + " " + connection.comment_dop);
                            listLabels[id].LinkColor = clsVisualControls.clrPingBusy;
                        }
                        else
                            if (!connection.enable) listLabels[id].LinkColor = clsVisualControls.clrPingDeactive;
                            else listLabels[id].LinkColor = (connection.active) ? clsVisualControls.clrPingEnable : clsVisualControls.clrPingDisable;
                        if (connection.change_status)
                        {
                            if (connection.active)
                                logQueue.Enqueue(new clsLog(DateTime.Now, 1, "Gate", 0, 1, DateTime.Now, DateTime.Now, "соединение установлено" + " " + connection.comment));
                            else
                                logQueue.Enqueue(new clsLog(DateTime.Now, 0, "Gate", 0, 1, DateTime.Now, DateTime.Now, "соединение потеряно" + " " + connection.comment));
                            connection.change_status = false;
                        }
                        ++id;
                    }
            }
            lnkIdentificationSynch.LinkColor = (timer_sync_identification.Enabled) ? clsVisualControls.clrPingEnable : clsVisualControls.clrPingDisable;
            lnkIdentificationSynch.Text = (timer_sync_identification.Enabled) ? "u" : "w";
            if (!timer_sync_identification.Enabled) lblIdentificationSynch.ForeColor = clsVisualControls.clrText;
        }
        public void show_log()
        {
            int maxCountItems = 1000;
            int countItems = lstbxLog1.Items.Count;
            clsLog log;
            if (logQueue.Count > 0)
            {
                lstbxLog1.BeginUpdate();
                while (logQueue.Count > 0)
                {
                    log = logQueue.Dequeue();
                    //System.Threading.Thread.Sleep(10);
                    if (!shedules.test) Gate_Logining.Server_insert(log);
                    lstbxLog1.Items.Insert(0,
                            clsLibrary.trancateLongString(log.time.ToString("HH:mm:ss"), 10) + 
                            " " + 
                            clsLibrary.trancateLongString(log.name, 50) + 
                            " " + 
                            clsLibrary.trancateLongString(log.comment,150));
                    if (countItems > maxCountItems) lstbxLog1.Items.RemoveAt(countItems);
                    else countItems++; ;
                }
                lstbxLog1.EndUpdate();
            }
        }
        public void show_Beats()
        {
            if (shedules.busy_Beats)
            {
                lblCountBeats.Text = "...";
                return;
            }
            List<clsBeat> beats_temp = new List<clsBeat>(shedules.beats);
            //Заводится второй список Битов для вывода на экран, чтоб не попасть на моменте обновления списка Битов 
            int item = 1;
            int itemsMaxCount = 53;
            lstbxBeats.DoubleBuffering(true);
            lstbxBeats.BeginUpdate();
            lstbxBeats.Items.Clear();
            foreach (clsBeat beat in beats_temp)
            {
                if (beat.guid_parent == Guid.Empty)
                    lstbxBeats.Items.Add(beat.time.ToString("HH:mm:ss") + "    " + beat.gate_nametask + "    " + beat.state.ToString());
                else
                    lstbxBeats.Items.Add("                      " + beat.gate_nametask + "    " + beat.state.ToString());
                if (item >= itemsMaxCount) break;
                else item++;
            }
            lstbxBeats.EndUpdate();
            lblCountBeats.Text = String.Format("{0}",beats_temp.Count);
        }
        public void show_Reglament()
        {
            if (shedules.busy_Reglament) return;
            shedules.busy_Reglament = true;
            for (int i = listPanelReglament.Count() - 1; i >= 0; i-- )
                {
                    fpnlReglament.Controls.Remove(listPanelReglament[i].pnlMain);
                    listPanelReglament[i].Dispose();
                }
            listPanelReglament.Clear();
            int id = -1;
            foreach (clsShedule shedul in shedules.shedule)
            {
                //Создаем панель
                id++;
                //clsPanelReglament
                listPanelReglament.Add(
                    new clsPanelReglament(
                        shedul.reglament_id,
                        shedul.shedule_id,
                        shedul.shedule_id_parent,
                        shedul.region_name,
                        shedul.reglament_name,
                        shedul.shedule_name,
                        //String.Format("Каждые {0} секунд", shedul.shedule_interval),
                        shedul.shedule_interval,
                        shedul.gate_nametask,
                        shedul.shedule_active
                        ));
                    listPanelReglament[id].pnlMain.Parent = fpnlReglament;
                //listLabels[id].Click += new System.EventHandler(ReversEnable_Pings);
            }
            shedules.refreshed = false;
            shedules.busy_Reglament = false;
            timer_RefreshReglament.Enabled = true;
        }

        private void Transfer_Files(object sender, EventArgs e)
        {
            foreach (clsTransport_files transport_files in Gate_Transport_files)
            {
                FileStream fileStream = new FileStream(String.Format(@"C:\temp\GATE_{0}.log", DateTime.Now.ToString("yyyyMMdd")), FileMode.Append);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                streamWriter.Write(String.Format("{0} | transport | {1}\r\n", DateTime.Now.ToString(), transport_files.name));
                streamWriter.Close();
                transport_files.start();
            }
            foreach (clsTransport_files_inpersonalfolder transport_files in Gate_Transport_files_inpersonalfolder)
            {
                FileStream fileStream = new FileStream(String.Format(@"C:\temp\GATE_{0}.log", DateTime.Now.ToString("yyyyMMdd")), FileMode.Append);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                streamWriter.Write(String.Format("{0} | transport personal | {1}\r\n", DateTime.Now.ToString(), transport_files.name));
                streamWriter.Close();
                transport_files.start();
            }
            foreach (clstransport_files_email transport_files in Gate_Transport_files_email)
            {
                FileStream fileStream = new FileStream(String.Format(@"C:\temp\GATE_{0}.log", DateTime.Now.ToString("yyyyMMdd")), FileMode.Append);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                streamWriter.Write(String.Format("{0} | send email | {1}\r\n", DateTime.Now.ToString(), transport_files.name));
                streamWriter.Close();
                transport_files.start();
            }
        }
 
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >=0)
            {
                SolidBrush brush = new SolidBrush(clsVisualControls.clrText);
                string text = lstbxBeats.Items[e.Index].ToString();
                if (text.Contains("error"))
                    brush = new SolidBrush(Color.DarkRed);
                else
                    if (text.Contains("wait"))
                        brush = new SolidBrush(Color.FromArgb(150, clsVisualControls.clrText));
                    else
                        if (text.Contains("finished") )
                            brush = new SolidBrush(Color.FromArgb(70, clsVisualControls.clrText));
                        else 
                            if (text.Contains("ignored"))
                                brush = new SolidBrush(Color.FromArgb(30, clsVisualControls.clrText));

                e.Graphics.DrawString(
                    lstbxBeats.Items[e.Index].ToString(),
                    e.Font, brush, e.Bounds,
                    StringFormat.GenericDefault
                    );

            }
            e.DrawFocusRectangle();
        }

        private void refresh_Listboxs_Tick(object sender, EventArgs e)
        {
            lblTimeShedule.Text = shedules.timeShedule.ToString();
            lnkStartBeats.Enabled = true;
        }

    

        private void lnkClear_log_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lstbxLog1.Items.Clear();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (fllpnl1.AutoSize)
            {
                fllpnl1.AutoSize = false;
                fllpnl1.Size = new Size(101, 20);
                linkLabel1.Text = "РЕГЛАМЕНТ...";
            }
            else
            {
                fllpnl1.Size = new Size(101, 20);
                fllpnl1.AutoSize = true;
                linkLabel1.Text = "РЕГЛАМЕНТ";
                //linkLabel1.LinkColor = clsVisualControls.colorBrightness(clsVisualControls.clrText, (float)-0.5);
            }

        }

        private void lnklblReglamentRoll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lnklblReglamentRoll.Text == "5")
            {
                fpnlReglament.Height = 0;
                lnklblReglamentRoll.Text = "6";
            }
            else
            {
                fpnlReglament.Height = 711;
                lnklblReglamentRoll.Text = "5";
            }

        }

        //-------------------------- Старые методы
        void threadReport_prickr()
        {
            //logQueue.Enqueue(DateTime.Now.ToString() + " отчет - Анализ прикрепления, выполняется");
            //Excel.Workbook fileExcel;
            String vFilename = "Анализ прикрепления";
            //String vFullPathFile = @"W:\AMIAC\" + vFilename + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".csv";
            String vFullPathFile = @"c:\temp\" + vFilename + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".csv";
            try
            {
                //string  sqlConnection = "uid=sa;pwd=Cvbqwe2!;server=server-r;database=srz3_00;";
                string query =
                    "declare @d date " +
                    "declare @Q varchar(5)  " +
                    "declare  @subj varchar(5)  " +
                    "select @subj = XVAL from srz3_00.dbo.settings where ckey like 'OKATO'  " +
                    "set @D=GETDATE()  " +
                    "if Object_ID('tempdb..#PeopleA') is not null drop table #PeopleA  " +
                    "create table #PeopleA (pid int)  " +
                    "create unique index PeopleA_ix on #PeopleA (pid)   " +
                    "declare @id int, @pid int, @lpu char(6), @smo int, @child int  " +
                    "insert into #PeopleA (pid)   " +
                    "select p.id from  srz3_00.dbo.people p  " +
                    "join srz3_00.dbo.polis d on  p.id = d.pid   " +
                    "where (isnull(d.st,0) <>2) and (isnull(d.dstop,'21000101')>=@d)   " +
                    "and (isnull(p.ds,'21000101')>= @d) and (left(d.okato,2) = left(@subj,2))  " +
                    "and (d.poltp in (1,2) or isnull(d.dend,'21000101')>=@d) and (d.poltp in( 2,3,4,5 ) or (d.poltp=1 and (isnull(d.dend,'21000101')>'20101231')))    " +
                    "and d.dbeg<=@d   " +
                    "group by p.id " +
                    "if  Object_ID('tempdb..#Tmp') is not null drop table #Tmp " +
                    "create table #Tmp (mcod varchar(6), naim varchar(250), ss varchar(11), cnt int, fam varchar(40), im varchar(40), ot varchar(40), contrl int) " +
                    "insert into #Tmp (mcod, naim, ss, cnt, fam, im, ot, contrl) " +
                    "select  " +
                    " mcod,name,isnull(tmp_lst.ss_doctor,'') ss_doctor,cnt, " +
                    " isnull(FAM,'') fam ,isnull(IM,'') im,isnull(ot,'') ot, case when cnt<100 then 1 else 0 end min_mr " +
                    " from " +
                    "( " +
                    "select hlpu2.lpu mcod, bd.nam_mok name, right('000'+hlpu2.SS_DOCTOR,11) ss_doctor, COUNT (1) cnt " +
                    "from #PeopleA pplA  " +
                    " join (select hlpu1.pid, max(hlpu1.id) id from HISTLPU hlpu1 where hlpu1.LPUDX is null group by  hlpu1.pid ) hlpu on hlpu.PID=pplA.pid " +
                    " join HISTLPU hlpu2 on hlpu.id=hlpu2.ID " +
                    " left outer join tmpForSRZ.dbo.libMO bd on bd.mcod=hlpu2.lpu " +
                    "group by hlpu2.lpu, bd.nam_mok, right('000'+hlpu2.SS_DOCTOR,11) " +
                    ") tmp_lst  " +
                    "left outer join  " +
                    " ( " +
                    " select ppl.SS SS_DOCTOR, ppl.FAM, ppl.IM, ppl.ot " +
	                " from people ppl " +
                    " join (select MAX(id) id, ss from people where SS is not null group by SS) list_ss on list_ss.SS = ppl.ss and list_ss.id = ppl.id " +
	                " where ppl.SS is not null " +
	                " group by ppl.SS, ppl.FAM, ppl.IM, ppl.ot) frmr on frmr.SS_DOCTOR=dbo.ss11_14(tmp_lst.ss_doctor) " +
	                " order by tmp_lst.mcod, tmp_lst.name, tmp_lst.ss_doctor " +
                    /*"select lst.lpu 'Код', isnull(lst.nam_mok,'') 'Наименование', convert(varchar,COUNT(1)) 'По МО', convert(varchar,sum(lpuauto)) 'По заявлению', convert(varchar,sum(ss)) 'По врачам' " +
                    ",convert(varchar,isnull(t1.cnt_med,0)) cnt_med1,convert(varchar,isnull(t2.cnt_med,0)) cnt_med2,convert(varchar,isnull(t3.cnt_med,0)) cnt_med3 " +
                    "from ( " +
                    " select case when hlpu2.LPUAUTO=2 then 1 else 0 end lpuauto, hlpu2.lpu, bd.nam_mok, case when isnull(hlpu2.SS_DOCTOR,'')='' then 0 else 1 end ss " +
                    " from #PeopleA pplA  " +
                    " join (select hlpu1.pid, max(hlpu1.id) id from HISTLPU hlpu1 where hlpu1.LPUDX is null group by  hlpu1.pid ) hlpu on hlpu.PID=pplA.pid " +
                    " join HISTLPU hlpu2 on hlpu.id=hlpu2.ID " +
                    " left outer join tmpForSRZ.dbo.libMO bd on bd.mcod=hlpu2.lpu) lst	 " +
                    "left outer join (select mcod, count(1) cnt_med  from #Tmp group by mcod) t1 on t1.mcod=lpu " +
                    "left outer join (select mcod, count(1) cnt_med  from #Tmp where contrl=1 group by mcod) t2 on t2.mcod=lpu " +
                    "left outer join (select mcod, sum(cnt) cnt_med from #Tmp where contrl=1 group by mcod) t3 on t3.mcod=lpu " +
                    "group by lst.lpu, lst.nam_mok,t1.cnt_med,t2.cnt_med,t3.cnt_med  " +
                    "order by lst.lpu, lst.nam_mok " +*/
                    "select 'mcod;naim;ss;cnt;fam;im;ot;contrl' union all " +
                    "select mcod + ';' + isnull(naim,'') +';' + isnull(dbo.ss11_14(ss),'') + ';' + convert(varchar,cnt) + ';' + fam + ';' + im + ';' + ot + ';' +convert(varchar,contrl) contrl from #Tmp";

                List<string> response = new List<string>();
                //response = clsLibrary.execQuery_getListString(ref Gate_Connections, "server-r", "srz3_00", query);
                clsLibrary.createFileTXT_FromList(response, vFullPathFile);
                //MessageBox.Show("Запрос (отчет прикрепления) обработался корректно судя по всему - " + @vFullPathFile);
                //String vFullPathFile2 = @"W:\FOMS\Bogomaz\" + vFilename + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
                //File.Copy(vFullPathFile, vFullPathFile2, true);

                logQueue.Enqueue(new clsLog(DateTime.Now, 9, "Отчет", 0, 0, DateTime.Now, DateTime.Now, "Анализ прикрепления, сформирован"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("threadReport_prickr - "+ex.Message);
                //logQueue.Enqueue(DateTime.Now.ToString() + " отчет - Анализ прикрепления, ОШИБКА");
                logQueue.Enqueue(new clsLog(DateTime.Now, 1, "Отчет", 0, 0, DateTime.Now, DateTime.Now, "Анализ прикрепления, Ошибка"));
            }
            //fileExcel = null;
        }

/*        void threadReport_prickr()
        {
            //logQueue.Enqueue(DateTime.Now.ToString() + " отчет - Анализ прикрепления, выполняется");
            Excel.Workbook fileExcel;
            String vFilename = "Анализ прикрепления";
            String vFullPathFile = @"W:\AMIAC\" + vFilename + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + @"\Templates\" + vFilename + "_шаблон.xlsx", vFullPathFile, true);
                Excel.Application exApp = new Excel.Application();
                fileExcel = exApp.Workbooks.Open(vFullPathFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                                Type.Missing, Type.Missing); //Add();

                SqlConnection sqlConnection1_2 = new SqlConnection("uid=sa;pwd=Cvbqwe2!;server=server-r;database=srz3_00;");
                SqlCommand cmd1_2 = new SqlCommand();
                cmd1_2.CommandText =
                    "declare @d date " +
                    "declare @Q varchar(5)  " +
                    "declare  @subj varchar(5)  " +
                    "select @subj = XVAL from srz3_00.dbo.settings where ckey like 'OKATO'  " +
                    "set @D=GETDATE()  " +
                    "if Object_ID('tempdb..#PeopleA') is not null drop table #PeopleA  " +
                    "create table #PeopleA (pid int)  " +
                    "create unique index PeopleA_ix on #PeopleA (pid)   " +
                    "declare @id int, @pid int, @lpu char(6), @smo int, @child int  " +
                    "insert into #PeopleA (pid)   " +
                    "select p.id from  srz3_00.dbo.people p  " +
                    "join srz3_00.dbo.polis d on  p.id = d.pid   " +
                    "where (isnull(d.st,0) <>2) and (isnull(d.dstop,'21000101')>=@d)   " +
                    "and (isnull(p.ds,'21000101')>= @d) and (left(d.okato,2) = left(@subj,2))  " +
                    "and (d.poltp in (1,2) or isnull(d.dend,'21000101')>=@d) and (d.poltp in( 2,3,4,5 ) or (d.poltp=1 and (isnull(d.dend,'21000101')>'20101231')))    " +
                    "and d.dbeg<=@d   " +
                    "group by p.id " +
                    "if  Object_ID('tempdb..#Tmp') is not null drop table #Tmp " +
                    "create table #Tmp (mcod varchar(6), naim varchar(250), ss varchar(11), cnt int, fam varchar(40), im varchar(40), ot varchar(40), contrl int) " +
                    "insert into #Tmp (mcod, naim, ss, cnt, fam, im, ot, contrl) " +
                    "select  " +
                    " mcod,name,isnull(tmp_lst.ss_doctor,'') ss_doctor,cnt, " +
                    " isnull(FAM,'') fam ,isnull(IM,'') im,isnull(ot,'') ot, case when cnt<100 then 1 else 0 end min_mr " +
                    " from " +
                    "( " +
                    "select hlpu2.lpu mcod, bd.nam_mok name, right('000'+hlpu2.SS_DOCTOR,11) ss_doctor, COUNT (1) cnt " +
                    "from #PeopleA pplA  " +
                    " join (select hlpu1.pid, max(hlpu1.id) id from HISTLPU hlpu1 where hlpu1.LPUDX is null group by  hlpu1.pid ) hlpu on hlpu.PID=pplA.pid " +
                    " join HISTLPU hlpu2 on hlpu.id=hlpu2.ID " +
                    " left outer join tmpForSRZ.dbo.libMO bd on bd.mcod=hlpu2.lpu " +
                    "group by hlpu2.lpu, bd.nam_mok, right('000'+hlpu2.SS_DOCTOR,11) " +
                    ") tmp_lst  " +
                    "left outer join  " +
                    " ( " +
                    " select ppl.SS SS_DOCTOR, ppl.FAM, ppl.IM, ppl.ot " +
                    " from people ppl " +
                    " join (select MAX(id) id, ss from people where SS is not null group by SS) list_ss on list_ss.SS = ppl.ss and list_ss.id = ppl.id " +
                    " where ppl.SS is not null " +
                    " group by ppl.SS, ppl.FAM, ppl.IM, ppl.ot) frmr on frmr.SS_DOCTOR=dbo.ss11_14(tmp_lst.ss_doctor) " +
                    " order by tmp_lst.mcod, tmp_lst.name, tmp_lst.ss_doctor " +
                    "select lst.lpu 'Код', isnull(lst.nam_mok,'') 'Наименование', convert(varchar,COUNT(1)) 'По МО', convert(varchar,sum(lpuauto)) 'По заявлению', convert(varchar,sum(ss)) 'По врачам' " +
                    ",convert(varchar,isnull(t1.cnt_med,0)) cnt_med1,convert(varchar,isnull(t2.cnt_med,0)) cnt_med2,convert(varchar,isnull(t3.cnt_med,0)) cnt_med3 " +
                    "from ( " +
                    " select case when hlpu2.LPUAUTO=2 then 1 else 0 end lpuauto, hlpu2.lpu, bd.nam_mok, case when isnull(hlpu2.SS_DOCTOR,'')='' then 0 else 1 end ss " +
                    " from #PeopleA pplA  " +
                    " join (select hlpu1.pid, max(hlpu1.id) id from HISTLPU hlpu1 where hlpu1.LPUDX is null group by  hlpu1.pid ) hlpu on hlpu.PID=pplA.pid " +
                    " join HISTLPU hlpu2 on hlpu.id=hlpu2.ID " +
                    " left outer join tmpForSRZ.dbo.libMO bd on bd.mcod=hlpu2.lpu) lst	 " +
                    "left outer join (select mcod, count(1) cnt_med  from #Tmp group by mcod) t1 on t1.mcod=lpu " +
                    "left outer join (select mcod, count(1) cnt_med  from #Tmp where contrl=1 group by mcod) t2 on t2.mcod=lpu " +
                    "left outer join (select mcod, sum(cnt) cnt_med from #Tmp where contrl=1 group by mcod) t3 on t3.mcod=lpu " +
                    "group by lst.lpu, lst.nam_mok,t1.cnt_med,t2.cnt_med,t3.cnt_med  " +
                    "order by lst.lpu, lst.nam_mok " +
                    "select mcod, isnull(naim,''),ss,convert(varchar,cnt) cnt, fam, im,ot, convert(varchar,contrl) contrl from #Tmp";
                // MessageBox.Show(cmd1_2.CommandText);
                cmd1_2.CommandType = CommandType.Text;
                cmd1_2.Connection = sqlConnection1_2;
                cmd1_2.CommandTimeout = 18000;
                sqlConnection1_2.Open();
                SqlDataReader rdr = cmd1_2.ExecuteReader();

                Worksheet workSheet = (Worksheet)exApp.Sheets[1];
                int i = 1;
                int row = 0;
                while (rdr.Read())
                {
                    row = 5 + i;
                    workSheet.Cells[row, 1] = rdr.GetString(0);
                    workSheet.Cells[row, 2] = rdr.GetString(1);
                    workSheet.Cells[row, 3] = rdr.GetString(2);
                    workSheet.Cells[row, 4] = rdr.GetString(3);
                    workSheet.Cells[row, 5] = rdr.GetString(4);
                    workSheet.Cells[row, 6].FormulaR1C1 = "=R" + row.ToString() + "C3-R" + row.ToString() + "C5";
                    workSheet.Cells[row, 7].FormulaR1C1 = "=(R" + row.ToString() + "C5/R" + row.ToString() + "C3)*100";
                    workSheet.Cells[row, 8] = rdr.GetString(5);
                    workSheet.Cells[row, 9] = rdr.GetString(6);
                    workSheet.Cells[row, 10] = rdr.GetString(7);
                    i++;
                }

                rdr.NextResult();
                workSheet = (Worksheet)exApp.Sheets[2];
                workSheet.Cells[3, 1] = "за период " + DateTime.Now.AddDays(-7).ToString("dd.MM.yyyy") + "г. - " + DateTime.Now.ToString("dd.MM.yyyy") + "г.";
                i = 1;
                while (rdr.Read())
                {
                    row = 1 + i;
                    workSheet.Cells[row, 1] = rdr.GetString(0);
                    workSheet.Cells[row, 2] = rdr.GetString(1);
                    workSheet.Cells[row, 3] = rdr.GetString(2);
                    workSheet.Cells[row, 4] = rdr.GetString(3);
                    workSheet.Cells[row, 5] = rdr.GetString(4);
                    workSheet.Cells[row, 6] = rdr.GetString(5);
                    workSheet.Cells[row, 7] = rdr.GetString(6);
                    workSheet.Cells[row, 8] = rdr.GetString(7);
                    i++;
                }
                sqlConnection1_2.Close();

                fileExcel.Save();
                fileExcel.Close();
                exApp.Quit();

                //MessageBox.Show("Запрос (отчет прикрепления) обработался корректно судя по всему - " + @vFullPathFile);
                String vFullPathFile2 = @"W:\FOMS\Bogomaz\" + vFilename + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
                File.Copy(vFullPathFile, vFullPathFile2, true);

                logQueue.Enqueue(new clsLog(DateTime.Now, 9, "Отчет", 0, 0, DateTime.Now, DateTime.Now, "Анализ прикрепления, сформирован"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("threadReport_prickr - " + ex.Message);
                //logQueue.Enqueue(DateTime.Now.ToString() + " отчет - Анализ прикрепления, ОШИБКА");
                logQueue.Enqueue(new clsLog(DateTime.Now, 1, "Отчет", 0, 0, DateTime.Now, DateTime.Now, "Анализ прикрепления, Ошибка"));
            }
            fileExcel = null;
        }
*/
        
        private void button3_Click_1(object sender, EventArgs e)
        {
            Thread Report_prickr = new Thread(threadReport_prickr);
            Report_prickr.IsBackground = true;
            
            Report_prickr.Start();
        }


        void threadReport_EIR(object dt1)
        {
            //logQueue.Enqueue(DateTime.Now.ToString() + " отчет - Анализ направлений, выполняется ");
            Excel.Workbook fileExcel;
            String vFilename = "Анализ направлений";
            //String vFullPathFile = @"W:\minzdrav\omo\" + vFilename + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
            String vFullPathFile = @"d:\temp\" + vFilename + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + @"\Templates\" + vFilename + "_шаблон.xlsx", vFullPathFile, true);
                Excel.Application exApp = new Excel.Application();
                fileExcel = exApp.Workbooks.Open(vFullPathFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                                Type.Missing, Type.Missing); //Add();

                SqlConnection sqlConnection1_2 = new SqlConnection("server=11.0.0.83;uid=Sa;pwd=Q1w2e3;database=srz_support;");
                SqlCommand cmd1_2 = new SqlCommand();
                cmd1_2.CommandText =
                    "declare @date date='" + DateTime.Now.ToString("yyyMMdd") + "' " +
                    "declare @d_week date='" + DateTime.Now.AddDays(-7).ToString("yyyMMdd") + "' " +
                    "select bfr.mcode2, isnull(mo.nam_mok,'') nam_mok, convert(varchar,COUNT(1)) 'заявок', convert(varchar, COUNT(sr.NAPR_NUM)) 'ответов'  from  " +
                    "	(select napr_num, MCODE2 from BUF_SN  where DATE_SYS>='" + DateTime.Now.ToString("yyyy") + "0101' and DATE_SYS<@date AND (MCODE2 IS NOT NULL and MCODE2<>MCODE1) group by NAPR_NUM, MCODE2) bfr  " +
                    "left outer join  " +
                    "	(select napr_num, MCODE2 from BUF_SR group by NAPR_NUM, MCODE2) sr on sr.NAPR_NUM=bfr.NAPR_NUM and sr.mcode2=bfr.MCODE2 " +
                    "left outer join dbo.libMO mo on mo.mcod=bfr.MCODE2 " +
                    "left outer join dbo.OMS_BIDDERS bds on bds.CODE=bfr.MCODE2 " +
                    "group by bds.report_sort, bfr.mcode2, mo.nam_mok " +
                    "order by bds.report_sort " +
                     "select bfr.mcode2, isnull(mo.nam_mok,'') nam_mok,  convert(varchar,COUNT(1)) 'заявок',  convert(varchar,COUNT(sr.NAPR_NUM)) 'ответов'  from " +
                     "	(select napr_num, MCODE2 from BUF_SN  where DATE_SYS>=@d_week and DATE_SYS<@date AND (MCODE2 IS NOT NULL and MCODE2<>MCODE1) group by NAPR_NUM, MCODE2) bfr " +
                     "left outer join " +
                         "(select napr_num, MCODE2 from BUF_SR group by NAPR_NUM, MCODE2) sr on sr.NAPR_NUM=bfr.NAPR_NUM and sr.mcode2=bfr.MCODE2 " +
                     "left outer join dbo.libMO mo on mo.mcod=bfr.MCODE2 " +
                     "left outer join dbo.OMS_BIDDERS bds on bds.CODE=bfr.MCODE2 " +
                     "group by bds.report_sort, bfr.mcode2, mo.nam_mok " +
                     "order by bds.report_sort";
                cmd1_2.CommandType = CommandType.Text;
                cmd1_2.Connection = sqlConnection1_2;
                cmd1_2.CommandTimeout = 18000;
                sqlConnection1_2.Open();
                SqlDataReader rdr = cmd1_2.ExecuteReader();

                Worksheet workSheet = (Worksheet)exApp.Sheets[1];
                (exApp.Sheets[1] as Worksheet).Name = (exApp.Sheets[1] as Worksheet).Name + " " + DateTime.Now.ToString("yyyy") + "г.";
                workSheet.Cells[3, 1] = "за период 01.01." + DateTime.Now.ToString("yyyy") + "г. - " + DateTime.Now.ToString("dd.MM.yyyy") + "г.";

                int i = 1;
                int row = 0;
                while (rdr.Read())
                {
                    row = 6 + i;
                    workSheet.Cells[row, 1] = rdr.GetString(0);
                    workSheet.Cells[row, 2] = rdr.GetString(1);
                    workSheet.Cells[row, 3] = rdr.GetString(2);
                    workSheet.Cells[row, 4] = rdr.GetString(3);
                    workSheet.Cells[row, 5].FormulaR1C1 = "=(R" + row.ToString() + "C4/R" + row.ToString() + "C3)*100";
                    i++;
                }
                rdr.NextResult();
                workSheet = (Worksheet)exApp.Sheets[2];
                workSheet.Cells[3, 1] = "за период " + DateTime.Now.AddDays(-7).ToString("dd.MM.yyyy") + "г. - " + DateTime.Now.ToString("dd.MM.yyyy") + "г.";
                i = 1;
                while (rdr.Read())
                {
                    row = 6 + i;
                    workSheet.Cells[row, 1] = rdr.GetString(0);
                    workSheet.Cells[row, 2] = rdr.GetString(1);
                    workSheet.Cells[row, 3] = rdr.GetString(2);
                    workSheet.Cells[row, 4] = rdr.GetString(3);
                    workSheet.Cells[row, 5].FormulaR1C1 = "=(R" + row.ToString() + "C4/R" + row.ToString() + "C3)*100";
                    i++;
                }
                sqlConnection1_2.Close();

                fileExcel.Save();
                fileExcel.Close();
                exApp.Quit();

                //MessageBox.Show("Запрос (Анализ ЕИР) обработался корректно судя по всему - " + vFullPathFile);
                logQueue.Enqueue(new clsLog(DateTime.Now, 9, "Отчет", 0, 0, DateTime.Now, DateTime.Now, "Анализ  (ЕИР)"));
                String vFullPathFile2 = @"W:\FOMS\Bogomaz\" + vFilename + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
                File.Copy(vFullPathFile, vFullPathFile2, true);
                //  logQueue.Enqueue(DateTime.Now.ToString() + " отчет - Анализ направлений, ГОТОВ ");
            }
            catch 
            {
                logQueue.Enqueue(new clsLog(DateTime.Now, 1, "Отчет", 0, 0, DateTime.Now, DateTime.Now, "Анализ направлений (ЕИР) - Ошибка"));
                //MessageBox.Show("threadReport_EIR - " + ex.Message);
                //logQueue.Enqueue(DateTime.Now.ToString() + " отчет - Анализ направлений, ОШИБКА ");
                return;
            }

            fileExcel = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Thread Report_EIR = new Thread(threadReport_EIR);
            Report_EIR.IsBackground = true;
            Report_EIR.Start();

        }

        private void lblSetTest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            shedules.test = !shedules.test;
            shedules.Stop();
            lnkStartBeats_Refresh();            
            if (shedules.test)
            {
                lblSetTest.Image = Pictures.file25_yellow;
                fpnlReglament.BackColor = clsVisualControls.clrPanelTest;
                new ToolTip().SetToolTip(this.lblSetTest, "Переход в рабочий режим");
            }
            else
            {
                lblSetTest.Image = Pictures.file25;
                fpnlReglament.BackColor = Color.Transparent;
                new ToolTip().SetToolTip(this.lblSetTest, "Переход в тестовый режим");
            }
            lnklblRefreshReglament_LinkClicked(null, null); //Обновляем Регламент
        }

        private void lnklblRefreshReglament_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            shedules.dateStart_GetReglament = shedules.dateStart_GetReglament - shedules.intervalSec_GetReglament;
        }

        private void timer_RefreshReglament_Tick(object sender, EventArgs e)
        {
            if (shedules.enableReglament && !shedules.busy_Reglament && shedules.refreshed)
            {
                show_Reglament();
                if (refresh_Listboxs.Enabled != true)
                {
                    refresh_Listboxs.Enabled = true;
                } 
            }

        }

        private void lnkStartBeats_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //bool status;
            if (shedules.active)
            {
                shedules.Stop();
                //status = false;
            }
            else
            {
                shedules.Start();
                //status = true;
            }
            lnkStartBeats_Refresh();
        }
        private void lnkStartBeats_Refresh()
        {
            lnkStartBeats.LinkColor = (shedules.active) ? clsVisualControls.clrPingEnable : clsVisualControls.clrPingDisable;
            lnkStartBeats.Text = (shedules.active) ? "u" : "w";
        }

        private void timer_Logining_Tick(object sender, EventArgs e)
        {
            show_log();
        }

        private void timer_showBeats_Tick(object sender, EventArgs e)
        {
            show_Beats();
        }

        private void timer_Finishing_Tick(object sender, EventArgs e)
        {
            if(ready_finishing)
            {
                //останавливаем логирование
                timer_Logining.Enabled = false;
                //пишем лог о закрытии
                logQueue.Enqueue(new clsLog(DateTime.Now, 0, "Gate", 0, 0, DateTime.Now, DateTime.Now, "Диспетчер остановлен!!!"));
                //записываем лог
                show_log();
                this.Close();
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (thread_identification_synch != null)
                {
                    //MessageBox.Show(thread_identification_synch.ThreadState.ToString());
                    if (!thread_identification_synch.IsAlive)
                    {
                        lblIdentificationSynch.ForeColor = clsVisualControls.clrPingDisable;
                        logQueue.Enqueue(new clsLog(DateTime.Now, 0, "thread_identification_synch", 0, 0, DateTime.Now, DateTime.Now, "stopped!!!"));
                        thread_identification_synch.Abort();
                        thread_identification_synch = null;                        
                    }
                }
                else
                {
                    thread_identification_synch = new Thread(new ParameterizedThreadStart(clsBeat.gate_identification_synch))
                    { IsBackground = true, Name = "thread_identification_synch" };
                    thread_identification_synch.Start(Gate_Connections);
                    lblIdentificationSynch.ForeColor = clsVisualControls.clrText;
                    logQueue.Enqueue(new clsLog(DateTime.Now, 0, "thread_identification_synch", 0, 0, DateTime.Now, DateTime.Now, "started"));
                }                
            }
            catch(Exception ex)
            {
                logQueue.Enqueue(new clsLog(DateTime.Now, 0, "Gate", 0, 0, DateTime.Now, DateTime.Now, "thread_identification_synch try attempt : " + ex.Message));
            }
            
        }

        private void LinkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (thread_identification_synch != null) thread_identification_synch.Abort();
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            timer_sync_identification.Enabled = !timer_sync_identification.Enabled;
            if (!timer_sync_identification.Enabled && thread_identification_synch != null)
            {
                logQueue.Enqueue(new clsLog(DateTime.Now, 0, "thread_identification_synch", 0, 0, DateTime.Now, DateTime.Now, "stopped!!!"));
                thread_identification_synch.Abort();
                thread_identification_synch = null;
            }
        }

        private void LnklblRefreshBeats_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void Panel1_MouseEnter(object sender, EventArgs e)
        {
            panel1.BackColor =  Color.FromArgb(10, 10, 40);
        }

        private void Panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(0, 0, 20);
        }

        private void PnlHeader_MouseEnter(object sender, EventArgs e)
        {
            pnlHeader.BackColor = Color.FromArgb(10, 10, 40);
        }

        private void PnlHeader_MouseLeave(object sender, EventArgs e)
        {
            pnlHeader.BackColor = Color.FromArgb(0, 0, 20);
        }
    }

   
}

