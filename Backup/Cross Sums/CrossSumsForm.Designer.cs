﻿namespace Cross_Sums
{
    partial class CrossSumsForm
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
          this.infoBox = new System.Windows.Forms.TextBox();
          this.SuspendLayout();
          // 
          // infoBox
          // 
          this.infoBox.AcceptsReturn = true;
          this.infoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.infoBox.BackColor = System.Drawing.SystemColors.Info;
          this.infoBox.Location = new System.Drawing.Point(12, 12);
          this.infoBox.Multiline = true;
          this.infoBox.Name = "infoBox";
          this.infoBox.ReadOnly = true;
          this.infoBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
          this.infoBox.Size = new System.Drawing.Size(652, 135);
          this.infoBox.TabIndex = 0;
          this.infoBox.TabStop = false;
          this.infoBox.Visible = false;
          this.infoBox.WordWrap = false;
          // 
          // CrossSumsForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(676, 430);
          this.Controls.Add(this.infoBox);
          this.Name = "CrossSumsForm";
          this.RightToLeft = System.Windows.Forms.RightToLeft.No;
          this.Text = "CrossSumsForm";
          this.Load += new System.EventHandler(this.CrossSumsForm_Load);
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox infoBox;

    }
}

