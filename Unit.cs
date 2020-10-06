using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Morrigan
{
    internal class Unit
    {

        private List<String> _LifeState = new List<string>() { "HEALTHY", "DEAD", "DEAD-RESPAWN", "DEAD-SWITCHING", "INCAPACITATED", "INJURED" };
        private List<String> _Fr_LifeState = new List<string>() { "Opérationnel", "Mort", "Mort", "Mort", "Inconscient", "Blessé" };
        private string _Name = String.Empty;
        public string Name { get => _Name; }
        public string Get_Hash()
        {

            try
            {
                string _Md5_Name = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(_Name));
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    _Md5_Name = sBuilder.ToString();
                }

                return _Md5_Name;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public string Get_Uid()
        {
            return this._Uid;
        }

        private string _Distance = "0";
        private string _Extend_Name = String.Empty;
        private string _Health = String.Empty;
        private string _Coordinates = String.Empty;
        private string _Group_Name = String.Empty;
        private string _Role = String.Empty;
        private string _Weapon = String.Empty;
        private string _Magazine_Name = String.Empty;
        private string _Ammunitions = String.Empty;
        private string _Medic = "false";
        private string _Enginer = "false";
        private string _Spec_Explo = "false";
        private string _Leader = "false";
        private string _AC_Strenght = "0";
        private string _EX_Strenght = "0";
        private string _Magazines = string.Empty;
        private string _Daytime = string.Empty;
        private string _Uid = string.Empty;
        private string _X_Y_Z = "[0,0,0]";
        private string _State = string.Empty;
        private string _Damages_Parts = string.Empty;

        public Unit(string[] in_Args = null)
        {
            try
            {
                if (in_Args != null)
                {
                    _Name = in_Args[0];
                    _Distance = this.clear_text(in_Args[1]);
                    _Extend_Name = in_Args[2];
                    _Health = in_Args[3];
                    _Coordinates = this.clear_text(in_Args[4]);
                    _Group_Name = in_Args[5];
                    _Role = in_Args[6];

                    //Arme principale
                    _Weapon = in_Args[7];
                    _Magazine_Name = in_Args[8];

                    //Specialités
                    _Medic = in_Args[9];
                    _Enginer = in_Args[10];
                    _Spec_Explo = in_Args[11];
                    _Leader = in_Args[12];

                    //Matériel
                    _AC_Strenght = in_Args[13];
                    _EX_Strenght = in_Args[14];

                    //Munitions
                    _Magazines = in_Args[15];

                    //Ingame time
                    _Daytime = in_Args[16];

                    //Identifiant unique de l'unité/joueur
                    _Uid = this.clear_text(in_Args[17]);

                    //Position en 3D
                    _X_Y_Z = in_Args[18];

                    //Etat du joueur in game
                    _State = in_Args[19];

                    //Dommages par zone
                    _Damages_Parts = in_Args[20];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string clear_text(string in_data)
        {
            return in_data.Replace(@"&quot;", " ").Trim();
        }
        public XElement Get_XML()
        {
            try
            {
                XElement Xet = new XElement("unit",
                    new XAttribute("uid", _Uid),
                    new XAttribute("hash", Get_Hash()),
                    new XAttribute("last_synch_us", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")),
                    new XAttribute("last_synch_fr", DateTime.Now.ToString()),
                    new XAttribute("is_medic", _Medic.ToString().ToLower()),
                    new XAttribute("is_enginer", _Enginer.ToString().ToLower()),
                    new XAttribute("is_leader", _Leader.ToString().ToLower()),
                    new XAttribute("is_explo", _Spec_Explo.ToString().ToLower()),
                    new XAttribute("daytime", _Daytime),
                    new XAttribute("str_explo", _EX_Strenght),
                    new XAttribute("str_ac", _AC_Strenght));

                Xet.Add(new XElement("in_game_state", new XCData(_State)));
                Xet.Add(new XElement("name", new XCData(_Name)));
                Xet.Add(new XElement("role", new XCData(_Role)));
                Xet.Add(new XElement("weapon", new XCData(_Weapon)));
                Xet.Add(new XElement("group_name", new XCData(_Group_Name)));
                Xet.Add(new XElement("extend", new XCData(_Extend_Name)));
                Xet.Add(new XElement("position",
                                            new XAttribute("distance", _Distance),
                                            new XAttribute("coordinates", _Coordinates),
                                            new XAttribute("x_y_z", _X_Y_Z)
                                       ));
                Xet.Add(new XElement("health", new XCData(_Health.ToString())));
                Xet.Add(new XElement("dammages_parts", new XCData(_Damages_Parts.ToString())));
                Xet.Add(new XElement("ammunitions", new XCData(_Magazines)));

                return Xet;
            }
            catch (Exception ex)
            {
                throw new Exception("Unit->Get_Xml() : " + ex.Message);
            }
        }




    }
}
