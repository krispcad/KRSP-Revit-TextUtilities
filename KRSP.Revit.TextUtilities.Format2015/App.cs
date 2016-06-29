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
using System.IO;
using System.Windows.Media;
using System.Reflection;

#endregion

namespace KRSP.Revit.TextUtilities.Format2015
{
    public class App : IExternalApplication
    {
        const string PANEL_NAME = "KrispCAD";
        static FileInfo addin = new FileInfo(Assembly.GetExecutingAssembly().Location);

        public Result OnStartup(UIControlledApplication a)
        {
            //declare variables
            PulldownButton pulldownButton;
            PulldownButtonData pulldownData;

            //create Ribbon panel
            List<RibbonPanel> panels = a.GetRibbonPanels();
            RibbonPanel krspPanel = null;
            foreach (RibbonPanel pnl in panels)
            {
                if (pnl.Name == PANEL_NAME)
                {
                    krspPanel = pnl;
                }
            }
            if (krspPanel == null)
            {
                krspPanel = a.CreateRibbonPanel(PANEL_NAME);
            }

            //Create contextual help
            ContextualHelp help = new ContextualHelp(ContextualHelpType.Url, @"http://krispcad.blogspot.com.au/2013/07/revit-exchange-app-store.html");

            //create pulldown data
            System.Drawing.Image image = Properties.Resources.KRSP_TextUtilities;
            ImageSource img = Utils.ImageUtils.GetSource(image);
            pulldownData = new PulldownButtonData("TextUtils", "Text\nUtilities")
            {
                LargeImage = img,
                Image = img
            };
            pulldownButton = krspPanel.AddItem(pulldownData) as PulldownButton;
            pulldownButton.SetContextualHelp(help);

            image = Properties.Resources.TextCase;
            AddPushButton("KRSP.Revit.TextChangeCase.Format2015.dll",
                "ChangeTextCase",
                "Change Case",
                "KRSP.Revit.TextChangeCase.Format2015.Command",
                "Change the case of the specified text notes in the project.",
                "",
                Utils.ImageUtils.GetSource(image),
                pulldownButton);

            image = Properties.Resources.TextAlign;
            AddPushButton("KRSP.Revit.TextAlign.Format2015.dll",
                "TextAlign",
                "Text Align",
                "KRSP.Revit.TextAlign.Format2015.Command",
                "Align selected text notes to the selected text note.",
                "",
                Utils.ImageUtils.GetSource(image),
                pulldownButton);


            //AddPushButton("KRSP.Revit.TextCreateStyles.Format_2013.dll",
            //    "CreateTextStyles",
            //    "KrispCAD Create Text Styles",
            //    "KRSP.Revit.TextCreateStyles.Format_2013.Command",
            //    "Create text styles based on predefined settings.",
            //    "",
            //    img,
            //    pulldownButton);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        void AddPushButton(string assName,
                        string name,
                        string title,
                        string className,
                        string description,
                        string longDescription,
                        ImageSource image,
                        PulldownButton pulldownButton)
        {
            try
            {
                string path = Path.Combine(addin.DirectoryName, assName);

                PushButtonData buttonData = new PushButtonData(
                                                name,
                                                title,
                                                path,
                                                className)
                {
                    ToolTip = description,
                    LongDescription = longDescription,
                    LargeImage = image,
                    Image = image
                };
                PushButton button = pulldownButton.AddPushButton(buttonData);

                //button.SetContextualHelp(help);
                button.Enabled = true;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ERROR", string.Format("Error creating button for the tool: {0}\n{1}", title, ex.ToString()));
                return;
            }
        }
    }
}
