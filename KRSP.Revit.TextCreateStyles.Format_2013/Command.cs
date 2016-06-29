#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Xml;
#endregion

namespace KRSP.Revit.TextCreateStyles.Format_2013
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public const string XML_NAME = "KRSP_Text_Style_Config.xml";
        public const string ALL_FONTS = "<All Fonts>";
        public const string ALL_SIZES = "<All Sizes>";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            UIDocument doc = app.ActiveUIDocument;

            //List<string> fonts = Utils.TextUtilities.InstalledFonts();
            //System.Windows.Forms.MessageBox.Show(string.Format("I found {0} fonts.\nThe first one is: {1}",fonts.Count,fonts[0]));

            //<LeaderStyle Name="ARROW (30°) FILLED"
            //         ArrowStyle="8"
            //         Angle="30"
            //         Filled="1"
            //         Size="3"
            //         HeavyEndWeight="5" />
            LeaderStyle leader = new LeaderStyle()
            {
                Name = "ARROW (30°) FILLED",
                Style = LeaderStyle.ArrowStyle.Arrow,
                Angle = 30,
                Filled = true,
                Size = 3,
                HeavyEndWeight = 5
            };

            //<Style Name=""
            //  Description="Bold"
            //  Background="1"
            //  Bold="1"
            //  Italic="0"
            //  Underline="0"
            //  Colour="0"
            //  Leader="ARROW (30°) FILLED"
            //  LeaderOffset="1.0"
            //  LineWeight="1"
            //  TextBox="0"
            //  TabSize="12.0"
            //  WidthFactor="1"
            //>
            TextStyle style = new TextStyle()
            {
                Name = "test",
                Description = "Bold",
                Background = true,
                Bold = true,
                Italic = false,
                Underline = false,
                Color = System.Drawing.Color.Black,
                Leader = leader,
                LeaderOffset = 1,
                LineWeight = 1,
                TextBox = false,
                TabSize = 12,
                WidthFactor = 1
            };

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><KRSP_Annotation_Styles />");
            
            XmlNode node = xdoc.DocumentElement;
            style.AddToXmlDoc(ref node);
            
            return Result.Succeeded;
        }
    }
}
