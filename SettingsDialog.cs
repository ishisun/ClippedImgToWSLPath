using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClippedImgToWSLPath
{
    public class SettingsDialog : Form
    {
        private TextBox pathTextBox = null!;
        private Button browseButton = null!;
        private Button okButton = null!;
        private Button cancelButton = null!;
        private Label pathLabel = null!;

        // Project Mode controls
        private CheckBox projectModeCheckBox = null!;
        private Label projectRootLabel = null!;
        private TextBox projectRootTextBox = null!;
        private Button projectRootBrowseButton = null!;
        private Label screenshotsDirLabel = null!;
        private TextBox screenshotsDirTextBox = null!;
        private GroupBox projectModeGroupBox = null!;

        public string SavePath { get; private set; }
        public bool ProjectModeEnabled { get; private set; }
        public string ProjectRootPath { get; private set; }
        public string ProjectScreenshotsDir { get; private set; }

        public SettingsDialog(SettingsManager settings)
        {
            InitializeComponent();
            SavePath = settings.SavePath;
            ProjectModeEnabled = settings.ProjectModeEnabled;
            ProjectRootPath = settings.ProjectRootPath;
            ProjectScreenshotsDir = settings.ProjectScreenshotsDir;

            pathTextBox.Text = settings.SavePath;
            projectModeCheckBox.Checked = settings.ProjectModeEnabled;
            projectRootTextBox.Text = settings.ProjectRootPath;
            screenshotsDirTextBox.Text = settings.ProjectScreenshotsDir;

            UpdateProjectModeControlsState();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Save Location section
            pathLabel = new Label
            {
                Text = "Save Location:",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pathTextBox = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(350, 25),
                ReadOnly = true
            };

            browseButton = new Button
            {
                Text = "Browse...",
                Location = new Point(380, 48),
                Size = new Size(80, 25)
            };
            browseButton.Click += BrowseButton_Click;

            // Project Mode section
            projectModeCheckBox = new CheckBox
            {
                Text = "Enable Project Mode",
                Location = new Point(20, 95),
                Size = new Size(200, 25),
                AutoSize = true
            };
            projectModeCheckBox.CheckedChanged += ProjectModeCheckBox_CheckedChanged;

            projectModeGroupBox = new GroupBox
            {
                Text = "Project Mode Settings",
                Location = new Point(20, 120),
                Size = new Size(440, 130)
            };

            projectRootLabel = new Label
            {
                Text = "Project Root:",
                Location = new Point(10, 25),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            projectRootTextBox = new TextBox
            {
                Location = new Point(10, 50),
                Size = new Size(320, 25),
                ReadOnly = true
            };

            projectRootBrowseButton = new Button
            {
                Text = "Browse...",
                Location = new Point(340, 48),
                Size = new Size(80, 25)
            };
            projectRootBrowseButton.Click += ProjectRootBrowseButton_Click;

            screenshotsDirLabel = new Label
            {
                Text = "Screenshots Folder:",
                Location = new Point(10, 80),
                Size = new Size(130, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            screenshotsDirTextBox = new TextBox
            {
                Location = new Point(10, 100),
                Size = new Size(200, 25)
            };

            projectModeGroupBox.Controls.Add(projectRootLabel);
            projectModeGroupBox.Controls.Add(projectRootTextBox);
            projectModeGroupBox.Controls.Add(projectRootBrowseButton);
            projectModeGroupBox.Controls.Add(screenshotsDirLabel);
            projectModeGroupBox.Controls.Add(screenshotsDirTextBox);

            // Buttons
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(280, 265),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(380, 265),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(pathLabel);
            this.Controls.Add(pathTextBox);
            this.Controls.Add(browseButton);
            this.Controls.Add(projectModeCheckBox);
            this.Controls.Add(projectModeGroupBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void BrowseButton_Click(object? sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder to save images";
                dialog.SelectedPath = pathTextBox.Text;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    pathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            SavePath = pathTextBox.Text;
            ProjectModeEnabled = projectModeCheckBox.Checked;
            ProjectRootPath = projectRootTextBox.Text;
            ProjectScreenshotsDir = screenshotsDirTextBox.Text;
        }

        private void ProjectModeCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateProjectModeControlsState();
        }

        private void ProjectRootBrowseButton_Click(object? sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the project root directory";
                dialog.SelectedPath = projectRootTextBox.Text;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    projectRootTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void UpdateProjectModeControlsState()
        {
            bool enabled = projectModeCheckBox.Checked;
            projectModeGroupBox.Enabled = enabled;
            projectRootTextBox.Enabled = enabled;
            projectRootBrowseButton.Enabled = enabled;
            screenshotsDirTextBox.Enabled = enabled;
        }
    }
}