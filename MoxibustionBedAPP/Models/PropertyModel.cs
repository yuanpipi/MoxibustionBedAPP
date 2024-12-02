using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MoxibustionBedAPP.Models
{
    /// <summary>
    /// 属性
    /// </summary>
    public class PropertyModel
    {
        /// <summary>
        /// 上舱温度
        /// 0~100
        /// </summary>
        private double upper_CabinTemperature;
        public double Upper_CabinTemperature
        {
            get 
            { 
                return upper_CabinTemperature; 
            }
            set 
            { 
                upper_CabinTemperature = value; 
            }
        }

        /// <summary>
        /// 背部温度
        /// 0~100
        /// </summary>
        private double backTemperature;
        public double BackTemperature
        {
            get
            {
                return backTemperature;
            }
            set
            {
                backTemperature = value;
            }
        }

        /// <summary>
        /// 腿部温度
        /// 0~100
        /// </summary>
        private double legTemperature;
        public double LegTemperature
        {
            get
            {
                return legTemperature;
            }
            set
            {
                legTemperature = value;
            }
        }

        /// <summary>
        /// 预热温度
        /// 0~100
        /// </summary>
        private double preheadTemperature;
        public double PreheadTemperature
        {
            get
            {
                return preheadTemperature;
            }
            set
            {
                preheadTemperature = value;
            }
        }

        /// <summary>
        /// 上舱报警温度
        /// 0~100
        /// </summary>
        private double upperAlarmCabinTemperature;
        public double UpperAlarmCabinTemperature
        {
            get
            {
                return upperAlarmCabinTemperature;
            }
            set
            {
                upperAlarmCabinTemperature = value;
            }
        }

        /// <summary>
        /// 背部报警温度
        /// 0~100
        /// </summary>
        private double backAlarmTemperature;
        public double BackAlarmTemperature
        {
            get
            {
                return backAlarmTemperature;
            }
            set
            {
                backAlarmTemperature = value;
            }
        }

        /// <summary>
        /// 腿部报警温度
        /// 0~100
        /// </summary>
        private double legAlarmTemperature;
        public double LegAlarmTemperature
        {
            get
            {
                return legAlarmTemperature;
            }
            set
            {
                legAlarmTemperature = value;
            }
        }

        /// <summary>
        /// 背部灸柱高度
        /// 0：0档
        /// 1：1档
        /// 2：2档
        /// 3：3档
        /// 4：4档
        /// </summary>
        private int backMoxibustionColumn_Height;
        public int BackMoxibustionColumn_Height
        {
            get
            {
                return backMoxibustionColumn_Height;
            }
            set
            {
                backMoxibustionColumn_Height = value;
            }
        }

        /// <summary>
        /// 腿部灸柱高度
        /// 0：0档
        /// 1：1档
        /// 2：2档
        /// 3：3档
        /// 4：4档
        /// </summary>
        private int legMoxibustionColumn_Height;
        public int LegMoxibustionColumn_Height
        {
            get
            {
                return legMoxibustionColumn_Height;
            }
            set
            {
                legMoxibustionColumn_Height= value;
            }
        }

        /// <summary>
        /// 电池电量
        /// 0：0档
        /// 1：1档
        /// 2：2档
        /// 3：3档
        /// </summary>
        private int batteryLevel;
        public int BatteryLevel
        {
            get
            {
                return batteryLevel;
            }
            set
            {
                batteryLevel = value;
            }
        }

        /// <summary>
        /// 红外线
        /// True：On
        /// False：Off
        /// </summary>
        private bool infraredLamp;
        public bool InfraredLamp
        {
            get
            {
                return infraredLamp;
            }
            set
            {
                infraredLamp = value;
            }
        }

        /// <summary>
        /// 排烟系统
        /// True：On
        /// False：Off
        /// </summary>
        private bool smokeExhaustSystem;
        public bool SmokeExhaustSystem
        {
            get
            {
                return smokeExhaustSystem;
            }
            set
            {
                smokeExhaustSystem = value;
            }
        }

        /// <summary>
        /// 净烟系统
        /// True：On
        /// False：Off
        /// </summary>
        private bool smokePurificationSystem;
        public bool SmokePurificationSystem
        {
            get
            {
                return smokePurificationSystem;
            }
            set
            {
                smokePurificationSystem = value;
            }
        }

        /// <summary>
        /// 摇摆系统
        /// True：On
        /// False：Off
        /// </summary>
        private bool swingSystem;
        public bool SwingSystem
        {
            get
            {
                return swingSystem;
            }
            set
            {
                swingSystem = value;
            }
        }

        /// <summary>
        /// 舱盖
        /// True：舱盖打开
        /// False：舱盖关闭
        /// </summary>
        private bool hatch;
        public bool Hatch
        {
            get
            {
                return hatch;
            }
            set
            {
                hatch = value;
            }
        }

        /// <summary>
        /// 点火模式
        /// True：已点火
        /// False：未点火
        /// </summary>
        private bool ignitionStatus;
        public bool InignitionStatus
        {
            get
            {
                return ignitionStatus;
            }
            set
            {
                ignitionStatus = value;
            }
        }

        /// <summary>
        /// 灸疗模式
        /// True：自动
        /// False：手动
        /// </summary>
        private bool moxibustionTherapyMode;
        public bool MoxibustionTherapyMode
        {
            get
            {
                return moxibustionTherapyMode;
            }
            set
            {
                moxibustionTherapyMode = value;
            }
        }

        /// <summary>
        /// 预热时间
        /// 分钟？
        /// </summary>
        private double preheadTime;
        public double PreheadTime
        {
            get
            {
                return preheadTime;
            }
            set
            {
                preheadTime = value;
            }
        }

        /// <summary>
        /// 点火时间
        /// 秒？
        /// </summary>
        private bool ignitionTime;
        public bool InignitionTime
        {
            get
            {
                return ignitionTime;
            }
            set
            {
                ignitionTime = value;
            }
        }

        /// <summary>
        /// 灸疗时间
        /// 分钟？
        /// </summary>
        private bool moxibustionTherapyTime;
        public bool MoxibustionTherapyTime
        {
            get
            {
                return moxibustionTherapyTime;
            }
            set
            {
                moxibustionTherapyTime = value;
            }
        }

        /// <summary>
        /// 治疗结束自动开盖
        /// True：自动开盖
        /// False：不自动开盖
        /// </summary>
        private bool automaticLidOpening;
        public bool AutomaticLidOpening
        {
            get
            {
                return automaticLidOpening;
            }
            set
            {
                automaticLidOpening = value;
            }
        }
    }
}
