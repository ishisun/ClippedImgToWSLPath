using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

namespace ClippedImgToWSLPath
{
    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon = null!;
        private string savePath;
        private string lastClipboardHash = "";
        private string logPath = Path.Combine(Application.StartupPath, "clipboard_log.txt");
        private bool isProcessingClipboard = false;
        private bool enableLogging = false; // Logging on/off

        // Utility class instances for path conversion, image hashing, and settings
        private readonly PathConverter pathConverter = new PathConverter();
        private readonly ImageHashCalculator imageHashCalculator = new ImageHashCalculator();
        private readonly SettingsManager settingsManager;
        private ProjectPathResolver projectPathResolver;

        // AddClipboardFormatListener API for event-driven clipboard monitoring
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        const int WM_CLIPBOARDUPDATE = 0x031D;

        public MainForm()
        {
            InitializeComponent();

            // Hide the form before doing anything else
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;

            // Load settings from file
            settingsManager = new SettingsManager(Application.StartupPath);
            settingsManager.Load();
            projectPathResolver = new ProjectPathResolver(settingsManager);
            savePath = projectPathResolver.GetSavePath();
            enableLogging = settingsManager.EnableLogging;

            SetupSystemTray();

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // Register for clipboard format listener notifications
            if (!AddClipboardFormatListener(this.Handle))
            {
                WriteLog("Failed to add clipboard format listener");
            }
        }

        private void SetupSystemTray()
        {
            trayIcon = new NotifyIcon();
            
            // アイコンファイルから読み込み、存在しない場合はデフォルトを使用
            string iconPath = Path.Combine(Application.StartupPath, "icon.ico");
            if (File.Exists(iconPath))
            {
                trayIcon.Icon = new Icon(iconPath);
            }
            else
            {
                // 埋め込みリソースから読み込みを試みる
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "ClippedImgToWSLPath.icon.ico";
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        trayIcon.Icon = new Icon(stream);
                    }
                    else
                    {
                        trayIcon.Icon = SystemIcons.Application;
                    }
                }
            }
            
            trayIcon.Text = "Clipboard Image to WSL Path";
            trayIcon.Visible = true;

            var contextMenu = new ContextMenuStrip();
            
            var settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += (s, e) => ShowSettingsDialog();
            contextMenu.Items.Add(settingsItem);

            var loggingItem = new ToolStripMenuItem("Enable Logging");
            loggingItem.Checked = enableLogging;
            loggingItem.Click += (s, e) =>
            {
                enableLogging = !enableLogging;
                loggingItem.Checked = enableLogging;
                settingsManager.EnableLogging = enableLogging;
                settingsManager.Save();
                if (enableLogging)
                {
                    WriteLog("Logging enabled");
                }
            };
            contextMenu.Items.Add(loggingItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => ExitApplication();
            contextMenu.Items.Add(exitItem);

            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.DoubleClick += (s, e) => ShowSettingsDialog();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLIPBOARDUPDATE:
                    ProcessClipboardChange();
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void ProcessClipboardChange()
        {
            if (isProcessingClipboard) return;

            try
            {
                isProcessingClipboard = true;

                if (Clipboard.ContainsImage())
                {
                    WriteLog("Clipboard update: contains image");
                    Image? image = Clipboard.GetImage();

                    if (image != null)
                    {
                        string hash = GetImageHash(image);

                        if (hash != lastClipboardHash)
                        {
                            WriteLog($"New image: {image.Width}x{image.Height}");
                            lastClipboardHash = hash;
                            SaveImageAndConvertPath(image);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Clipboard processing error: {ex.Message}");
            }
            finally
            {
                isProcessingClipboard = false;
            }
        }

        private string GetImageHash(Image image)
        {
            return imageHashCalculator.ComputeHash(image);
        }

        private void SaveImageAndConvertPath(Image image)
        {
            try
            {
                string fileName = $"clipboard_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(savePath, fileName);

                image.Save(filePath, ImageFormat.Png);

                string clipboardPath = projectPathResolver.GetClipboardPath(filePath);

                Clipboard.SetText(clipboardPath);

                string balloonMessage;
                if (projectPathResolver.IsProjectModeActive())
                {
                    balloonMessage = $"Saved to: {filePath}\nRelative Path: {clipboardPath}\n(Relative path copied to clipboard)";
                }
                else
                {
                    balloonMessage = $"Saved to: {filePath}\nWSL Path: {clipboardPath}\n(WSL path copied to clipboard)";
                }

                ShowBalloonTip("Image Saved", balloonMessage, ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                ShowBalloonTip("Error", $"Failed to save image: {ex.Message}", ToolTipIcon.Error);
            }
        }

        private string ConvertToWSLPath(string windowsPath)
        {
            return pathConverter.ConvertToWSLPath(windowsPath);
        }

        private void ShowBalloonTip(string title, string text, ToolTipIcon icon)
        {
            trayIcon.ShowBalloonTip(3000, title, text, icon);
        }

        private void ShowSettingsDialog()
        {
            using (var dialog = new SettingsDialog(settingsManager))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Update settings from dialog
                    settingsManager.SavePath = dialog.SavePath;
                    settingsManager.ProjectModeEnabled = dialog.ProjectModeEnabled;
                    settingsManager.ProjectRootPath = dialog.ProjectRootPath;
                    settingsManager.ProjectScreenshotsDir = dialog.ProjectScreenshotsDir;
                    settingsManager.Save();

                    // Update save path using ProjectPathResolver
                    savePath = projectPathResolver.GetSavePath();

                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                }
            }
        }

        private void ExitApplication()
        {
            RemoveClipboardFormatListener(this.Handle);
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void SetVisibleCore(bool value)
        {
            // Prevent the form from becoming visible on startup
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
                value = false;
            }
            base.SetVisibleCore(value);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
            }
            else
            {
                RemoveClipboardFormatListener(this.Handle);
            }
            base.OnFormClosing(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MainForm";
            this.Text = "Clipboard Image to WSL Path";
            this.ResumeLayout(false);
        }

        private void WriteLog(string message)
        {
            if (!enableLogging) return;
            
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // ログ書き込みエラーは無視
            }
        }

        private Image GetImageFromDIB(MemoryStream dibStream)
        {
            try
            {
                WriteLog("Converting DIB to Image");
                byte[] dibBytes = dibStream.ToArray();
                
                // DIBヘッダーのサイズを取得
                int headerSize = BitConverter.ToInt32(dibBytes, 0);
                int width = BitConverter.ToInt32(dibBytes, 4);
                int height = BitConverter.ToInt32(dibBytes, 8);
                short planes = BitConverter.ToInt16(dibBytes, 12);
                short bpp = BitConverter.ToInt16(dibBytes, 14);
                
                WriteLog($"DIB info: HeaderSize={headerSize}, Width={width}, Height={height}, BPP={bpp}");
                
                // BitmapFileHeaderを作成
                byte[] bmpBytes = new byte[14 + dibBytes.Length];
                bmpBytes[0] = 0x42; // 'B'
                bmpBytes[1] = 0x4D; // 'M'
                BitConverter.GetBytes(bmpBytes.Length).CopyTo(bmpBytes, 2);
                BitConverter.GetBytes(14 + headerSize + (bpp <= 8 ? (1 << bpp) * 4 : 0)).CopyTo(bmpBytes, 10);
                
                // DIBデータをコピー
                Array.Copy(dibBytes, 0, bmpBytes, 14, dibBytes.Length);
                
                using (var ms = new MemoryStream(bmpBytes))
                {
                    return new Bitmap(ms);
                }
            }
            catch (Exception ex)
            {
                WriteLog($"GetImageFromDIB error: {ex.Message}");
                return null!;
            }
        }
    }
}