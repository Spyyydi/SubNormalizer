using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            List<string> output = new List<string>();
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

                    Normalize(path, fileName, output, ref subCounter, ref nOfSubs);
                    messageResults += $"{fileName}\nNumber of subs: {nOfSubs}\nNumber of lines went through: {subCounter}";
                    nOfSubs = 0;
                    subCounter = 0;

                    if (chkSync.Checked)
                    {
                        Synchronize(path, fileName, output, ref timestampsSynced);
                        messageResults += $"\nNumber of timestamps synchronized: {timestampsSynced}";
                        timestampsSynced = 0;
                    }
                    if (chkDash.Checked)
                    {
                        manageDashes(path, fileName, output, ref noOfDashes);
                        messageResults += $"\nNumber of dashes placed: {noOfDashes}";
                        noOfDashes = 0;
                    }
                    replaceWithSpaces(output);
                    File.WriteAllLines(Path.Combine(path, fileName), output);
                    messageResults += "\n\n";
                    output.Clear();
                }
                files.Clear();

                lblNormalized.ForeColor = Color.Green;

                MessageBox.Show(messageResults);
            }
            else
            {
                int constTextLen = "File path: ".Length;

                Normalize(lblFileNamePath.Text.Substring(constTextLen), oldFileName, output, ref subCounter, ref nOfSubs);
                messageResults = $"Number of subs: {nOfSubs}\nNumber of lines went through: {subCounter}";

                if (chkSync.Checked)
                {
                    Synchronize(lblFileNamePath.Text.Substring(constTextLen), oldFileName, output, ref timestampsSynced);
                    messageResults += $"\nNumber of timestamps synchronized: {timestampsSynced}";
                }
                if (chkDash.Checked)
                {
                    manageDashes(lblFileNamePath.Text.Substring(constTextLen), oldFileName, output, ref noOfDashes);
                    messageResults += $"\nNumber of dashes placed: {noOfDashes}";
                }
                replaceWithSpaces(output);
                string newFileName = txtFileName.Text;
                if (newFileName.Substring(newFileName.Length - 4).ToLower() != ".srt")
                    newFileName += ".srt";
                File.WriteAllLines(Path.Combine(lblFileNamePath.Text.Substring(constTextLen), newFileName), output);
                output.Clear();
                lblNormalized.ForeColor = Color.Green;

                MessageBox.Show(messageResults);
            }

            btnNormalize.Enabled = false;
            chkSync.Enabled = false;
            chkDash.Enabled = false;
            if (chkSync.Checked || chkDash.Checked) btnCheckSync.Visible = true;
            chkSync.Checked = false;
            chkDash.Checked = false;
        }

        private void Normalize(string filePath, string oldFileName, List<string> output, ref int subCounter, ref int nOfSubs)
        {
            string temp = "";
            int flag = 0;

            // Read the file line by line.
            string[] subs = File.ReadAllLines(Path.Combine(filePath, oldFileName));
            foreach (string line in subs)
            {
                int count;
                if (int.TryParse(line, out count) && flag == 2)
                {
                    output.Add(temp.Trim());
                    output.Add("");
                    nOfSubs = count;
                    temp = "";
                    flag = 0;
                }

                if (flag == 0)
                {
                    //line with subtitle number
                    output.Add(line);
                    flag = 1;
                }
                else if (flag == 1)
                {
                    //line with subtitle timestamps
                    output.Add(line);
                    flag = 2;
                }
                else if (flag == 2)
                {
                    //line with subtitle text
                    if (line.Length > 0)
                    {
                        temp += line.Trim() + " ";
                    }
                }
                subCounter++;
            }
            if (temp != "")
            {
                output.Add(temp.Trim());
            }
        }

        private void Synchronize(string filePath, string oldFileName, List<string> output, ref int timestampsSynced)
        {
            string fileToSync = Path.Combine(filePath, oldFileName);
            if (prefixedFiles[fileToSync].Value)
            {
                List<string> oldOutput = new List<string>(output);
                output.Clear();
                List<string> prefixedSubs = new List<string>(File.ReadAllLines(prefixedFiles[fileToSync].Key));

                for (int i = 0; i < oldOutput.Count; i++)
                {
                    if (i % 4 == 1)
                    {
                        //timestamp
                        output.Add(prefixedSubs[i]);
                        timestampsSynced++;
                    }
                    else
                    {
                        output.Add(oldOutput[i]);
                    }
                }
                oldOutput.Clear();
                prefixedSubs.Clear();
            }
        }

        private void manageDashes(string filePath, string oldFileName, List<string> output, ref int noOfDashes)
        {
            string fileToSync = Path.Combine(filePath, oldFileName);
            if (prefixedFiles[fileToSync].Value)
            {
                List<string> oldOutput = new List<string>(output);
                output.Clear();
                List<string> prefixedSubs = new List<string>(File.ReadAllLines(prefixedFiles[fileToSync].Key));

                for (int i = 0; i < oldOutput.Count; i++)
                {
                    if (i % 4 == 2)
                    {
                        const string firstDashOccurrencePattern = @"^[\p{Pd}]+";
                        Match dashFoundInPrefixedSubs = Regex.Match(prefixedSubs[i], firstDashOccurrencePattern);
                        Match dashFoundInOldOutput = Regex.Match(oldOutput[i], firstDashOccurrencePattern);
                        if (prefixedSubs[i].StartsWith("{"))
                        {
                            output.Add("- " + oldOutput[i]);
                            noOfDashes++;
                        }
                        else if(dashFoundInOldOutput.Success && dashFoundInPrefixedSubs.Success)
                        {
                            output.Add(Regex.Replace(oldOutput[i], firstDashOccurrencePattern, "").Trim());
                        }
                        else
                        {
                            output.Add(oldOutput[i]);
                        }
                    }
                    else
                    {
                        output.Add(oldOutput[i]);
                    }
                }
                oldOutput.Clear();
                prefixedSubs.Clear();
            }
        }

        private void replaceWithSpaces(List<string> output)
        {
            const string whitespaceCharPattern = @"\s+";
            const string space = " ";

            List<string> oldOutput = new List<string>(output);
            output.Clear();
            for (int i = 0; i < oldOutput.Count; i++)
            {
                if (i % 4 == 2)
                {
                    output.Add(Regex.Replace(oldOutput[i], whitespaceCharPattern, space));
                }
                else
                {
                    output.Add(oldOutput[i]);
                }
            }
            oldOutput.Clear();
        }

        private void btnChooseFileNamePath_Click(object sender, EventArgs e)
        {
            btnCheckSync.Visible = false;
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
            btnCheckSync.Visible = false;
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

        private void btnCheckSync_Click(object sender, EventArgs e)
        {
            //show files synchronized and not synchronized 
            //10 ot of 10 were synced or 8 of 10 were synced and print file names that were not synced
            string notSynced = "Not synchronized files:\n";
            int synced = 0;
            int total = 0;

            foreach (var file in prefixedFiles)
            {
                if (file.Key.Contains("\\"))
                {
                    total++;

                    if (file.Value.Value)
                    {
                        synced++;
                    }
                    else
                    {
                        int fileNameStartIndex = file.Key.LastIndexOf('\\') + 1;
                        notSynced += file.Key.Substring(fileNameStartIndex) + "\n";
                    }
                }
            }
            string msg = "";
            if (total == synced)
            {
                msg = $"{synced}/{total} synchronized";
            }
            else
            {
                msg = $"{synced}/{total} synchronized\n\n";
                msg += notSynced;
            }
            MessageBox.Show(msg);
        }
    }
}
