#region Namespaces
using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace KRSP.Revit.TextChangeCase.Format_2013
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        private enum CaseType : int
        {
            Upper, Lower, Title, Sentence
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            UIDocument doc = app.ActiveUIDocument;

            //text note collector
            FilteredElementCollector col = new FilteredElementCollector(doc.Document);
            string mes = "This application will change the case of the selected text notes.";

            if (doc.Selection.Elements.Size > 0)
            {
                List<ElementId> ids = new List<ElementId>();
                foreach (Element elem in doc.Selection.Elements)
                {
                    ids.Add(elem.Id);
                }

                col = new FilteredElementCollector(doc.Document, ids);
                col.OfClass(typeof(TextNote));

                if (doc.Selection.Elements.Size > 0 && !(col.ToElements().Count > 0))
                {
                    col = new FilteredElementCollector(doc.Document);
                    col.OfClass(typeof(TextNote));
                    mes = "This application will change the case of all text notes in the entire project.";
                }
            }
            else
            {
                //nothing selected
                col.OfClass(typeof(TextNote));
                mes = "This application will change the case of all text notes in the entire project.";
            }

            IEnumerable<TextNote> textNotes = col.Cast<TextNote>();

            //ask the use which type of casing they want
            TaskDialog td = new TaskDialog("Case Type")
            {
                MainContent = mes,
                CommonButtons = TaskDialogCommonButtons.Cancel
            };
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "UPPER", "Change text to be in upper case.");
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "lower", "Change text to be in lower case.");
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, "Title", "Change text to be in title case.");
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink4, "Sentence", "Change text to be in sentence case.");

            //get result
            TaskDialogResult result = td.Show();
            CaseType caseType;

            switch (result)
            {
                case TaskDialogResult.Cancel:
                    return Result.Cancelled;
                case TaskDialogResult.CommandLink2:
                    caseType = CaseType.Lower;
                    break;
                case TaskDialogResult.CommandLink3:
                    caseType = CaseType.Title;
                    break;
                case TaskDialogResult.CommandLink4:
                    caseType = CaseType.Sentence;
                    break;
                case TaskDialogResult.CommandLink1:
                default:
                    caseType = CaseType.Upper;
                    break;
            }

            //make the changes
            Transaction t = new Transaction(doc.Document);
            t.Start("KrispCAD Change Case");
            foreach (TextNote tn in textNotes)
            {
                tn.Text = ChangeCase(tn.Text, caseType);
            }
            t.Commit();

            return Result.Succeeded;
        }

        private string ChangeCase(string original, CaseType caseType)
        {
            bool fixUnits = true;
            bool fixMultipliers = true;

            string units = "mm,m";
            string result = original;

            switch (caseType)
            {
                case CaseType.Upper:
                    result = original.ToUpper();
                    break;
                case CaseType.Lower:
                    result = original.ToLower();
                    break;
                case CaseType.Title:
                    System.Globalization.TextInfo textInfo = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;
                    result = textInfo.ToTitleCase(original.ToLower());
                    break;
                case CaseType.Sentence:
                    result = Utils.TextUtilities.SentenceCase(original);
                    break;
                default:
                    result = original;
                    break;
            }

            if (fixUnits) result = Utils.TextUtilities.FixUnits(result, units.Split(','));
            if (fixMultipliers) result = Utils.TextUtilities.FixMultiplier(result);

            return result;
        }
    }
}
