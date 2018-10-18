using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CAD
{
    public partial class LineTool : BaseTool
    {
        public override void mouseDown(object sender, MouseEventArgs e,CADFrame objC)//重写线的鼠标按下
        {
            this.setOperShape(new LineShape());
            this.getOperShape().setP1(this.getDownPoint());
            this.getOperShape().penColor = objC.clr;
            this.getOperShape().penwidth = objC.lineWidth;
            this.getRefCADPanel().getCurrentShapes().Add(this.getOperShape());//
        }

        public override void mouseDrag(object sender, MouseEventArgs e)//重写线的鼠标拖动
        {
            this.getOperShape().setP2(this.getNewDragPoint());
            this.getRefCADPanel().Refresh();
        }

        public override void mouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void mouseUp(object sender, MouseEventArgs e)
        {
        }

        public override void unSet()
        {
        }

        public override void set()
        {
        }
    }
}
