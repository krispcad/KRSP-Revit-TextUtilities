using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Runtime.Serialization;

namespace KRSP.Revit.TextCreateStyles
{
    public class LeaderStyle
    {
        public enum ArrowStyle : int
        {
            Diagonal = 0,
            Dot = 3,
            HeavyEndTickMark = 7,
            Arrow = 8,
            DatumTriangle = 9,
            Box = 10,
            ElevationTarget = 11,
            Loop = 12
        }
        //<LeaderStyle Name="ARROW (30°) FILLED"
        //         ArrowStyle="8"
        //         Angle="30"
        //         Filled="1"
        //         Size="3"
        //         HeavyEndWeight="5" />
        public string Name { get; set; }
        public ArrowStyle Style { get; set; }
        public int Angle { get; set; }
        public bool Filled { get; set; }
        public double Size { get; set; }
        public int HeavyEndWeight { get; set; }

        public static LeaderStyle getLeaderStyle(List<LeaderStyle> leaders, string styleName)
        {
            foreach (LeaderStyle leader in leaders)
            {
                if (leader.Name == styleName)
                {
                    return leader;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return Name;
        }

        public void AddToXmlDoc(ref XmlNode parentNode)
        {
            XmlDocument xdoc = parentNode.OwnerDocument;
            XmlElement newNode = xdoc.CreateElement("LeaderStyle");

            XmlAttribute newAtt = xdoc.CreateAttribute("Name");
            newAtt.Value = Name;
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("ArrowStyle");
            newAtt.Value = ((int)Style).ToString();
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Angle");
            newAtt.Value = Angle.ToString();
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Filled");
            newAtt.Value = Filled ? "1" : "0";
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Size");
            newAtt.Value = Size.ToString();
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("HeavyEndWeight");
            newAtt.Value = HeavyEndWeight.ToString();
            newNode.Attributes.Append(newAtt);

            parentNode.AppendChild(newNode);
        }
    }

    public class TextStyle
    {
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
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Background { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
        public Color Color { get; set; }
        public LeaderStyle Leader { get; set; }
        public double LeaderOffset { get; set; }
        public int LineWeight { get; set; }
        public bool TextBox { get; set; }
        public double TabSize { get; set; }
        public double WidthFactor { get; set; }

        public override string ToString()
        {
            return Description;
        }

        public void AddToXmlDoc(ref XmlNode parentNode)
        {
            XmlDocument xdoc = parentNode.OwnerDocument;
            XmlElement newNode = xdoc.CreateElement("TextStyle");

            XmlAttribute newAtt = xdoc.CreateAttribute("Name");
            newAtt.Value = Name;
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Description");
            newAtt.Value = Description;
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Background");
            newAtt.Value = Background ? "1" : "0";
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Bold");
            newAtt.Value = Bold ? "1" : "0";
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Italic");
            newAtt.Value = Italic ? "1" : "0";
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Underline");
            newAtt.Value = Underline ? "1" : "0";
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Color");
            newAtt.Value = string.Format("{0}-{1}-{2}", Color.R.ToString("D3"), Color.G.ToString("D3"), Color.B.ToString("D3"));
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("Leader");
            newAtt.Value = Leader.Name;
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("LeaderOffset");
            newAtt.Value = LeaderOffset.ToString();
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("LineWeight");
            newAtt.Value = LineWeight.ToString();
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("TextBox");
            newAtt.Value = TextBox ? "1" : "0";
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("TabSize");
            newAtt.Value = TabSize.ToString();
            newNode.Attributes.Append(newAtt);

            newAtt = xdoc.CreateAttribute("WidthFactor");
            newAtt.Value = WidthFactor.ToString();
            newNode.Attributes.Append(newAtt);



            parentNode.AppendChild(newNode);
        }
    }
}
