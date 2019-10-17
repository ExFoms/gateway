using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Security.Principal;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

namespace WindowsFormsApplication3
{
    public delegate void Metod_Log(clsLog log);
       
    #region класс основного контура в расписании clsGateway
    public class clsShedules  
    {
        public List<clsConnections> link_connections = new List<clsConnections>(); // Ссылка на список подключений
        public bool test = true;

        public List<clsShedule> shedule = new List<clsShedule>();
        public List<clsBeat> beats = new List<clsBeat>();

        public DateTime dateStart_GetReglament;
        public TimeSpan intervalSec_GetReglament;

        public DateTime dateStart_SetBeats;
        public TimeSpan interval_SetBeats;

        public DateTime dateStart_cleanBeats;
        public TimeSpan interval_cleanBeats;

        public DateTime timeShedule; //текущее время цикла основного контура
        public bool active;

        public bool finishing = false; //переход в завершение работы
        public bool enableReglament = true; //обновляется Регламент!?
        public bool refreshed = false; //обновлен ли
        public bool needRefresh = false; //поставлена задача обновить регламент
        public bool busy_Reglament; // = true;
        public bool busy_Beats = true;

        Thread SheduleController;

        public Metod_Log metod_log; //для передачи метода публикации лога из основного класса

        public clsShedules(Metod_Log metod_log_, ref List<clsConnections> link_connections_)
        {
            metod_log = metod_log_;
            link_connections = link_connections_;

            intervalSec_GetReglament = TimeSpan.FromHours(3); //3 часа 
            dateStart_GetReglament = DateTime.Now-intervalSec_GetReglament;

            interval_SetBeats = TimeSpan.FromMinutes(15); 
            dateStart_SetBeats = DateTime.Now - interval_SetBeats;

            interval_cleanBeats = TimeSpan.FromSeconds(10);
            dateStart_cleanBeats = DateTime.Now;

            active = false;
            SheduleController = new Thread(threadSheduleController) { IsBackground = true };

            //создаем и запускаем основной контур
            if (!SheduleController.IsAlive)
            {
                SheduleController = new Thread(threadSheduleController) { IsBackground = true };
                busy_Reglament = false;
                busy_Beats = false;
                SheduleController.Start();
            }
        }

        public void Start()
        // Запуск sheduleGate
        {
            if (!active)
            {
                active = true;
                metod_log(new clsLog(DateTime.Now, 1, "shedule", 0, 0, DateTime.Now, DateTime.Now, "Запущен основной контур"));
                //создаем и запускаем основной контур
                if (!SheduleController.IsAlive)
                {
                    SheduleController = new Thread(threadSheduleController) { IsBackground = true };
                    SheduleController.Start();
                }
            }
        }
        public void Stop()
        // Останавливаем Gateway
        {
            active = false;
            metod_log(new clsLog(DateTime.Now, 0, "shedule",0, 0, DateTime.Now, DateTime.Now, "Остановлен основной контур"));
        }
        private void SetSheduleError(int id)
        {
            foreach (clsShedule shedul in shedule)
            {
                if (shedul.shedule_id == id)
                {
                    shedul.error_count++;
                    break;
                }
            }
        }
        private void setRemove_beats()
        {
            clsBeat.Log_status queue_status;
            clsBeat beat_depended;
            foreach (clsBeat beat in beats)
            {
                // логинем комментарии процесса работы
                while (beat.queue_status.Count > 0)
                {
                    queue_status = beat.queue_status.Dequeue();
                    metod_log(new clsLog(DateTime.Now, 9, queue_status.text1, beat.region_id, beat.task_id, beat.time, DateTime.Now, (string)(queue_status.comment + "  " + queue_status.text2).Trim()));
                }
                if (!beat.remove) //если не помечен уже на удаление нужно пометить и опубликовать в лог
                {
                    switch (beat.state)
                    {
                        case Thread_state.broke:
                            metod_log(new clsLog(DateTime.Now, 0, beat.gate_nametask, beat.region_id, beat.task_id, beat.time, DateTime.Now, clsErrors.ToString(beat.error)));
                            beat.remove = true;
                            break;
                        case Thread_state.ignored:
                             beat_depended = beats.Find(a => a.guid_parent == beat.guid);
                             while (beat_depended != null)
                             {
                                 beat_depended.state = beat.state;
                                 beat_depended.remove = true;
                                 beat_depended = beats.Find(a => a.guid_parent == beat_depended.guid);
                             }
                            beat.remove = true;
                            break;
                        case Thread_state.error:
                        case Thread_state.notresponding:
                            SetSheduleError(beat.shedule_id);
                            metod_log(new clsLog(DateTime.Now, 0, beat.gate_nametask, beat.region_id, beat.task_id, beat.time, DateTime.Now, clsErrors.ToString(beat.error)));
                            //if (beat.guid_parent == Guid.Empty) 
                            beat.remove = true;
                            break;
                        case Thread_state.finished:
                            bool finish_depended = true; //при входе в это условие головной бит завершился
                            object parameters;
                            if (beat.guid_parent == Guid.Empty)
                            {
                                parameters = beat.parameters;
                                beat_depended = beats.Find(a => a.guid_parent == beat.guid);
                                while (beat_depended != null)
                                {
                                    finish_depended = (beat_depended.state == Thread_state.finished);
                                    if (finish_depended)
                                    {
                                        parameters = beat_depended.parameters;
                                        beat_depended = beats.Find(a => a.guid_parent == beat_depended.guid);
                                    }
                                    else
                                    {
                                        if (beat_depended.state == Thread_state.wait)
                                        {
                                            beat_depended.parameters = parameters; //передаем подчиненному параметры из вышестоящего
                                            beat_depended.Start();
                                        }
                                        beat_depended = null;
                                    }
                                }
                                if (finish_depended) //помечаем на удаление подчиненные если последний завершился
                                {
                                    beat_depended = beats.Find(a => a.guid_parent == beat.guid);
                                    while (beat_depended != null)
                                    {
                                        beat_depended.remove = true;
                                        beat_depended = beats.Find(a => a.guid_parent == beat_depended.guid);
                                    }
                                }
                                beat.remove = finish_depended;
                            }
                            break;
                    }
                }
            }
        }
        private void threadSheduleController()
        // Запуск бесконечного цикла работы 
        {
            while (true)
            {
                timeShedule = DateTime.Now;
                if ((timeShedule - dateStart_GetReglament) >= intervalSec_GetReglament && !busy_Reglament && !finishing)  //Не должно быть одновременного редактирования
                    GetReglament();
                //формирование расписания запусков, только если Регламент доступен и не находится в стадии завершения работы
                if (((timeShedule - dateStart_SetBeats) >= interval_SetBeats) && enableReglament && !busy_Reglament && !finishing)
                    SetBeats();
                //Очищаем отмеченные на удаление Биты (remove) 
                if (((timeShedule - dateStart_cleanBeats) >= interval_cleanBeats))
                    cleanBeats();
                //ЗАПУСКИ с отметками в логах и удаление тех у которых наступило время но предыдущий еще работает
                foreach (clsBeat beat in beats)
                    if (beat.guid_parent == Guid.Empty && beat.time <= timeShedule && beat.state == Thread_state.wait)
                    {
                        bool find = false; //Пробежимся по битам в поисках альтенативного в процессе или ожидающего выполнения
                        for (int i = 0; i <= beats.Count - 1; i++)
                        {
                            if ((beats[i].guid_parent == Guid.Empty && 
                                beats[i].time >= beat.time && beats[i].guid != beat.guid) || find) break;
                            if (beats[i].guid_parent == Guid.Empty && beats[i].guid != beat.guid && beats[i].shedule_id == beat.shedule_id &&
                                ((beats[i].state == Thread_state.notresponding || beats[i].state == Thread_state.starting || beats[i].state == Thread_state.wait) ||
                                (beats[i].state == Thread_state.finished && !beats[i].remove)))
                                find = true; ;
                        }
                        if (find) //Если найден альтернативный бит
                            beat.state = Thread_state.ignored;
                        else
                            if (active && !finishing) beat.thread.Start();
                    }

                //Проставляем отметку об удалении
                setRemove_beats();
                //Делаем паузу
                Thread.Sleep(100);
            }
        }
        public void cleanBeats()
        // зачистка битов
        {
            busy_Beats = true;
            dateStart_cleanBeats = timeShedule;
            for (int i = beats.Count - 1; i >= 0; i--)
                if (beats[i].remove) beats.Remove(beats[i]);
            busy_Beats = false;
        }
        public bool GetReglament()
        // Считываем регламент из базы
        {
            busy_Reglament = true;
            bool active_incom = active;
            if (active) active = false;
            DateTime time = DateTime.Now;
            dateStart_GetReglament = time;
            List<clsShedule> shedule_temp = new List<clsShedule>(); 
            SqlConnection Connection = new SqlConnection("uid=sa;pwd=Wedfzx8!;server=SERVER-SHRK\\SQLEXPRESS;database=gate");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText =
                    "SELECT " +
                    "/* from shedule*/ " +
                    "shl.ID SHEDULE_ID, shl.ID_PARENT SHEDULE_ID_PARENT, shl.NAME SHEDULE_NAME, isnull(shl.COMMENT,'') SHEDULE_COMMENT, shl.ACTIVE SHEDULE_ACTIVE, shl.INTERVAL SHEDULE_INTERVAL, shl.TIME_EXECUTION TIME_EXECUTION, " +
                    "shl.WAIT_INTERVAL WAIT_INTERVAL, shl.ACTIVE ACTIVE, shl.DAY_1 DAY_1, shl.DAY_2, shl.DAY_3, shl.DAY_4, shl.DAY_5, shl.DAY_6, shl.DAY_7, " +
                    "convert(varchar,shl.DATETIME_START,20) DATETIME_START, " +
                    "/* from Reglament*/ " +
                    "rgl.ID REGLAMENT_ID, rgl.NAME REGLAMENT_NAME, isnull(rgl.CONECTIONS,'') CONECTIONS, isnull(rgl.FOLDERS,'') FOLDERS, rgl.GATE_NAMETASK GATE_NAMETASK, isnull(rgl.COMMENT,'') REGLAMENT_COMMENT, " +
                    "/* from Tasks*/ " +
                    "tsk.ID TASK_ID, tsk.NAME TASK_NAME, isnull(tsk.COMMENT,'') TASK_COMMENT, " +
                    "/* from Region*/ " +
                    "rgi.ID REGION_ID, rgi.NAME REGION_NAME, isnull(rgi.COMMENT,'') REGION_COMMENT " +
                    "FROM schedules shl " +
                    "join Reglament rgl on rgl.id = shl.ID_REGLAMENT " +
                    "join libTasks tsk on tsk.id = rgl.ID_TASK " +
                    "join libFoms_regions rgi on rgi.id = rgl.ID_FOMS_REGION " +
                    " WHERE shl.TEST = " + ((test)? "1":"0") + " " +
                    "order by shl.ID, shl.ID_PARENT";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            try
            {
                Connection.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    shedule_temp.Clear();
                    while (rdr.Read())
                    {
                        shedule_temp.Add(new clsShedule()
                        {
                            region_id = (int)rdr["REGION_ID"],
                            region_name = (string)rdr["REGION_NAME"],
                            region_comment = (string)rdr["REGION_COMMENT"],
                            reglament_id = (int)rdr["REGLAMENT_ID"],
                            reglament_name = (string)rdr["REGLAMENT_NAME"],
                            reglament_connections = (string)rdr["CONECTIONS"],
                            reglament_folders = (string)rdr["FOLDERS"],
                            gate_nametask = (string)rdr["GATE_NAMETASK"],
                            reglament_comment = (string)rdr["REGLAMENT_COMMENT"],
                            task_id = (int)rdr["TASK_ID"],
                            task_name = (string)rdr["TASK_NAME"],
                            task_comment = (string)rdr["TASK_COMMENT"],
                            shedule_id = (int)rdr["SHEDULE_ID"],
                            shedule_id_parent = (int)rdr["SHEDULE_ID_PARENT"],
                            shedule_name = (string)rdr["SHEDULE_NAME"],
                            shedule_comment = (string)rdr["SHEDULE_COMMENT"],
                            shedule_days = new bool[] {(bool)rdr["DAY_1"],(bool)rdr["DAY_2"],(bool)rdr["DAY_3"],(bool)rdr["DAY_4"],(bool)rdr["DAY_5"],(bool)rdr["DAY_6"],(bool)rdr["DAY_7"]},
                            shedule_interval = (int)rdr["SHEDULE_INTERVAL"],
                            shedule_time_execution = (int)rdr["TIME_EXECUTION"],
                            shedule_wait_interval = (int)rdr["WAIT_INTERVAL"],
                            shedule_active = (bool)rdr["ACTIVE"],
                            shedule_start = DateTime.Parse((string)rdr["DATETIME_START"])
                        });
                    }
                }
                rdr.Close();
                rdr.Dispose();
                for (int i = beats.Count - 1; i >= 0; i--)
                    if (beats[i].time >= time && beats[i].state != Thread_state.starting) beats.Remove(beats[i]);
                shedule = new List<clsShedule>(shedule_temp);
                Connection.Close();
                Connection.Dispose();
                metod_log(new clsLog(DateTime.Now, 1, "shedule", 0, 0, time, DateTime.Now, "Регламент обновлен"));
                dateStart_SetBeats = time - interval_SetBeats; //Сдвигаем последнее время обновления Битов назад для обновления после обновления регламента
                enableReglament = true;
                refreshed = true;
                busy_Reglament = false;
                if (active_incom) active = true;
                return true;
            }
            catch
            {
                metod_log(new clsLog(DateTime.Now, 0, "shedule", 0, 0, time, DateTime.Now, "Не удалось получить Регламент"));
                enableReglament = false;
                busy_Reglament = false;
                if (active_incom) active = true;
                return false;
            }
        }
        private void SetBeats()
        // Формируем Биты на ближайший 1 час, список запусков
        {
            busy_Beats = true;
            dateStart_SetBeats = DateTime.Now; // момент начала создания Битов

            DateTime date_CheckStart = dateStart_SetBeats;
            date_CheckStart = new DateTime(date_CheckStart.Year, date_CheckStart.Month, date_CheckStart.Day, date_CheckStart.Hour, date_CheckStart.Minute, date_CheckStart.Second); 
            //date_CheckStart - TimeSpan.FromMilliseconds(date_CheckStart.Millisecond); - такое удаление миллисекунд дает погрешность
            TimeSpan interval_GetBeat = TimeSpan.FromMinutes(15); //5 минут 
            TimeSpan interval_move = TimeSpan.FromSeconds(1); //1 секунд
            DateTime date_CheckEnd = date_CheckStart + interval_GetBeat;

            DateTime date_Check = date_CheckStart;
            TimeSpan stepReqursion;
            double move;// смещение в секундах от даты старта шедула

            int id = 0;
            Guid guid_parent; //GUID головного бита
            int shedul_id_parent; //ID головного бита
            Guid guid; //GUID бита
            //int shedul_id; //ID бита
            clsShedule shedul_depended; //подчиненная строка расписания

            foreach (clsShedule shedul in shedule)
            {
                if (!shedul.shedule_active || shedul.shedule_id_parent != 0) continue;
                stepReqursion = TimeSpan.FromSeconds(shedul.shedule_interval);

                move = (date_CheckStart - shedul.shedule_start).TotalSeconds % shedul.shedule_interval;
                date_Check = date_CheckStart - TimeSpan.FromSeconds(move);

                while (date_Check <= date_CheckEnd)
                {
                    bool find = false;
                    foreach (clsBeat beat in beats) if (beat.shedule_id == shedul.shedule_id && beat.time == date_Check) {find = true; break;}
                    if (!find && shedul.error_count < 3 && date_Check >= date_CheckStart)
                    {
                        guid_parent = Guid.NewGuid(); //GUID головного бита
                        shedul_id_parent = shedul.shedule_id;
                        beats.Add(
                            new clsBeat(shedul.region_id, shedul.task_id, shedul_id_parent, date_CheckStart, date_Check, guid_parent, Guid.Empty, shedul.gate_nametask,
                                link_connections, shedul.reglament_connections, shedul.reglament_folders,
                                shedul.shedule_wait_interval)
                            );
                        if (beats[id].error == clsErrors.GateError.errorInicializationMetod) shedul.error_count++;
                        else
                        { //формируем подчиненные биты
                            shedul_depended = shedule.Find(a => a.shedule_id_parent == shedul.shedule_id && a.shedule_active == true);
                            while (shedul_depended != null)
                            {
                                guid = Guid.NewGuid();
                                beats.Add(
                                    new clsBeat(shedul_depended.region_id, shedul_depended.task_id, shedul_depended.shedule_id, date_CheckStart, date_Check, guid, guid_parent, shedul_depended.gate_nametask,
                                        link_connections, shedul_depended.reglament_connections, shedul_depended.reglament_folders,
                                        shedul_depended.shedule_wait_interval));
                                guid_parent = guid;
                                shedul_depended = shedule.Find(a => a.shedule_id_parent == shedul_depended.shedule_id && a.shedule_active == true);
                            }
                        }
                        id++;
                    }
                    if (shedul.error_count == 3) shedul.task_name = shedul.task_name + "error";
                    date_Check = date_Check + stepReqursion;
                }
                //-----------------------------------------------------------------------
            }
            beats = beats.OrderBy(a => a.time).ThenBy(b => b.createDate).ToList();
            busy_Beats = false;
        }
    }
    #endregion

 
    #region Класс Расписания clsShedule
    public class clsShedule
    /* Класс процесса-расписания 
     */
    {
        public int region_id;
        // Системный ID в таблице Области ФОМС
        public string region_name;
        // Наименование расписания
        public string gate_nametask;
        // Наименование сопоставляемого метода
        public string region_comment;
        // Комментарий к задаче

        public int reglament_id;
        // Системный ID в таблице Регламента
        public string reglament_name;
        // Наименование расписания
        public string reglament_connections;
        public string reglament_folders;
        // Наименование расписания
        public string reglament_comment;
        // Комментарий к задаче

        public int task_id;
        // Системный ID в таблице задач
        public string task_name;
        // Наименование расписания
        public string task_comment;
        // Комментарий к задаче

        public int shedule_id;
        // Системный ID в таблице расписания базы данных Gateway
        public int shedule_id_parent;
        // Системный ID_PARENT в таблице расписания базы данных Gateway
        public string shedule_name;
        // Наименование расписания
        public DateTime shedule_start;
        // Дата с которой начинает работать задача
        public string shedule_comment;
        // Комментарий к задаче
        public int shedule_type;
        // Типы расписания
        /* 1 - запуск с интервалом и контролем дня недели
         * 2 - запуск в назначенное время с контролем дня недели
         */
        public bool[] shedule_days;
        // Порядковые дни недели в которые должны выполнятся процессы
        /* массив из порядковых номеров дней недели
         */
        public int shedule_interval;
        // Интервал между запусками
        public int shedule_time_execution;
        // Время запуска
        public int shedule_wait_interval;
        // Интервал ожидания выполнения процессы, для оценки зависания
        public bool shedule_active; //расписание запущено или нет
        public Thread shedule_thread;
        public DateTime lastDatetime;
        /* Дата последнего запуска процесса
         * указывается в секундах
         */
        public int error_count = 0;
        public void start_shedule()
        /* Заппуск расписания
         */
        {
            //createThread(); предполагаем что процесс был создан ранее
            shedule_thread.Start();
        }
        public void stop_shedule()
        /* Остановка расписания 
         */
        {
            shedule_active = false;
        }
 
    }
    #endregion

    public class clsGateConfig
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlRootAttribute("configuration", Namespace = "", IsNullable = false)]
        public partial class configurationType
        {

            private reglamentType reglamentField;

            private gatewayType gatewayField;

            private connectionsTypeConnection[] connectionsField;

            private processesType processesField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public reglamentType reglament
            {
                get
                {
                    return this.reglamentField;
                }
                set
                {
                    this.reglamentField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public gatewayType gateway
            {
                get
                {
                    return this.gatewayField;
                }
                set
                {
                    this.gatewayField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("connection", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public connectionsTypeConnection[] connections
            {
                get
                {
                    return this.connectionsField;
                }
                set
                {
                    this.connectionsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public processesType processes
            {
                get
                {
                    return this.processesField;
                }
                set
                {
                    this.processesField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class reglamentType
        {

            private bool enableField;

            private bool testField;

            private string connectionStringField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool enable
            {
                get
                {
                    return this.enableField;
                }
                set
                {
                    this.enableField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool test
            {
                get
                {
                    return this.testField;
                }
                set
                {
                    this.testField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string connectionString
            {
                get
                {
                    return this.connectionStringField;
                }
                set
                {
                    this.connectionStringField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class processesType
        {

            private processesTypeZap[] transport_filesField;

            private processesTypeZap2[] transport_files_inpersonalfolderField;

            private processesTypeZap5[] transport_files_emailField;

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("zap", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public processesTypeZap[] transport_files
            {
                get
                {
                    return this.transport_filesField;
                }
                set
                {
                    this.transport_filesField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("zap", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
            public processesTypeZap2[] transport_files_inpersonalfolder
            {
                get
                {
                    return this.transport_files_inpersonalfolderField;
                }
                set
                {
                    this.transport_files_inpersonalfolderField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("zap", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
            public processesTypeZap5[] transport_files_email
            {
                get
                {
                    return this.transport_files_emailField;
                }
                set
                {
                    this.transport_files_emailField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class processesTypeZap
        {

            private string nameField;

            private int tickField;

            private bool enableField;

            private int nrecordField;

            private string prefixField;

            private bool rewriteField;

            private string commentField;

            private string sourceField;

            private string destinationField;

            private string[] masksField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int tick
            {
                get
                {
                    return this.tickField;
                }
                set
                {
                    this.tickField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool enable
            {
                get
                {
                    return this.enableField;
                }
                set
                {
                    this.enableField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int nrecord
            {
                get
                {
                    return this.nrecordField;
                }
                set
                {
                    this.nrecordField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string prefix
            {
                get
                {
                    return this.prefixField;
                }
                set
                {
                    this.prefixField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool rewrite
            {
                get
                {
                    return this.rewriteField;
                }
                set
                {
                    this.rewriteField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string comment
            {
                get
                {
                    return this.commentField;
                }
                set
                {
                    this.commentField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string source
            {
                get
                {
                    return this.sourceField;
                }
                set
                {
                    this.sourceField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string destination
            {
                get
                {
                    return this.destinationField;
                }
                set
                {
                    this.destinationField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("mask", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
            public string[] masks
            {
                get
                {
                    return this.masksField;
                }
                set
                {
                    this.masksField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class processesTypeZap2
        {

            private string nameField;

            private int tickField;

            private bool enableField;

            private int nrecordField;

            private string prefixField;

            private bool rewriteField;

            private string commentField;

            private string sourceField;

            private string destinationField;

            private string[] recipientsField;

            private string[] masksField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int tick
            {
                get
                {
                    return this.tickField;
                }
                set
                {
                    this.tickField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool enable
            {
                get
                {
                    return this.enableField;
                }
                set
                {
                    this.enableField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int nrecord
            {
                get
                {
                    return this.nrecordField;
                }
                set
                {
                    this.nrecordField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string prefix
            {
                get
                {
                    return this.prefixField;
                }
                set
                {
                    this.prefixField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool rewrite
            {
                get
                {
                    return this.rewriteField;
                }
                set
                {
                    this.rewriteField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string comment
            {
                get
                {
                    return this.commentField;
                }
                set
                {
                    this.commentField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string source
            {
                get
                {
                    return this.sourceField;
                }
                set
                {
                    this.sourceField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string destination
            {
                get
                {
                    return this.destinationField;
                }
                set
                {
                    this.destinationField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("recipient", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string[] recipients
            {
                get
                {
                    return this.recipientsField;
                }
                set
                {
                    this.recipientsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("mask", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
            public string[] masks
            {
                get
                {
                    return this.masksField;
                }
                set
                {
                    this.masksField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class processesTypeZap5
        {

            private string nameField;

            private int tickField;

            private bool enableField;

            private int nrecordField;

            private string commentField;

            private string sourceField;

            private string emailField;

            private string captionField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int tick
            {
                get
                {
                    return this.tickField;
                }
                set
                {
                    this.tickField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool enable
            {
                get
                {
                    return this.enableField;
                }
                set
                {
                    this.enableField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int nrecord
            {
                get
                {
                    return this.nrecordField;
                }
                set
                {
                    this.nrecordField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string comment
            {
                get
                {
                    return this.commentField;
                }
                set
                {
                    this.commentField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string source
            {
                get
                {
                    return this.sourceField;
                }
                set
                {
                    this.sourceField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string email
            {
                get
                {
                    return this.emailField;
                }
                set
                {
                    this.emailField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string caption
            {
                get
                {
                    return this.captionField;
                }
                set
                {
                    this.captionField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class gatewayType
        {

            private gatewayTypeIdentificationSynch identificationSynchField;

            private string[] connectionFoldersField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public gatewayTypeIdentificationSynch identificationSynch
            {
                get
                {
                    return this.identificationSynchField;
                }
                set
                {
                    this.identificationSynchField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("folder", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string[] connectionFolders
            {
                get
                {
                    return this.connectionFoldersField;
                }
                set
                {
                    this.connectionFoldersField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class gatewayTypeIdentificationSynch
        {

            private bool enableField;

            private int tickField;

            private int countFatalRestartsField;

            private int limitCountFatalRestartsField;

            private bool emergencyPuaseEnableField;

            private System.DateTime emergencyPuaseStartField;

            private int emergencyPuaseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool enable
            {
                get
                {
                    return this.enableField;
                }
                set
                {
                    this.enableField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int tick
            {
                get
                {
                    return this.tickField;
                }
                set
                {
                    this.tickField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int countFatalRestarts
            {
                get
                {
                    return this.countFatalRestartsField;
                }
                set
                {
                    this.countFatalRestartsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int limitCountFatalRestarts
            {
                get
                {
                    return this.limitCountFatalRestartsField;
                }
                set
                {
                    this.limitCountFatalRestartsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool emergencyPuaseEnable
            {
                get
                {
                    return this.emergencyPuaseEnableField;
                }
                set
                {
                    this.emergencyPuaseEnableField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.DateTime emergencyPuaseStart
            {
                get
                {
                    return this.emergencyPuaseStartField;
                }
                set
                {
                    this.emergencyPuaseStartField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int emergencyPuase
            {
                get
                {
                    return this.emergencyPuaseField;
                }
                set
                {
                    this.emergencyPuaseField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class connectionsTypeConnection
        {

            private int nrecordField;

            private int connectiontypeField;

            private bool enableField;

            private string nameField;

            private string connectionStringField;

            private int restartIntervalField;

            private bool restartIntervalFieldSpecified;

            private string serverIpField;

            private string programField;

            private string[] connectionFoldersField;

            private string commentsField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int nrecord
            {
                get
                {
                    return this.nrecordField;
                }
                set
                {
                    this.nrecordField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int connectiontype
            {
                get
                {
                    return this.connectiontypeField;
                }
                set
                {
                    this.connectiontypeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public bool enable
            {
                get
                {
                    return this.enableField;
                }
                set
                {
                    this.enableField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string connectionString
            {
                get
                {
                    return this.connectionStringField;
                }
                set
                {
                    this.connectionStringField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public int restartInterval
            {
                get
                {
                    return this.restartIntervalField;
                }
                set
                {
                    this.restartIntervalField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool restartIntervalSpecified
            {
                get
                {
                    return this.restartIntervalFieldSpecified;
                }
                set
                {
                    this.restartIntervalFieldSpecified = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string serverIp
            {
                get
                {
                    return this.serverIpField;
                }
                set
                {
                    this.serverIpField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string program
            {
                get
                {
                    return this.programField;
                }
                set
                {
                    this.programField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            [System.Xml.Serialization.XmlArrayItemAttribute("folder", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
            public string[] connectionFolders
            {
                get
                {
                    return this.connectionFoldersField;
                }
                set
                {
                    this.connectionFoldersField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string comments
            {
                get
                {
                    return this.commentsField;
                }
                set
                {
                    this.commentsField = value;
                }
            }
        }
    }
}