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
        bool multipleFiles = false;
        List<string> files = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnNormalize_Click(object sender, EventArgs e)
        {
            string output = "";
            int subCounter = 0;
            int nOfSubs = 0;

            if(multipleFiles)
            {
                string messageResults = "";
                foreach(string file in files)
                {
                    int fileNameStartIndex = file.LastIndexOf('\\') + 1;
                    string path = file.Substring(0, fileNameStartIndex);
                    string fileName = file.Substring(fileNameStartIndex);

                    Normalize(path, fileName, ref output, ref subCounter, ref nOfSubs);

                    File.WriteAllText(Path.Combine(path, fileName), output);
                    messageResults += $"{fileName}\nNumber of subs: {nOfSubs}\nNumber of lines went through: {subCounter}\n\n";
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
                
                File.WriteAllText(Path.Combine(lblFileNamePath.Text.Substring(constTextLen), txtFileName.Text), output);
                lblNormalized.ForeColor = Color.Green;

                MessageBox.Show($"Number of subs: {nOfSubs}\nNumber of lines went through: {subCounter}");
            }
            btnNormalize.Enabled = false;
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
                    if(line.Length > 0)
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

        private void btnChooseFileNamePath_Click(object sender, EventArgs e)
        {
            lblFileNamePath.Text = "File path: ";
            txtFileName.Text = "";
            lblNormalized.ForeColor = Color.Red;

            openFileDialog.Title = "Choose sub file";
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Subtitles|*.srt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int fileNameStartIndex = openFileDialog.FileName.LastIndexOf('\\') + 1;
                lblFileNamePath.Text = "File path: " + openFileDialog.FileName.Substring(0, fileNameStartIndex);
                txtFileName.Text = openFileDialog.FileName.Substring(fileNameStartIndex);
                oldFileName = txtFileName.Text;

                txtFileName.Enabled = true;
                btnNormalize.Enabled = true;
                multipleFiles = false;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            lblFileNamePath.Text = "File path: ";
            txtFileName.Text = "";
            lblNormalized.ForeColor = Color.Red;

            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int srtCount = 0;
            if (fileList.Length > 1)
            {
                foreach (string file in fileList)
                {
                    if (file.Substring(file.Length - 4).ToLower() == ".srt")
                    {
                        files.Add(file);
                        srtCount++;
                    }
                }
                
                if(srtCount == 1)
                {
                    multipleFiles = false;
                    txtFileName.Enabled = true;

                    int fileNameStartIndex = files[0].LastIndexOf('\\') + 1;
                    lblFileNamePath.Text = "File path: " + files[0].Substring(0, fileNameStartIndex);
                    txtFileName.Text = files[0].Substring(fileNameStartIndex);
                    oldFileName = txtFileName.Text;

                    files.Clear();

                    MessageBox.Show("Number of files to normalize: " + srtCount);
                }
                else if(srtCount > 0)
                {
                    multipleFiles = true;
                    txtFileName.Text = "";
                    txtFileName.Enabled = false;
                    MessageBox.Show("Number of files to normalize: " + srtCount);
                }
                else
                {
                    MessageBox.Show("There is no .srt files");
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
                }
                else
                {
                    MessageBox.Show("There is no .srt files");
                    srtCount = 0;
                }
                multipleFiles = false;
            }

            if(srtCount > 0) btnNormalize.Enabled = true;
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
