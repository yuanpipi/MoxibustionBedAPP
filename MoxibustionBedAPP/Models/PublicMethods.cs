using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace MoxibustionBedAPP.Models
{
    public class PublicMethods
    {
        /// <summary>
        /// 保存进json文件
        /// </summary>
        /// <param name="isSmokePurificationSystem"></param>
        public static void SaveToJson()
        {
            var data = new SaveMode
            {
                MoxibustionTherapyMode = App.PropertyModelInstance.MoxibustionTherapyMode,
                BackMoxibustionColumn_Height = App.PropertyModelInstance.BackMoxibustionColumn_Height,
                LegMoxibustionColumn_Height = App.PropertyModelInstance.LegMoxibustionColumn_Height,
                SmokeExhaustSystem = App.PropertyModelInstance.SmokeExhaustSystem,
                SmokePurificationSystem = App.PropertyModelInstance.SmokePurificationSystem,
                IsSmokePurificationSystem = App.PropertyModelInstance.IsSmokePurificationSystem,
                Upper_CabinTemperature = App.PropertyModelInstance.Upper_CabinTemperature,
                BackTemperature = App.PropertyModelInstance.BackTemperature,
                LegTemperature = App.PropertyModelInstance.LegTemperature,
                PreheadTemperature = App.PropertyModelInstance.PreheadTemperature,
                UpperAlarmCabinTemperature = App.PropertyModelInstance.UpperAlarmCabinTemperature,
                BackAlarmTemperature = App.PropertyModelInstance.BackAlarmTemperature,
                LegAlarmTemperature=App.PropertyModelInstance.LegAlarmTemperature,
                PreheadTime = App.PropertyModelInstance.PreheadTime,
                InignitionTime = App.PropertyModelInstance.InignitionTime,
                MoxibustionTherapyTime = App.PropertyModelInstance.MoxibustionTherapyTime,
                AutoMusic=App.PropertyModelInstance.AutoMusic
            };
            string json = JsonConvert.SerializeObject(data);
            try
            {
                File.WriteAllText("SaveData.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 从json文件中读取文件
        /// </summary>
        public static void ReadFromJson()
        {
            if (File.Exists("SaveData.json"))//判断文件是否存在
            {
                try
                {
                    string json = File.ReadAllText("SaveData.json");
                    var data = JsonConvert.DeserializeObject<SaveMode>(json);
                    if (data != null)
                    {
                        App.PropertyModelInstance.MoxibustionTherapyMode = data.MoxibustionTherapyMode;
                        App.PropertyModelInstance.BackMoxibustionColumn_Height = data.BackMoxibustionColumn_Height;
                        App.PropertyModelInstance.LegMoxibustionColumn_Height = data.LegMoxibustionColumn_Height;
                        App.PropertyModelInstance.SmokeExhaustSystem = data.SmokeExhaustSystem;
                        App.PropertyModelInstance.SmokePurificationSystem = data.SmokePurificationSystem;
                        App.PropertyModelInstance.Upper_CabinTemperature = data.Upper_CabinTemperature;
                        App.PropertyModelInstance.BackTemperature = data.BackTemperature;
                        App.PropertyModelInstance.LegTemperature = data.LegTemperature;
                        App.PropertyModelInstance.PreheadTemperature = data.PreheadTemperature;
                        App.PropertyModelInstance.UpperAlarmCabinTemperature = data.UpperAlarmCabinTemperature;
                        App.PropertyModelInstance.BackAlarmTemperature = data.BackAlarmTemperature;
                        App.PropertyModelInstance.LegAlarmTemperature = data.LegAlarmTemperature;
                        App.PropertyModelInstance.PreheadTime = data.PreheadTime;
                        App.PropertyModelInstance.InignitionTime = data.InignitionTime;
                        App.PropertyModelInstance.MoxibustionTherapyTime = data.MoxibustionTherapyTime;
                        App.PropertyModelInstance.IsSmokePurificationSystem = data.IsSmokePurificationSystem;
                        App.PropertyModelInstance.AutoMusic = data.AutoMusic;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 保存COM设置进json文件
        /// </summary>
        public static void SaveToCOMJson()
        {
            var data = new COMSetting
            {
                MotherboardCOM = App.PropertyModelInstance.MotherboardCOM,
                AICOM = App.PropertyModelInstance.AICOM
            };
            string json = JsonConvert.SerializeObject(data);
            try
            {
                File.WriteAllText("COMSettings.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 从json文件中读取COM设置
        /// </summary>
        public static void ReadFromCOMJson()
        {
            if (File.Exists("COMSettings.json"))
            {
                try
                {
                    string json = File.ReadAllText("COMSettings.json");
                    var data = JsonConvert.DeserializeObject<COMSetting>(json);
                    if (data != null)
                    {
                        App.PropertyModelInstance.MotherboardCOM = data.MotherboardCOM;
                        App.PropertyModelInstance.AICOM = data.AICOM;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
