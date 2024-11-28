using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoxibustionBedAPP.ViewModes;

namespace MoxibustionBedAPP.ViewModes
{

    public class MainWindowCopyViewModel
    {
        public PlayMusicViewModel SharedVM { get; set; }

        public MainWindowCopyViewModel()
        {
            SharedVM = new PlayMusicViewModel();
        }
    }
}
