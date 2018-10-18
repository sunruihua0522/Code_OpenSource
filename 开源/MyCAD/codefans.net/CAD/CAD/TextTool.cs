using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CAD
{
    public class TextTool : BaseTool
    {
        public override void mouseDown(object sender, MouseEventArgs e, CADFrame objC)
        {
            this.setOperShape(new TextShape());
            this.getOperShape().setP1(this.getDownPoint());
            this.getOperShape().penColor = objC.clr;
            this.getOperShape().penwidth = objC.lineWidth;
            this.getRefCADPanel().getCurrentShapes().Add(this.getOperShape());//
        }

        public override void mouseDrag(object sender, MouseEventArgs e)
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

        public override void set()
        {
            
        }

        public override void unSet()
        {
            
        }
    }
}
