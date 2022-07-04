using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace labelAndTrain
{
    public class Label
    {
        public string imagePath { get; set; }
        public List<LabelRegion> labelRegions = new List<LabelRegion>();
        public Label(Label l)
        {
            this.imagePath = l.imagePath;
            this.labelRegions = l.labelRegions;
        }

        public Label()
        {

        }

        public string ToText(int index)
        {
            string txt = labelRegions[index].rect.X.ToString();
            txt += ", " + labelRegions[index].rect.Y.ToString();
            txt += ", " + labelRegions[index].rect.Width.ToString();
            txt += ", " + labelRegions[index].rect.Height.ToString();
            return txt;
        }
    }

    public class LabelRegion
    {
        public LabelRect rect = new LabelRect();

        public Rectangle imageCoord { get; set; }

        public int classId { get; set; }

        public LabelRegion(LabelRect r, int classCode, Rectangle rc)
        {
            this.rect = r;
            this.classId = classCode;
            this.imageCoord = rc;
        }

        public LabelRegion(LabelRegion lr)
        {
            this.classId = lr.classId;
            this.rect = lr.rect;
            this.imageCoord = lr.imageCoord;
        }

        public LabelRegion()
        {

        }
    }

    public class LabelRect
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }


        public LabelRect()
        {

        }

    }
}
