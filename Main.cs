using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GorillaCheatManager
{
    public partial class Form1 : Form
    {
        private string tempDir;
        private List<string> stagedFiles = new List<string>();

        public Form1()
        {
            InitializeComponent();
            this.Text = "Gorilla Cheat Manager";
            StageFilesToTemp();
            ShowFilesInList();
        }

        private void StageFilesToTemp()
        {
            string[] filesToStage = { "example1.txt", "example2.dll", "example3.exe" };

            tempDir = Path.Combine(Path.GetTempPath(), "GorillaCheatManager_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            foreach (var file in filesToStage)
            {
                if (File.Exists(file))
                {
                    var dest = Path.Combine(tempDir, Path.GetFileName(file));
                    File.Copy(file, dest, true);
                    stagedFiles.Add(dest);
                }
            }
        }

        private void ShowFilesInList()
        {
            checkedListBox1.Items.Clear();
            foreach (var filePath in stagedFiles)
            {
                checkedListBox1.Items.Add(Path.GetFileName(filePath));
            }
        }

        private void buttonFinalize_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Pick at least one file to install.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Choose where to install your files";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string installDir = folderDialog.SelectedPath;
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        if (checkedListBox1.GetItemChecked(i))
                        {
                            string fileName = checkedListBox1.Items[i].ToString();
                            string source = Path.Combine(tempDir, fileName);
                            string dest = Path.Combine(installDir, fileName);
                            File.Copy(source, dest, true);
                        }
                    }
                    MessageBox.Show("Done! Your selected files have been installed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
            catch
            {
            }
            base.OnFormClosed(e);
        }
    }
}
