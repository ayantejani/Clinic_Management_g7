namespace group7_Clinic_Management
{
    partial class AddAppointmentForm
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
            this.comboBoxPatientName = new System.Windows.Forms.ComboBox();
            this.comboBoxDoctorName = new System.Windows.Forms.ComboBox();
            this.datePickerAppointmentDate = new System.Windows.Forms.DateTimePicker();
            this.timePickerAppointmentTime = new System.Windows.Forms.DateTimePicker();
            this.textBoxReason = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // comboBoxPatientName
            // 
            this.comboBoxPatientName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPatientName.FormattingEnabled = true;
            this.comboBoxPatientName.Location = new System.Drawing.Point(130, 30);
            this.comboBoxPatientName.Name = "comboBoxPatientName";
            this.comboBoxPatientName.Size = new System.Drawing.Size(200, 24);
            this.comboBoxPatientName.TabIndex = 0;

            // 
            // comboBoxDoctorName
            // 
            this.comboBoxDoctorName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDoctorName.FormattingEnabled = true;
            this.comboBoxDoctorName.Location = new System.Drawing.Point(130, 70);
            this.comboBoxDoctorName.Name = "comboBoxDoctorName";
            this.comboBoxDoctorName.Size = new System.Drawing.Size(200, 24);
            this.comboBoxDoctorName.TabIndex = 1;

            // 
            // datePickerAppointmentDate
            // 
            this.datePickerAppointmentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerAppointmentDate.Location = new System.Drawing.Point(130, 110);
            this.datePickerAppointmentDate.Name = "datePickerAppointmentDate";
            this.datePickerAppointmentDate.Size = new System.Drawing.Size(200, 22);
            this.datePickerAppointmentDate.TabIndex = 2;

            // 
            // timePickerAppointmentTime
            // 
            this.timePickerAppointmentTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerAppointmentTime.Location = new System.Drawing.Point(130, 150);
            this.timePickerAppointmentTime.Name = "timePickerAppointmentTime";
            this.timePickerAppointmentTime.ShowUpDown = true;
            this.timePickerAppointmentTime.Size = new System.Drawing.Size(200, 22);
            this.timePickerAppointmentTime.TabIndex = 3;

            // 
            // textBoxReason
            // 
            this.textBoxReason.Location = new System.Drawing.Point(130, 190);
            this.textBoxReason.Multiline = true;
            this.textBoxReason.Name = "textBoxReason";
            this.textBoxReason.Size = new System.Drawing.Size(200, 80);
            this.textBoxReason.TabIndex = 4;

            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(130, 290);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 30);
            this.btnSubmit.TabIndex = 5;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);

            // 
            // AddAppointmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.textBoxReason);
            this.Controls.Add(this.timePickerAppointmentTime);
            this.Controls.Add(this.datePickerAppointmentDate);
            this.Controls.Add(this.comboBoxDoctorName);
            this.Controls.Add(this.comboBoxPatientName);
            this.Name = "AddAppointmentForm";
            this.Text = "Add Appointment";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxPatientName;
        private System.Windows.Forms.ComboBox comboBoxDoctorName;
        private System.Windows.Forms.DateTimePicker datePickerAppointmentDate;
        private System.Windows.Forms.DateTimePicker timePickerAppointmentTime;
        private System.Windows.Forms.TextBox textBoxReason;
        private System.Windows.Forms.Button btnSubmit;
    }
}
