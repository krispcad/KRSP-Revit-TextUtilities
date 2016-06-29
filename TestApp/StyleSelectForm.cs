using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KRSP.Revit.TextCreateStyles;
using System.Xml;
using System.IO;

namespace TestApp
{
    public partial class StyleSelectForm : Form
    {
        static List<LeaderStyle> Leaders;
        static List<TextStyle> TextStyles;
        //static List<string> FontNames;
        static List<double> Sizes;

        public StyleSelectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Leaders = new List<LeaderStyle>();
            TextStyles = new List<TextStyle>();
            //FontNames = new List<string>();
            Sizes = new List<double>();

            LeaderStyle leader = new LeaderStyle()
            {
                Name = "ARROW (30°) FILLED",
                Style = LeaderStyle.ArrowStyle.Arrow,
                Angle = 30,
                Filled = true,
                Size = 3,
                HeavyEndWeight = 5
            };

            Leaders.Add(leader);

            TextStyle style = new TextStyle()
            {
                Name = "test",
                Description = "Bold",
                Background = true,
                Bold = true,
                Italic = false,
                Underline = false,
                Color = System.Drawing.Color.Red,
                Leader = leader,
                LeaderOffset = 1,
                LineWeight = 1,
                TextBox = false,
                TabSize = 12,
                WidthFactor = 1
            };
            TextStyles.Add(style);


            XmlDocument xdoc = Serialise();
            xdoc.Save(@"C:\Temp\test.xml");
        }

        private bool Deserialise(string pathToConfigXml, out string error)
        {
            FileInfo file = new FileInfo(pathToConfigXml);
            if (file.Exists)
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(file.FullName);

                XmlNodeList leaderStyles = xdoc.GetElementsByTagName("LeaderStyle");
                if (leaderStyles.Count == 0)
                {
                    error = string.Format("The config file at {0} does not contain any LeaderStyles.", pathToConfigXml);
                    return false;
                }
                XmlNodeList sizes = xdoc.GetElementsByTagName("Size");
                if (sizes.Count == 0)
                {
                    error = string.Format("The config file at {0} does not contain any Sizes.", pathToConfigXml);
                    return false;
                }
                XmlNodeList textStyles = xdoc.GetElementsByTagName("TextStyle");
                if (textStyles.Count == 0)
                {
                    error = string.Format("The config file at {0} does not contain any TextStyles.", pathToConfigXml);
                    return false;
                }

                //at this point the xml file contains all relevant information
                //load sizes
                Sizes = new List<double>();
                foreach (XmlNode node in sizes)
                {
                    XmlAttribute att = node.Attributes["Value"];
                    if (att != null)
                    {
                        Sizes.Add(Convert.ToDouble(att.Value));
                    }
                }

                //load leaderStyles
                Leaders = new List<LeaderStyle>();
                foreach (XmlNode node in leaderStyles)
                {
                    //<LeaderStyle HeavyEndWeight="5" Size="3" Filled="1" Angle="30" ArrowStyle="8" Name="ARROW (30°) FILLED"/>
                    LeaderStyle leader = new LeaderStyle();
                    XmlAttribute att = node.Attributes["Name"];
                    leader.Name = att.Value;
                    att = node.Attributes["ArrowStyle"];
                    leader.Style = (LeaderStyle.ArrowStyle)(Convert.ToInt32(att.Value));
                    att = node.Attributes["Angle"];
                    leader.Angle = Convert.ToInt32(att.Value);
                    att = node.Attributes["Filled"];
                    leader.Filled = att.Value == "1";
                    att = node.Attributes["Size"];
                    leader.Size = Convert.ToDouble(att.Value);
                    att = node.Attributes["HeavyEndWeight"];
                    leader.HeavyEndWeight = Convert.ToInt32(att.Value);

                    Leaders.Add(leader);
                }

                //load textStyles
                TextStyles = new List<TextStyle>();
                foreach (XmlNode node in textStyles)
                {
                    //<TextStyle Name="test" WidthFactor="1" TabSize="12" TextBox="0" LineWeight="1" LeaderOffset="1" 
                    //Leader="ARROW (30°) FILLED" Color="255-000-000" Underline="0" Italic="0" Bold="1" Background="1" Description="Bold"/>
                    TextStyle textStyle = new TextStyle();
                    XmlAttribute att = node.Attributes["Name"];
                    textStyle.Name = att.Value;
                    att = node.Attributes["Description"];
                    textStyle.Description = att.Value;
                    att = node.Attributes["Background"];
                    textStyle.Background = att.Value == "1";
                    att = node.Attributes["Bold"];
                    textStyle.Bold = att.Value == "1";
                    att = node.Attributes["Italic"];
                    textStyle.Italic = att.Value == "1";
                    att = node.Attributes["Underline"];
                    textStyle.Underline = att.Value == "1";
                    att = node.Attributes["Color"];
                    string[] colorStrings = att.Value.Split('-');
                    if (colorStrings.Length != 3)
                    {
                        error = string.Format("Invalid color string found in xml: {0}.\n{1}", att.Value, pathToConfigXml);
                        return false;
                    }
                    Color color = Color.FromArgb(Convert.ToInt32(colorStrings[0]), Convert.ToInt32(colorStrings[1]), Convert.ToInt32(colorStrings[2]));
                    textStyle.Color = color;
                    att = node.Attributes["Leader"];
                    textStyle.Leader = LeaderStyle.getLeaderStyle(Leaders, att.Value);
                    att = node.Attributes["LeaderOffset"];
                    textStyle.LeaderOffset = Convert.ToDouble(att.Value);
                    att = node.Attributes["LineWeight"];
                    textStyle.LineWeight = Convert.ToInt32(att.Value);
                    att = node.Attributes["TextBox"];
                    textStyle.TextBox = att.Value == "1";
                    att = node.Attributes["TabSize"];
                    textStyle.TabSize = Convert.ToDouble(att.Value);
                    att = node.Attributes["WidthFactor"];
                    textStyle.WidthFactor = Convert.ToDouble(att.Value);

                    TextStyles.Add(textStyle);
                }

                error = "";
                return true;
            }
            else
            {
                error = string.Format("Config file could not be found at: {0}", pathToConfigXml);
                return false;
            }

        }

        private XmlDocument Serialise()
        {
            //create XmlDocument
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<KRSP_Annotation_Styles />");
            XmlNode root = xdoc.DocumentElement;

            //Create LeaderStyles node
            XmlNode node = xdoc.CreateElement("LeaderStyles");
            XmlComment comment = xdoc.CreateComment("Angle = Decimal Degrees, Size = Millimeters");
            node.AppendChild(comment);
            comment = xdoc.CreateComment("For TRUE/FALSE or ON/OFF parameters: 0 = FALSE/OFF, 1 = TRUE/ON");
            node.AppendChild(comment);
            comment = xdoc.CreateComment("ArrowStyles\nArrow:  8\nBox:  10\nDatum Triangle: 9\nDiagonal: 0\nDot:  3\nElevation Target: 11\nHeavy end tick mark:  7\nLoop: 12");
            node.AppendChild(comment);

            foreach (LeaderStyle leader in Leaders)
            {
                leader.AddToXmlDoc(ref node);
            }

            root.AppendChild(node);

            //Create TextStyles node
            node = xdoc.CreateElement("TextStyles");

            //Create Fonts node
            XmlNode subNode = xdoc.CreateElement("Fonts");
            comment = xdoc.CreateComment("Note that the font names listed must be Match the case of the font name as found in the Revit interface\ni.e. \"Arial\" is not the same as \"ARIAL\" or \"arial\"");
            subNode.AppendChild(comment);

            //for each font create a Font node
            XmlNode child = xdoc.CreateElement("Font");
            XmlAttribute xatt = xdoc.CreateAttribute("Value");
            xatt.Value = "Arial";
            child.Attributes.Append(xatt);
            subNode.AppendChild(child);

            //append the Fonts node to the TextStyles node
            node.AppendChild(subNode);

            //create Sizes node
            subNode = xdoc.CreateElement("Sizes");
            xatt = xdoc.CreateAttribute("Units");
            xatt.Value = "mm";
            subNode.Attributes.Append(xatt);
            comment = xdoc.CreateComment("Enter either \"mm\" or \"in\" for units");
            subNode.AppendChild(comment);
            comment = xdoc.CreateComment("All sizes must be in decimal format");
            subNode.AppendChild(comment);

            //for each size create a Size node
            child = xdoc.CreateElement("Size");
            xatt = xdoc.CreateAttribute("Value");
            xatt.Value = "2.5";
            child.Attributes.Append(xatt);
            subNode.AppendChild(child);

            //append the Sizes node to the TextStyles node
            node.AppendChild(subNode);

            //create styles node
            subNode = xdoc.CreateElement("Styles");
            foreach (TextStyle style in TextStyles)
            {
                style.AddToXmlDoc(ref subNode);
            }


            //append the Styles node to the TextStyles node
            node.AppendChild(subNode);

            //append the TextStyles node to the Document Element
            root.AppendChild(node);

            return xdoc;
        }

        private void btnDeseriliase_Click(object sender, EventArgs e)
        {
            string err_msg;
            if (Deserialise(@"C:\Temp\test.xml", out err_msg))
            {
                cmbSizes.Items.Add("<All Sizes>");
                foreach (double size in Sizes)
                {
                    cmbSizes.Items.Add(size.ToString("0.##"));
                }
                cmbSizes.SelectedIndex = 0;
                cmbStyles.Items.Add("<All Styles>");
                cmbStyles.Items.AddRange(TextStyles.ToArray());
                cmbStyles.SelectedIndex = 0;
                textBox1.Text = TextStyles[0].Description;
            }
            else
            {
                MessageBox.Show(err_msg);
            }


        }
    }
}
