using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuickLaunchBar
{
    /// <summary>
    /// QuickLaunchBar.xaml 的交互逻辑
    /// </summary>
    public partial class QuickLaunchPanel : UserControl
    {
        private string _iniFileFullPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "settings.ini");

        private string _shortcutFolderFullPath = @"%APPDATA%\Microsoft\Internet Explorer\Quick Launch";

        private Color _backgroundColor = Colors.Red;
        private Color _buttonMouseOverColor = Colors.Transparent;
        private Color _buttonMouseDownColor = Colors.Transparent;

        private double _buttonMouseOverBrightnessChange = 0.1;
        private double _buttonMouseDownBrightnessChange = 0.15;
        private double _buttonPadding = 3;

        public QuickLaunchPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            stackPanelMain.Children.Clear();

            LoadConfigAndApply();

            string[] shortcutFiles = Directory.GetFiles(_shortcutFolderFullPath, "*.lnk", SearchOption.TopDirectoryOnly);
            Array.Sort(shortcutFiles);

            foreach (string shortcutFilePath in shortcutFiles)
            {
                ShortcutInfo shortcutInfo = GetShortcutInfo(shortcutFilePath);
                if (shortcutInfo == null)
                {
                    continue;
                }

                Button button = AddShortcutIconButton(shortcutInfo);
                if (button == null)
                {
                    continue;
                }

                string dirPath = Path.GetFileNameWithoutExtension(shortcutFilePath);
            }
        }

        private void LoadConfigAndApply()
        {
            // Read values from INI file

            ReadIniStringValue("location", "shortcutFolder", ref _shortcutFolderFullPath);
            _shortcutFolderFullPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(_shortcutFolderFullPath));

            ReadIniColorValue("appearance", "backgroundColor", ref _backgroundColor);

            ReadIniDoubleValue("appearance", "buttonMouseOverBrightnessChange", ref _buttonMouseOverBrightnessChange);
            ReadIniDoubleValue("appearance", "buttonMouseDownBrightnessChange", ref _buttonMouseDownBrightnessChange);

            ReadIniColorValue("appearance", "buttonMouseOverColor", ref _buttonMouseOverColor);
            ReadIniColorValue("appearance", "buttonMouseDownColor", ref _buttonMouseDownColor);

            ReadIniDoubleValue("appearance", "buttonPadding", ref _buttonPadding);

            // Compute colors

            if (_buttonMouseOverColor == Colors.Transparent)
            {
                _buttonMouseOverColor = ChangeColorBrightness(_backgroundColor, (float)_buttonMouseOverBrightnessChange);
            }

            if (_buttonMouseDownColor == Colors.Transparent)
            {
                _buttonMouseDownColor = ChangeColorBrightness(_backgroundColor, (float)_buttonMouseDownBrightnessChange);
            }

            // Apply values

            thisUserControl.Resources["BackgroundColorBrush"] = new SolidColorBrush(_backgroundColor);
            thisUserControl.Resources["ButtonMouseOverColorBrush"] = new SolidColorBrush(_buttonMouseOverColor);
            thisUserControl.Resources["ButtonMouseDownColorBrush"] = new SolidColorBrush(_buttonMouseDownColor);

            thisUserControl.Resources["ButtonPadding"] = new Thickness(_buttonPadding);
        }

        // from https://stackoverflow.com/questions/801406/c-create-a-lighter-darker-color-based-on-a-system-color/12598573#12598573
        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        private static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

        private void ReadIniStringValue(string iniSection, string iniKey, ref string destValue)
        {
            string iniValue = Settings.Read(iniSection, iniKey, string.Empty, _iniFileFullPath);
            if (string.IsNullOrWhiteSpace(iniValue))
            {
                return;
            }

            destValue = iniValue;
        }

        private void ReadIniColorValue(string iniSection, string iniKey, ref Color destValue)
        {
            string iniValue = Settings.Read(iniSection, iniKey, string.Empty, _iniFileFullPath);
            if (string.IsNullOrWhiteSpace(iniValue))
            {
                return;
            }

            Color? convertedColor = ColorConverter.ConvertFromString(iniValue) as Color?;
            if (convertedColor == null)
            {
                return;
            }

            destValue = (Color)convertedColor;
        }

        private void ReadIniDoubleValue(string iniSection, string iniKey, ref double destValue)
        {
            string iniValue = Settings.Read(iniSection, iniKey, string.Empty, _iniFileFullPath);
            if (string.IsNullOrWhiteSpace(iniValue))
            {
                return;
            }

            if (!double.TryParse(iniValue, out double parsedDoubleValue))
            {
                return;
            }

            destValue = parsedDoubleValue;
        }

        private ShortcutInfo GetShortcutInfo(string shortcutFilePath)
        {
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutFilePath);

            string targetPath = link.TargetPath;
            if (!File.Exists(targetPath))
            {
                return null;
            }

            // Use prefix "nn_" for sorting
            string title = Path.GetFileNameWithoutExtension(shortcutFilePath);
            if (title.Length >= 3)
            {
                if (char.IsDigit(title[0]) && char.IsDigit(title[1]) && char.Equals(title[2], '_'))
                {
                    title = title.Substring(3);
                }
            }

            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(targetPath);

            ShortcutInfo shortcutInfo = new ShortcutInfo()
            {
                FilePath = shortcutFilePath,

                TargetPath = targetPath,
                IconLocation = link.IconLocation,
                FullName = link.FullName,
                Description = link.Description,

                Title = title,
                Icon = icon
            };

            return shortcutInfo;
        }

        private Button AddShortcutIconButton(ShortcutInfo shortcutInfo)
        {
            ImageSource iconImageSource = Imaging.CreateBitmapSourceFromHIcon(
                shortcutInfo.Icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            Image image = new Image();
            image.Source = iconImageSource;

            Button button = new Button();
            button.Content = image;
            button.Tag = shortcutInfo;
            button.ToolTip = shortcutInfo.Title;
            button.Click += ShortcutIconButton_Click;

            stackPanelMain.Children.Add(button);

            return button;
        }

        private void ShortcutIconButton_Click(object sender, RoutedEventArgs e)
        {
            ShortcutInfo shortcutInfo = (sender as Button)?.Tag as ShortcutInfo;
            if (shortcutInfo == null)
            {
                return;
            }

            string shortcutFilePath = shortcutInfo.FilePath;
            if (string.IsNullOrEmpty(shortcutFilePath))
            {
                return;
            }

            Process process = new Process();
            process.StartInfo.FileName = shortcutFilePath;
            process.Start();
        }

        private void MenuItemOpenShortcutFolder_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "explorer.exe";
            process.StartInfo.Arguments = _shortcutFolderFullPath;
            process.Start();
        }

        private void MenuItemOpenIniFile_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "notepad.exe";
            process.StartInfo.Arguments = _iniFileFullPath;
            process.Start();
        }

        private void MenuItemReloadAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Init();

                MessageBox.Show("重新加载配置成功", "Quick Launch Bar",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载配置时发生错误: " + ex.Message, "Quick Launch Bar",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
