using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickLaunchBar
{
    public class ShortcutInfo
    {
        public string FilePath { get; set; }

        public string TargetPath { get; set; }
        public string IconLocation { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }

        public string Title { get; set; }

        public System.Drawing.Icon Icon { get; set; }
    }
}
