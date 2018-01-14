using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using OfficeOpenXml;
using System.Diagnostics;
using OfficeOpenXml.Style;

namespace Space_Manager
{
    public class Space_Manager : IExternalApplication
    {
        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            string path = Assembly.GetExecutingAssembly().Location;
            string myRibbon = "BIM Coordination";
            try
            {
                application.CreateRibbonTab(myRibbon);
            }
            catch { }

            RibbonPanel panel1 = application.CreateRibbonPanel(myRibbon, "Gestion des espaces");
            
            PushButton space_creation = (PushButton)panel1.AddItem(new PushButtonData("Créer des espaces", "Créer des espaces", path, "Space_Manager.Space_creation"));

            space_creation.LargeImage = PngImageSource("Space_Manager.Resources.Space_Creation_32.png");
            space_creation.Image = PngImageSource("Space_Manager.Resources.Space_Creation_16.png");
            //space_creation.ToolTipImage = PngImageSource("Space_Manager.Resources.Space_Creation_16.png");
            space_creation.ToolTip = "Crée les espaces MEP sur la base des pièces architectes.";
            space_creation.LongDescription = "Sélectionnez les options souhaitées dans la boîte de dialogue pour générer les espaces MEP à partir des pièces d'une maquette en lien.";

            PushButton space_analyse = (PushButton)panel1.AddItem(new PushButtonData("Analyser les écarts espaces/pièces", "Analyser les espaces", path, "Space_Manager.Space_analyse"));

            //space_creation.LargeImage = PngImageSource("Space_Manager.Resources.Space_Analyse_32.png");
            //space_creation.Image = PngImageSource("Space_Manager.Resources.Space_Analyse_16.png");
            //space_creation.ToolTipImage = PngImageSource("Space_Manager.Resources.Space_Analyse_16.png");
            space_analyse.ToolTip = "Analyse les écarts entre les espaces MEP et les pièces architectes.";
            space_analyse.LongDescription = "Sélectionnez le ou les liens contenant les pièces pour lancer l'analyse.";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Cancelled;
        }

        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class Space_analyse : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("d8fb0ae9-81e7-4524-8362-13b0fc76e2d0"));

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

        public enum SpaceState
        {
            Unknown,
            Placed,
            NotPlaced,
            NotEnclosed,
            Redundant
        }

        // Distinguish 'Not Placed',  'Redundant' and 'Not Enclosed' rooms.

        SpaceState DistinguishSpace(Space space)
        {
            SpaceState res = SpaceState.Unknown;
            if (space.Area > 0)
            {      // Placed if having Area
                res = SpaceState.Placed;
            }
            else if (null == space.Location)
            {      // No Area and No Location => Unplaced
                res = SpaceState.NotPlaced;
            }
            else
            {      // must be Redundant or NotEnclosed        
                IList<IList<BoundarySegment>> segs = space.GetBoundarySegments(new SpatialElementBoundaryOptions());
                res = (null == segs || segs.Count == 0)
                 ? SpaceState.NotEnclosed
                 : SpaceState.Redundant;
            }
            return res;
        }

        public List<string> Get_Space_Info(Space space, Room associated_room, string écart, string commentaire)
        {
            string none = "-";
            List<string> list = new List<string>();
            list.Add(space.Id.ToString());
            list.Add(space.Level.Name);
            list.Add(space.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
            list.Add(space.get_Parameter(BuiltInParameter.ROOM_NAME).AsString());            
            list.Add(UnitUtils.ConvertFromInternalUnits(space.Area, DisplayUnitType.DUT_SQUARE_METERS).ToString());
            if(associated_room!=null)
            {
                list.Add(associated_room.Id.ToString());
                list.Add(associated_room.Level.Name);
                list.Add(associated_room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
                list.Add(associated_room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString());
                list.Add(UnitUtils.ConvertFromInternalUnits(associated_room.Area, DisplayUnitType.DUT_SQUARE_METERS).ToString());
                list.Add(écart);
                list.Add(commentaire);
            }
            else
            {
                list.Add(none);
                list.Add(none);
                list.Add(none);
                list.Add(none);
                list.Add(none);
                list.Add(écart);
                list.Add(commentaire);
            }

            return list;
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<string> liste = new List<string>();
            foreach (RevitLinkInstance instance in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements().Cast<RevitLinkInstance>())
            {
                try
                {
                    List<Room> rooms = new FilteredElementCollector(instance.GetLinkDocument()).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>().ToList();
                    if (rooms.Count != 0)
                    {
                        string RVT_link = instance.GetLinkDocument().PathName;
                        liste.Add(RVT_link);
                    }
                }
                catch
                {
                }
            }

            if (liste.Count == 0)
            {
                TaskDialog td = new TaskDialog("Avertissement");
                td.Title = "Avertissement";
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                td.MainContent = "Aucun lien Revit contenant des pièces n'est chargé dans le projet actif.";
                td.Show();
                return Result.Cancelled;
            }

            Space_Analyse_Form form = new Space_Analyse_Form(doc);

            if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                return Result.Cancelled;
            }

            if (DialogResult.OK == form.ShowDialog())
            {
                List<Level> levels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements().Cast<Level>().ToList();

                using (Transaction t = new Transaction(doc, "Analyser les espaces"))
                {
                    t.Start();
                    Document linkDoc = null;
                    string test = "";
                    string test2 = "";
                    string filename = form.GetSelectedRVTlink();
                    List<Room> roomList = new List<Room>();
                    List<ElementId> roomId = new List<ElementId>();
                    List<string> roomNamelist = new List<string>();

                    List<List<string>> master_list = new List<List<string>>();

                    foreach (RevitLinkInstance instance in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements().Cast<RevitLinkInstance>())
                    {
                        linkDoc = instance.GetLinkDocument();
                        if (linkDoc != null && Path.GetFileName(linkDoc.PathName) == filename)
                        {
                            foreach (Room r in new FilteredElementCollector(linkDoc).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>())
                            {
                                if (DistinguishRoom(r) != RoomState.NotPlaced)
                                {
                                    roomList.Add(r);
                                    roomId.Add(r.Id);
                                    roomNamelist.Add(r.Name);
                                }
                            }
                        }
                    }

                    test += string.Join(" / ", roomNamelist);

                    List<Room> associatedroomList = new List<Room>();
                    List<string> associatedroomNameList = new List<string>();
                    List<ElementId> associatedroomList_Id = new List<ElementId>();

                    List<string> spaceNamelist = new List<string>();
                    foreach (Autodesk.Revit.DB.Mechanical.Space s in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MEPSpaces).Cast<Autodesk.Revit.DB.Mechanical.Space>())
                    {
                        spaceNamelist.Add(s.Name);
                    }

                    foreach (Autodesk.Revit.DB.Mechanical.Space s in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MEPSpaces).Cast<Autodesk.Revit.DB.Mechanical.Space>())
                    {
                        Parameter spaceName = s.get_Parameter(BuiltInParameter.ROOM_NAME);
                        Parameter spaceNumber = s.get_Parameter(BuiltInParameter.ROOM_NUMBER);
                        Parameter roomName = s.get_Parameter(BuiltInParameter.SPACE_ASSOC_ROOM_NAME);
                        Parameter roomNumber = s.get_Parameter(BuiltInParameter.SPACE_ASSOC_ROOM_NUMBER);

                        Room associated_room;
                        try
                        {
                            associated_room = (from element in new FilteredElementCollector(linkDoc).
                                               OfClass(typeof(SpatialElement)).
                                               OfCategory(BuiltInCategory.OST_Rooms)
                                               where element.Name == roomName.AsString() + " " + roomNumber.AsString()
                                               select element).Cast<Room>().ToList().First();
                            associatedroomList.Add(associated_room);
                            associatedroomNameList.Add(associated_room.Name);
                            associatedroomList_Id.Add(associated_room.Id);
                           
                        }
                        catch
                        {
                            associated_room = null;
                        }

                        string space = s.Id.ToString() + " " + spaceName.AsString() + " " + spaceNumber.AsString();
                        bool existing_room = false;
                        if (roomNamelist.Contains(spaceName.AsString() + " " + spaceNumber.AsString()))
                            existing_room = true;

                        string comment = "";
                        {
                        if (associated_room != null && associated_room.Name != space && existing_room == true && !spaceNamelist.Contains(associated_room.Name))
                           comment = "Pièce déplacée : " + space;

                        if (associated_room != null && associated_room.Name != space && existing_room == true && spaceNamelist.Contains(associated_room.Name))
                           comment = "Pièce échangée : " + space;
                        }

                        if (DistinguishSpace(s) == SpaceState.NotPlaced)
                            master_list.Add(Get_Space_Info(s, null, "Espace non placé",comment));

                        else if (DistinguishSpace(s) == SpaceState.NotEnclosed)
                            master_list.Add(Get_Space_Info(s,null,"Espace non fermé",comment));

                        else if (DistinguishSpace(s) == SpaceState.Redundant)
                            master_list.Add(Get_Space_Info(s, null,"Espace superflu",comment)); 

                        else if (DistinguishSpace(s) == SpaceState.Placed&&(roomName.AsString() == "Non occupé"|| roomName.AsString() == "Unoccupied"))
                            master_list.Add(Get_Space_Info(s, null,"Espace sans pièce associée",comment)); 

                        else// (DistinguishSpace(s) == SpaceState.Placed && roomName.AsString() != "Non occupé" && roomName.AsString() != "Unoccupied")
                        {
                            List<string> écart = new List<string>();
                            
                            if (spaceName.AsString() == roomName.AsString()&& spaceNumber.AsString() == roomNumber.AsString()&& associated_room != null && s.Area == associated_room.Area)
                                master_list.Add(Get_Space_Info(s, associated_room, "_Aucun_",comment));

                            if (spaceName.AsString() != roomName.AsString())
                                écart.Add("Nom");
                          
                            if (spaceNumber.AsString() != roomNumber.AsString())
                                écart.Add("Numéro");

                            if (associated_room != null && s.Area != associated_room.Area)
                                écart.Add("Surface");

                            if ((spaceName.AsString() != roomName.AsString())||(spaceNumber.AsString() != roomNumber.AsString())||(associated_room != null && s.Area != associated_room.Area))
                                master_list.Add(Get_Space_Info(s, associated_room, string.Join(" + ",écart),comment));
                        }                    
                    }

                    foreach (Room room in roomList)
                    {
                        if (DistinguishRoom(room) == RoomState.Placed)
                        {
                            ElementId eid = room.Id;
                            if (!associatedroomList_Id.Contains(eid))
                                test += "\n" + "Pièce sans espace associé : " + room.Name;
                        }
                    }

                    //test
                    string xls_filename = Path.Combine(Path.GetTempPath(), "myExcelOutput.xlsx");
                    using (ExcelPackage package = new ExcelPackage(new FileInfo(xls_filename)))
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("My Data");
                        int row = 2;
                        int row2 = 1;
                        List<int> columns = new List<int>();
                        int[] nb = {1,2,3,4,5,6,7,8,9,10,11,12};
                        columns.AddRange(nb);

                        sheet.Cells[row2, 1].Value = "ID";
                        sheet.Cells[row2, 2].Value = "Niveau";
                        sheet.Cells[row2, 3].Value = "Numéro";
                        sheet.Cells[row2, 4].Value = "Nom";
                        sheet.Cells[row2, 5].Value = "Surface (m²)";
                        sheet.Cells[row2, 6].Value = "ID pièce";
                        sheet.Cells[row2, 7].Value = "Niveau pièce";
                        sheet.Cells[row2, 8].Value = "Numéro pièce";
                        sheet.Cells[row2, 9].Value = "Nom pièce";
                        sheet.Cells[row2, 10].Value = "Surface pièce";
                        sheet.Cells[row2, 11].Value = "Ecart espace-pièce";
                        sheet.Cells[row2, 12].Value = "Commentaire";

                        foreach (List<string> list in master_list)
                        {
                           foreach(int i in columns)
                            {
                                sheet.Cells[row, i].Value = list[i-1];                               
                            }
                            row++;
                        }

                        using (ExcelRange rng = sheet.Cells["A1:L1"])
                        {
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(191, 191, 191)); 
                         }

                        using (ExcelRange rng = sheet.Cells["A:L"])
                        {
                            rng.AutoFilter = true;
                            rng.AutoFitColumns();
                        }

                        using (ExcelRange rng = sheet.Cells["F:J"])
                        {
                            rng.Style.Font.Italic = true;
                        }
                        package.Save();
                    }
                    Process.Start(xls_filename);
                    t.Commit();
                    return Result.Succeeded;
                }

            }
            return Result.Succeeded;

        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class Space_creation : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("fd2c0718-acc8-4b2e-a888-f92185a87cc0"));

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

        private Level GetNearestLevel(XYZ point, List<Level> levels)
        {
            Level nearestLevel = levels.FirstOrDefault();
            double delta = Math.Abs(nearestLevel.ProjectElevation - point.Z);

            foreach (Level currentLevel in levels)
            {
                if (Math.Abs(currentLevel.ProjectElevation - point.Z) < delta)
                {
                    nearestLevel = currentLevel;
                    delta = Math.Abs(currentLevel.ProjectElevation - point.Z);
                }
            }

            return nearestLevel;
        }

      
        private List<double> GetDistancesToFloors(Document doc, View3D view3D, Room room)
        {
            LocationPoint point = room.Location as LocationPoint;
            XYZ origin = point.Point;
            XYZ center = new XYZ(point.Point.X, point.Point.Y, point.Point.Z + 0.01);
            // Project in the positive Z direction up to the floor.
            XYZ rayDirection = new XYZ(0, 0, 1);

            ElementClassFilter filter = new ElementClassFilter(typeof(Floor));

            ReferenceIntersector refIntersector = new ReferenceIntersector(filter, FindReferenceTarget.Face, view3D);
            refIntersector.FindReferencesInRevitLinks = true;
            IList<ReferenceWithContext> referencesWithContext = refIntersector.Find(center, rayDirection);

            IList<Reference> intersectRefs = new List<Reference>();
            Dictionary<Reference, XYZ> dictProvisionForVoidRefs = new Dictionary<Reference, XYZ>();
            List<double> result = new List<double>();

            foreach (ReferenceWithContext r in referencesWithContext)
            {
                XYZ intersection = r.GetReference().GlobalPoint;
                Line line = Line.CreateBound(origin, intersection);
                result.Add(line.Length);
            }
            result.Sort();
            return result;
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
           
            List<string> liste = new List<string>();
           
            foreach (RevitLinkInstance instance in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements().Cast<RevitLinkInstance>())
            {
                try
                {
                    List<Room> rooms = new FilteredElementCollector(instance.GetLinkDocument()).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>().ToList();
                    
                    if (rooms.Count != 0)
                    {
                    string RVT_link = instance.GetLinkDocument().PathName;
                    liste.Add(RVT_link);
                    }
                }
                catch
                {
                }
            }

            if (liste.Count==0)
                {
                TaskDialog td = new TaskDialog("Avertissement");
                td.Title = "Avertissement";
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                td.MainContent = "Aucun lien Revit contenant des pièces n'est chargé dans le projet actif.";
                td.Show();
                return Result.Cancelled;
                }

            Space_Creator form = new Space_Creator(doc);

            if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                return Result.Cancelled;
            }

            if (DialogResult.OK == form.ShowDialog())
            {
                List<Level> levels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements().Cast<Level>().ToList();

                using (Transaction t = new Transaction(doc, "Créer des espaces à partir des pièces"))
                {
                    t.Start();
                    string filename = form.GetSelectedRVTlink();
                    //var viewFamilyType = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);
                    //View3D view3D = View3D.CreateIsometric(doc, viewFamilyType.Id);
                    //view3D.Name = "__TEST A EFFACER";
                    //string room_param = "FICHE";
                    //string space_param = "INGP_FICHE";
                    int unplaced_rooms = 0;
                    int unclosed_rooms = 0;
                    int redundant_rooms = 0;
                    int created_spaces = 0;
                    List<string> unclosed_rooms_levels = new List<string>();
                    List<string> redundant_rooms_levels = new List<string>();

                    List<Level> MEP_levels = new List<Level>();
                    MEP_levels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements().Cast<Level>().ToList();

                    foreach (RevitLinkInstance instance in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements().Cast<RevitLinkInstance>())
                    {
                        Document linkDoc = instance.GetLinkDocument();

                            if (linkDoc != null && Path.GetFileName(linkDoc.PathName) == filename)
                            {
                            List<string> rooms = new List<string>();
                            foreach(Space s in new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_MEPSpaces).Cast<Space>())
                                {
                                Parameter room_name = s.get_Parameter(BuiltInParameter.SPACE_ASSOC_ROOM_NAME);
                                Parameter room_number = s.get_Parameter(BuiltInParameter.SPACE_ASSOC_ROOM_NUMBER);
                                rooms.Add(room_name.AsString()+ " "+room_number.AsString());
                                }

                            Transform tr = instance.GetTotalTransform();

                            foreach (Room room in new FilteredElementCollector(linkDoc).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>())
                                {
                                    if (DistinguishRoom(room) == RoomState.Placed || DistinguishRoom(room) == RoomState.NotEnclosed || DistinguishRoom(room) == RoomState.Redundant)
                                    {
                                        Parameter room_name = room.get_Parameter(BuiltInParameter.ROOM_NAME);
                                        Parameter room_number = room.get_Parameter(BuiltInParameter.ROOM_NUMBER);
                                        //Parameter room_level = room.LookupParameter("Niveau");
                                        LocationPoint point = room.Location as LocationPoint;
                                        XYZ pt = new XYZ(point.Point.X, point.Point.Y, point.Point.Z);
                                        pt = tr.OfPoint(pt);
                                        
                                        Level level = GetNearestLevel(pt, levels);
                                        Phase phase = doc.GetElement(doc.ActiveView.get_Parameter(BuiltInParameter.VIEW_PHASE).AsElementId()) as Phase;

                                        if (!rooms.Contains(room_name.AsString() + " " + room_number.AsString()))
                                        {
                                        Space space = doc.Create.NewSpace(level, phase, new UV(pt.X,pt.Y));
                                        created_spaces += 1;

                                        if (form.roomData() == true)
                                        {
                                            space.Name = room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
                                            space.Number = room.Number;
                                        }
                                        //space.LookupParameter(space_param).Set(room.LookupParameter(room_param).AsString());
                                        space.BaseOffset = room.BaseOffset;

                                        if (form.spaceGeometry() == true)
                                        {
                                            Parameter limitOffset = space.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET);
                                            if (room.LimitOffset > 0)
                                                limitOffset.Set(room.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).AsDouble());
                                            if (space.get_Parameter(BuiltInParameter.ROOM_HEIGHT).AsDouble() != room.get_Parameter(BuiltInParameter.ROOM_HEIGHT).AsDouble())
                                                limitOffset.Set(room.get_Parameter(BuiltInParameter.ROOM_HEIGHT).AsDouble() + room.get_Parameter(BuiltInParameter.ROOM_LOWER_OFFSET).AsDouble());
                                        }

                                        if (form.spaceGeometry() == false)
                                        {
                                            Parameter limitOffset = space.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET);
                                            List<double> distances = GetDistancesToFloors(doc, doc.ActiveView as View3D, room);
                                            try
                                            {
                                             if (distances[0] < room.BaseOffset)
                                                {
                                                    if (distances[1] < room.BaseOffset)
                                                        //limitOffset.Set(Math.Round(distances[2], 2));
                                                        limitOffset.Set(distances[2]);
                                                    else
                                                        //limitOffset.Set(Math.Round(distances[1], 2));
                                                        limitOffset.Set(distances[1]);
                                                }
                                                else
                                                    //limitOffset.Set(Math.Round(distances.First(), 2));
                                                    limitOffset.Set(distances.First());

                                                for (int i = 0; i < 10; i++)
                                                {
                                                     if (space.UnboundedHeight < UnitUtils.ConvertToInternalUnits(1.5, DisplayUnitType.DUT_METERS))
                                                    {
                                                        limitOffset.Set(Math.Round(distances[i], 5));
                                                        //limitOffset.Set(distances[i]);
                                                        if (space.UnboundedHeight > UnitUtils.ConvertToInternalUnits(1.5, DisplayUnitType.DUT_METERS))
                                                        break;
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                            if (room.LimitOffset > 0)
                                                limitOffset.Set(room.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).AsDouble());
                                            if (space.get_Parameter(BuiltInParameter.ROOM_HEIGHT).AsDouble() != room.get_Parameter(BuiltInParameter.ROOM_HEIGHT).AsDouble())
                                                limitOffset.Set(room.get_Parameter(BuiltInParameter.ROOM_HEIGHT).AsDouble() + room.get_Parameter(BuiltInParameter.ROOM_LOWER_OFFSET).AsDouble());
                                            }
                                        }

                                        if (DistinguishRoom(room) == RoomState.Redundant)
                                        {
                                            LocationPoint pt2 = space.Location as LocationPoint;
                                            XYZ xyz = new XYZ(0.001, 0, 0);
                                            XYZ xyz2 = new XYZ(0.001, 0, 0);
                                            pt2.Move(xyz);
                                            pt2.Move(-xyz);
                                        }
                                    }
                                    }
                                    if (DistinguishRoom(room) == RoomState.NotPlaced)
                                        unplaced_rooms += 1;

                                    if (DistinguishRoom(room) == RoomState.NotEnclosed)
                                    {
                                        unclosed_rooms += 1;
                                        if (!unclosed_rooms_levels.Contains(room.Level.Name))
                                            unclosed_rooms_levels.Add(room.Level.Name);
                                    }
                                    if (DistinguishRoom(room) == RoomState.Redundant)
                                    {
                                        redundant_rooms += 1;
                                        if (!redundant_rooms_levels.Contains(room.Level.Name))
                                            redundant_rooms_levels.Add(room.Level.Name);
                                    }
                                }
                            }                
                    }

                    TaskDialog td = new TaskDialog("Confirmation");
                    td.AllowCancellation = true;
                    td.TitleAutoPrefix = true;
                   

                    if (created_spaces > 0)
                    {
                        td.Title = "Confirmation";
                        td.MainInstruction = created_spaces.ToString() + " espaces vont être crées.";

                        if (unclosed_rooms == 0 && redundant_rooms == 0)
                            td.MainContent = "Confirmer la création ?";
                        if (unclosed_rooms > 0 && redundant_rooms > 0)
                            td.MainContent = "dont espaces non fermés : " + unclosed_rooms.ToString() + " (" + string.Join("; ", unclosed_rooms_levels) + ")"
                                 + Environment.NewLine + "dont espaces superflus : " + redundant_rooms.ToString() + " (" + string.Join("; ", redundant_rooms_levels) + ")"
                                 + Environment.NewLine + "Confirmer la création ?";
                        if (unclosed_rooms > 0 && redundant_rooms == 0)
                            td.MainContent = "dont espaces non fermés : " + unclosed_rooms.ToString() + " (" + string.Join("; ", unclosed_rooms_levels) + ")"
                                 + Environment.NewLine + "Confirmer la création ?"; 
                        if (unclosed_rooms == 0 && redundant_rooms > 0)
                            td.MainContent = "dont espaces superflus : " + redundant_rooms.ToString() + " (" + string.Join("; ", redundant_rooms_levels) + ")"
                                 + Environment.NewLine + "Confirmer la création ?";
                       
                        td.CommonButtons = TaskDialogCommonButtons.Cancel | TaskDialogCommonButtons.Ok;
                        TaskDialogResult result = td.Show();
                        if (result == TaskDialogResult.Ok)
                            t.Commit();
                        return Result.Succeeded;
                    }

                  }
                return Result.Succeeded;
            }
            return Result.Succeeded;
        }

    }
}