using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Morrigan
{
    public class Orders_Functions
    {
        string _version = "1.4.0.0 (Update 25/09/2020 14:06)";
        private string _Name_Payload = String.Empty;
        private string[] _Args = new string[] { };
        private string _Config_Path = String.Empty;
        private string _Datas_Path = String.Empty;
        private string _App_Path = String.Empty;
        private string _Units_Path = String.Empty;
        private string _Units_Lock_Path = String.Empty;
        private string _Path_Doc_Arma3 = String.Empty;
        private string _Path_Export_Map = String.Empty;
        private Unit _Unit = null;

        //Constructeur 
        public Orders_Functions(string in_function, string[] in_args = null)
        {
            try
            {
                this._Name_Payload = in_function.ToLower();
                if (in_args != null) this._Args = in_args;
                this._Config_Path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.xml");
                this._Datas_Path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "datas.xml");
                this._Units_Path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"app\units.xml");
                this._App_Path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"app\UX_Api_Caller.exe");
                this._Units_Lock_Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"app\units.lock";
                this._Path_Doc_Arma3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Arma 3");
                this._Path_Export_Map = Path.Combine(this._Path_Doc_Arma3, "maps_exports");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Exécution automatique
        public int Auto_Execute(StringBuilder in_output)
        {
            try
            {
                switch (this._Name_Payload)
                {
                    //VERSION
                    case "get_str_version":
                        in_output.AppendLine("version " + _version);
                        if (File.Exists(this._Config_Path) == false)
                        {
                            in_output.AppendLine("Fichier de configuration introuvable " + this._Config_Path);
                        }

                        //Supprime le fichier des unités pour partir sur une base "saine" 
                        if (File.Exists(this._Units_Path))
                        {
                            File.Delete(this._Units_Path);
                        }

                        XElement Xet = new XElement("root", new XElement("units"));
                        Xet.Save(this._Units_Path);


                        Start_Process(this._App_Path, "UX_Api_Caller");
                        break;
                    case "medic_init":

                        if (File.Exists(this._Datas_Path) == false)
                        {
                            in_output.AppendLine("Fichier de données introuvable " + this._Datas_Path);
                        }
                        else
                        {
                            this.Medic_Count(true);
                            in_output.AppendLine("Compteur de soins actif");
                        }

                        break;
                    //COMLINK
                    case "get_use_compass":
                        in_output.Append(this.Get_Indicator_Enabled("use_compass"));
                        break;
                    case "transcode_name":
                        in_output.Append(this.Transcode_Name());
                        break;
                    //EXTERNAL UI
                    case "set_unit_state":
                        in_output.Append(this.Set_Unit_State());
                        break;
                    //COMPTEUR
                    case "medic_count":
                        in_output.AppendLine("Assistance soins : " + this.Medic_Count(false));
                        break;
                    //EXPORT MAP 
                    case "export":
                        Write_File_Export(((string)this._Args[0]), ((string)this._Args[1]));
                        break;
                    default:

                        break;
                }

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private string Write_File_Export(string in_file_name, string in_content = null)
        {
            try
            {
                string path_dir_export = Path.Combine(this._Path_Export_Map, in_file_name.ToLower());
                if (Directory.Exists(path_dir_export) == false)
                {
                    Directory.CreateDirectory(path_dir_export);
                }

                string path_file_export = Path.Combine(path_dir_export, "export.txt");
                if (File.Exists(path_file_export))
                {
                    File.Create(path_file_export);
                }
                if (!String.IsNullOrEmpty(in_content))
                {
                    using (StreamWriter sw = File.AppendText(path_file_export))
                    {
                        sw.Write(in_content);
                    }
                }

                return "ok";
            }
            catch (Exception ex)
            {
                return "erreur " + ex.Message;
            }
        }

        private void Start_Process(string in_path_exec, string in_process_name, List<String> args = null)
        {
            Process[] List_Process = Process.GetProcessesByName(in_process_name);
            if (List_Process.Count() == 0)
            {
                if (args == null) args = new List<string>();
                String str_args = string.Join("-", args.ToArray());
                ProcessStartInfo process = new ProcessStartInfo(in_path_exec, str_args);
                Process.Start(process);
            }

        }

        private string Set_Unit_State()
        {
            try
            {
                XElement Xet = XElement.Load(this._Units_Path);
                _Unit = new Unit(this._Args);
                var Possible_Unit = Xet.Element("units").Elements("unit").Where(x => x.Attribute("uid") != null && x.Attribute("uid").Value == _Unit.Get_Uid());
                if (Possible_Unit.Count() == 1)
                {
                    Possible_Unit.First().Remove();
                }

                Xet.Element("units").Add(_Unit.Get_XML());
                Xet.Save(this._Units_Path, SaveOptions.None);

                return "Synchronisation terminée";
            }
            catch (Exception ex)
            {
                return "erreur " + ex.Message;
            }
        }

        private string Medic_Count(bool in_reset = false)
        {
            try
            {
                XElement Xet = XElement.Load(this._Datas_Path);
                XElement Medic_Count = Xet.Element("datas").Elements("row").Where(n => n.Attribute("key") != null && n.Attribute("key").Value.ToLower() == "medic_count").First();
                if (in_reset)
                {
                    Medic_Count.Attribute("value").Value = "0";
                }
                else
                {
                    Medic_Count.Attribute("value").Value = (Int32.Parse(Medic_Count.Attribute("value").Value) + 1).ToString();
                }

                Medic_Count.Attribute("update").Value = DateTime.Now.ToString("dd/MM/yy H:mm:ss");

                Xet.Save(this._Datas_Path);

                return Medic_Count.Attribute("value").Value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string Get_Indicator_Enabled(string in_type)
        {
            try
            {
                XElement Xet = XElement.Load(this._Config_Path);
                return Xet.Element("settings").Element(in_type).Attribute("enabled").Value.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string Transcode_Name()
        {
            try
            {
                string uid = ((string)this._Args[1]);
                string Bad_Name = ((string)this._Args[0]);
                string Name = Bad_Name.Substring(1, Bad_Name.Length - 2);
                string Profil_Name = Name.ToLower();
                string Gui_Name = Name;

                XElement Xet = XElement.Load(this._Config_Path);
                var Profils_Players = Xet.Element("players").Elements("player").Where(n => n.Attribute("profil_name") != null && n.Attribute("profil_name").Value.ToLower() == Profil_Name || n.Attribute("uid") != null && n.Attribute("uid").Value == uid);
                if (Profils_Players != null && Profils_Players.Count() == 1)
                {
                    Gui_Name = Profils_Players.First().Attribute("gui_name").Value;

                    //Assigne le UID du joueur 
                    if (Profils_Players.First().Attribute("uid") == null)
                    {
                        Profils_Players.First().Add(new XAttribute("uid", uid));
                        Xet.Save(this._Config_Path);
                    }
                }

                return Gui_Name;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }




    }
}
