namespace FindByColumnAndRow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBoxResult = new System.Windows.Forms.PictureBox();
            this.buttonGetBox = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxColumn = new System.Windows.Forms.TextBox();
            this.textBoxRow = new System.Windows.Forms.TextBox();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.pictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBoxResult);
            this.panel1.Controls.Add(this.buttonGetBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxColumn);
            this.panel1.Controls.Add(this.textBoxRow);
            this.panel1.Controls.Add(this.buttonOpenFile);
            this.panel1.Controls.Add(this.pictureBoxOriginal);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1191, 806);
            this.panel1.TabIndex = 0;
            // 
            // pictureBoxResult
            // 
            this.pictureBoxResult.Location = new System.Drawing.Point(3, 402);
            this.pictureBoxResult.Name = "pictureBoxResult";
            this.pictureBoxResult.Size = new System.Drawing.Size(1000, 398);
            this.pictureBoxResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxResult.TabIndex = 9;
            this.pictureBoxResult.TabStop = false;
            // 
            // buttonGetBox
            // 
            this.buttonGetBox.Location = new System.Drawing.Point(1109, 136);
            this.buttonGetBox.Name = "buttonGetBox";
            this.buttonGetBox.Size = new System.Drawing.Size(78, 54);
            this.buttonGetBox.TabIndex = 8;
            this.buttonGetBox.Text = "Get Bounding Box";
            this.buttonGetBox.UseVisualStyleBackColor = true;
            this.buttonGetBox.Click += new System.EventHandler(this.buttonGetBox_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1107, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Col";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1107, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Row";
            // 
            // textBoxColumn
            // 
            this.textBoxColumn.Location = new System.Drawing.Point(1110, 70);
            this.textBoxColumn.Name = "textBoxColumn";
            this.textBoxColumn.Size = new System.Drawing.Size(54, 20);
            this.textBoxColumn.TabIndex = 5;
            // 
            // textBoxRow
            // 
            this.textBoxRow.Location = new System.Drawing.Point(1110, 109);
            this.textBoxRow.Name = "textBoxRow";
            this.textBoxRow.Size = new System.Drawing.Size(54, 20);
            this.textBoxRow.TabIndex = 4;
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(1110, 3);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(77, 48);
            this.buttonOpenFile.TabIndex = 3;
            this.buttonOpenFile.Text = "Open File";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // pictureBoxOriginal
            // 
            this.pictureBoxOriginal.Location = new System.Drawing.Point(3, 1);
            this.pictureBoxOriginal.Name = "pictureBoxOriginal";
            this.pictureBoxOriginal.Size = new System.Drawing.Size(1000, 398);
            this.pictureBoxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxOriginal.TabIndex = 2;
            this.pictureBoxOriginal.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1212, 823);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonGetBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxColumn;
        private System.Windows.Forms.TextBox textBoxRow;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.PictureBox pictureBoxOriginal;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox pictureBoxResult;
    }
}

