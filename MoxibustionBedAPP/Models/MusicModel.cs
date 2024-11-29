using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoxibustionBedAPP.Models
{
    public class MusicModel
    {
        /// <summary>
        /// 音乐地址
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 音乐名称
        /// </summary>
        public string MusicName {  get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}
