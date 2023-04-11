using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubNormalizer
{
    public partial class Form1 : Form
    {
        string oldFileName = "";
        const string PREFIX = "VEED-subtitles_";
        bool multipleFiles = false;
        List<string> files = new List<string>();
        Dictionary<string, KeyValuePair<string, bool>> prefixedFiles = new Dictionary<string, KeyValuePair<string, bool>>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnNormalize_Click(object sender, EventArgs e)
        {
            string output = "";
            int subCounter = 0;
            int nOfSubs = 0;
            int timestampsSynced = 0;
            int noOfDashes = 0;

            string messageResults = "";

            if (multipleFiles)
            {
                foreach (string file in files)
                {
                    int fileNameStartIndex = file.LastIndexOf('\\') + 1;
                    string path = file.Substring(0, fileNameStartIndex);
                    string fileName = file.Substring(fileNameStartIndex);

                    Normalize(path, fileName, ref output, ref subCounter, ref nOfSubs);
                    messageResults += $"{fileName}\nNumber of subs: {nOfSubs}\nNumber of lines went through: {subCounter}";
                    nOfSubs = 0;
                    subCounter = 0;

                    if (chkSync.Enabled)
                    {
                        Synchronize(path, fileName, ref output, ref timestampsSynced);
                        messageResults += $"\nNumber of timestamps synchronized: {timestampsSynced}";
                        timestampsSynced = 0;
                    }
                    if (chkDash.Enabled)
                    {
                        putDashes(path, fileName, ref output, ref noOfDashes);
                        messageResults += $"\nNumber of dashes placed: {noOfDashes}";
                        noOfDashes = 0;
                    }

                    File.WriteAllText(Path.Combine(path, fileName), output);
                    messageResults += "\n\n";
                    output = "";
                }
                files.Clear();

                lblNormalized.ForeColor = Color.Green;

                MessageBox.Show(messageResults);
            }
            else
            {
                int constTextLen = "File path: ".Length;

                Normalize(lblFileNamePath.Text.Substring(constTextLen), oldFileName, ref output, ref subCounter, ref nOfSubs);
                messageResults = $"Number of subs: {nOfSubs}\nNumber of lines went through: {subCounter}";

                if (chkSync.Enabled)
                {
                    Synchronize(lblFileNamePath.Text.Substring(constTextLen), oldFileName, ref output, ref timestampsSynced);
                    messageResults += $"\nNumber of timestamps synchronized: {timestampsSynced}";
                }
                if (chkDash.Enabled)
                {
                    putDashes(lblFileNamePath.Text.Substring(constTextLen), oldFileName, ref output, ref noOfDashes);
                    messageResults += $"\nNumber of dashes placed: {noOfDashes}";
                }

                string newFileName = txtFileName.Text;
                if (newFileName.Substring(newFileName.Length - 4).ToLower() != ".srt")
                    newFileName += ".srt";
                File.WriteAllText(Path.Combine(lblFileNamePath.Text.Substring(constTextLen), newFileName), output);
                lblNormalized.ForeColor = Color.Green;

                MessageBox.Show(messageResults);
            }

            btnNormalize.Enabled = false;
            chkSync.Enabled = false;
            chkDash.Enabled = false;
        }

        private void Normalize(string filePath, string oldFileName, ref string output, ref int subCounter, ref int nOfSubs)
        {
            string temp = "";
            int flag = 0;

            // Read the file line by line.
            foreach (string line in File.ReadLines(Path.Combine(filePath, oldFileName)))
            {
                int count;
                if (int.TryParse(line, out count) && flag == 2)
                {
                    output += temp.Trim() + "\n\n";
                    nOfSubs = count;
                    temp = "";
                    flag = 0;
                }

                if (flag == 0)
                {
                    output += line + "\n";
                    flag = 1;
                }
                else if (flag == 1)
                {
                    output += line + "\n";
                    flag = 2;
                }
                else if (flag == 2)
                {
                    if (line.Length > 0)
                    {
                        temp += line.Trim() + " ";
                    }
                }
                subCounter++;
            }
            if (temp != "")
            {
                output += temp.Trim() + "\n";
            }
        }

        private void Synchronize(string filePath, string oldFileName, ref string output, ref int timestampsSynced)
        {

        }

        private void putDashes(string filePath, string oldFileName, ref string output, ref int noOfDashes)
        {

        }

        private void btnChooseFileNamePath_Click(object sender, EventArgs e)
        {
            prefixedFiles.Clear();

            lblFileNamePath.Text = "File path: ";
            txtFileName.Text = "";
            lblNormalized.ForeColor = Color.Red;

            openFileDialog.Title = "Choose sub file";
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Subtitles|*.srt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int fileNameStartIndex = openFileDialog.FileName.LastIndexOf('\\') + 1;
                string path = openFileDialog.FileName.Substring(0, fileNameStartIndex);
                lblFileNamePath.Text = "File path: " + path;
                txtFileName.Text = openFileDialog.FileName.Substring(fileNameStartIndex);
                oldFileName = txtFileName.Text;

                string prefixedFile = Path.Combine(path, PREFIX + oldFileName);
                if (File.Exists(prefixedFile))
                {
                    prefixedFiles.Add(Path.Combine(path, oldFileName), new KeyValuePair<string, bool>(prefixedFile, true));
                    chkSync.Enabled = true;
                    chkDash.Enabled = true;
                }

                txtFileName.Enabled = true;
                btnNormalize.Enabled = true;
                multipleFiles = false;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            prefixedFiles.Clear();

            lblFileNamePath.Text = "File path: ";
            txtFileName.Text = "";
            lblNormalized.ForeColor = Color.Red;

            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int srtCount = 0;

            if (fileList.Length > 1)
            {
                bool isWithPrefix = false;
                foreach (string file in fileList)
                {
                    if (file.Substring(file.Length - 4).ToLower() == ".srt")
                    {
                        int fileNameStartIndex = file.LastIndexOf('\\') + 1;
                        string path = file.Substring(0, fileNameStartIndex);
                        string fileName = file.Substring(fileNameStartIndex);

                        //separate file with and without prefix
                        if (fileName.Substring(0, PREFIX.Length) == PREFIX)
                        {
                            //if non prefixed file already read modify dictionary that both files are found
                            if (prefixedFiles.ContainsKey(Path.Combine(path, fileName.Substring(PREFIX.Length))))
                            {
                                prefixedFiles[Path.Combine(path, fileName.Substring(PREFIX.Length))] = new KeyValuePair<string, bool>(file, true);
                            }
                            else
                            {
                                prefixedFiles.Add(fileName.Substring(PREFIX.Length), new KeyValuePair<string, bool>(file, false));
                            }
                        }
                        else
                        {
                            //if prefixed file already read modify dictionary that both files are found
                            if (prefixedFiles.ContainsKey(fileName))
                            {
                                KeyValuePair<string, bool> oldPair = prefixedFiles[fileName];
                                prefixedFiles.Remove(fileName);
                                prefixedFiles.Add(file, new KeyValuePair<string, bool>(oldPair.Key, true));
                            }
                            else
                            {
                                prefixedFiles.Add(file, new KeyValuePair<string, bool>("", false));
                            }

                            files.Add(file);
                            srtCount++;
                        }
                    }
                }

                //check for files that are without pair for synchronization and dashes
                Dictionary<string, KeyValuePair<string, bool>> oldPrefixedFiles = new Dictionary<string, KeyValuePair<string, bool>>(prefixedFiles);
                prefixedFiles.Clear();
                foreach (var prefixedFile in oldPrefixedFiles)
                {
                    if (!prefixedFile.Value.Value)
                    {
                        int fileNameStartIndex = prefixedFile.Key.LastIndexOf('\\') + 1;
                        string path = prefixedFile.Key.Substring(0, fileNameStartIndex);
                        bool found = false;

                        //if prefixed file not exist check if it in same directory as non prefixed file
                        if (prefixedFile.Value.Key == "")
                        {
                            string prefixedFileName = PREFIX + prefixedFile.Key.Substring(fileNameStartIndex);
                            if (File.Exists(Path.Combine(path, prefixedFileName)))
                            {
                                prefixedFiles.Add(prefixedFile.Key, new KeyValuePair<string, bool>(Path.Combine(path, prefixedFileName), true));
                                found = true;
                            }
                        }
                        else
                        {
                            string nonPrefixedFileName = prefixedFile.Key.Substring(fileNameStartIndex + PREFIX.Length);
                            if (File.Exists(Path.Combine(path, nonPrefixedFileName)))
                            {
                                KeyValuePair<string, bool> oldPair = oldPrefixedFiles[nonPrefixedFileName];
                                prefixedFiles.Add(Path.Combine(path, nonPrefixedFileName), new KeyValuePair<string, bool>(oldPair.Key, true));
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            prefixedFiles.Add(prefixedFile.Key, prefixedFile.Value);
                        }
                        else
                        {
                            isWithPrefix = true;
                        }
                    }
                    else
                    {
                        prefixedFiles.Add(prefixedFile.Key, prefixedFile.Value);
                        isWithPrefix = true;
                    }
                }
                oldPrefixedFiles.Clear();

                //check how many subs found
                if (srtCount == 1)
                {
                    multipleFiles = false;
                    txtFileName.Enabled = true;
                    if (isWithPrefix)
                    {
                        chkSync.Enabled = true;
                        chkDash.Enabled = true;
                    }

                    int fileNameStartIndex = files[0].LastIndexOf('\\') + 1;
                    lblFileNamePath.Text = "File path: " + files[0].Substring(0, fileNameStartIndex);
                    txtFileName.Text = files[0].Substring(fileNameStartIndex);
                    oldFileName = txtFileName.Text;

                    files.Clear();

                    MessageBox.Show("Number of files to normalize: " + srtCount);
                }
                else if (srtCount > 0)
                {
                    multipleFiles = true;
                    txtFileName.Text = "";
                    txtFileName.Enabled = false;
                    if (isWithPrefix)
                    {
                        chkSync.Enabled = true;
                        chkDash.Enabled = true;
                    }

                    MessageBox.Show("Number of files to normalize: " + srtCount);
                }
                else
                {
                    MessageBox.Show("There is no .srt files to normalize");
                    multipleFiles = false;
                }
            }
            else
            {
                txtFileName.Enabled = true;

                if (fileList[0].Substring(fileList[0].Length - 4).ToLower() == ".srt")
                {
                    int fileNameStartIndex = fileList[0].LastIndexOf('\\') + 1;
                    lblFileNamePath.Text = "File path: " + fileList[0].Substring(0, fileNameStartIndex);
                    txtFileName.Text = fileList[0].Substring(fileNameStartIndex);
                    oldFileName = txtFileName.Text;
                    srtCount = 1;

                    //check for file for synchronization and dashes
                    string prefixedFile = Path.Combine(fileList[0].Substring(0, fileNameStartIndex), PREFIX + oldFileName);
                    if (File.Exists(prefixedFile))
                    {
                        prefixedFiles.Add(fileList[0], new KeyValuePair<string, bool>(prefixedFile, true));
                        chkSync.Enabled = true;
                        chkDash.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("There is no .srt files to normalize");
                    srtCount = 0;
                }
                multipleFiles = false;
            }

            if (srtCount > 0) btnNormalize.Enabled = true;
            else btnNormalize.Enabled = false;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {
            if (txtFileName.Text == "")
            {
                btnNormalize.Enabled = false;
            }
            else
            {
                btnNormalize.Enabled = true;
            }
        }
    }
}
