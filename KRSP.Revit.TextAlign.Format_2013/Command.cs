#region Namespaces
using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace KRSP.Revit.TextAlign.Format_2013
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
            if (doc.Selection.Elements.IsEmpty)
            {
                message = "Nothing Selected!\nPlease select the text notes you want to align and try again.";
                return Result.Failed;
            }

            //check is there are textNotes in the current selection
            List<ElementId> ids = new List<ElementId>();
            foreach (Element elem in doc.Selection.Elements)
            {
                ids.Add(elem.Id);
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
            doc.Selection.Elements.Clear();
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
            Parameter param;

            //if the user select "Match Selection" for justification, get that now
            if (alignment == TextAlignment.MatchSelection)
            {
                param = alignObject.get_Parameter(BuiltInParameter.TEXT_ALIGN_HORZ);
                alignment =  (TextAlignment)param.AsInteger();
            }

            //apply the alignment and justification to all selected text notes
            IEnumerable<TextNote> textNotes = col.Cast<TextNote>();
            foreach (TextNote tn in textNotes)
            {
                param = tn.get_Parameter(BuiltInParameter.TEXT_ALIGN_HORZ);
                param.Set((int)alignment);
                tn.Coord = new XYZ(xValue, tn.Coord.Y, tn.Coord.Z);
            }

            //commit the transaction
            t.Commit();

            return Result.Succeeded;
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
