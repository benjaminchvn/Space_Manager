using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.IO;

namespace Space_Manager
{
    public partial class Space_Analyse_Form : System.Windows.Forms.Form
    {
        Document _doc;
        public Space_Analyse_Form(Document doc)
        {
            InitializeComponent();
            _doc = doc;
        }             

        private void Space_Analyse_Form_Load(object sender, EventArgs e)
        {
            foreach (RevitLinkInstance instance in new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements().Cast<RevitLinkInstance>())
            {
                try
                {
                    List<Room> rooms = new FilteredElementCollector(instance.GetLinkDocument()).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>().ToList();
                    if (rooms.Count != 0)
                    {
                        string RVT_link = instance.GetLinkDocument().PathName;
                        RVTlinks_list.Items.Add(RVT_link.Substring(RVT_link.LastIndexOf('\\') + 1));
                    }
                }
                catch
                {
                }
            }

            if (RVTlinks_list.Items.Count > 0)
            {
                RVTlinks_list.Sorted = true;
                RVTlinks_list.SelectedIndex = 0;
            }
        }

        public string GetSelectedRVTlink()
        {
            return RVTlinks_list.SelectedItem.ToString();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
