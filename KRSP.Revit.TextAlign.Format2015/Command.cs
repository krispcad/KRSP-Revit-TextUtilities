#region Imported Namespaces

//.NET common used namespaces
using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

//Revit.NET common used namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion

namespace KRSP.Revit.TextAlign.Format2015
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        enum TextAlignment : int
        {
            NoChange = -1,
            MatchSelection = 0,
            AlignLeft = 64,
            AlignCentre = 128,
            AlignRight = 256
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            UIDocument doc = app.ActiveUIDocument;

            //check if there is anything selected
            if (doc.Selection.GetElementIds().Count == 0)
            {
                message = "Nothing Selected!\nPlease select the text notes you want to align and try again.";
                return Result.Failed;
            }

            //check is there are textNotes in the current selection
            List<ElementId> ids = new List<ElementId>();
            foreach (ElementId id in doc.Selection.GetElementIds())
            {
                ids.Add(id);
            }
            FilteredElementCollector col = new FilteredElementCollector(doc.Document, ids);
            col.OfClass(typeof(TextNote));
            if (col.ToElements().Count == 0)
            {
                message = "No Text Notes selected!\nPlease select the text notes you want to align and try again.";
                return Result.Failed;
            }

            //at this point we know that text notes are selected so we can proceed.
            TaskDialog td = new TaskDialog("Justification")
            {
                MainInstruction = "Select Justification",
                MainContent = "Select a justification option from the list below and then select the text note object to align to.",
                CommonButtons = TaskDialogCommonButtons.Cancel
            };
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,
                            "Left Justified", "This will set all selected text notes to have left justification.");
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2,
                            "Centre Justified", "This will set all selected text notes to have centre justification.");
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink3,
                            "Right Justified", "This will set all selected text notes to have right justification.");
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink4,
                            "Match Selection", "This will set all selected text notes to have the same justification as the select alignment text note.");

            //get the results from the task dialog
            TaskDialogResult result = td.Show();
            TextAlignment alignment = TextAlignment.NoChange;
            switch (result)
            {
                case TaskDialogResult.Cancel:
                    return Result.Cancelled;
                case TaskDialogResult.CommandLink2:
                    alignment = TextAlignment.AlignCentre;
                    break;
                case TaskDialogResult.CommandLink3:
                    alignment = TextAlignment.AlignRight;
                    break;
                case TaskDialogResult.CommandLink4:
                    alignment = TextAlignment.MatchSelection;
                    break;
                case TaskDialogResult.CommandLink1:
                default:
                    alignment = TextAlignment.AlignLeft;
                    break;
            }

            //clear current selection and ask the user to select the alignment text note
            doc.Selection.SetElementIds(new List<ElementId>());
            ISelectionFilter filter = new TextNoteSelectionFilter();
            Reference picked = doc.Selection.PickObject(ObjectType.Element, filter, "Select the text note to align to.");

            //check that the user picked something
            if (picked == null)
            {
                message = "Nothing selected!\nYou did not select a text note to align to.";
                return Result.Failed;
            }

            //start a new transaction
            Transaction t = new Transaction(doc.Document);
            t.Start("KrispCAD Text Align");

            TextNote alignObject = doc.Document.GetElement(picked.ElementId) as TextNote;
            double xValue = alignObject.Coord.X;
            ProjectLocation curLocation = doc.Document.ActiveProjectLocation;
            ProjectPosition projPosition = curLocation.get_ProjectPosition(alignObject.Coord);

            XYZ orign = XYZ.Zero;
            XYZ up = doc.ActiveView.UpDirection;

            //TaskDialog.Show("DEBUG", string.Format("Up Direction: ({0}, {1}, {2}))", up.X, up.Y, up.Z));
            // check page for shortest distance calc: http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment

            Parameter param;

            //if the user select "Match Selection" for justification, get that now
            if (alignment == TextAlignment.MatchSelection)
            {
                param = alignObject.get_Parameter(BuiltInParameter.TEXT_ALIGN_HORZ);
                alignment = (TextAlignment)param.AsInteger();
            }

            //apply the alignment and justification to all selected text notes
            IEnumerable<TextNote> textNotes = col.Cast<TextNote>();
            foreach (TextNote tn in textNotes)
            {
                param = tn.get_Parameter(BuiltInParameter.TEXT_ALIGN_HORZ);
                param.Set((int)alignment);
                double dist = MathUtils.DistanceToLine2D(new double[] { orign.X, orign.Y}, new double[] { up.X, up.Y}, new double[] { tn.Coord.X, tn.Coord.Y}, false);
                TaskDialog.Show("DEBUG", string.Format("Distance from line described by UP ({0}, {1}) to point ({2}, {3}) = {4}", up.X, up.Y, tn.Coord.X, tn.Coord.Y, dist));
                tn.Coord = new XYZ(xValue, tn.Coord.Y, tn.Coord.Z);
            }

            //commit the transaction
            t.Commit();

            return Result.Succeeded;
        }
    }

    public class MathUtils {

        private static double DotProduct(double[] pointA, double[] pointB, double[] pointC)
        {
            double[] AB = new double[2];
            double[] BC = new double[2];

            AB[0] = pointB[0] - pointA[0];
            AB[1] = pointB[1] - pointA[1];

            BC[0] = pointC[0] - pointB[0];
            BC[1] = pointC[1] - pointB[1];

            double dot = AB[0] * BC[0] + AB[1] * BC[1];

            return dot;
        }

        private static double CrossProduct(double[] pointA, double[] pointB, double[] pointC)
        {
            double[] AB = new double[2];
            double[] AC = new double[2];

            AB[0] = pointB[0] - pointA[0];
            AB[1] = pointB[1] - pointA[1];

            AC[0] = pointC[0] - pointA[0];
            AC[1] = pointC[1] - pointA[1];

            double cross = AB[0] * AC[1] - AB[1] * AC[0];

            return cross;
        }

        public static double Distance(double[] pointA, double[] pointB)
        {
            double d1 = pointA[0] - pointB[0];
            double d2 = pointA[1] - pointB[1];

            return Math.Sqrt(d1 * d1 + d2 * d2);
        }

        public static double DistanceToLine2D(double[] pointA, double[] pointB, double[] pointC, bool isSegment)
        {
            double dist = CrossProduct(pointA, pointB, pointC) / Distance(pointA, pointB);

            if (isSegment)
            {
                double dot1 = DotProduct(pointA, pointB, pointC);
                if (dot1 > 0)
                    return Distance(pointB, pointC);

                double dot2 = DotProduct(pointB, pointA, pointC);
                if (dot2 > 0)
                    return Distance(pointA, pointC);
            }

            return Math.Abs(dist);
        }
    }


    public class TextNoteSelectionFilter : ISelectionFilter
    {

        public bool AllowElement(Element elem)
        {
            return elem is TextNote;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
