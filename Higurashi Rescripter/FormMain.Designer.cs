namespace Higurashi_Rescripter
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonGenerateSheet1 = new CustomUI.Controls.CustomButton();
            this.labelScriptToSheetDescription = new CustomUI.Controls.CustomLabel();
            this.groupBoxScriptToSheet = new CustomUI.Controls.CustomGroupBox();
            this.groupBoxSheetToScript = new CustomUI.Controls.CustomGroupBox();
            this.labelSheetToScriptDescription = new CustomUI.Controls.CustomLabel();
            this.buttonGenerateScript = new CustomUI.Controls.CustomButton();
            this.labelCredits = new CustomUI.Controls.CustomLabel();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.groupBoxScriptToSheet.SuspendLayout();
            this.groupBoxSheetToScript.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonGenerateSheet1
            // 
            this.buttonGenerateSheet1.Location = new System.Drawing.Point(9, 79);
            this.buttonGenerateSheet1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonGenerateSheet1.Name = "buttonGenerateSheet1";
            this.buttonGenerateSheet1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.buttonGenerateSheet1.Size = new System.Drawing.Size(517, 36);
            this.buttonGenerateSheet1.TabIndex = 2;
            this.buttonGenerateSheet1.Text = "Genera Fogli";
            this.buttonGenerateSheet1.Click += new System.EventHandler(this.buttonGenerateSheet_Click);
            // 
            // labelScriptToSheetDescription
            // 
            this.labelScriptToSheetDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.labelScriptToSheetDescription.Location = new System.Drawing.Point(8, 20);
            this.labelScriptToSheetDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelScriptToSheetDescription.Name = "labelScriptToSheetDescription";
            this.labelScriptToSheetDescription.Size = new System.Drawing.Size(517, 55);
            this.labelScriptToSheetDescription.TabIndex = 1;
            this.labelScriptToSheetDescription.Text = "Converte gli script originali di Higurashi in fogli per Google Sheet.\r\n\r\nNota: De" +
    "vi prima inserire gli script all\'interno della cartella Input/Script";
            // 
            // groupBoxScriptToSheet
            // 
            this.groupBoxScriptToSheet.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.groupBoxScriptToSheet.Controls.Add(this.labelScriptToSheetDescription);
            this.groupBoxScriptToSheet.Controls.Add(this.buttonGenerateSheet1);
            this.groupBoxScriptToSheet.Location = new System.Drawing.Point(28, 170);
            this.groupBoxScriptToSheet.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxScriptToSheet.Name = "groupBoxScriptToSheet";
            this.groupBoxScriptToSheet.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxScriptToSheet.Size = new System.Drawing.Size(533, 122);
            this.groupBoxScriptToSheet.TabIndex = 0;
            this.groupBoxScriptToSheet.TabStop = false;
            this.groupBoxScriptToSheet.Text = "Script -> Sheet";
            // 
            // groupBoxSheetToScript
            // 
            this.groupBoxSheetToScript.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.groupBoxSheetToScript.Controls.Add(this.labelSheetToScriptDescription);
            this.groupBoxSheetToScript.Controls.Add(this.buttonGenerateScript);
            this.groupBoxSheetToScript.Location = new System.Drawing.Point(28, 299);
            this.groupBoxSheetToScript.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxSheetToScript.Name = "groupBoxSheetToScript";
            this.groupBoxSheetToScript.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxSheetToScript.Size = new System.Drawing.Size(533, 151);
            this.groupBoxSheetToScript.TabIndex = 4;
            this.groupBoxSheetToScript.TabStop = false;
            this.groupBoxSheetToScript.Text = "Sheet -> Script";
            // 
            // labelSheetToScriptDescription
            // 
            this.labelSheetToScriptDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.labelSheetToScriptDescription.Location = new System.Drawing.Point(8, 20);
            this.labelSheetToScriptDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSheetToScriptDescription.Name = "labelSheetToScriptDescription";
            this.labelSheetToScriptDescription.Size = new System.Drawing.Size(517, 85);
            this.labelSheetToScriptDescription.TabIndex = 5;
            this.labelSheetToScriptDescription.Text = resources.GetString("labelSheetToScriptDescription.Text");
            // 
            // buttonGenerateScript
            // 
            this.buttonGenerateScript.Location = new System.Drawing.Point(8, 108);
            this.buttonGenerateScript.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonGenerateScript.Name = "buttonGenerateScript";
            this.buttonGenerateScript.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.buttonGenerateScript.Size = new System.Drawing.Size(517, 36);
            this.buttonGenerateScript.TabIndex = 6;
            this.buttonGenerateScript.Text = "Genera Script";
            this.buttonGenerateScript.Click += new System.EventHandler(this.buttonGenerateScript_Click);
            // 
            // labelCredits
            // 
            this.labelCredits.AutoSize = true;
            this.labelCredits.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.labelCredits.Location = new System.Drawing.Point(360, 461);
            this.labelCredits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCredits.Name = "labelCredits";
            this.labelCredits.Size = new System.Drawing.Size(201, 17);
            this.labelCredits.TabIndex = 7;
            this.labelCredits.Text = "by Xorboth (CTH Translations)";
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLogo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(588, 162);
            this.pictureBoxLogo.TabIndex = 4;
            this.pictureBoxLogo.TabStop = false;
            this.pictureBoxLogo.Click += new System.EventHandler(this.pictureBoxLogo_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 484);
            this.Controls.Add(this.labelCredits);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.groupBoxSheetToScript);
            this.Controls.Add(this.groupBoxScriptToSheet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(605, 531);
            this.MinimumSize = new System.Drawing.Size(605, 531);
            this.Name = "FormMain";
            this.Text = "Higurashi Rescripter v0.2";
            this.groupBoxScriptToSheet.ResumeLayout(false);
            this.groupBoxSheetToScript.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomUI.Controls.CustomButton buttonGenerateSheet1;
        private CustomUI.Controls.CustomLabel labelScriptToSheetDescription;
        private CustomUI.Controls.CustomGroupBox groupBoxScriptToSheet;
        private CustomUI.Controls.CustomGroupBox groupBoxSheetToScript;
        private CustomUI.Controls.CustomLabel labelSheetToScriptDescription;
        private CustomUI.Controls.CustomButton buttonGenerateScript;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private CustomUI.Controls.CustomLabel labelCredits;
    }
}

