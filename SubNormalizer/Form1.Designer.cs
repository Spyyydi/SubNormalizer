namespace SubNormalizer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblFileNamePath = new System.Windows.Forms.Label();
            this.btnChooseFileNamePath = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.lblNormalized = new System.Windows.Forms.Label();
            this.btnNormalize = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.chkSync = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkDash = new System.Windows.Forms.CheckBox();
            this.btnCheckSync = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFileNamePath
            // 
            this.lblFileNamePath.AutoSize = true;
            this.lblFileNamePath.Location = new System.Drawing.Point(18, 25);
            this.lblFileNamePath.Name = "lblFileNamePath";
            this.lblFileNamePath.Size = new System.Drawing.Size(53, 13);
            this.lblFileNamePath.TabIndex = 1;
            this.lblFileNamePath.Text = "File path: ";
            // 
            // btnChooseFileNamePath
            // 
            this.btnChooseFileNamePath.Location = new System.Drawing.Point(12, 52);
            this.btnChooseFileNamePath.Name = "btnChooseFileNamePath";
            this.btnChooseFileNamePath.Size = new System.Drawing.Size(75, 23);
            this.btnChooseFileNamePath.TabIndex = 2;
            this.btnChooseFileNamePath.Text = "Select File";
            this.toolTip1.SetToolTip(this.btnChooseFileNamePath, resources.GetString("btnChooseFileNamePath.ToolTip"));
            this.btnChooseFileNamePath.UseVisualStyleBackColor = true;
            this.btnChooseFileNamePath.Click += new System.EventHandler(this.btnChooseFileNamePath_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(93, 108);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(392, 20);
            this.txtFileName.TabIndex = 3;
            this.txtFileName.TextChanged += new System.EventHandler(this.txtFileName_TextChanged);
            // 
            // lblNormalized
            // 
            this.lblNormalized.AutoSize = true;
            this.lblNormalized.BackColor = System.Drawing.SystemColors.Control;
            this.lblNormalized.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblNormalized.ForeColor = System.Drawing.Color.Red;
            this.lblNormalized.Location = new System.Drawing.Point(179, 270);
            this.lblNormalized.Name = "lblNormalized";
            this.lblNormalized.Size = new System.Drawing.Size(151, 24);
            this.lblNormalized.TabIndex = 4;
            this.lblNormalized.Text = "Subs normalized";
            // 
            // btnNormalize
            // 
            this.btnNormalize.Enabled = false;
            this.btnNormalize.Location = new System.Drawing.Point(144, 166);
            this.btnNormalize.Name = "btnNormalize";
            this.btnNormalize.Size = new System.Drawing.Size(222, 52);
            this.btnNormalize.TabIndex = 5;
            this.btnNormalize.Text = "Normalize";
            this.btnNormalize.UseVisualStyleBackColor = true;
            this.btnNormalize.Click += new System.EventHandler(this.btnNormalize_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "New File Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // chkSync
            // 
            this.chkSync.AutoSize = true;
            this.chkSync.Enabled = false;
            this.chkSync.Location = new System.Drawing.Point(12, 81);
            this.chkSync.Name = "chkSync";
            this.chkSync.Size = new System.Drawing.Size(192, 17);
            this.chkSync.TabIndex = 7;
            this.chkSync.Text = "Synchronize with subs from veed.io";
            this.chkSync.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "Synchronization";
            // 
            // chkDash
            // 
            this.chkDash.AutoSize = true;
            this.chkDash.Enabled = false;
            this.chkDash.Location = new System.Drawing.Point(210, 81);
            this.chkDash.Name = "chkDash";
            this.chkDash.Size = new System.Drawing.Size(79, 17);
            this.chkDash.TabIndex = 8;
            this.chkDash.Text = "Put dashes";
            this.chkDash.UseVisualStyleBackColor = true;
            // 
            // btnCheckSync
            // 
            this.btnCheckSync.Location = new System.Drawing.Point(144, 166);
            this.btnCheckSync.Name = "btnCheckSync";
            this.btnCheckSync.Size = new System.Drawing.Size(222, 52);
            this.btnCheckSync.TabIndex = 9;
            this.btnCheckSync.Text = "Check synchronized files";
            this.btnCheckSync.UseVisualStyleBackColor = true;
            this.btnCheckSync.Visible = false;
            this.btnCheckSync.Click += new System.EventHandler(this.btnCheckSync_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 359);
            this.Controls.Add(this.btnCheckSync);
            this.Controls.Add(this.chkDash);
            this.Controls.Add(this.chkSync);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnNormalize);
            this.Controls.Add(this.lblNormalized);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.btnChooseFileNamePath);
            this.Controls.Add(this.lblFileNamePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SRT Sub Normalizer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFileNamePath;
        private System.Windows.Forms.Button btnChooseFileNamePath;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label lblNormalized;
        private System.Windows.Forms.Button btnNormalize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox chkSync;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkDash;
        private System.Windows.Forms.Button btnCheckSync;
    }
}

