using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;


namespace WindowsFormsApplication3
{
    public enum Thread_state { notInicialized, wait, starting, finished, error, notresponding, broke, ignored, process /*для обозначения законченности задачи, но ожидания завершения подчиненных*/};
    public class clsBeat : clsErrors
    {
        public struct Log_status
        {
            public string text1, text2;
            public string comment;
            public Log_status(string text1_, string text2_, string comment_)
            {
                text1 = text1_;
                text2 = text2_;
                comment = comment_;
            }
        }

        public DateTime time;
        public Guid guid;
        public object parameters; //параметры передаваемые в бит или передаваемые в подчиненый бит 
        public Guid guid_parent = Guid.Empty;
        public int shedule_id;
        public int region_id;
        public int task_id;
        public DateTime createDate;
        public string gate_nametask;
        public int wait_interval;
        public bool remove = false;
        public Thread_state state = Thread_state.notInicialized;
        public Thread thread;
        public GateError error;
        public List<clsConnections> link_connections = new List<clsConnections>(); //ссылка на подключение
        public string folders_connections;
        public string reglament_connections;
        public Queue<Log_status> queue_status = new Queue<Log_status>();
        public clsBeat()
        {
            Inicialization();
        }
        public clsBeat(int region_id_, int task_id_, int shedule_id_, DateTime createDate_,
            DateTime time_, Guid guid_, Guid guid_parent_,
            string gate_nametask_ = "",
            List<clsConnections> link_connections_ = null, string reglament_connections_ = "",
            string folders_connections_ = "",
            int wait_interval_ = 0, object parameters_ = null)
        {
            region_id = region_id_;
            task_id = task_id_;
            shedule_id = shedule_id_;
            createDate = createDate_;
            time = time_;
            guid = guid_;
            guid_parent = guid_parent_;
            gate_nametask = gate_nametask_;
            link_connections = link_connections_;
            reglament_connections = reglament_connections_;
            folders_connections = folders_connections_;
            wait_interval = wait_interval_;
            parameters = parameters_;
            Inicialization();
        }
        public void Start()
        // Запускаем поток задачи
        {
            if (state == Thread_state.wait && time < DateTime.Now)
            {
                try
                {
                    thread.Start();
                }
                catch
                {
                    queue_status.Enqueue(new Log_status(thread.Name, string.Empty, "обшика при старте метоа"));
                }
            }
        }
        public void Stop()
        // Останавливаем поток задачи по требованию
        {
            if (state == Thread_state.starting)
            {
                thread.Abort();
                thread.Join();
                state = Thread_state.broke;
                error = GateError.StopedMetodOnDemand;
            }
        }
        private void Inicialization()
        // Назначение метода для данной задачи
        // определяет наличие в классе соответствующего метода по имени 
        {
            bool result = true;
            switch (gate_nametask)
            {
                case "import_prikreplenie": thread = new Thread(import_prikreplenie) { IsBackground = true, Name = gate_nametask }; break;
                case "control_inostr": thread = new Thread(control_inostr) { IsBackground = true, Name = gate_nametask }; break;
                case "upload_polis_goznak": thread = new Thread(upload_polis_goznak) { IsBackground = true, Name = gate_nametask }; break;
                case "import_military": thread = new Thread(import_military) { IsBackground = true, Name = gate_nametask }; break;
                case "handling_military": thread = new Thread(handling_military) { IsBackground = true, Name = gate_nametask }; break;
                case "unloading_protocols_military": thread = new Thread(unloading_protocols_military) { IsBackground = true, Name = gate_nametask }; break;
                case "find_narushsrokov": thread = new Thread(find_narushsrokov) { IsBackground = true, Name = gate_nametask }; break;
                case "unloading_narushsrokov": thread = new Thread(unloading_narushsrokov) { IsBackground = true, Name = gate_nametask }; break;
                case "unloading_prikreplenie_AnswerQueryUsers": thread = new Thread(unloading_prikreplenie_AnswerQueryUsers) { IsBackground = true, Name = gate_nametask }; break;
                case "upload_io": thread = new Thread(upload_io) { IsBackground = true, Name = gate_nametask }; break;

                case "identification_smev_mp": thread = new Thread(identification_smev_mp) { IsBackground = true, Name = gate_nametask }; break;
                case "identification_gate": thread = new Thread(identification_gate) { IsBackground = true, Name = gate_nametask }; break;
                case "identification_history_gate": thread = new Thread(identification_history_gate) { IsBackground = true, Name = gate_nametask }; break;
                case "identification_create_response_01": thread = new Thread(identification_create_response_01) { IsBackground = true, Name = gate_nametask }; break;
                case "cleaner_identificationPeople": thread = new Thread(cleaner_identificationPeople) { IsBackground = true, Name = gate_nametask }; break;
                case "serverC_get_request_idetification": thread = new Thread(serverC_get_request_idetification) { IsBackground = true, Name = gate_nametask }; break;
                case "ServerC_unload_identification": thread = new Thread(ServerC_unload_identification) { IsBackground = true, Name = gate_nametask }; break;
                case "gateBackup": thread = new Thread(gateBackup) { IsBackground = true, Name = gate_nametask }; break;
                case "eirBackup": thread = new Thread(eirBackup) { IsBackground = true, Name = gate_nametask }; break;
                case "unloading_ZLDNforSMO": thread = new Thread(unloading_ZLDNforSMO) { IsBackground = true, Name = gate_nametask }; break;
                case "responseAsync_SMEV": thread = new Thread(responseAsync_SMEV) { IsBackground = true, Name = gate_nametask }; break;

                // ----------------------- Gate 
                case "gate_handling_files": thread = new Thread(gate_handling_files) { IsBackground = true }; break;
                case "gate_create_response_on_eir": thread = new Thread(gate_create_response_on_eir) { IsBackground = true, Name = gate_nametask }; break;
                case "gate_send_response_from_eir": thread = new Thread(gate_send_response_from_eir) { IsBackground = true, Name = gate_nametask }; break;

                case "gate_get_flk_from_eir": thread = new Thread(gate_get_flk_from_eir) { IsBackground = true, Name = gate_nametask }; break;
                case "gate_get_prt_from_eir": thread = new Thread(gate_get_prt_from_eir) { IsBackground = true, Name = gate_nametask }; break;
                case "gate_get_info_from_eir": thread = new Thread(gate_get_info_from_eir) { IsBackground = true, Name = gate_nametask }; break;

                case "gate_get_request_identification_from_eir": thread = new Thread(gate_get_request_identification_from_eir) { IsBackground = true, Name = gate_nametask }; break;
                case "gate_send_response_identification_to_eir": thread = new Thread(gate_send_response_identification_to_eir) { IsBackground = true, Name = gate_nametask }; break;

                case "gate_send_pids_to_eir": thread = new Thread(gate_send_pids_to_eir) { IsBackground = true, Name = gate_nametask }; break;
                                                                                                                                                                                     // Reports --------
                case "gate_report_si_schema_1_0": thread = new Thread(gate_report_si_schema_1_0) { IsBackground = true, Name = gate_nametask }; break;
                // ----------------------- EIR
                case "eir_event_flk": thread = new Thread(eir_event_flk) { IsBackground = true, Name = gate_nametask }; break;
                case "eir_event_identy": thread = new Thread(eir_event_identy) { IsBackground = true, Name = gate_nametask }; break; //new
                case "eir_event_prt": thread = new Thread(eir_event_prt) { IsBackground = true, Name = gate_nametask }; break; //new
                case "eir_event_import": thread = new Thread(eir_event_import) { IsBackground = true, Name = gate_nametask }; break;

                default: result = false; break;
            }
            if (result) state = Thread_state.wait;
            else
            {
                state = Thread_state.error;
                error = GateError.errorInicializationMetod;
            }
        }

        //----------------------------------- методы задач 

        public void import_prikreplenie()
        // Загрузка прикрепления 
        {
            state = Thread_state.starting;
            string[] folders = null;
            string[] dirs = null;
            string folder_in;
            string folder_out;
            //queue_status.Enqueue(new Log_status("import_prikreplenie", string.Empty, "Начали!"));
            try
            {
                folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                folder_in = folders[0];
                folder_out = folders[1];
                dirs = Directory.GetFiles(folder_in, "MO128*.csv");
                if (dirs.Count() == 0)
                {
                    state = Thread_state.finished;
                    return;
                }
                //queue_status.Enqueue(new Log_status("import_prikreplenie", dirs.Count().ToString(), "Файлов найдено"));
                #region Загрузка файлов
                #region
                List<clsLibrary.Id_row> list_fileId = new List<clsLibrary.Id_row>();
                //создаём таблицу
                System.Data.DataTable table = new System.Data.DataTable();
                //добавляем колонки в таблицу
                table.Columns.AddRange(new DataColumn[] {
                new DataColumn("NREC", typeof(String)), new DataColumn("MO_LOG", typeof(String)), new DataColumn("OP", typeof(String)), new DataColumn("TDOC", typeof(String)),
                new DataColumn("SPOL", typeof(String)), new DataColumn("NPOL", typeof(String)), new DataColumn("ENP", typeof(String)), new DataColumn("FAM", typeof(String)),
                new DataColumn("IM", typeof(String)), new DataColumn("OT", typeof(String)), new DataColumn("DR", typeof(String)), new DataColumn("MR", typeof(String)),
                new DataColumn("DOCTP", typeof(String)), new DataColumn("DOCS", typeof(String)), new DataColumn("DOCN", typeof(String)), new DataColumn("DOCDT", typeof(String)),
                new DataColumn("DOCORG", typeof(String)), new DataColumn("SS", typeof(String)), new DataColumn("LPU", typeof(String)), new DataColumn("LPUAUTO", typeof(String)),
                new DataColumn("LPUTYPE", typeof(String)), new DataColumn("LPUDT", typeof(String)), new DataColumn("LPUDX", typeof(String)), new DataColumn("OID", typeof(String)),
                new DataColumn("SUBDIV", typeof(String)), new DataColumn("DISTRICT", typeof(String)), new DataColumn("SS_DOCTOR", typeof(String)), new DataColumn("KATEG", typeof(String)),
                new DataColumn("DOCDATE", typeof(String))});
                #endregion

                foreach (string dir in dirs) //перебираем файлы
                {
                    //queue_status.Enqueue(new Log_status("import_prikreplenie", "начали файл - ", Path.GetFileName(dir)));
                    string smo_sender = Path.GetFileNameWithoutExtension(dir).Substring(3, 5);
                    int MO_LOG = clsLibrary.InsertNameFile(Path.GetFileName(dir));
                    //queue_status.Enqueue(new Log_status("import_prikreplenie", MO_LOG.ToString(), "Вставка имени файла"));
                    if (MO_LOG == -1)
                    {
                        string file_back = folder_in + smo_sender + @"\exist" + Path.GetFileName((string)dir);
                        if (File.Exists(file_back)) File.Delete(file_back);
                        File.Move(dir, file_back);
                        queue_status.Enqueue(new Log_status(Path.GetFileName((string)dir), string.Empty, "загружен ранее, возвращен"));
                        continue;
                    }
                    DataRow row = null;
                    int row_rec = 0;
                    int row_newrec = 0;
                    string[] tblValues = null;
                    string[] tblLines = null;
                    List<clsLibrary.Id_row> tblLine_failed = new List<clsLibrary.Id_row>();
                    //queue_status.Enqueue(new Log_status("import_prikreplenie", string.Empty, "Начинаем чтение"));
                    try //контроль ошибки чтения файла
                    {
                        tblLines = File.ReadAllLines((string)dir, Encoding.Default);
                        //queue_status.Enqueue(new Log_status("import_prikreplenie", tblLines.Length.ToString(),"Записей - "));
                        
                        table.Rows.Clear();
                        for (int i = 0; i < tblLines.Length; i++)
                        {
                            row_rec++;
                            try //контроль на ошибки в строках 
                            {
                                #region
                                tblValues = tblLines[i].Split(';');
                                string SPOL = "", NPOL = tblValues[2];
                                int index = NPOL.IndexOf("№");
                                if (index >= 0)
                                {
                                    SPOL = NPOL.Substring(0, index);
                                    NPOL = NPOL.Substring(index + 1);
                                }
                                row = table.NewRow();
                                row["NREC"] = row_rec; row["MO_LOG"] = MO_LOG; row["OP"] = clsLibrary.string_ForDB(tblValues[0]);
                                row["TDOC"] = clsLibrary.string_ForDB(tblValues[1]); row["SPOL"] = clsLibrary.string_ForDB(SPOL); row["NPOL"] = clsLibrary.string_ForDB(NPOL);
                                row["ENP"] = clsLibrary.string_ForDB(tblValues[3]); row["FAM"] = clsLibrary.string_ForDB(tblValues[4]); row["IM"] = clsLibrary.string_ForDB(tblValues[5]);
                                row["OT"] = clsLibrary.string_ForDB(tblValues[6]); row["DR"] = clsLibrary.string_ForDB(tblValues[7]); row["MR"] = clsLibrary.string_ForDB(tblValues[8]);
                                row["DOCTP"] = clsLibrary.string_ForDB(tblValues[9]); row["DOCS"] = clsLibrary.string_ForDB(string.Empty); row["DOCN"] = clsLibrary.string_ForDB(tblValues[10]);
                                row["DOCDT"] = clsLibrary.string_ForDB(tblValues[11]); row["DOCORG"] = clsLibrary.string_ForDB(tblValues[12]); row["SS"] = clsLibrary.string_ForDB(tblValues[13]);
                                row["LPU"] = clsLibrary.string_ForDB(tblValues[14]); row["LPUAUTO"] = clsLibrary.string_ForDB(tblValues[15]); row["LPUTYPE"] = clsLibrary.string_ForDB(tblValues[16]);
                                row["LPUDT"] = clsLibrary.string_ForDB(tblValues[17]); row["LPUDX"] = clsLibrary.string_ForDB(tblValues[18]); row["OID"] = clsLibrary.string_ForDB(tblValues[19]);
                                row["SUBDIV"] = clsLibrary.string_ForDB(tblValues[20]); row["DISTRICT"] = clsLibrary.string_ForDB(tblValues[21]); row["SS_DOCTOR"] = clsLibrary.string_ForDB(tblValues[22]);
                                if (tblValues.Count() >= 24) row["KATEG"] = clsLibrary.string_ForDB(tblValues[23]); else row["KATEG"] = clsLibrary.string_ForDB(string.Empty);
                                if (tblValues.Count() >= 25) row["DOCDATE"] = clsLibrary.string_ForDB(tblValues[24]); else row["DOCDATE"] = clsLibrary.string_ForDB(string.Empty);
                                table.Rows.Add(row);
                                #endregion
                            }
                            catch(Exception e) //добавляем в список ошибочных записи
                            {
                                //queue_status.Enqueue(new Log_status("import_prikreplenie", "Ошибка в записи - ", tblLines[i]));
                                tblLine_failed.Add(new clsLibrary.Id_row(i + 1, tblLines[i]));
                                //queue_status.Enqueue(new Log_status(dir, string.Empty, "Ошибка чтения"));
                            }
                        }
                        #region
                        List<string> values = new List<string>();
                        foreach (DataRow new_row in table.Rows)
                        {
                            values.Add(
                                new_row["NREC"].ToString() + "," + new_row["MO_LOG"].ToString() + "," + clsLibrary.string_Apostrophe(new_row["OP"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["TDOC"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["SPOL"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["NPOL"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["ENP"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["FAM"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["IM"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["OT"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["DR"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["MR"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["DOCTP"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["DOCS"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["DOCN"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["DOCDT"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["DOCORG"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["SS"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["LPU"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["LPUAUTO"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["LPUTYPE"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["LPUDT"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["LPUDX"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["OID"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["SUBDIV"].ToString()) + "," +
                                clsLibrary.string_Apostrophe(new_row["DISTRICT"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["SS_DOCTOR"].ToString()) + "," + clsLibrary.string_Apostrophe(new_row["KATEG"].ToString())
                            //+ "," + clsLibrary.string_Apostrophe(new_row["DOCDT"].ToString())
                            );
                        }
                        #endregion
                        List<int> list_error =
                                clsLibrary.execQuery_insertList_list(
                                "uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;",
                                "INSERT INTO srz3_00.dbo.MO_BUFFER ([NREC],[MO_LOG],[OP],[TDOC],[SPOL],[NPOL],[ENP],[FAM],[IM],[OT],[DR],[MR],[DOCTP],[DOCS],[DOCN],[DOCDT],[DOCORG],[SS],[LPU],[LPUAUTO],[LPUTYPE],[LPUDT],[LPUDX],[OID],[SUBDIV],[DISTRICT],[SS_DOCTOR],[KATEG]) VALUES ",
                                values);

                        if (list_error == null)
                            queue_status.Enqueue(new Log_status(Path.GetFileName((string)dir), string.Empty, "Ошибка записи в БД данных"));
                        else
                        {
                            foreach (int i in list_error) //добавляем в список ошибочных записи не загрузившиеся в БД
                            {
                                tblLine_failed.Add(new clsLibrary.Id_row(i + 1, tblLines[i]));
                            }
                        }
                        tblLine_failed = tblLine_failed.OrderBy(a => a.id).ToList();
                        string old_file = Path.GetDirectoryName(dir) + @"\ok\" + Path.GetFileName((string)dir);
                        if (File.Exists(old_file))
                            File.Move(old_file, Path.GetDirectoryName(old_file) + "/" + Path.GetFileNameWithoutExtension(old_file) + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + Path.GetExtension(old_file));
                        File.Move((string)dir, old_file);

                        row_newrec = row_rec - tblLine_failed.Count; //колличество записей загруженных
                        queue_status.Enqueue(new Log_status(Path.GetFileName((string)dir), "записей принято " + (string)(row_newrec).ToString() + " из " + row_rec.ToString(), "Загружен"));
                        if (tblLine_failed.Count > 0) // при наличии ошибочных записей создаем одноименный текстовый файл с префиксом error
                        {
                            string file_error_first = Path.GetDirectoryName(dir) + @"\err" + Path.GetFileName((string)dir);
                            string file_error_last = folder_out /*+ smo_sender + @"\" */+ Path.GetFileName((string)file_error_first);
                            clsLibrary.createFileTXT_FromListAndId(tblLine_failed, file_error_first);
                            if (File.Exists(file_error_last)) File.Delete(file_error_last);
                            File.Move(file_error_first, file_error_last);
                            queue_status.Enqueue(new Log_status(file_error_last, "записей " + tblLine_failed.Count().ToString(), "Выгружены ошибки"));
                        }
                        //Обновляем данные по числу в файле строк и принятых
                        clsLibrary.execQuery(
                                "uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;",
                                "UPDATE srz3_00.dbo.MO_LOG SET NREC = " + row_newrec.ToString() + ", NERR = 0 WHERE ID = " + MO_LOG.ToString()
                            );
                        list_fileId.Add(new clsLibrary.Id_row(MO_LOG, Path.GetFileNameWithoutExtension(dir))); //добавляем ссылку на загруженный файл
                    }
                    catch // ошибка чтения файла
                    {
                        queue_status.Enqueue(new Log_status(dir, string.Empty, "Ошибка чтения"));
                    }
                } //цикл обхода файлов
                  //--------------------------------------------------
                #endregion
                #region Обработка файлов
                foreach (clsLibrary.Id_row list_file in list_fileId) //обходим все загруженные файлы
                {
                    string smo_sender = list_file.row.Substring(3, 5);
                    int result_int = 0; //результат выполнения запроса, определяет какая выполнилась комманда в запросе по очереди
                    try
                    {
                        result_int = clsLibrary.execQuery_getInt(
                            "uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;",
                            String.Format("DECLARE @return_value int EXEC @return_value = dbo.gate_upload_prikreplenie @MO_LOG={0} SELECT @return_value", list_file.id)
                            );
                    }
                    catch
                    { // нет лога ошибки процесса выполнения запроса
                        queue_status.Enqueue(
                            new Log_status(
                                list_file.row.Substring(3),
                                "",
                                "Ошибка проведения идентификации"
                                ));
                    }
                    if (result_int < 7 /*колличество задач в процессе*/ ) continue; //идем к началу цикла
                    List<string> result_list = new List<string>(clsLibrary.execQuery_getListString(
                            "uid=sa;pwd=Cvbqwe2!;server=server-r;database=srz3_00;",
                            String.Format("select convert(varchar,NREC)+';'+ENP+';'+CASE WHEN PEOPLE IS NULL THEN '404' else RINTECHERR END RINTECHERR from MO_BUFFER where MO_LOG={0} AND (isnull(RINTECHERR,'')<>'' OR PEOPLE IS NULL) ORDER BY NREC", list_file.id)
                        ));
                    int result_list_count = result_list.Count();
                    if (!(result_list_count > 0)) //если нет ошибок то тело ФЛК будет N
                        result_list.Add("N");
                    if (clsLibrary.createFileTXT_FromList(result_list,
                        Path.Combine(folder_out, String.Format("LO1{0}.csv", list_file.row.Substring(3)))
                        ))
                        queue_status.Enqueue(
                            new Log_status(
                                Path.Combine(folder_out, String.Format("LO1{0}.csv", list_file.row.Substring(3))),
                                "записей - " + result_list_count.ToString(),
                                String.Format("Сформирован файл ФЛК на {0}", list_file.row)));
                    else
                        queue_status.Enqueue(
                            new Log_status(
                                Path.Combine(folder_out, String.Format("LO1{0}.csv", list_file.row.Substring(3))),
                                "записей - 0",
                                String.Format("Ошибка формирования файла ФЛК на {0}", list_file.row)));
                }
                #endregion
                state = Thread_state.finished;
            }
            catch(Exception e)
            {
                queue_status.Enqueue(
                            new Log_status("import_prikreplenie", string.Empty,e.Message));
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
                return;
            }            
        }

        public void control_inostr()
        // Контроль окончания срока права на ОМС у иностр.
        {
            state = Thread_state.starting;
            int CloseInostrDays = 30; //контролируются предыдущие 30 дней
            int result_int = 2; //результат выполнения запроса, определяет сколько закрыто иностранцев шаг начинается с 2+1
            try
            {
                result_int = clsLibrary.execQuery_getInt(
                    "uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;",
                    String.Format("DECLARE @return_value int EXEC @return_value = dbo.GATE_control_inostr @DAYS = {0} SELECT @return_value", CloseInostrDays,
                    wait_interval)
                    );
                if (result_int < 2) //ошибка выполнения
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
                else
                    if (result_int > 2) //есть результирующие записи
                {
                    queue_status.Enqueue(
                        new Log_status(
                            gate_nametask,
                             String.Format("иностранцы - {0}", (result_int - 2).ToString()),
                            "Закрытие страхования"));
                }
                /*else 
                {
                    queue_status.Enqueue(
                        new Log_status(
                            gate_nametask,"не найдено","Закрытие страхования"));
                }*/
                state = Thread_state.finished;
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void upload_polis_goznak()
        // Загрузка файлов сформированных на сайте ГОЗНАКа, о выпущенных полисах, по факту получения АОФОМС
        {
            state = Thread_state.starting;
            List<string> list = new List<string>();
            string[] folders = null;
            string[] files = null;
            string folder_in;
            string folder_out;
            try
            {
                folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                folder_in = folders[0];
                folder_out = folders[1];
                files = Directory.GetFiles(folder_in, "gzn*.csv");
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
                return;
            }

            foreach (string file in files)
            {
                try
                {
                    list.Clear();
                    string[] gznValues = null;
                    string[] gznLines = File.ReadAllLines((string)file, Encoding.Default);
                    int count_row = 0;
                    for (int i = 0; i < gznLines.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(gznLines[i]))
                        {
                            gznValues = gznLines[i].Split(';');
                            string ENP1 = "", VS1 = "", ENP2 = "", VS2 = "", ENP3 = "", VS3 = "";
                            //создаём новую строку
                            ENP1 = gznValues[0];
                            VS1 = gznValues[1];
                            ENP2 = gznValues[2];
                            VS2 = gznValues[3];
                            ENP3 = gznValues[4];
                            VS3 = gznValues[5];
                            //string Value = "";
                            if (ENP1.Length == 16 && VS1.Length == 11) { count_row++; list.Add(clsLibrary.string_Apostrophe(ENP1) + "," + clsLibrary.string_Apostrophe(VS1)); }
                            if (ENP2.Length == 16 && VS2.Length == 11) { count_row++; list.Add(clsLibrary.string_Apostrophe(ENP2) + "," + clsLibrary.string_Apostrophe(VS2)); }
                            if (ENP3.Length == 16 && VS3.Length == 11) { count_row++; list.Add(clsLibrary.string_Apostrophe(ENP3) + "," + clsLibrary.string_Apostrophe(VS3)); }
                        }
                    }
                    if (list.Count > 0)
                    {
                        if (!clsLibrary.execQuery_insertList_bool("uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;", "INSERT INTO tmpForSRZ.dbo.GOZNAK (ENP, BLANK) VALUES ", list))
                            queue_status.Enqueue(
                                new Log_status(
                                    gate_nametask,
                                     "вставка данных в базу",
                                    "ошибка"));

                        if (clsLibrary.execQuery_getInt("uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;", "DECLARE @return_value int EXEC @return_value = tmpForSRZ.dbo.GATE_insert_npol SELECT 'Return Value' = @return_value") == -1)
                            queue_status.Enqueue(
                                new Log_status(
                                    gate_nametask,
                                     "расстановка номеров полисов",
                                    "ошибка"));
                    }
                    queue_status.Enqueue(
                        new Log_status(
                            Path.GetFileName((string)file),
                             String.Format("информация о полисах - {0}", count_row.ToString()),
                            "Сведения ГОЗНАКа"));
                    //File.Copy((string)dir, @"D:\Tmp\" + Path.GetFileName((string)dir), true);
                    File.Copy((string)file, folder_out + @"\28004\" + Path.GetFileName((string)file), true);
                    File.Delete((string)file);

                }
                catch
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
            }
            state = Thread_state.finished;
        }

        public void import_military()
        // Загрузка данных о военнослужащих, получивших МП от МО ОМС
        {
            state = Thread_state.starting;
            string[] folders = null;
            string[] files = null;
            string folder_in;
            try
            {
                folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                folder_in = folders[0];
                files = Directory.GetFiles(folder_in, "military*.csv");
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
                return;
            }
            foreach (string file in files)
            {
                try
                {
                    int count = clsLibrary.import_FileInDB(
                            file,
                            1, //пропускаемые строки
                            8,
                            "set dateformat dmy INSERT INTO tmpForSRZ.dbo.military (LPU,FAM,IM,OT,DR,DSTART,DFINISH,COMMENTS) VALUES ",
                            "uid=sa;pwd=Cvbqwe2!;server=server-r",
                            "database=tmpForSRZ"
                        );
                    if (count != -1)
                        queue_status.Enqueue(new Log_status(Path.GetFileName((string)file), String.Format("записей принято - {0}", count), "Сведения о военнослужащих"));
                    else
                        queue_status.Enqueue(new Log_status(gate_nametask, "вставка данных в базу", "ошибка"));

                    File.Copy(file, folder_in + @"ok\" + Path.GetFileName(file), true);
                    File.Delete(file);
                }
                catch
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
            }
            state = Thread_state.finished;
        }

        public void handling_military()
        // Обработка данных о военнослужащих, получивших МП от МО ОМС
        {
            state = Thread_state.starting;
            try
            {
                int count = clsLibrary.execQuery_getInt("uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;", "DECLARE @return_value int EXEC @return_value = tmpForSRZ.dbo.military_close SELECT 'Return Value' = @return_value");
                if (count == -1) throw new ArgumentException("Ошибка");
                if (count > 0) queue_status.Enqueue(new Log_status(gate_nametask, String.Format("случаев - {0}", count.ToString()), "Закрытие страхования военнослужащих"));
                state = Thread_state.finished;
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void unloading_protocols_military()
        // Выгрузка протоколов обработки данных о военнослужащих, получивших МП от МО ОМС
        {
            state = Thread_state.starting;
            string[] folders = null;
            try
            {
                folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                string folder = folders[1]; //исходящие данные
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
                return;
            }
            try
            {
                // выполняем процесс
                //Thread.Sleep(10000);
                for (int i = 0; i < 10000; i++) i = i; //Эмитация процесса
                state = Thread_state.finished;
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void find_narushsrokov()
        // Поиск случаев нарушения сроков представления сведений по ЗЛ СМО
        {
            state = Thread_state.starting;
            parameters = 2;
            string connection = string.Empty;
            try
            {
                clsLibrary.get_stringSplitPos(ref connection, reglament_connections, ';', 0);
                parameters = clsLibrary.execQuery_getInt(
                    @link_connections.Find(x => x.name == connection).connectionString + ";database=tmpForSRZ;",
                    //"uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;",
                    "DECLARE @return_value int EXEC @return_value = dbo.GATE_find_narushsrokov SELECT @return_value",
                    wait_interval
                    );
                if ((int)parameters < 2) //ошибка выполнения
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
                else
                {
                    queue_status.Enqueue(
                        new Log_status(
                            gate_nametask,
                             String.Format("случаев - {0}", ((int)parameters - 2).ToString()),
                            "Нарушение сроков"));
                }
                state = Thread_state.finished;
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void unloading_narushsrokov()
        // Выгрузка найденых случаев нарушения сроков представления сведений
        {
            state = Thread_state.starting;
            if ((int)parameters > 2) //есть ли причина выполнения бита
            {
                try
                {
                    string[] folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                    string folder = folders[1];
                    string connection = string.Empty;
                    clsLibrary.get_stringSplitPos(ref connection, reglament_connections, ';', 0);
                    string connectionString = @link_connections.Find(x => x.name == connection).connectionString;

                    System.Data.DataTable table = new System.Data.DataTable();
                    //добавляем колонки в таблицу
                    table.Columns.AddRange(new DataColumn[] {
                        new DataColumn("col1", typeof(String)),
                        new DataColumn("col2", typeof(String)),
                        new DataColumn("col3", typeof(String)),
                        new DataColumn("col4", typeof(String)),
                        new DataColumn("col5", typeof(String)),
                        new DataColumn("col6", typeof(String)),
                        new DataColumn("col7", typeof(String)),
                        new DataColumn("col8", typeof(String)),
                        new DataColumn("col9", typeof(String)),
                        new DataColumn("col10", typeof(String)),
                        new DataColumn("col11", typeof(String)),
                        new DataColumn("col12", typeof(String)),
                        new DataColumn("col13", typeof(String))
                    });
                    bool result = clsLibrary.ExecQurey_GetTable(
                        connectionString + ";database=tmpForSRZ;",
                        //"uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ",
                        "SELECT ID, CONVERT(varchar(10), dviz, 104) dv, CONVERT(varchar(10), date_income, 104) date_income,CONVERT(varchar,inwork) inwork,CONVERT(varchar,q) q,npol, " +
                        "CONVERT(varchar(10), dbeg, 104) dbeg, '-' + isnull(enp,'') + '-' enp, isnull(fam,'') fam, isnull(im,'') im,isnull(ot,'') ot, CONVERT(varchar(10), dr, 104) dr,op " +
                        "FROM tmpForSRZ.dbo.control_polis_incom " +
                        "where send = 0 order by dviz",
                        ref table,
                        wait_interval);

                    if (!result)
                    {
                        state = Thread_state.error;
                        error = GateError.errorPerformanceMetod;
                        return;
                    }
                    List<string> result_list = new List<string>();
                    string parameter = "";
                    foreach (DataRow row in table.Rows)
                    {
                        result_list.Add(row[1] + ";" + row[2] + ";" + row[3] + ";" + row[4] + ";" + row[5] + ";" + row[6] + ";" + row[7] + ";" + row[8] + ";" + row[9] + ";" + row[10] + ";" + row[11] + ";" + row[12]);
                        parameter += "," + (string)row[0];
                    }
                    parameter = parameter.Substring(1);
                    int result_list_count = result_list.Count();
                    if (result_list_count > 0)
                    {
                        string strFile = String.Format(@"{0}FOMS\Bogomaz\narush_{1}.csv", folder, DateTime.Now.ToString("yyyyMMdd-HHmm"));
                        if (clsLibrary.createFileTXT_FromList(result_list, strFile))
                        {
                            queue_status.Enqueue(
                                new Log_status(
                                    strFile,
                                    "записей - " + result_list_count.ToString(),
                                    String.Format("Сформирован файл на {0}", result_list_count)));
                            clsLibrary.execQuery(
                                connectionString + ";database=tmpForSRZ;",
                                //"uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;",
                                string.Format("update dbo.control_polis_incom set send = 1 where id in ({0})", parameter),
                            wait_interval);
                            state = Thread_state.finished;
                        }
                        else
                        {
                            queue_status.Enqueue(
                                new Log_status(
                                    strFile,
                                    "",
                                    "Ошибка формирования файла "));
                            state = Thread_state.error;
                            error = GateError.errorPerformanceMetod;
                        }
                    }
                    else
                        state = Thread_state.finished;
                }
                catch
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
            }
            else
                state = Thread_state.finished;
        }

        public void unloading_prikreplenie_AnswerQueryUsers()
        // Формирование сведений о прикреплении по запросам
        {
            state = Thread_state.starting;
            try
            {
                List<string> list = new List<string>();
                string connection0 = "";
                string connection1 = "";
                string connection2 = "";
                //Получаем подключения
                clsLibrary.get_stringSplitPos(ref connection0, reglament_connections, ';', 0);
                clsLibrary.get_stringSplitPos(ref connection1, reglament_connections, ';', 1);
                clsLibrary.get_stringSplitPos(ref connection2, reglament_connections, ';', 2);

                string connectionString0 = @link_connections.Find(x => x.name
                    == connection0
                    ).connectionString + ";database=tmpForSRZ";
                string connectionString1 = @link_connections.Find(x => x.name
                    == connection1
                    ).connectionString + ";database=GATE";
                string connectionString2 = @link_connections.Find(x => x.name
                    == connection2
                    ).connectionString + ";database=SRZ_SUPPORT";

                list = clsLibrary.execQuery_getListString(
                        connectionString2,
                        "select top 1 convert(varchar,ID)+';'+ convert(varchar,BIDDER)+ ';' + convert(varchar,TYP) + ';' + convert(varchar,USER_ID) from SRZ_SUPPORT.dbo.QUERY_USERS where ACCEPT=0 order by ID"
                        );
                foreach (string row in list)
                {
                    if (!String.IsNullOrEmpty(row)) //есть в строке значения 
                    {
                        string[] columns = row.Split(';');
                        if (clsLibrary.execQuery_getInt(
                            connectionString0,
                            String.Format("DECLARE @return_value int EXEC @return_value = dbo.createQUERY_USER_ANSWER {0}, {1}, {2}, {3} SELECT @return_value", columns[1], columns[2], columns[3], columns[0])
                        ) == -1)
                        {
                            throw new Exception();
                        }

                        if (clsLibrary.execQuery_getInt(
                            connectionString1,
                            String.Format("DECLARE @return_value int EXEC @return_value = dbo.createFile_QUERY_USER_ANSER {0}, {1}, {2}, {3} SELECT @return_value", columns[1], columns[2], columns[3], columns[0])
                        ) == -1)
                        {
                            throw new Exception();
                        }


                        if (clsLibrary.execQuery_getInt(
                            connectionString2,
                            String.Format("set dateformat dmy update QUERY_USERS set ACCEPT=1, ACCEPT_DATE='{0}' where ID={1} SELECT @@ROWCOUNT", DateTime.Now.ToString(), columns[0])
                        ) == -1)
                        {
                            throw new Exception();
                        }

                        queue_status.Enqueue(
                            new Log_status(
                                columns[0] + "|" + columns[1] + "|" + columns[3],
                                "по прикреплению",
                                "Выгружены данные"));
                    }
                }
                state = Thread_state.finished;
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void upload_io()
        // Загрузка имен и отчеств полученных от СМО в справочники
        {
            state = Thread_state.starting;
            List<string> list = new List<string>();
            string[] folders = null;
            string[] files = null;
            string folder_in;
            string folder_out;
            try
            {
                folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                folder_in = folders[0];
                folder_out = folders[1];
                files = Directory.GetFiles(folder_in, "errors_21-22*.xls");
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
                return;
            }
            foreach (string file in files)
            {
                try
                {
                    if (!File.Exists(file)) return;
                    DataSet ds = new DataSet();
                    string connectionString = clsLibrary.GetConnectionString_XLS(file);
                    //MessageBox.Show(/*list_table.Count.ToString()+*/"-2");
                    using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connectionString))
                    {
                        //MessageBox.Show(/*list_table.Count.ToString()+*/"-1");
                        conn.Open();
                        System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand();
                        cmd.Connection = conn;
                        // Get all Sheets in Excel File
                        //MessageBox.Show(/*list_table.Count.ToString()+*/"0");
                        System.Data.DataTable dtSheet = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                        // Loop through all Sheets to get data
                        //MessageBox.Show(/*list_table.Count.ToString()+*/"1");
                        foreach (DataRow dr in dtSheet.Rows)
                        {
                            string sheetName = dr["TABLE_NAME"].ToString();
                            // Get all rows from the Sheet
                            cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                            System.Data.DataTable dt = new System.Data.DataTable();
                            dt.TableName = sheetName;

                            System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(cmd);
                            da.Fill(dt);
                            ds.Tables.Add(dt);
                        }
                        //MessageBox.Show(/*list_table.Count.ToString()+*/"2");
                        List<List<string>> list_table = new List<List<string>>();
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            List<string> list_row = new List<string>();
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                list_row.Add(ds.Tables[0].Rows[i].ItemArray[j].ToString());
                            list_table.Add(list_row);
                        }
                        //MessageBox.Show(/*list_table.Count.ToString()+*/"3");
                        conn.Close();
                    }


                    string IM = "", OT = "", W = "";

                    System.Data.SqlClient.SqlConnection sqlConnection1 =
                        new System.Data.SqlClient.SqlConnection("uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;");

                    System.Data.SqlClient.SqlCommand cmd_db = new System.Data.SqlClient.SqlCommand();
                    cmd_db.CommandType = System.Data.CommandType.Text;
                    cmd_db.Connection = sqlConnection1;
                    int count = 0;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        IM = row["ИМЯ"].ToString();
                        OT = row["ОТЧЕСТВО"].ToString();
                        W = row["ПОЛ"].ToString();
                        string Value = "";

                        if (IM.Length != 0 || OT.Length != 0)
                        {
                            Value = " ('" + IM + "','" + OT + "','" + W + "')";
                        }

                        if (Value != "")
                        {
                            cmd_db.CommandText = "INSERT INTO tmpForSRZ.dbo.nsi_imot (IM, OT, W) VALUES " + Value;
                            sqlConnection1.Open();
                            cmd_db.ExecuteNonQuery();
                            sqlConnection1.Close();
                            count++;
                        }
                    }
                    cmd_db.CommandText = "exec tmpForSRZ.dbo.INSERT_IO_NSI";
                    sqlConnection1.Open();
                    cmd_db.ExecuteNonQuery();
                    sqlConnection1.Close();
                    File.Move((string)file, folder_out + @"28004\" + Path.GetFileName((string)file));
                    queue_status.Enqueue(
                        new Log_status(
                            "error_21_22",
                            "",
                            String.Format("Загружено записей: {0}", count)));
                }
                catch
                {
                    File.Move((string)file, folder_out + @"28004\err-" + Path.GetFileName((string)file));
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
            }
            state = Thread_state.finished;
        }

        public void identification_smev_mp()
        // Идентификация
        {
            state = Thread_state.starting;
            if (
                clsLibrary.execQuery_getInt(
                    ref link_connections, reglament_connections,
                    "tmpForSRZ", "DECLARE @return_value int EXEC @return_value = tmpForSRZ.dbo.GATE_Identification_SMEV_MP SELECT @return_value"
                    ) == -1
                )
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
                state = Thread_state.finished;
            //Thread.Sleep(1000);

        }

        public void identification_gate()
        // Идентификация
        {
            state = Thread_state.starting;
            int limit_transaction = 10000;
            if (
                clsLibrary.execQuery_getInt(
                    ref link_connections, reglament_connections,
                    "tmpForSRZ",
                    string.Format("DECLARE @return_value int EXEC @return_value = tmpForSRZ.dbo.GATE_Identification {0} SELECT @return_value", limit_transaction)
                    ) == -1
                )
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
                state = Thread_state.finished;
        }

        public void identification_history_gate()
        // Идентификация с учетом исторических данных
        {
            state = Thread_state.starting;
            int limit_transaction = 300;
            if (
                clsLibrary.execQuery_getInt(
                    ref link_connections, reglament_connections,
                    "tmpForSRZ",
                    string.Format("DECLARE @return_value int EXEC @return_value = tmpForSRZ.dbo.GATE_Identification_History {0} SELECT @return_value", limit_transaction)
                    ) == -1
                )
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
                state = Thread_state.finished;
        }

        public void identification_create_response_01()
        // Идентификация с учетом исторических данных
        {
            state = Thread_state.starting;
            try
            {
                int limit_transaction = 5000;
                if (
                    clsLibrary.execQuery_getInt(
                        ref link_connections, reglament_connections,
                        "tmpForSRZ",
                        string.Format("DECLARE @return_value int EXEC @return_value = tmpForSRZ.dbo.GATE_Identification_Create_Response_01 {0} SELECT @return_value", limit_transaction)
                        ) == -1
                    )
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
                else
                    state = Thread_state.finished;
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void serverC_get_request_idetification()
        // Получение запросов на идентификацию
        {
            state = Thread_state.starting;
            try
            {
                int limit_transaction = 5000;
                List<string[]> response = new List<string[]>();
                if (!clsLibrary.ExecQurey_PGR_GetListStrings(
                    "Server=192.168.1.4;Port=5432;ApplicationName = Dispetcher;User Id=gate;Password=Ghnmop0!;Database=my_db;",
                    //string.Format("select '{0}' from public.identifications where state = 0 order by id desc limit {1}", "Server-c", limit_transaction), ref response, 0, "'"))
                    string.Format("SET enable_seqscan TO on; select '{0}', id, fam, im, ot, to_char(dr,'YYYY-MM-DD'), snils, opdoc, spolis, npolis, doctp, docser, docnum, enp, keys/*, coalesce(actual,0)*/ from static.identifications where state = 0 order by id desc limit {1}", "Server-c", limit_transaction), ref response, 0, "'"))
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
                else
                {
                    if (response.Count() != 0)
                    {
                        if (!clsLibrary.execQuery_insertList_bool("uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;",
                            "INSERT INTO Gate_IdentificationPeople (subsystem, clientid, fam, im, ot, dr, SNILS, OPDOC, SPOLIS,NPOLIS,DOCTP,DOCSER,DOCNUM,enp, keys/*, actual*/) VALUES ", response, 1))
                        {
                            state = Thread_state.error;
                            error = GateError.errorPerformanceMetod;
                        }
                        else
                        {
                            List<string> values = new List<string>();
                            foreach (string[] row in response) values.Add(row[1]);
                            if (!clsLibrary.execQuery_PGR_Update(
                                "Server=192.168.1.4;Port=5432;ApplicationName = Dispetcher;User Id=gate;Password=Ghnmop0!;Database=my_db;"
                                , string.Format("SET enable_seqscan TO on; Update static.identifications set state = 1, identification_date = '{0}' where id in ({1})", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), string.Join(",", values.ToArray()))
                                ))
                            {
                                state = Thread_state.error;
                                error = GateError.errorPerformanceMetod;
                            }
                            else state = Thread_state.finished;
                        }
                    }
                    else state = Thread_state.finished;
                }
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void gate_get_request_identification_from_eir()
        // Получение запросов на идентификацию
        {
            state = Thread_state.starting;
            try
            {
                int limit_transaction = 5000;
                List<string[]> response = new List<string[]>();
                if (!clsLibrary.ExecQurey_PGR_GetListStrings(
                    ref link_connections
                    , null
                    , "postgres"
                    , string.Format("select '{0}', scheme, id, fam, im, ot, to_char(dr,'YYYY-MM-DD') dr, snils, opdoc, spolis, npolis, doctp, docser, docnum, enp, keys, pid, request from identy.identifications where identification_state = 0 order by DATE_SYS limit {1};", "eir", limit_transaction)
                    , ref response
                    ))
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
                else
                {
                    if (response.Count() != 0)
                    {
                        if (!clsLibrary.execQuery_insertList_bool("uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;",
                            "INSERT INTO Gate_IdentificationPeople (subsystem, scheme, clientid, fam, im, ot, dr, SNILS, OPDOC, SPOLIS,NPOLIS,DOCTP,DOCSER,DOCNUM,enp, keys, pid, request) VALUES ", response, 10))
                        {
                            state = Thread_state.error;
                            error = GateError.errorPerformanceMetod;
                        }
                        else
                        {
                            List<string> values = new List<string>();
                            foreach (string[] row in response) values.Add(row[2]);
                            if (!clsLibrary.execQuery_PGR(
                                ref link_connections
                                , "postgres"
                                , string.Format("Update identy.identifications set identification_state = 1, identification_date = '{0}' where id in ({1});", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), string.Join(",", values.ToArray()))
                                , wait_interval))
                            {
                                state = Thread_state.error;
                                error = GateError.errorPerformanceMetod;
                            }
                            else state = Thread_state.finished;
                        }
                    }
                    else
                        state = Thread_state.finished;
                }
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void gate_send_response_identification_to_eir()
        // Выгрузка запросов на идентификацию
        {
            state = Thread_state.starting;
            try
            {
                string _date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                int limit_transaction = 5000;
                List<string[]> response = new List<string[]>();
                if (!clsLibrary.execQuery_getListString(
                    ref response, ref link_connections, reglament_connections, "tmpForSRZ"
                    , string.Format("SELECT TOP {0} [ID], PID, isnull(KEYS_RESULT,'none'), RESPONSE, clientId FROM [tmpForSRZ].[dbo].[Gate_IdentificationPeople] where state = 99 and DATE_SENDING is null and subsystem = '{1}' order by id", limit_transaction, "eir"),
                    wait_interval
                    ))
                {
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
                else
                {
                    if (response.Count() != 0)
                    {
                        List<string> values = new List<string>();
                        IdentificationResponse_01.IdentificationResponse_01Type response_ = new IdentificationResponse_01.IdentificationResponse_01Type();

                        foreach (string[] row in response)
                        {
                            //response_ = null;                        
                            //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(IdentificationResponse_01.IdentificationResponse_01Type));
                            //    StringReader stringReader = new StringReader(row[2]);
                            //    response_ = (IdentificationResponse_01.IdentificationResponse_01Type)xmlSerializer.Deserialize(stringReader);*/
                            values.Add(string.Format("update identy.identifications set pid = {0}, KEYS_RESULT = '{1}', RESPONSE = '{2}', identification_state = 99, identification_date = '{3}' where id = {4};"
                                , row[1]
                                , row[2]
                                , row[3]
                                , _date
                                , row[4]
                                )
                            );
                        }
                        clsLibrary.VarResult varResult = clsLibrary.execQuery_PGR_updateList_varResult(ref link_connections, null, "postgres", ref values, 100, wait_interval);
                        if (!varResult.result)
                        {
                            queue_status.Enqueue(new Log_status("gate_send_response_identification_to_eir", string.Empty, varResult.comment));
                            state = Thread_state.error;
                            error = GateError.errorPerformanceMetod;
                        }
                        else
                        {
                            values.Clear();
                            foreach (string[] row in response) values.Add(row[0]);
                            varResult = clsLibrary.execQuery_VarResult(ref link_connections, reglament_connections, "tmpForSRZ",
                                 string.Format("Update [tmpForSRZ].[dbo].[Gate_IdentificationPeople] set DATE_SENDING = '{0}' where id in ({1})", _date, string.Join(",", values.ToArray())), wait_interval);
                            if (!varResult.result)
                            //"uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;"
                            //, string.Format("Update [tmpForSRZ].[dbo].[Gate_IdentificationPeople] set DATE_SENDING = '{0}' where id in ({1})", _date, string.Join(",", values.ToArray()))

                            {
                                queue_status.Enqueue(new Log_status("gate_send_response_identification_to_eir", string.Empty, varResult.comment));
                                state = Thread_state.error;
                                error = GateError.errorPerformanceMetod;
                            }
                            else state = Thread_state.finished;
                        }
                    }
                    else state = Thread_state.finished;
                }
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            
        }
        
        public void gate_send_pids_to_eir()
        // Выгрузка запросов на идентификацию
        {
            state = Thread_state.starting;
            string comment = string.Empty;
            try
            {
                if (!reglamentGATE.send_pids_to_eir(ref link_connections, wait_interval, ref comment))
                {
                    queue_status.Enqueue(new Log_status("gate_send_pids_to_eir", comment, "error"));
                    state = Thread_state.error;
                    error = GateError.errorPerformanceMetod;
                }
                else
                {
                    state = Thread_state.finished;
                    queue_status.Enqueue(new Log_status("gate_send_pids_to_eir", comment, "Ok "));
                }
            }
            catch(Exception e)
            {
                queue_status.Enqueue(new Log_status("gate_send_pids_to_eir", string.Empty, e.Message));
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void cleaner_identificationPeople()
        // Зачистка таблицы запросов на идентификацию
        {
            state = Thread_state.starting;
            int limit_time = 24;
            if (!clsLibrary.execQuery(
                    ref link_connections, reglament_connections, "tmpForSRZ",
                    string.Format("EXEC dbo.GATE_cleaner_IdentificationPeople {0}", limit_time)
                    )
                )
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
                state = Thread_state.finished;
        }

        public void ServerC_unload_identification()
        // Выгрузка запросов на идентификацию
        {
            state = Thread_state.starting;
            int limit_transaction = 5000;
            List<string[]> response = new List<string[]>();
            if (!clsLibrary.execQuery_getListString(
                ref response, ref link_connections, reglament_connections, "tmpForSRZ"
                , string.Format("SELECT TOP {0} [ID],[CLIENTID],[PID],[KEYS_RESULT] FROM [tmpForSRZ].[dbo].[Gate_IdentificationPeople] where state = 99 and DATE_SENDING is null and subsystem = '{1}' order by id desc", limit_transaction, "Server-c")
                ))
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
            {
                if (response.Count() != 0)
                {
                    List<string> values = new List<string>();
                    foreach (string[] row in response)
                        values.Add(string.Format("SET enable_seqscan TO on; Update static.identifications set pid = {0}, state = 99, identification_date = '{1}', KEYS_RESULT = '{2}' where id = {3}"
                            , row[2]
                            , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                            , row[3]
                            , row[1])
                        );
                    if (!clsLibrary.execQuery_PGR_updateList(
                        "Server=192.168.1.4;Port=5432;ApplicationName = Dispetcher;User Id=gate;Password=Ghnmop0!;Database=my_db;"
                        , values, 1, 1000000))
                    {
                        state = Thread_state.error;
                        error = GateError.errorPerformanceMetod;
                    }
                    else
                    {
                        values.Clear();
                        foreach (string[] row in response) values.Add(row[0]);
                        if (!clsLibrary.execQuery(
                            "uid=sa;pwd=Cvbqwe2!;server=server-r;database=tmpForSRZ;"
                            , string.Format("Update [tmpForSRZ].[dbo].[Gate_IdentificationPeople] set DATE_SENDING = '{0}' where id in ({1})", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), string.Join(",", values.ToArray()))
                            ))
                        {
                            state = Thread_state.error;
                            error = GateError.errorPerformanceMetod;
                        }
                        else state = Thread_state.finished;
                    }
                }
                else state = Thread_state.finished;
            }
        }

        public void eir_event_flk()
        {
            state = Thread_state.starting;            
            if (clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres", "select buf_checking.event_flk();", wait_interval) == -1)
            {
                //queue_status.Enqueue(new Log_status("eir_event_flk", string.Empty, wait_interval.ToString()));
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
                state = Thread_state.finished;                
        }
        public void eir_event_identy()
        // 
        {
            state = Thread_state.starting;
            if (clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres", "select buf_checking.event_identy();", wait_interval) == -1)
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
                state = Thread_state.finished;
        }
        public void eir_event_prt()
        // 
        {
            state = Thread_state.starting;
            clsLibrary.VarResult varResult = clsLibrary.execQuery_PGR_varResult(ref link_connections, "postgres", "select result, comment, code from buf_checking.event_prt();", wait_interval);
            if (varResult == null)
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
            {
                if (!varResult.result)
                {
                    queue_status.Enqueue(new Log_status("eir_event_prt", string.Empty, varResult.comment));
                    state = Thread_state.error;
                }
                else
                    state = Thread_state.finished;
            }
        }
        public void eir_event_import()
        // Импортируем из буфера SI
        {
            state = Thread_state.starting;
            clsLibrary.VarResult varResult = clsLibrary.execQuery_PGR_varResult(ref link_connections, "postgres", "select result, comment, code from import.import();", wait_interval);
            if (varResult == null)
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
            {
                if (!varResult.result)
                {
                    queue_status.Enqueue(new Log_status("eir_event_import", string.Empty, varResult.comment));
                    state = Thread_state.error;
                }
                else
                    state = Thread_state.finished;
            }

        }

        public void gate_get_info_from_eir()
        //отправка информации по результатам загрузки пакетов /пока только SI
        {
            state = Thread_state.starting;
            try
            {
                string[] folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                if (!Directory.Exists(folders[0]) || !Directory.Exists(folders[1])) throw new System.ArgumentException("Не определены или не найдены папки входящих и исходящих файлов");
                List<string[]> requests = new List<string[]>();
                if (clsLibrary.ExecQurey_PGR_GetListStrings(
                        ref link_connections, null, "postgres"
                        , "select id, mnemonics, filename, info_content from buf_eir.request where state::int >= 100 and info isnull and info_content is not null and " +
                            "schema_name in (select unnest(schemas_table) schema_name from buf_eir.config_tables where 'INFO' = any (events)order by \"order\" limit 10;"
                        , ref requests
                    )
                )
                {
                    foreach (string[] request in requests)
                    {
                        List<string> response = new List<string>();
                        response.Add(request[3]);
                        clsLibrary.createFileTXT_FromList(response, Path.Combine(folders[1], string.Format("info{0}-{1}.txt", request[1], request[2])));
                        clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres"
                            , String.Format("update buf_eir.request set info = '{0}' where id = '{1}';", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request[0])
                            , 120000);
                    }
                }
                state = Thread_state.finished;
            }
            catch (Exception e)
            {
                queue_status.Enqueue(new Log_status("gate_get_prt_from_eir", string.Empty, e.Message));
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void gate_get_prt_from_eir()
        // Выгрузка прикладной проверки

        {
            try
            {
                state = Thread_state.starting;
                string[] folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                if (!Directory.Exists(folders[0]) || !Directory.Exists(folders[1])) throw new System.ArgumentException("Не определены или не найдены папки входящих и исходящих файлов");
                string prefix = "";
                string response_id = "";
                string response_filename = "";
                List<string[]> requests = new List<string[]>();
                if (clsLibrary.ExecQurey_PGR_GetListStrings(
                        ref link_connections, null, "postgres"
                        , "select id, mnemonics, schema_name, filename, count_row, prt_error_count from buf_eir.request where state::int >= 100 and prt_response isnull and " +
                            "schema_name in (select unnest(schemas_table) schema_name from buf_eir.config_tables where 'PRT' = any (events) order by \"order\" limit 10;"
                        , ref requests
                    )
                    && requests.Count > 0)
                {
                    foreach (string[] request in requests)
                    {
                        switch (request[2])
                        {
                            /*case "zldn_schema_2_1":
                                prefix = "DN";
                                break;*/
                            default:
                                prefix = "";
                                break;
                        }
                        response_filename = prefix + "SF" + "T" + request[1] + "_" + DateTime.Now.ToString("yyMMdd");
                        string numbe_file = "000" + clsLibrary.execQuery_PGR_getString(
                                ref link_connections, null, "postgres"
                                , "with recursive r as(select 1 as i union select i+1 as i from r where exists(select id from buf_eir.response where filename = '" + response_filename + "'||right('000'||i::text,3)||'.XML')) select i from r order by i desc limit 1;");
                        response_filename += numbe_file.Substring(numbe_file.Length - 3) + ".XML";
                        sf_schema.HEADER header = new sf_schema.HEADER()
                        {
                            VERS = "1.0",
                            FNAME = response_filename,
                            FNAME_1 = request[3],
                            NRECORDS = request[4],
                            FAILS = request[5]
                        };
                        //header.FAILS = file_item[4];
                        List<sf_schema.FLK_PRES> body = new List<sf_schema.FLK_PRES>();
                        List<string[]> response_row = new List<string[]>();
                        if (clsLibrary.ExecQurey_PGR_GetListStrings(
                            ref link_connections, null, "postgres"
                            , string.Format("select * from buf_eir.get_prt_content('{0}', '{1}');", request[0], request[2])
                            , ref response_row
                            , 120000
                            )
                            &&
                            response_row.Count > 0)
                        {
                            foreach (string[] row in response_row)
                            {
                                body.Add(new sf_schema.FLK_PRES
                                {
                                    NREC = row[0],
                                    RESUL = row[1],
                                    COMMENT = row[2]
                                });
                            }
                        }
                        sf_schema.FLK_P response_rows = new sf_schema.FLK_P();
                        response_rows.HEADER = header;
                        response_rows.RES = body.ToArray();
                        clsLibrary.SaveXML_prt(response_rows, Path.Combine(folders[1], response_filename));
                        response_id = Guid.NewGuid().ToString();
                        clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres"
                                , String.Format("insert into buf_eir.response (id, mnemonics, schema_name, id_request, header, filename, date_send) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                                    response_id, request[1], request[2], request[0],
                                    XmlHelper.SerializeTo<sf_schema.HEADER>(header as sf_schema.HEADER),
                                    response_filename,
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                , 120000)
                            );
                        clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres"
                            , String.Format("update buf_eir.request set prt_response = '{0}' where id = '{1}';", response_id, request[0])
                            , 120000);
                    }
                }
                state = Thread_state.finished;
            }
            catch (Exception e)
            {
                queue_status.Enqueue(new Log_status("gate_get_prt_from_eir", string.Empty, e.Message));
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void gate_get_flk_from_eir()
        // Выгрузка flk проверки
        {
            state = Thread_state.starting;
            try
            {
                string[] folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                if (!Directory.Exists(folders[0]) || !Directory.Exists(folders[1])) throw new System.ArgumentException("Не определены или не найдены папки входящих и исходящих файлов");
                string prefix = "";
                string response_id = "";
                string response_filename = "";
                List<string[]> requests = new List<string[]>();
                if (clsLibrary.ExecQurey_PGR_GetListStrings(
                        ref link_connections, null, "postgres"
                        , "select id, mnemonics, schema_name, filename, count_row from buf_eir.request where state::int >= 1 and flk_response isnull and " +
                            "schema_name in (select unnest(schemas_table) schema_name from buf_eir.config_tables where 'FLK' = any (events) order by \"order\" limit 10;"
                        , ref requests
                    )
                    && requests.Count > 0)
                {
                    foreach (string[] request in requests)
                    {
                        switch (request[2])
                        {
                            /*case "zldn_schema_2_1":
                                prefix = "DN";
                                break;*/
                            default:
                                prefix = "";
                                break;
                        }
                        response_filename = prefix + "SP" + "T" + request[1] + "_" + DateTime.Now.ToString("yyMMdd");
                        string numbe_file = "000" + clsLibrary.execQuery_PGR_getString(
                                ref link_connections, null, "postgres"
                                , "with recursive r as(select 1 as i union select i+1 as i from r where exists(select id from buf_eir.response where filename = '" + response_filename + "'||right('000'||i::text,3)||'.XML')) select i from r order by i desc limit 1;");
                        response_filename += numbe_file.Substring(numbe_file.Length - 3) + ".XML";
                        sp_schema.HEADER header = new sp_schema.HEADER()
                        {
                            VERS = "1.0",
                            FNAME = response_filename,
                            FNAME_1 = request[3],
                            NRECORDS = request[4]
                        };
                        //header.FAILS = file_item[4];
                        List<sp_schema.FLK_POSH> body = new List<sp_schema.FLK_POSH>();
                        List<string[]> response_row = new List<string[]>();
                        if (clsLibrary.ExecQurey_PGR_GetListStrings(
                            ref link_connections, null, "postgres"
                            , string.Format("select * from buf_eir.get_flk_content('{0}', '{1}');", request[0], request[2])
                            , ref response_row
                            , 120000
                            )
                            &&
                            response_row.Count > 0)
                        {
                            foreach (string[] row in response_row)
                            {
                                body.Add(new sp_schema.FLK_POSH
                                {
                                    NREC = row[0],
                                    OSHIB = row[1],
                                    COMMENT = row[2]
                                });
                            }
                        }
                        sp_schema.FLK_P response_rows = new sp_schema.FLK_P();
                        response_rows.HEADER = header;
                        response_rows.OSH = body.ToArray();
                        clsLibrary.SaveXML_flk(response_rows, Path.Combine(folders[1], response_filename));
                        response_id = Guid.NewGuid().ToString();
                        clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres"
                                , String.Format("insert into buf_eir.response (id, mnemonics, schema_name, id_request, header, filename, date_send) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                                    response_id, request[1], request[2], request[0],
                                    XmlHelper.SerializeTo<sp_schema.HEADER>(header as sp_schema.HEADER),
                                    response_filename,
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                , 120000));

                        clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres"
                            , String.Format("update buf_eir.request set flk_response = '{0}' where id = '{1}';", response_id, request[0])
                            , 120000);
                    }
                }
                state = Thread_state.finished;
            }
            catch (Exception e)
            {
                queue_status.Enqueue(new Log_status("gate_get_flk_from_eir", string.Empty, e.Message));
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }
        public void gate_report_si_schema_1_0()
        //отправка информации по результатам загрузки пакетов /пока только SI
        {
            state = Thread_state.starting;
            try
            {
                string[] folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                if (!Directory.Exists(folders[0]) || !Directory.Exists(folders[1])) throw new System.ArgumentException("Не определены или не найдены папки входящих и исходящих файлов");
                List<string[]> requests = new List<string[]>();
                if (clsLibrary.ExecQurey_PGR_GetListStrings(
                        ref link_connections, null, "postgres",
                            "with lst_pln as ( select smocode, content->> 0 mcode, form, content->> 1 cnt from( " +
                            "select content#>>'{form}' form, content#>>'{parent}' smocode, jsonb_array_elements(content#>'{content}') as content from ( " +
			                "select jsonb_array_elements(content) as content from statistic.control_benchmarks where year = 2019 and schema_name = 'si_schema_1_0' ) rows ) row ), " + 
	                        "lst_fct as (select smocode, mcode, form, count(1) cnt from prod_eir.plan_profilactics where old = false group by smocode, mcode, form) " +
                            "select pln.mcode, lib.nam_mok, pln.smocode, pln.form, pln.cnt, coalesce(fct.cnt, 0), case when pln.cnt::int = fct.cnt then 'Ok' else '' end from lst_pln pln " +
                            "left outer join lst_fct fct on fct.smocode = pln.smocode and fct.mcode = pln.mcode and fct.form::text = pln.form " +
                            "left outer join library.f003 lib on lib.mcod = pln.mcode " +
                            "where pln.mcode <> 'total' and pln.smocode <> 'total' " +
                            "order by pln.mcode, pln.smocode, pln.form; "
                        , ref requests
                    )
                )
                {
                    List<string> response = new List<string>();
                    response.Add("МО; Наименование; СМО; Форма; План; Факт; Результат");
                    foreach (string[] request in requests)
                    {

                        string row = string.Empty;
                        for(int col_numb = 0; col_numb < 7; col_numb++)
                        {
                            if (col_numb != 0) row += ";";
                            row += (string.IsNullOrEmpty(request[col_numb])) ? string.Empty : (request[col_numb]);
                        }
                        response.Add(row);
                        
                    }
                    clsLibrary.createFileTXT_FromList(response, Path.Combine(folders[1], string.Format("report_si_schema_1_0_{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmm"))));
                }
                state = Thread_state.finished;
            }
            catch (Exception e)
            {
                queue_status.Enqueue(new Log_status("gate_reports_si_schecma_report_0_1", string.Empty, e.Message));
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void gateBackup()
        // Резервное копирование Gate
        {
            state = Thread_state.starting;
            string file = clsLibrary.execQuery_getString(ref link_connections, reglament_connections, "gate", "exec dbo.create_Backup");
            if (file != null)
            {
                state = Thread_state.finished;
                queue_status.Enqueue(new Log_status("Резервное копирование. Gate", string.Empty, file));
            }
            else
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }

        public void eirBackup()
        // Резервное копирование Gate
        {
            state = Thread_state.starting;
            string file = clsLibrary.execQuery_getString(ref link_connections, reglament_connections, "eir", "exec dbo.create_Backup");
            if (file != null)
            {
                state = Thread_state.finished;
                queue_status.Enqueue(new Log_status("Резервное копирование. EIR", string.Empty, file));
            }
            else
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }


        public void gate_handling_files()
        // Резервное копирование Gate
        {
            state = Thread_state.starting;

            //
            string nameForMessage = string.Empty;
            clsLibrary.unpack("*");
            try
            {
                bool result = false;
                string result_comment = String.Empty;
                bool ignore;
                string[] folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                if (!Directory.Exists(folders[0]) || !Directory.Exists(folders[1])) throw new System.ArgumentException("Не определены или не найдены папки входящих и исходящих файлов");

                List<string> files = new List<string>(Directory.GetFiles(folders[0], "*.XML"));
                int count = 0; //подсчет обработанных
                int count_row = -1;
                if (files.Count() > 0)
                {
                    ReglamentLinker reglamentLinker = new ReglamentLinker();
                    files.Sort();
                    foreach (string file in files)
                    {
                        nameForMessage = file;
                        result_comment = String.Empty;
                        string owner = String.Empty;
                        string filename = Path.GetFileName(file);
                        reglamentLinker.getLink(null, null, null, filename);
                        ignore = (reglamentLinker.link == null);
                        if (!ignore) //принадлежность файла к регламенту установлена, далее в случае ошибок в обработке он будет перенесен в fail иначе в ок
                        {
                            result = false; //по умолчанию ошибка обработки                            
                            switch (reglamentLinker.link.reglament_owner)
                            {
                                // ----------------- Обработка пакетов Регламентов АОФОМС
                                case Reglament_owner.FFOMS:
                                    result = reglamentFFOMS.handling_file(file, link_connections, folders, reglamentLinker, out result_comment);
                                    queue_status.Enqueue(new Log_status(filename, string.Empty, result_comment));
                                    break;
                                // ----------------- Обработка пакетов АОФОМС
                                case Reglament_owner.AOFOMS:
                                    // проверяем наличие необходимых соединений
                                    result = reglamentAOFOMS.handling_file(file, link_connections, folders, reglamentLinker, out result_comment, out count_row);
                                    queue_status.Enqueue(new Log_status(filename, count_row.ToString(), result_comment));
                                    break;
                                // ----------------- Обработка пакетов СМЭВ
                                case Reglament_owner.SMEV12:
                                    // проверяем наличие необходимых соединений
                                    /*Server-c:
                                     * main_db
                                     * 
                                     *Server-r:
                                     * srz3_00_adapter
                                     * srz3_00
                                     *Server-SHRK:
                                     * libraries
                                     *  
                                     */
                                    result = reglamentSMEV.getRequest_file12(file, link_connections, folders, reglamentLinker, out result_comment);
                                    queue_status.Enqueue(new Log_status(filename, string.Empty, result_comment));
                                    break;
                                case Reglament_owner.SMEV13:
                                    result = reglamentSMEV.getRequest_file13(file, link_connections, folders, reglamentLinker, out result_comment);
                                    queue_status.Enqueue(new Log_status(filename, string.Empty, result_comment));
                                    break;
                                default: // Если нет обработки, он оставется без внимания
                                    ignore = true;
                                    break;
                            }
                        }
                        //статус ignore мог измениться после обработки
                        if (!ignore)
                        {
                            string subFolder = (result) ? "ok" : "fail";
                            if (!clsLibrary.moveFile(file, Path.Combine(folders[0], subFolder), out result_comment)) throw new Exception(result_comment);
                            ++count;
                        }
                        if (count >= 10) break;
                    }
                }
                state = Thread_state.finished;
            }
            catch (Exception exception)
            {
                queue_status.Enqueue(new Log_status(nameForMessage, string.Empty, exception.Message));
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }
        public void gate_create_response_on_eir()
        {
            state = Thread_state.starting;
            if (clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres", "select buf_checking.event_create_response();", wait_interval) == -1)
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else
                state = Thread_state.finished;
        }
        public void gate_send_response_from_eir()
        {
            state = Thread_state.starting;
            /*if (clsLibrary.execQuery_PGR_function_bool(ref link_connections, "postgres", "select buf_checking.event_create_response();", wait_interval) == -1)
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
            else*/
                state = Thread_state.finished;
        }
        public void unloading_ZLDNforSMO()
        // Резервное копирование Gate
        {
            state = Thread_state.starting;
            try
            {
                string[] folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;

                if (!Directory.Exists(folders[0]) || !Directory.Exists(folders[1])) throw new System.ArgumentException("Не определены или не найдены папки входящих и исходящих файлов");
                string filename = String.Format("ZLDNT{0}_{1}.xml", "28004", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                clsLibrary.createFileTXT_FromList(
                    clsLibrary.execQuery_getListString(
                        ref link_connections,
                        ref reglament_connections,
                        "tmpForSRZ", String.Format("EXEC [dbo].[get_tmpZLDNforSMO] '{0}', '{1}'", "28004", filename)),
                    Path.Combine(folders[1], filename)
                );

                filename = String.Format("ZLDNT{0}_{1}.xml", "28001", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                clsLibrary.createFileTXT_FromList(
                    clsLibrary.execQuery_getListString(
                        ref link_connections,
                        ref reglament_connections,
                        "tmpForSRZ", String.Format("EXEC [dbo].[get_tmpZLDNforSMO] '{0}', '{1}'", "28001", filename)),
                    Path.Combine(folders[1], filename)
                );

                state = Thread_state.finished;
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }







        //----- СМЭВ -------
        public void responseAsync_SMEV()
        // Отправка ответов на Асинхронные запросы СМЭВ 
        {
            state = Thread_state.starting;
            bool result = false;
            string result_comment = string.Empty;
            try
            {
                string[] folders = (string[])@link_connections.Find(x => x.name == folders_connections).ping.ping_resource;
                if (!Directory.Exists(folders[1])) throw new System.ArgumentException("Не определена папка исходящих файлов");

                List<string[]> requests = new List<string[]>();
                clsLibrary.ExecQurey_GetListStrings(link_connections, null, "srz3_00_adapter"
                    , "select Id, MessageId, TipData from SMEV_MESSAGES where isnull(state,0) = 0",
                    ref requests);
                foreach (string[] request in requests)
                {
                    result = false;
                    result_comment = String.Empty;
                    clsLibrary.execQuery(ref link_connections, null, "srz3_00_adapter", String.Format("update SMEV_MESSAGES set STATE = '{0}' where ID = '{1}'", "1", request[0]));
                    switch (request[2])
                    {
                        case "POLIS":
                            //sendResponse_Polis(request_row);
                            break;
                        case "USLUGI":
                        //result = reglamentSMEV.sendResponse_USLUGI(request, link_connections, folders, out result_comment); break;
                        case "FATALZP":
                        case "ROGDZP":
                        case "PERNAMEZP":
                            result = reglamentSMEV.sendResponse(request, ref link_connections, ref folders, out result_comment);
                            break;
                    }
                    if (result)
                        queue_status.Enqueue(new Log_status(request[1], "", "Send Response / " + request[2]));
                    else
                        queue_status.Enqueue(new Log_status(request[1], "", result_comment));
                    clsLibrary.execQuery(ref link_connections, null, "srz3_00_adapter",
                        String.Format("update SMEV_MESSAGES set STATE = '{0}' where ID = '{1}'", (result) ? 99 : 5, request[0]));
                }
                state = Thread_state.finished;
            }
            catch
            {
                state = Thread_state.error;
                error = GateError.errorPerformanceMetod;
            }
        }




        // ------------------- Сопутствующие методы -----------------------






    }
}
