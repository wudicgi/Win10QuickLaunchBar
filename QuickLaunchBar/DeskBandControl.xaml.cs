using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsDeskBand.DeskBand.BandParts;
using WPFBand;

namespace QuickLaunchBar
{
    /// <summary>
    /// DeskBandControl.xaml 的交互逻辑
    /// </summary>
    [ComVisible(true)]
    [Guid("eabd5a5b-4273-4fb8-a851-aa0d4b803534")]
    [BandRegistration(Name = "CloseMonitor", ShowDeskBand = true)]
    public partial class DeskBandControl : WPFBandControl
    {
        public DeskBandControl()
        {
            Options.MinHorizontalSize.Width = 24;

            InitializeComponent();

            Options.ContextMenuItems = bandControlT.DeskBandContextMenu;
        }
    }
}
