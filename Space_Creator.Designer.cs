namespace Space_Manager
{
    partial class Space_Creator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Space_Creator));
            this.RVTlinks_list = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.get_room_data = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.geometry_from_floors = new System.Windows.Forms.RadioButton();
            this.geometry_from_rooms = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.total_rooms = new System.Windows.Forms.Label();
            this.redundant_rooms_nb = new System.Windows.Forms.Label();
            this.unclosed_rooms_nb = new System.Windows.Forms.Label();
            this.placed_rooms_nb = new System.Windows.Forms.Label();
            this.unplaced_rooms_nb = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.phase = new System.Windows.Forms.Label();
            this.sous_projet = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // RVTlinks_list
            // 
            this.RVTlinks_list.FormattingEnabled = true;
            this.RVTlinks_list.HorizontalScrollbar = true;
            this.RVTlinks_list.Location = new System.Drawing.Point(15, 26);
            this.RVTlinks_list.Name = "RVTlinks_list";
            this.RVTlinks_list.ScrollAlwaysVisible = true;
            this.RVTlinks_list.Size = new System.Drawing.Size(300, 82);
            this.RVTlinks_list.TabIndex = 0;
            this.RVTlinks_list.SelectedIndexChanged += new System.EventHandler(this.RVTlinks_list_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(267, 535);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Annuler";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(183, 535);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // get_room_data
            // 
            this.get_room_data.AutoSize = true;
            this.get_room_data.Location = new System.Drawing.Point(15, 28);
            this.get_room_data.Name = "get_room_data";
            this.get_room_data.Size = new System.Drawing.Size(225, 17);
            this.get_room_data.TabIndex = 4;
            this.get_room_data.Text = "Récupérer le nom et le numéro des pièces";
            this.get_room_data.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.geometry_from_floors);
            this.groupBox1.Controls.Add(this.geometry_from_rooms);
            this.groupBox1.Location = new System.Drawing.Point(12, 354);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 96);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Géométrie des espaces";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(251, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "( commande disponible depuis une vue 3D globale )";
            // 
            // geometry_from_floors
            // 
            this.geometry_from_floors.AutoSize = true;
            this.geometry_from_floors.Location = new System.Drawing.Point(15, 52);
            this.geometry_from_floors.Name = "geometry_from_floors";
            this.geometry_from_floors.Size = new System.Drawing.Size(242, 17);
            this.geometry_from_floors.TabIndex = 7;
            this.geometry_from_floors.TabStop = true;
            this.geometry_from_floors.Text = "Détecter les dalles pour fixer le décalage limite";
            this.geometry_from_floors.UseVisualStyleBackColor = true;
            // 
            // geometry_from_rooms
            // 
            this.geometry_from_rooms.AutoSize = true;
            this.geometry_from_rooms.Checked = true;
            this.geometry_from_rooms.Location = new System.Drawing.Point(15, 26);
            this.geometry_from_rooms.Name = "geometry_from_rooms";
            this.geometry_from_rooms.Size = new System.Drawing.Size(200, 17);
            this.geometry_from_rooms.TabIndex = 6;
            this.geometry_from_rooms.TabStop = true;
            this.geometry_from_rooms.Text = "Récupérer les contraintes des pièces";
            this.geometry_from_rooms.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.total_rooms);
            this.groupBox2.Controls.Add(this.redundant_rooms_nb);
            this.groupBox2.Controls.Add(this.unclosed_rooms_nb);
            this.groupBox2.Controls.Add(this.placed_rooms_nb);
            this.groupBox2.Controls.Add(this.unplaced_rooms_nb);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.RVTlinks_list);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(330, 239);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sélection du lien Revit contenant les pièces";
            // 
            // total_rooms
            // 
            this.total_rooms.AutoSize = true;
            this.total_rooms.Location = new System.Drawing.Point(164, 123);
            this.total_rooms.Name = "total_rooms";
            this.total_rooms.Size = new System.Drawing.Size(13, 13);
            this.total_rooms.TabIndex = 14;
            this.total_rooms.Text = "0";
            // 
            // redundant_rooms_nb
            // 
            this.redundant_rooms_nb.AutoSize = true;
            this.redundant_rooms_nb.Location = new System.Drawing.Point(164, 212);
            this.redundant_rooms_nb.Name = "redundant_rooms_nb";
            this.redundant_rooms_nb.Size = new System.Drawing.Size(13, 13);
            this.redundant_rooms_nb.TabIndex = 13;
            this.redundant_rooms_nb.Text = "0";
            // 
            // unclosed_rooms_nb
            // 
            this.unclosed_rooms_nb.AutoSize = true;
            this.unclosed_rooms_nb.Location = new System.Drawing.Point(164, 194);
            this.unclosed_rooms_nb.Name = "unclosed_rooms_nb";
            this.unclosed_rooms_nb.Size = new System.Drawing.Size(13, 13);
            this.unclosed_rooms_nb.TabIndex = 12;
            this.unclosed_rooms_nb.Text = "0";
            // 
            // placed_rooms_nb
            // 
            this.placed_rooms_nb.AutoSize = true;
            this.placed_rooms_nb.Location = new System.Drawing.Point(164, 175);
            this.placed_rooms_nb.Name = "placed_rooms_nb";
            this.placed_rooms_nb.Size = new System.Drawing.Size(13, 13);
            this.placed_rooms_nb.TabIndex = 11;
            this.placed_rooms_nb.Text = "0";
            // 
            // unplaced_rooms_nb
            // 
            this.unplaced_rooms_nb.AutoSize = true;
            this.unplaced_rooms_nb.Location = new System.Drawing.Point(164, 150);
            this.unplaced_rooms_nb.Name = "unplaced_rooms_nb";
            this.unplaced_rooms_nb.Size = new System.Drawing.Size(13, 13);
            this.unplaced_rooms_nb.TabIndex = 10;
            this.unplaced_rooms_nb.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 212);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "dont pièces superflues :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "dont pièces non fermées :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(73, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Pièces placées :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Pièces non placées :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Nombre total de pièces :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.get_room_data);
            this.groupBox3.Location = new System.Drawing.Point(12, 456);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(330, 64);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Informations des espaces";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.phase);
            this.groupBox4.Controls.Add(this.sous_projet);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(12, 261);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(330, 81);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Paramètres de base";
            // 
            // phase
            // 
            this.phase.AutoSize = true;
            this.phase.Location = new System.Drawing.Point(134, 52);
            this.phase.Name = "phase";
            this.phase.Size = new System.Drawing.Size(37, 13);
            this.phase.TabIndex = 18;
            this.phase.Text = "Phase";
            // 
            // sous_projet
            // 
            this.sous_projet.AutoSize = true;
            this.sous_projet.Location = new System.Drawing.Point(101, 28);
            this.sous_projet.Name = "sous_projet";
            this.sous_projet.Size = new System.Drawing.Size(94, 13);
            this.sous_projet.TabIndex = 17;
            this.sous_projet.Text = "Projet non partagé";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Phase de la vue active :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Sous-projet actif :";
            // 
            // Space_Creator
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 569);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Space_Creator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Créer les espaces";
            this.Load += new System.EventHandler(this.Space_Creator_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox RVTlinks_list;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox get_room_data;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton geometry_from_floors;
        private System.Windows.Forms.RadioButton geometry_from_rooms;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label redundant_rooms_nb;
        private System.Windows.Forms.Label unclosed_rooms_nb;
        private System.Windows.Forms.Label placed_rooms_nb;
        private System.Windows.Forms.Label unplaced_rooms_nb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label total_rooms;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label phase;
        private System.Windows.Forms.Label sous_projet;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
    }
}