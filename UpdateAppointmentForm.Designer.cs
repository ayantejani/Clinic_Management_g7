namespace group7_Clinic_Management
{
    partial class UpdateAppointmentForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.datePickerAppointmentDate = new System.Windows.Forms.DateTimePicker();
            this.timePickerAppointmentTime = new System.Windows.Forms.DateTimePicker();
            this.comboBoxPatientName = new System.Windows.Forms.ComboBox();
            this.comboBoxDoctorName = new System.Windows.Forms.ComboBox();
            this.textBoxReason = new System.Windows.Forms.TextBox();
            this.labelPatient = new System.Windows.Forms.Label();
            this.labelDoctor = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelReason = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // datePickerAppointmentDate
            // 
            this.datePickerAppointmentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerAppointmentDate.Location = new System.Drawing.Point(140, 90);
            this.datePickerAppointmentDate.Name = "datePickerAppointmentDate";
            this.datePickerAppointmentDate.Size = new System.Drawing.Size(200, 22);
            this.datePickerAppointmentDate.TabIndex = 0;

            // 
            // timePickerAppointmentTime
            // 
            this.timePickerAppointmentTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerAppointmentTime.Location = new System.Drawing.Point(140, 130);
            this.timePickerAppointmentTime.Name = "timePickerAppointmentTime";
            this.timePickerAppointmentTime.ShowUpDown = true;
            this.timePickerAppointmentTime.Size = new System.Drawing.Size(200, 22);
            this.timePickerAppointmentTime.TabIndex = 1;

            // 
            // comboBoxPatientName
            // 
            this.comboBoxPatientName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPatientName.FormattingEnabled = true;
            this.comboBoxPatientName.Location = new System.Drawing.Point(140, 20);
            this.comboBoxPatientName.Name = "comboBoxPatientName";
            this.comboBoxPatientName.Size = new System.Drawing.Size(200, 24);
            this.comboBoxPatientName.TabIndex = 2;

            // 
            // comboBoxDoctorName
            // 
            this.comboBoxDoctorName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDoctorName.FormattingEnabled = true;
            this.comboBoxDoctorName.Location = new System.Drawing.Point(140, 50);
            this.comboBoxDoctorName.Name = "comboBoxDoctorName";
            this.comboBoxDoctorName.Size = new System.Drawing.Size(200, 24);
            this.comboBoxDoctorName.TabIndex = 3;

            // 
            // textBoxReason
            // 
            this.textBoxReason.Location = new System.Drawing.Point(140, 170);
            this.textBoxReason.Multiline = true;
            this.textBoxReason.Name = "textBoxReason";
            this.textBoxReason.Size = new System.Drawing.Size(200, 60);
            this.textBoxReason.TabIndex = 4;

            // 
            // labelPatient
            // 
            this.labelPatient.AutoSize = true;
            this.labelPatient.Location = new System.Drawing.Point(20, 20);
            this.labelPatient.Name = "labelPatient";
            this.labelPatient.Size = new System.Drawing.Size(52, 17);
            this.labelPatient.Text = "Patient:";

            // 
            // labelDoctor
            // 
            this.labelDoctor.AutoSize = true;
            this.labelDoctor.Location = new System.Drawing.Point(20, 50);
            this.labelDoctor.Name = "labelDoctor";
            this.labelDoctor.Size = new System.Drawing.Size(51, 17);
            this.labelDoctor.Text = "Doctor:";

            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Location = new System.Drawing.Point(20, 90);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(42, 17);
            this.labelDate.Text = "Date:";

            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(20, 130);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(43, 17);
            this.labelTime.Text = "Time:";

            // 
            // labelReason
            // 
            this.labelReason.AutoSize = true;
            this.labelReason.Location = new System.Drawing.Point(20, 170);
            this.labelReason.Name = "labelReason";
            this.labelReason.Size = new System.Drawing.Size(61, 17);
            this.labelReason.Text = "Reason:";

            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(140, 250);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 30);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);

            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(250, 250);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 30);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);

            // 
            // UpdateAppointmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.datePickerAppointmentDate);
            this.Controls.Add(this.timePickerAppointmentTime);
            this.Controls.Add(this.comboBoxPatientName);
            this.Controls.Add(this.comboBoxDoctorName);
            this.Controls.Add(this.textBoxReason);
            this.Controls.Add(this.labelPatient);
            this.Controls.Add(this.labelDoctor);
            this.Controls.Add(this.labelDate);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.labelReason);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.Name = "UpdateAppointmentForm";
            this.Text = "Update Appointment";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.DateTimePicker datePickerAppointmentDate;
        private System.Windows.Forms.DateTimePicker timePickerAppointmentTime;
        private System.Windows.Forms.ComboBox comboBoxPatientName;
        private System.Windows.Forms.ComboBox comboBoxDoctorName;
        private System.Windows.Forms.TextBox textBoxReason;
        private System.Windows.Forms.Label labelPatient;
        private System.Windows.Forms.Label labelDoctor;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelReason;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
    }
}
