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
    public partial class Space_Creator : System.Windows.Forms.Form
    {
        Document _doc;
      
        public Space_Creator(Document doc)
        {
            InitializeComponent();
            _doc = doc;
        }

                private void Space_Creator_Load(object sender, EventArgs e)
        {
            
            foreach (RevitLinkInstance instance in new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements().Cast<RevitLinkInstance>())
            {
                try
                {
                    List<Room> rooms = new FilteredElementCollector(instance.GetLinkDocument()).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>().ToList();
                    if(rooms.Count!=0)
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

            phase.Text = GetActivePhase(_doc).Name;
            if(_doc.IsWorkshared)
                sous_projet.Text = GetActiveWorkset(_doc).Name;

            if (_doc.ActiveView.ViewType == ViewType.ThreeD)
            {
                Parameter crop = _doc.ActiveView.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION);
                Parameter clip_box = _doc.ActiveView.get_Parameter(BuiltInParameter.VIEWER_MODEL_CLIP_BOX_ACTIVE);
                if(crop.AsInteger()==0 && clip_box.AsInteger()==0)
                    geometry_from_floors.Enabled = true;
            }
            else
                geometry_from_floors.Enabled = false;

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public Workset GetActiveWorkset(Document doc)
        {
            WorksetTable worksetTable = doc.GetWorksetTable();
            WorksetId activeId = worksetTable.GetActiveWorksetId();
            Workset workset = worksetTable.GetWorkset(activeId);
            return workset;
        }

        public Phase GetActivePhase(Document doc)
        {
            PhaseArray phases = doc.Phases;
            Phase phase = null;
            if (0 != phases.Size)
            {
                phase = doc.GetElement(doc.ActiveView.get_Parameter(BuiltInParameter.VIEW_PHASE).AsElementId()) as Phase;
            }
            return phase;
        }

        public bool GetRoomLimitP(RevitLinkType type)
        {
            bool room_limit=true;
                    
            return room_limit;
        }

    
        public string GetSelectedRVTlink()
        {
            return RVTlinks_list.SelectedItem.ToString();
        }

      
        public bool roomData()
        {
            if (get_room_data.Checked)
                return true;
            else
                return false;
        }

        public bool spaceGeometry()
        {
            if (geometry_from_rooms.Checked==true)
                return true;
            else
                return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public enum RoomState
        {
            Unknown,
            Placed,
            NotPlaced,
            NotEnclosed,
            Redundant
        }

        // Distinguish 'Not Placed',  'Redundant' and 'Not Enclosed' rooms.

        private RoomState DistinguishRoom(Room room)
        {
            RoomState res = RoomState.Unknown;
            if (room.Area > 0)
            {      // Placed if having Area
                res = RoomState.Placed;
            }
            else if (null == room.Location)
            {      // No Area and No Location => Unplaced
                res = RoomState.NotPlaced;
            }
            else
            {      // must be Redundant or NotEnclosed        
                IList<IList<BoundarySegment>> segs = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                res = (null == segs || segs.Count == 0)
                 ? RoomState.NotEnclosed
                 : RoomState.Redundant;
            }
            return res;
        }

        private void RVTlinks_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            int unplaced = 0;
            int placed = 0;
            int unclosed = 0;
            int redundant = 0;
            int total = 0;

            foreach (RevitLinkInstance instance in new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements().Cast<RevitLinkInstance>())
            {
                try
                {
                    if(Path.GetFileName(instance.GetLinkDocument().PathName)==RVTlinks_list.SelectedItem.ToString())
                    {
                        foreach (Room room in new FilteredElementCollector(instance.GetLinkDocument()).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>())
                        {
                            total += 1;
                            if (DistinguishRoom(room) == RoomState.NotPlaced)
                                unplaced += 1;
                            if (DistinguishRoom(room) == RoomState.Placed||DistinguishRoom(room) == RoomState.NotEnclosed|| DistinguishRoom(room) == RoomState.Redundant)
                                placed += 1;
                            if (DistinguishRoom(room) == RoomState.NotEnclosed)
                                unclosed += 1;
                            if (DistinguishRoom(room) == RoomState.Redundant)
                                redundant += 1;
                        }
                        total_rooms.Text = total.ToString();
                        unplaced_rooms_nb.Text = unplaced.ToString();
                        placed_rooms_nb.Text = placed.ToString();
                        unclosed_rooms_nb.Text = unclosed.ToString();
                        redundant_rooms_nb.Text = redundant.ToString();
                    }
                }
                catch
                {
                }
            }

        }
    }
}
