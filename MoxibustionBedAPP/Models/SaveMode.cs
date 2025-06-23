using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoxibustionBedAPP.Models
{
    public class SaveMode
    {
        /// <summary>
        /// 灸疗模式
        /// true:自动
        /// false:手动
        /// </summary>
        public bool MoxibustionTherapyMode { get; set; }

        /// <summary>
        /// 背部灸柱高度
        /// 0：0档
        /// 1：1档
        /// 2：2档
        /// 3：3档
        /// 4：4档
        /// </summary>
        public int BackMoxibustionColumn_Height { get; set; }

        /// <summary>
        /// 腿部灸柱高度
        /// 0：0档
        /// 1：1档
        /// 2：2档
        /// 3：3档
        /// 4：4档
        /// </summary>
        public int LegMoxibustionColumn_Height { get; set; }

        /// <summary>
        /// 排烟系统
        /// 00:关
        /// 01：1档
        /// 02：2档
        /// 03：3档
        /// </summary>
        public int SmokeExhaustSystem { get; set; }

        /// <summary>
        /// 净烟系统
        /// True：On
        /// False：Off
        /// </summary>
        public bool SmokePurificationSystem {  get; set; }

        /// <summary>
        /// 是否是净烟系统
        /// true:净烟系统
        /// false:排烟系统
        /// </summary>
        public bool IsSmokePurificationSystem { get; set; }

        /// <summary>
        /// 上舱温度
        /// 0~100
        /// </summary>
        public int Upper_CabinTemperature { get; set; }

        /// <summary>
        /// 背部温度
        /// 0~100
        /// </summary>
        public int BackTemperature{ get; set; }

        /// <summary>
        /// 腿部温度
        /// 0~100
        /// </summary>
        public int LegTemperature { get; set; }

        /// <summary>
        /// 预热温度
        /// 0~100
        /// </summary>
        public int PreheadTemperature { get; set; }

        /// <summary>
        /// 上舱报警温度
        /// 0~100
        /// </summary>
        public int UpperAlarmCabinTemperature{ get; set; }

        /// <summary>
        /// 背部报警温度
        /// 0~100
        /// </summary>
        public int BackAlarmTemperature{ get; set; }

        /// <summary>
        /// 腿部报警温度
        /// 0~100
        /// </summary>
        public int LegAlarmTemperature { get; set; }

        /// <summary>
        /// 预热时间
        /// 分钟
        /// </summary>
        public int PreheadTime {  get; set; }


        /// <summary>
        /// 点火时间
        /// 秒
        /// </summary>
        public int InignitionTime {  get; set; }

        /// <summary>
        /// 灸疗时间
        /// 分钟
        /// </summary>
        public int MoxibustionTherapyTime {  get; set; }

        /// <summary>
        /// 结束后自动开舱
        /// </summary>
        public bool AutomaticLidOpening { get; set; }

        /// <summary>
        /// 是否开始治疗后自动跳转到音乐播放界面
        /// </summary>
        public bool AutoMusic { get; set; }
    }
}
