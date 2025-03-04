using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoxibustionBedAPP.Models
{
    /// <summary>
    /// 属性
    /// </summary>
    public class PropertyModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 上舱温度
        /// 0~100
        /// </summary>
        private int upper_CabinTemperature;
        public int Upper_CabinTemperature
        {
            get 
            { 
                return upper_CabinTemperature; 
            }
            set 
            { 
                upper_CabinTemperature = value; 
                OnPropertyChanged(nameof(Upper_CabinTemperature));
            }
        }

        /// <summary>
        /// 背部温度
        /// 0~100
        /// </summary>
        private int backTemperature;
        public int BackTemperature
        {
            get
            {
                return backTemperature;
            }
            set
            {
                backTemperature = value;
                OnPropertyChanged(nameof(BackTemperature));
            }
        }

        /// <summary>
        /// 腿部温度
        /// 0~100
        /// </summary>
        private int legTemperature;
        public int LegTemperature
        {
            get
            {
                return legTemperature;
            }
            set
            {
                legTemperature = value;
                OnPropertyChanged(nameof(LegTemperature));
            }
        }

        /// <summary>
        /// 预热温度
        /// 0~100
        /// </summary>
        private int preheadTemperature;
        public int PreheadTemperature
        {
            get
            {
                return preheadTemperature;
            }
            set
            {
                preheadTemperature = value;
                OnPropertyChanged(nameof(PreheadTemperature));
            }
        }



        /// <summary>
        /// 上舱报警温度
        /// 0~100
        /// </summary>
        private int upperAlarmCabinTemperature;
        public int UpperAlarmCabinTemperature
        {
            get
            {
                return upperAlarmCabinTemperature;
            }
            set
            {
                upperAlarmCabinTemperature = value;
                OnPropertyChanged(nameof(UpperAlarmCabinTemperature));
            }
        }

        /// <summary>
        /// 背部报警温度
        /// 0~100
        /// </summary>
        private int backAlarmTemperature;
        public int BackAlarmTemperature
        {
            get
            {
                return backAlarmTemperature;
            }
            set
            {
                backAlarmTemperature = value;
                OnPropertyChanged(nameof(BackAlarmTemperature));
            }
        }

        /// <summary>
        /// 腿部报警温度
        /// 0~100
        /// </summary>
        private int legAlarmTemperature;
        public int LegAlarmTemperature
        {
            get
            {
                return legAlarmTemperature;
            }
            set
            {
                legAlarmTemperature = value;
                OnPropertyChanged(nameof(LegAlarmTemperature));
            }
        }



        /// <summary>
        /// 上舱实时温度
        /// </summary>
        private int upper_CabinTemperatureNow;
        public int Upper_CabinTemperatureNow
        {
            get
            {
                return upper_CabinTemperatureNow;
            }
            set
            {
                upper_CabinTemperatureNow = value;
                OnPropertyChanged(nameof(Upper_CabinTemperatureNow));
            }
        }

        /// <summary>
        /// 背部实时温度
        /// </summary>
        private int backTemperatureNow;
        public int BackTemperatureNow
        {
            get
            {
                return backTemperatureNow;
            }
            set
            {
                backTemperatureNow = value;
                OnPropertyChanged(nameof(BackTemperatureNow));
            }
        }

        /// <summary>
        /// 腿部实时温度
        /// </summary>
        private int legTemperatureNow;
        public int LegTemperatureNow
        {
            get
            {
                return legTemperatureNow;
            }
            set
            {
                legTemperatureNow = value;
                OnPropertyChanged(nameof(LegTemperatureNow));
            }
        }


        /// <summary>
        /// 上舱温度是否报警
        /// </summary>
        private bool isUpperAlarm;
        public bool IsUpperAlarm
        {
            get
            {
                return isUpperAlarm;
            }
            set
            {
                isUpperAlarm = value;
                OnPropertyChanged(nameof(IsUpperAlarm));
            }
        }

        /// <summary>
        /// 背部温度是否报警
        /// </summary>
        private bool isBackAlarm;
        public bool IsBackAlarm
        {
            get
            {
                return isBackAlarm;
            }
            set
            {
                isBackAlarm = value;
                OnPropertyChanged(nameof(IsBackAlarm));
            }
        }

        /// <summary>
        /// 腿部温度是否报警
        /// </summary>
        private bool isLegAlarm;
        public bool IsLegAlarm
        {
            get
            {
                return isLegAlarm;
            }
            set
            {
                isLegAlarm = value;
                OnPropertyChanged(nameof(IsLegAlarm));
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
                OnPropertyChanged(nameof(backMoxibustionColumn_Height));
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
                OnPropertyChanged(nameof(legMoxibustionColumn_Height));
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
                OnPropertyChanged(nameof(BatteryLevel));
            }
        }



        /// <summary>
        /// 红外线
        /// 00:关
        /// 01：1档
        /// 02：2档
        /// 03：3档
        /// </summary>
        private int infraredLamp;
        public int InfraredLamp
        {
            get
            {
                return infraredLamp;
            }
            set
            {
                infraredLamp = value;
                OnPropertyChanged(nameof(InfraredLamp));
            }
        }

        /// <summary>
        /// 排烟系统
        /// 00:关
        /// 01：1档
        /// 02：2档
        /// 03：3档
        /// </summary>
        private int smokeExhaustSystem;
        public int SmokeExhaustSystem
        {
            get
            {
                return smokeExhaustSystem;
            }
            set
            {
                smokeExhaustSystem = value;
                OnPropertyChanged(nameof(SmokeExhaustSystem));
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
                OnPropertyChanged(nameof(SmokePurificationSystem));
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
                OnPropertyChanged(nameof(SwingSystem));
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
                OnPropertyChanged(nameof(Hatch));
            }
        }

        /// <summary>
        /// 点火模式
        /// True：点火中
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
                OnPropertyChanged(nameof(InignitionStatus));
            }
        }

        /// <summary>
        /// 点火状态
        /// True：已点火
        /// False：未点火
        /// </summary>
        private bool isIgnitionStatus;
        public bool IsInignitionStatus
        {
            get
            {
                return isIgnitionStatus;
            }
            set
            {
                isIgnitionStatus = value;
                OnPropertyChanged(nameof(IsInignitionStatus));
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
                OnPropertyChanged(nameof(MoxibustionTherapyMode));
            }
        }

        /// <summary>
        /// 是否处于治疗
        /// True：治疗中
        /// False：未治疗
        /// </summary>
        private bool isMoxibustionTherapyMode;
        public bool IsMoxibustionTherapyMode
        {
            get
            {
                return isMoxibustionTherapyMode;
            }
            set
            {
                isMoxibustionTherapyMode = value;
                OnPropertyChanged(nameof(IsMoxibustionTherapyMode));
            }
        }

        /// <summary>
        /// 预热时间
        /// 分钟
        /// </summary>
        private int preheadTime;
        public int PreheadTime
        {
            get
            {
                return preheadTime;
            }
            set
            {
                preheadTime = value;
                OnPropertyChanged(nameof(PreheadTime));
            }
        }

        /// <summary>
        /// 预热模式
        /// true为：预热开始
        /// false为：停止预热
        /// </summary>
        private bool preheadMode;
        public bool PreheadMode
        {
            get
            {
                return preheadMode;
            }
            set
            {
                preheadMode = value;
                OnPropertyChanged(nameof(PreheadMode));
            }
        }

        /// <summary>
        /// 点火时间
        /// 秒
        /// </summary>
        private int ignitionTime;
        public int InignitionTime
        {
            get
            {
                return ignitionTime;
            }
            set
            {
                ignitionTime = value;
                OnPropertyChanged(nameof(InignitionTime));
            }
        }

        /// <summary>
        /// 灸疗时间
        /// 分钟
        /// </summary>
        private int moxibustionTherapyTime;
        public int MoxibustionTherapyTime
        {
            get
            {
                return moxibustionTherapyTime;
            }
            set
            {
                moxibustionTherapyTime = value;
                OnPropertyChanged(nameof(MoxibustionTherapyTime));
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
                OnPropertyChanged(nameof(AutomaticLidOpening));
            }
        }

        /// <summary>
        /// 倒计时秒
        /// </summary>
        private int _countdownSeconds;
        public int CountdownSeconds
        {
            get { return _countdownSeconds; }
            set
            {
                _countdownSeconds = value;
                OnPropertyChanged(nameof(CountdownSeconds));
            }
        }

        /// <summary>
        /// 倒计时分钟
        /// </summary>
        private int _countdownMinutes;
        public int CountdownMinutes
        {
            get { return _countdownMinutes; }
            set
            {
                _countdownMinutes = value;
                OnPropertyChanged(nameof(CountdownMinutes));
            }
        }


        /// <summary>
        /// 是否是净烟系统
        /// true:净烟系统
        /// false:排烟系统
        /// </summary>
        private bool isSmokePurificationSystem;
        public bool IsSmokePurificationSystem
        {
            get
            {
                return isSmokePurificationSystem;
            }
            set
            {
                isSmokePurificationSystem = value;
                OnPropertyChanged(nameof (IsSmokePurificationSystem));
            }
        }

        /// <summary>
        /// 是否打开排烟
        /// </summary>
        private bool isSmokeSystemOn;
        public bool IsSmokeSystemOn
        {
            get
            {
                return isSmokeSystemOn;
            }
            set
            {
                isSmokeSystemOn = value;
                OnPropertyChanged(nameof (IsSmokeSystemOn));
            }
        }



        /// <summary>
        /// 是否点击问号
        /// </summary>
        private bool _isClickQ;
        public bool IsClickQ
        {
            get
            {
                return _isClickQ;
            }
            set
            {
                _isClickQ = value;
                OnPropertyChanged(nameof(IsClickQ));
            }
        }

        /// <summary>
        /// 是否点击设置
        /// </summary>
        private bool _isClickS;
        public bool IsClickS
        {
            get
            {
                return _isClickS;
            }
            set
            {
                _isClickS = value;
                OnPropertyChanged(nameof(IsClickS));
            }
        }

        /// <summary>
        /// 是否开始治疗后自动跳转到音乐播放界面
        /// </summary>
        private bool _autoMusic;
        public bool AutoMusic
        {
            get
            {
                return _autoMusic;
            }
            set
            {
                _autoMusic = value;
                OnPropertyChanged(nameof(AutoMusic));
            }
        }


        /// <summary>
        /// 自定义控件
        /// </summary>
        private UserControl _currentUserControl;
        public UserControl CurrentUserControl
        {
            get
            {
                return _currentUserControl;
            }
            set
            {
                _currentUserControl = value;
                OnPropertyChanged(nameof(CurrentUserControl));
            }
        }

        /// <summary>
        /// 是否处于开舱状态
        /// </summary>
        private bool isOpen;
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        /// <summary>
        /// 是否处于关舱状态
        /// </summary>
        private bool isClose;
        public bool IsClose
        {
            get
            {
                return isClose;
            }
            set
            {
                isClose = value;
                OnPropertyChanged(nameof(IsClose));
            }
        }

        /// <summary>
        /// 开舱背景图
        /// </summary>
        private string _openHatch;
        public string OpenHatch
        {
            get
            {
                return _openHatch;
            }
            set
            {
                _openHatch = value;
                OnPropertyChanged(nameof(OpenHatch));
            }
        }

        /// <summary>
        /// 关舱背景图
        /// </summary>
        private string _closeHatch;
        public string CloseHatch
        {
            get
            {
                return _closeHatch;
            }
            set
            {
                _closeHatch = value;
                OnPropertyChanged(nameof(CloseHatch));
            }
        }

        /// <summary>
        /// 是否处于语音控制中
        /// </summary>
        private bool _isOnVoice;
        public bool IsOnVoice
        {
            get
            {
                return _isOnVoice;
            }
            set
            {
                _isOnVoice = value;
                OnPropertyChanged(nameof(IsOnVoice));
            }
        }

        /// <summary>
        /// 主板COM口
        /// </summary>
        private string _motherboardCOM;
        public string MotherboardCOM
        {
            get
            {
                return _motherboardCOM;
            }
            set
            {
                _motherboardCOM = value;
                OnPropertyChanged(nameof(MotherboardCOM));
            }
        }

        /// <summary>
        /// 语音助手COM口
        /// </summary>
        private string _aiCOM;
        public string AICOM
        {
            get
            {
                return _aiCOM;
            }
            set
            {
                _aiCOM = value;
                OnPropertyChanged(nameof(AICOM));
            }
        }

    }
}
