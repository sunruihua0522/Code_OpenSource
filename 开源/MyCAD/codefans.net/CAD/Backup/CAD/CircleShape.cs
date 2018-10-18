using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CAD
{
    [Serializable]
    class CircleShape : BaseShape
    {
        public static bool IsInCircle(Point p1, Point p2, Point p3)//判断鼠标位置p3是否在线段p1、p2上（0.1范围内），如果是返回真，否则返回假
        {
            int r = (int)Math.Pow(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2), 0.5);
            Point p4 = new Point();//矩形区域的左上点
            p4.X = p1.X - r;
            if (Math.Abs(Math.Pow(Math.Pow(p1.X-p3.X,2)+Math.Pow(p1.Y-p3.Y,2),0.5)-r)< 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool catchShape(Point testPoint)//重写图形的捕捉。如果鼠标点testPoint在图形周围，返回真，否则返回假
        {
            return IsInCircle(this.getP1(), this.getP2(), testPoint);
        }

        public override void draw(Graphics g)//重写画图
        {
            g.DrawLine(new Pen(Color.Black, 1), this.getP1(), this.getP2());
        }

        public override Point[] getAllHitPoint()//返回所有热点
        {
            Point[] allHitPoint = new Point[2];
            allHitPoint[0] = this.getP1();
            allHitPoint[1] = this.getP2();
            return allHitPoint;
        }

        public override void setHitPoint(int hitPointIndex, Point newPoint)//重写设置热点的方法
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

        public override BaseShape copySelf()//重写身复制方法
        {
            CircleShape copyCircleShape = new CircleShape();
            copyCircleShape.setP1(this.getP1());//复制起点
            copyCircleShape.setP2(this.getP2());//复制终点
            copyCircleShape.penColor = this.penColor;
            copyCircleShape.penwidth = this.penwidth;
            return copyCircleShape;
        }
    }
}
