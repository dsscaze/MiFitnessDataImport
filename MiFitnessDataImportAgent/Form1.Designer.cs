namespace MiFitnessDataImportAgent
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
            this.btImportSportData = new System.Windows.Forms.Button();
            this.btnGerarJsonStrava = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btImportSportData
            // 
            this.btImportSportData.Location = new System.Drawing.Point(120, 126);
            this.btImportSportData.Name = "btImportSportData";
            this.btImportSportData.Size = new System.Drawing.Size(210, 50);
            this.btImportSportData.TabIndex = 0;
            this.btImportSportData.Text = "Processar Arquivos";
            this.btImportSportData.UseVisualStyleBackColor = true;
            this.btImportSportData.Click += new System.EventHandler(this.btImportSportData_Click);
            // 
            // btnGerarJsonStrava
            // 
            this.btnGerarJsonStrava.Location = new System.Drawing.Point(120, 220);
            this.btnGerarJsonStrava.Name = "btnGerarJsonStrava";
            this.btnGerarJsonStrava.Size = new System.Drawing.Size(210, 50);
            this.btnGerarJsonStrava.TabIndex = 1;
            this.btnGerarJsonStrava.Text = "Gerar JSON Strava";
            this.btnGerarJsonStrava.UseVisualStyleBackColor = true;
            this.btnGerarJsonStrava.Click += new System.EventHandler(this.btnGerarJsonStrava_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnGerarJsonStrava);
            this.Controls.Add(this.btImportSportData);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btImportSportData;
        private System.Windows.Forms.Button btnGerarJsonStrava;
    }
}

