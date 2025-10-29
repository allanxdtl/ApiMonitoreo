namespace MonitoreoBridge
{
    partial class FrmPrincipal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.TxtNoSerie = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtNombre = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtFechaProd = new System.Windows.Forms.DateTimePicker();
            this.BtnClear = new System.Windows.Forms.Button();
            this.BtnResistencia = new System.Windows.Forms.Button();
            this.BtnContinuidad = new System.Windows.Forms.Button();
            this.BtnDecibeles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Numero de Serie";
            // 
            // TxtNoSerie
            // 
            this.TxtNoSerie.Location = new System.Drawing.Point(139, 7);
            this.TxtNoSerie.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TxtNoSerie.Name = "TxtNoSerie";
            this.TxtNoSerie.Size = new System.Drawing.Size(353, 22);
            this.TxtNoSerie.TabIndex = 1;
            this.TxtNoSerie.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtNoSerie_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Id";
            // 
            // TxtId
            // 
            this.TxtId.Location = new System.Drawing.Point(69, 58);
            this.TxtId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TxtId.Name = "TxtId";
            this.TxtId.ReadOnly = true;
            this.TxtId.Size = new System.Drawing.Size(36, 22);
            this.TxtId.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Nombre";
            // 
            // TxtNombre
            // 
            this.TxtNombre.Location = new System.Drawing.Point(181, 58);
            this.TxtNombre.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TxtNombre.Name = "TxtNombre";
            this.TxtNombre.ReadOnly = true;
            this.TxtNombre.Size = new System.Drawing.Size(199, 22);
            this.TxtNombre.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(389, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Fecha";
            // 
            // dtFechaProd
            // 
            this.dtFechaProd.Enabled = false;
            this.dtFechaProd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFechaProd.Location = new System.Drawing.Point(447, 62);
            this.dtFechaProd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtFechaProd.Name = "dtFechaProd";
            this.dtFechaProd.Size = new System.Drawing.Size(112, 22);
            this.dtFechaProd.TabIndex = 7;
            // 
            // BtnClear
            // 
            this.BtnClear.Location = new System.Drawing.Point(568, 62);
            this.BtnClear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnClear.Name = "BtnClear";
            this.BtnClear.Size = new System.Drawing.Size(116, 38);
            this.BtnClear.TabIndex = 8;
            this.BtnClear.Text = "Limpiar";
            this.BtnClear.UseVisualStyleBackColor = true;
            this.BtnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // BtnResistencia
            // 
            this.BtnResistencia.Location = new System.Drawing.Point(43, 117);
            this.BtnResistencia.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnResistencia.Name = "BtnResistencia";
            this.BtnResistencia.Size = new System.Drawing.Size(192, 47);
            this.BtnResistencia.TabIndex = 9;
            this.BtnResistencia.Text = "Realizar prueba de resistencia";
            this.BtnResistencia.UseVisualStyleBackColor = true;
            this.BtnResistencia.Click += new System.EventHandler(this.BtnResistencia_Click);
            // 
            // BtnContinuidad
            // 
            this.BtnContinuidad.Location = new System.Drawing.Point(243, 117);
            this.BtnContinuidad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnContinuidad.Name = "BtnContinuidad";
            this.BtnContinuidad.Size = new System.Drawing.Size(192, 47);
            this.BtnContinuidad.TabIndex = 10;
            this.BtnContinuidad.Text = "Realizar prueba de continudad";
            this.BtnContinuidad.UseVisualStyleBackColor = true;
            this.BtnContinuidad.Click += new System.EventHandler(this.BtnContinuidad_Click);
            // 
            // BtnDecibeles
            // 
            this.BtnDecibeles.Location = new System.Drawing.Point(443, 117);
            this.BtnDecibeles.Margin = new System.Windows.Forms.Padding(4);
            this.BtnDecibeles.Name = "BtnDecibeles";
            this.BtnDecibeles.Size = new System.Drawing.Size(192, 47);
            this.BtnDecibeles.TabIndex = 11;
            this.BtnDecibeles.Text = "Realizar prueba de decibeles";
            this.BtnDecibeles.UseVisualStyleBackColor = true;
            this.BtnDecibeles.Click += new System.EventHandler(this.BtnDecibeles_Click);
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 190);
            this.Controls.Add(this.BtnDecibeles);
            this.Controls.Add(this.BtnContinuidad);
            this.Controls.Add(this.BtnResistencia);
            this.Controls.Add(this.BtnClear);
            this.Controls.Add(this.dtFechaProd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TxtNombre);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtNoSerie);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Realizar pruebas";
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtNoSerie;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtNombre;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtFechaProd;
        private System.Windows.Forms.Button BtnClear;
        private System.Windows.Forms.Button BtnResistencia;
        private System.Windows.Forms.Button BtnContinuidad;
        private System.Windows.Forms.Button BtnDecibeles;
    }
}

