using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CAD
{
    public class TextShape : BaseShape
    {
        public static  bool IsInRectangle(Point p1, Point p2, Point p3)//判断鼠标位置p3是否在线段p1、p2上（0.1范围内），如果是返回真，否则返回假
        {
            Rectangle rect = new Rectangle(p1.X, p2.Y, p2.X - p1.X, p2.Y - p1.Y);
            rect = new Rectangle(p1, new Size() { Width = p2.X - p1.X, Height = p2.Y - p1.Y });
            bool bIn= rect.Contains(p3);
            Console.WriteLine(bIn);
            return bIn;
        }
        public override bool catchShape(Point testPoint)
        {
            return IsInRectangle(this.getP1(), this.getP2(), testPoint);
        }

        public override BaseShape copySelf()
        {
            TextShape copyTextShape = new TextShape();
            copyTextShape.setP1(this.getP1());//复制起点
            copyTextShape.setP2(this.getP2());//复制终点
            copyTextShape.penColor = this.penColor;
            copyTextShape.penwidth = this.penwidth;
            return copyTextShape;
        }

        public override void draw(Graphics g)
        {
           
        }

        public override Point[] getAllHitPoint()
        {
            Point[] allHitPoint = new Point[2];
            allHitPoint[0] = this.getP1();
            allHitPoint[1] = this.getP2();
            return allHitPoint;
        }

        public override void setHitPoint(int hitPointIndex, Point newPoint)
        {
            switch (hitPointIndex)
            {
                case 0:
                    {
                        Point tempPoint;//0索引相对的坐标
                        tempPoint = new Point();
                        tempPoint.X = this.getP1().X + newPoint.X;//加上X坐标的增量
                        tempPoint.Y = this.getP1().Y + newPoint.Y;//加上Y坐标的增量
                        this.setP1(tempPoint);
                        tempPoint = new Point();
                        tempPoint.X = this.getP2().X + newPoint.X;
                        tempPoint.Y = this.getP2().Y + newPoint.Y;
                        this.setP2(tempPoint);
                        break;
                    }
                case 1:
                    {
                        this.setP1(newPoint);//设置P1的热点
                        break;
                    }
                case 2:
                    {
                        this.setP2(newPoint);//设置P2的热点
                        break;
                    }
            }
        }
    }
}
