namespace coolRAT.Master
{
    partial class ShellWindow
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
            this.ShellEmulator = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // ShellEmulator
            // 
            this.ShellEmulator.BackColor = System.Drawing.Color.Black;
            this.ShellEmulator.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ShellEmulator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShellEmulator.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ShellEmulator.ForeColor = System.Drawing.Color.White;
            this.ShellEmulator.Location = new System.Drawing.Point(0, 0);
            this.ShellEmulator.Name = "ShellEmulator";
            this.ShellEmulator.ReadOnly = true;
            this.ShellEmulator.Size = new System.Drawing.Size(718, 418);
            this.ShellEmulator.TabIndex = 0;
            this.ShellEmulator.Text = "";
            this.ShellEmulator.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ShellEmulator_KeyDown);
            this.ShellEmulator.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ShellEmulator_KeyPress);
            // 
            // ShellWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 418);
            this.Controls.Add(this.ShellEmulator);
            this.Name = "ShellWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShellWindow_FormClosing);
            this.Shown += new System.EventHandler(this.ShellWindow_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ShellEmulator;
    }
}