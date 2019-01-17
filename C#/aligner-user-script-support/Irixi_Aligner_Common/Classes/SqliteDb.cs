using Irixi_Aligner_Common.Classes.BaseClass;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Irixi_Aligner_Common.Classes
{
    public class SqliteDb : IDisposable
    {
        const string DB_PATH = "my.db";
        const string TBL_PRESETPOSITION = "preset_position";

        SQLiteConnection conn;

        public SqliteDb()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder()
            {
                DataSource = DB_PATH
            };

            conn = new SQLiteConnection(builder.ConnectionString);

            try
            {
                conn.Open();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        #region Properties
        public string LastError { private set; get; }
        #endregion


        /// <summary>
        /// Get the name list of preset positions for the specified motion component
        /// </summary>
        /// <param name="MotionComponentHashCode"></param>
        /// <returns></returns>
        public string[] GetPresetPositionNames(int MotionComponentHashCode)
        {
            List<String> names = new List<string>();

            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = string.Format("select distinct name from {0} where motioncomponent_hashcode = '{1:X}'", TBL_PRESETPOSITION, MotionComponentHashCode);
                SQLiteDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    names.Add(dr["name"].ToString());
                }

                dr.Close();
            }

            return names.ToArray();
        }

        /// <summary>
        /// Get the detail of preset position
        /// </summary>
        /// <param name="MotionComponentHashCode"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ReadPresetPosition(int MotionComponentHashCode, string Name, out PresetPosition Preset)
        {
            bool ret = false;

            List<PresetPositionItem> detail = new List<PresetPositionItem>();
            Preset = new PresetPosition()
            {
                MotionComponentHashCode = MotionComponentHashCode,
                Name = Name
            };

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = string.Format("select * from {0} where motioncomponent_hashcode = '{1:X}' and name = '{2}'",
                        TBL_PRESETPOSITION, MotionComponentHashCode, Name);
                    SQLiteDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        detail.Add(new PresetPositionItem()
                        {
                            Id = 0,
                            IsAbsMode = dr["absmode"].ToString() == "1" ? true : false,
                            Position = double.Parse(dr["position"].ToString()),
                            Speed = int.Parse(dr["speed"].ToString()),
                            MoveOrder = int.Parse(dr["move_order"].ToString()),
                            HashCode = int.Parse(dr["logicalaxis_hashcode"].ToString(), System.Globalization.NumberStyles.HexNumber)
                        });
                    }

                    dr.Close();

                    if (detail.Count == 0)
                    {
                        Preset = null;
                    }
                    else
                    {
                        Preset.Items = detail.ToArray();
                    }

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Preset = null;
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Save the preset position
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public bool SavePresetPosition(PresetPosition Position)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                StringBuilder sb = new StringBuilder();

                foreach (var item in Position.Items)
                {
                    sb.Append(string.Format("('{0}', '{1}', '{2}', {3}, {4}, {5}, '{6}')",
                        new object[]
                        {
                            Position.MotionComponentHashCode.ToString("X"),
                            item.HashCode.ToString("X"),
                            Position.Name,
                            item.MoveOrder,
                            item.Speed,
                            item.Position,
                            item.IsAbsMode ? "1" : "0"

                        }));

                    sb.Append(", ");
                }

                // delete the last comma
                sb.Remove(sb.Length - 2, 2);

                cmd.CommandText = string.Format("insert into {0} ([motioncomponent_hashcode], [logicalaxis_hashcode], [name], [move_order], [speed], [position], [absmode]) VALUES {1}", TBL_PRESETPOSITION, sb.ToString());
                cmd.ExecuteNonQuery();
 
            }

            return true;
        }

        /// <summary>
        /// Delete the specified preset position
        /// </summary>
        /// <param name="MotionComponentHashCode"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool DeletePresetPosition(int MotionComponentHashCode, string Name)
        {
            bool ret = false;

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = string.Format("delete from {0} where motioncomponent_hashcode = '{1:X}' and name = '{2}'",
                        TBL_PRESETPOSITION, MotionComponentHashCode, Name);

                    cmd.ExecuteNonQuery();
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                ret = false;
            }

            return ret;
        }

        public void Dispose()
        {
            try
            {
                conn.Close();
            }
            catch(Exception ex)
            {

            }
            finally
            {
                conn.Dispose();
            }

        }

    }
}
