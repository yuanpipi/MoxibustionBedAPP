using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoxibustionBedAPP.Models
{
    public class COMSetting
    {
        /// <summary>
        /// 主板COM口
        /// </summary>
        public string MotherboardCOM { get; set; }

        /// <summary>
        /// 语音助手COM口
        /// </summary>
        public string AICOM { get; set; }
    }
}
