using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CAD
{
    [Serializable]
    public abstract class BaseShape
    {
        private bool isSelected = false;//标识图形是否被选中
        
        private Point p1;//第一个点
        private Point p2;//第二个点

        public  Color penColor;
        public  int penwidth ;

        public void setSelected()//设置为选中状态
        {
            this.isSelected = true;
        }
        public void setUnSelected()//设置为非选中状态
        {
            this.isSelected = false;
        }
        public Point getP1()
        {
            return p1;
        }
        public void setP1(Point p1)
        {
            this.p1 = p1;
        }
        public Point getP2()
        {
            return p2;
        }
        public void setP2(Point p2)
        {
            this.p2 = p2;
        }

        public abstract void draw(Graphics g);//画图形

        public abstract Point[] getAllHitPoint();//得到所有图形
        public abstract void setHitPoint(int hitPointIndex, Point newPoint);//设定热点
        public abstract BaseShape copySelf();//复制


        public bool catchHitPoint(Point hitPoint, Point testPoint)//测试热点捕捉
        {
            return this.getHitPointRectangle(hitPoint).Contains(testPoint);
        }

        public int catchShapPoint(Point testPoint)//捕捉图形
        {
            int hitPointIndex = -1;
            Point[] allHitPoint = this.getAllHitPoint();//的到所有的热点
            for (int i = 0; i < allHitPoint.Length; i++)//循环捕捉判断
            {
                if (this.catchHitPoint(allHitPoint[i], testPoint))
                {
                    return i + 1;//如果捕捉到了热点，返回热点的索引
                }
            }
            if(this.catchShape(testPoint)) return 0;//没有捕捉到热点，捕捉到了图形，返回特别热点
            return hitPointIndex;//返回捕捉到的人点
            }
        public void drawHitPoint(Point hitPoint, Graphics g)//画热点
        {
            g.DrawRectangle(new Pen(Color.Red,1), this.getHitPointRectangle(hitPoint));
        }

        public void drawAllHitPoint(Graphics g)//画所有热点
        {
            Point[] allHitPoint=this.getAllHitPoint();
            for(int i=0;i<2;i++)
            {
                this.drawHitPoint(allHitPoint[i],g);
            }
        }

        public Rectangle getHitPointRectangle(Point hitPoint)//得到热点矩形，以热点为中心高宽5像素的矩形
        {
            Rectangle rect=new Rectangle();
            rect.X=hitPoint.X-2;
            rect.Y=hitPoint.Y-2;
            rect.Width=5;
            rect.Height=5;
            return rect;
        }

        public abstract bool catchShape(Point testPoint);//图形捕捉

        public void superDraw(Graphics g)//公共画法
        {
            if(this.isSelected) this.drawAllHitPoint(g);
        }

        public static Pen getPen(CADFrame objCAD)//得到画笔
        {
            return new Pen(objCAD.clr,objCAD.lineWidth);
        }
    }
}