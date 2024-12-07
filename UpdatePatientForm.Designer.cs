namespace g7_Clinic_Management
{
    partial class UpdatePatientForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblDOB;
        private System.Windows.Forms.DateTimePicker dtpDOB;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExit; // Exit button declaration

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
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblDOB = new System.Windows.Forms.Label();
            this.dtpDOB = new System.Windows.Forms.DateTimePicker();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();  // Exit button declaration

            this.SuspendLayout();

            // lblName
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Name = "lblName";
            this.lblName.Text = "Name:";

            // txtName
            this.txtName.Location = new System.Drawing.Point(150, 20);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 20);

            // lblDOB
            this.lblDOB.AutoSize = true;
            this.lblDOB.Location = new System.Drawing.Point(20, 60);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Text = "Date of Birth:";

            // dtpDOB
            this.dtpDOB.Location = new System.Drawing.Point(150, 60);
            this.dtpDOB.Name = "dtpDOB";
            this.dtpDOB.Size = new System.Drawing.Size(200, 20);

            // lblPhone
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(20, 100);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Text = "Phone Number:";

            // txtPhone
            this.txtPhone.Location = new System.Drawing.Point(150, 100);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(200, 20);

            // lblAddress
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(20, 140);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Text = "Address:";

            // txtAddress
            this.txtAddress.Location = new System.Drawing.Point(150, 140);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(200, 60);

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(150, 220);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // btnExit (Exit Button)
            this.btnExit.Location = new System.Drawing.Point(260, 220);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 30);
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);

            // UpdatePatientForm
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblDOB);
            this.Controls.Add(this.dtpDOB);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExit);  // Adding Exit button to the form
            this.Name = "UpdatePatientForm";
            this.Text = "Update Patient";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
