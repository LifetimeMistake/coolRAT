namespace coolRAT.Master
{
    partial class ScreenWindow
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
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.Start_Btn = new System.Windows.Forms.Button();
            this.Stop_Btn = new System.Windows.Forms.Button();
            this.MouseControl_Checkbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox
            // 
            this.ImageBox.BackColor = System.Drawing.Color.Black;
            this.ImageBox.Location = new System.Drawing.Point(12, 12);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(687, 364);
            this.ImageBox.TabIndex = 0;
            this.ImageBox.TabStop = false;
            // 
            // Start_Btn
            // 
            this.Start_Btn.Location = new System.Drawing.Point(12, 382);
            this.Start_Btn.Name = "Start_Btn";
            this.Start_Btn.Size = new System.Drawing.Size(75, 23);
            this.Start_Btn.TabIndex = 1;
            this.Start_Btn.Text = "Start";
            this.Start_Btn.UseVisualStyleBackColor = true;
            this.Start_Btn.Click += new System.EventHandler(this.Start_Btn_Click);
            // 
            // Stop_Btn
            // 
            this.Stop_Btn.Location = new System.Drawing.Point(93, 382);
            this.Stop_Btn.Name = "Stop_Btn";
            this.Stop_Btn.Size = new System.Drawing.Size(75, 23);
            this.Stop_Btn.TabIndex = 2;
            this.Stop_Btn.Text = "Stop";
            this.Stop_Btn.UseVisualStyleBackColor = true;
            this.Stop_Btn.Click += new System.EventHandler(this.Stop_Btn_Click);
            // 
            // MouseControl_Checkbox
            // 
            this.MouseControl_Checkbox.AutoSize = true;
            this.MouseControl_Checkbox.Enabled = false;
            this.MouseControl_Checkbox.Location = new System.Drawing.Point(606, 386);
            this.MouseControl_Checkbox.Name = "MouseControl_Checkbox";
            this.MouseControl_Checkbox.Size = new System.Drawing.Size(93, 17);
            this.MouseControl_Checkbox.TabIndex = 3;
            this.MouseControl_Checkbox.Text = "Mouse control";
            this.MouseControl_Checkbox.UseVisualStyleBackColor = true;
            // 
            // ScreenWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 413);
            this.Controls.Add(this.MouseControl_Checkbox);
            this.Controls.Add(this.Stop_Btn);
            this.Controls.Add(this.Start_Btn);
            this.Controls.Add(this.ImageBox);
            this.Name = "ScreenWindow";
            this.Text = "ScreenWindow";
            this.Shown += new System.EventHandler(this.ScreenWindow_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImageBox;
        private System.Windows.Forms.Button Start_Btn;
        private System.Windows.Forms.Button Stop_Btn;
        private System.Windows.Forms.CheckBox MouseControl_Checkbox;
    }
}