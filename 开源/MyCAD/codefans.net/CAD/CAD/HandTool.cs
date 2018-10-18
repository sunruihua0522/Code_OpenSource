using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace CAD
{
    public partial class HandTool : BaseTool
    {
        public int catchPointIndex = -1;//捕捉热点的索引

        public override void mouseDown(object sender, MouseEventArgs e,CADFrame objC)//重写鼠标的按下
        {
            catchPointIndex = -1;//重置捕捉热点的索引
            if (this.getOperShape() != null) this.getOperShape().setUnSelected();//清除前操作对象中选中的状态
            ArrayList allShapes = this.getRefCADPanel().getCurrentShapes();//得到画板上的所有图形
            int catchPoint = -1;
            int i = 0;
            for (; i < allShapes.Count; i++)//对每个图形进行捕捉测试
            {
                catchPoint = ((BaseShape)allShapes[i]).catchShapPoint(this.getNewMovePoint());//捕捉集合中的一个图形
                if (catchPoint > -1) break;//捕获到后，跳出循环
            }
            if (catchPoint > -1)
            {
                catchPointIndex = catchPoint;//捕获到后，将临时的热点设置到工具属性中
                ((BaseShape)allShapes[i]).setSelected();//设置捕捉到的图形为选中状态
                this.setOperShape(((BaseShape)allShapes[i]));//把选中的图形设定到本类的操作图形的状态中
            }
            this.getRefCADPanel().Refresh();//刷新画板
        }
        public override void mouseDrag(object sender, MouseEventArgs e)//重写鼠标的拖动事件
        {
            if (this.getOperShape() != null)//当有选中的图形时
            {
                Point setPoint = this.getNewDragPoint();
                if (catchPointIndex == 0)//如果捕捉到移动点时
                {
                    setPoint = new Point();
                    setPoint.X = this.getNewDragPoint().X - this.getOldDragPoint().X;//计算增量点
                    setPoint.Y = this.getNewDragPoint().Y - this.getOldDragPoint().Y;//计算增量点
                }
                this.getOperShape().setHitPoint(catchPointIndex, setPoint);//设置热点
                this.getRefCADPanel().Refresh();//刷新画板
            }
        }

        public BaseShape oldMoveShap = null;//移动处理的图形

        public override void mouseMove(object sender, MouseEventArgs e)//重写鼠标的移动
        {
            if (oldMoveShap != null) oldMoveShap.setUnSelected();//清除移动图形选中的状态
            ArrayList allShapes = this.getRefCADPanel().getCurrentShapes();//得到画板上的图形集合
            int catchPoint = -1;//临时处理的捕捉热点
            int i = 0;
            for (; i < allShapes.Count; i++)//对每个图形捕捉测试
            {
                catchPoint = ((BaseShape)allShapes[i]).catchShapPoint(this.getNewMovePoint());
                if (catchPoint > -1) break;//捕捉到跳出循环
            }
            if (catchPoint > -1)//捕捉到后
            {
                ((BaseShape)allShapes[i]).setSelected();//设定捕捉到的图形为选中状态
                oldMoveShap = (BaseShape)allShapes[i];//将选中的图形设定到本类的操作图形的状态中去
            }
            this.getRefCADPanel().Refresh();//刷新画板
        }

        public override void mouseUp(object sender, MouseEventArgs e)//重写鼠标的释放
        {
            this.getRefCADPanel().Refresh();//刷新画板
        }

        public override void unSet()//工具的卸载
        {
            ArrayList allShapes = this.getRefCADPanel().getCurrentShapes();//得到画板上的图形集合
            for (int i = 0; i < allShapes.Count; i++)//清除所有图形的选中状态
            {
                ((BaseShape)allShapes[i]).setUnSelected();
            }
            this.getRefCADPanel().Refresh();
        }

        public override void set()
        {
            
        }
    }
}
