namespace group7_Clinic_Management
{
    partial class AddAppointmentForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox comboBoxPatientName;
        private System.Windows.Forms.ComboBox comboBoxDoctorName;
        private System.Windows.Forms.DateTimePicker datePickerAppointmentDate;
        private System.Windows.Forms.DateTimePicker timePickerAppointmentTime;
        private System.Windows.Forms.TextBox textBoxReason;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelPatientName;
        private System.Windows.Forms.Label labelDoctorName;
        private System.Windows.Forms.Label labelAppointmentDate;
        private System.Windows.Forms.Label labelAppointmentTime;
        private System.Windows.Forms.Label labelReason;

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
            this.comboBoxPatientName = new System.Windows.Forms.ComboBox();
            this.comboBoxDoctorName = new System.Windows.Forms.ComboBox();
            this.datePickerAppointmentDate = new System.Windows.Forms.DateTimePicker();
            this.timePickerAppointmentTime = new System.Windows.Forms.DateTimePicker();
            this.textBoxReason = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelPatientName = new System.Windows.Forms.Label();
            this.labelDoctorName = new System.Windows.Forms.Label();
            this.labelAppointmentDate = new System.Windows.Forms.Label();
            this.labelAppointmentTime = new System.Windows.Forms.Label();
            this.labelReason = new System.Windows.Forms.Label();

            // Label: Patient Name
            this.labelPatientName.Text = "Patient Name:";
            this.labelPatientName.Location = new System.Drawing.Point(30, 20);
            this.labelPatientName.Size = new System.Drawing.Size(100, 20);

            // ComboBox: Patient Name
            this.comboBoxPatientName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPatientName.Location = new System.Drawing.Point(140, 20);
            this.comboBoxPatientName.Size = new System.Drawing.Size(200, 25);

            // Label: Doctor Name
            this.labelDoctorName.Text = "Doctor Name:";
            this.labelDoctorName.Location = new System.Drawing.Point(30, 60);
            this.labelDoctorName.Size = new System.Drawing.Size(100, 20);

            // ComboBox: Doctor Name
            this.comboBoxDoctorName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDoctorName.Location = new System.Drawing.Point(140, 60);
            this.comboBoxDoctorName.Size = new System.Drawing.Size(200, 25);

            // Label: Appointment Date
            this.labelAppointmentDate.Text = "Appointment Date:";
            this.labelAppointmentDate.Location = new System.Drawing.Point(30, 100);
            this.labelAppointmentDate.Size = new System.Drawing.Size(120, 20);

            // DateTimePicker: Appointment Date
            this.datePickerAppointmentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerAppointmentDate.Location = new System.Drawing.Point(160, 100);
            this.datePickerAppointmentDate.Size = new System.Drawing.Size(180, 25);

            // Label: Appointment Time
            this.labelAppointmentTime.Text = "Appointment Time:";
            this.labelAppointmentTime.Location = new System.Drawing.Point(30, 140);
            this.labelAppointmentTime.Size = new System.Drawing.Size(120, 20);

            // DateTimePicker: Appointment Time
            this.timePickerAppointmentTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerAppointmentTime.Location = new System.Drawing.Point(160, 140);
            this.timePickerAppointmentTime.Size = new System.Drawing.Size(180, 25);

            // Label: Reason
            this.labelReason.Text = "Reason:";
            this.labelReason.Location = new System.Drawing.Point(30, 180);
            this.labelReason.Size = new System.Drawing.Size(100, 20);

            // TextBox: Reason
            this.textBoxReason.Location = new System.Drawing.Point(140, 180);
            this.textBoxReason.Size = new System.Drawing.Size(200, 25);

            // Button: Submit
            this.btnSubmit.Location = new System.Drawing.Point(140, 230);
            this.btnSubmit.Size = new System.Drawing.Size(95, 30);
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.ButtonSubmit_Click);

            // Button: Cancel
            this.btnCancel.Location = new System.Drawing.Point(245, 230);
            this.btnCancel.Size = new System.Drawing.Size(95, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.ButtonCancel_Click);

            // AddAppointmentForm
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.labelPatientName);
            this.Controls.Add(this.comboBoxPatientName);
            this.Controls.Add(this.labelDoctorName);
            this.Controls.Add(this.comboBoxDoctorName);
            this.Controls.Add(this.labelAppointmentDate);
            this.Controls.Add(this.datePickerAppointmentDate);
            this.Controls.Add(this.labelAppointmentTime);
            this.Controls.Add(this.timePickerAppointmentTime);
            this.Controls.Add(this.labelReason);
            this.Controls.Add(this.textBoxReason);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnCancel);
            this.Text = "Add Appointment";
        }
    }
}
