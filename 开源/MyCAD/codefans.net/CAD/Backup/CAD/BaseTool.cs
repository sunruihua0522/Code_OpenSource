using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CAD
{
    public abstract class BaseTool
    {
        private CADFrame refCADPanel = null;//关联画板

        private Point upPoint = new Point();//鼠标弹起点
        private Point downPoint = new Point();//鼠标按下点
        private Point newMovePoint = new Point();//新的鼠标移动点
        private Point oldMovePoint = new Point();//老的鼠标移动点
        private Point newDragPoint = new Point();//新的鼠标拖动点
        private Point oldDragPoint = new Point();//老的鼠标拖动点

        private BaseShape operShape = null;//操作图形

        public Point getDownPoint()
        {
            return downPoint;
        }
        public void setDownPoint(Point downPoint)
        {
            this.downPoint = downPoint;
        }
        public Point getNewDragPoint()
        {
            return newDragPoint;
        }
        public void setNewDragPoint(Point newDragPoint)
        {
            this.newDragPoint = newDragPoint;
        }
        public Point getNewMovePoint()
        {
            return newMovePoint;
        }
        public void setNewMovePoint(Point newMovePoint)
        {
            this.newMovePoint = newMovePoint;
        }
        public Point getOldDragPoint()
        {
            return oldDragPoint;
        }
        public void setOldDragPoint(Point oldDragPoint)
        {
            this.oldDragPoint = oldDragPoint;
        }
        public Point getOldMovePoint()
        {
            return oldMovePoint;
        }
        public void setOldMovePoint(Point oldMovePoint)
        {
            this.oldMovePoint = oldMovePoint;
        }
        public Point getUpPoint()
        {
            return upPoint;
        }
        public void setUpPoint(Point upPoint)
        {
            this.upPoint = upPoint;
        }
        public CADFrame getRefCADPanel()
        {
            return refCADPanel;
        }
        public void setRefCADPanel(CADFrame refCADPanel)
        {
            this.refCADPanel = refCADPanel;
        }
        public BaseShape getOperShape()
        {
            return operShape;
        }
        public void setOperShape(BaseShape operShape)
        {
            this.operShape = operShape;
        }

        public abstract void mouseUp(object sender, MouseEventArgs e);//鼠标弹起的处理
        public abstract void mouseDown(object sender, MouseEventArgs e,CADFrame objC);//鼠标按下的处理
        public abstract void mouseMove(object sender, MouseEventArgs e);//鼠标移动的处理
        public abstract void mouseDrag(object sender, MouseEventArgs e);//鼠标拖动的处理

        public void superMouseUp(object sender, MouseEventArgs e)//鼠标释放
        {
            this.setUpPoint(new Point(e.X, e.Y));//鼠标的弹起点的设定
            this.mouseUp(sender, e);//鼠标的弹起的设定
            this.setUpPoint(new Point());//鼠标弹起点的设定
            this.setDownPoint(new Point());//鼠标按下点的设定
            this.setOldMovePoint(new Point());//老的鼠标移动点的设定
            this.setNewMovePoint(new Point());//新的鼠标移动点的设定
            this.setOldDragPoint(new Point());//老的鼠标拖动点的设定
            this.setNewDragPoint(new Point());//新的鼠标拖动点的设定
            this.getRefCADPanel().record();//保存
        }

        public void superMouseDown(object sender, MouseEventArgs e,CADFrame objCAD)//鼠标按下
        {
            this.setUpPoint(new Point(e.X, e.Y));//鼠标的弹起点的设定
            this.setDownPoint(new Point(e.X, e.Y));//鼠标按下点的设定
            this.setOldMovePoint(new Point(e.X, e.Y));//老的鼠标移动点的设定
            this.setNewMovePoint(new Point(e.X, e.Y));//新的鼠标移动点的设定
            this.setOldDragPoint(new Point(e.X, e.Y));//老的鼠标拖动点的设定
            this.setNewDragPoint(new Point(e.X, e.Y));//新的鼠标拖动点的设定
            this.mouseDown(sender, e,objCAD);//鼠标按下的处理
        }

        public void superMouseMove(object sender, MouseEventArgs e)//鼠标移动
        {
            this.setNewMovePoint(new Point(e.X, e.Y));//新的鼠标移动点的设定
            this.mouseMove(sender, e);//鼠标移动
            this.setOldMovePoint(this.getNewMovePoint());//老的鼠标移动点的设定
        }

        public void superMouseDrag(object sender, MouseEventArgs e)//鼠标拖动
        {
            this.setNewDragPoint(new Point(e.X, e.Y));//新的鼠标拖动点的设定
            this.mouseDrag(sender, e);//鼠标拖动
            this.setOldDragPoint(this.getNewDragPoint());//老的鼠标拖动点的设定
        }

        public abstract void set();//装载
        public abstract void unSet();//卸载
    }
}
