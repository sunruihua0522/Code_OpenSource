using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CAD
{
    [Serializable]
    class EllipseShape : BaseShape
    {
        public static bool IsInEllipse(Point p1, Point p2, Point p3)//判断鼠标位置p3是否在椭圆上（0.1范围内），如果是返回真，否则返回假
        {
            Point pa1 = new Point();//左焦点
            Point pa2 = new Point();//右焦点
            double iLen2 = 0;
            if (Math.Abs(p1.X - p2.X) > Math.Abs(p1.Y - p2.Y))//长大于高
            {
                double iLen1 = Math.Pow(Math.Abs(Math.Pow(p2.X - p1.X, 2) - Math.Pow(p2.Y - p1.Y, 2)), 0.5) / 2;//焦距的长度
                iLen2 = Math.Abs(p2.X - p1.X);//距离两个焦点的固定距离和
                if (p2.X > p1.X&&p2.Y>p1.Y)//左上到右下
                { 
                    pa1.X = (int)(p1.X + iLen2 / 2 - iLen1);
                    pa1.Y = (int)(p1.Y + Math.Abs(p2.Y - p1.Y) / 2);
                    pa2.X = (int)(p1.X + iLen2 / 2 + iLen1);
                    pa2.Y = (int)(p1.Y + Math.Abs(p2.Y - p1.Y) / 2);
                }
                if(p2.X<p1.X&&p2.Y>p1.Y)//右上到左下
                {
                    pa1.X = (int)(p2.X + iLen2 / 2 - iLen1);
                    pa1.Y = (int)(p1.Y + Math.Abs(p2.Y - p1.Y) / 2);
                    pa2.X = (int)(p2.X + iLen2 / 2 + iLen1);
                    pa2.Y = (int)(p1.Y + Math.Abs(p2.Y - p1.Y) / 2);
                }
                if (p2.X < p1.X && p2.Y < p1.Y)//右下到左上
                {
                    pa1.X = (int)(p2.X + iLen2 / 2 - iLen1);
                    pa1.Y = (int)(p2.Y + Math.Abs(p1.Y - p2.Y) / 2);
                    pa2.X = (int)(p2.X + iLen2 / 2 + iLen1);
                    pa2.Y = (int)(p2.Y + Math.Abs(p1.Y - p2.Y) / 2);
                }
                if (p1.X < p2.X && p1.Y > p2.Y)//左下到右上
                {
                    pa1.X = (int)(p1.X + iLen2 / 2 - iLen1);
                    pa1.Y = (int)(p2.Y + Math.Abs(p1.Y - p2.Y) / 2);
                    pa2.X = (int)(p1.X + iLen2 / 2 + iLen1);
                    pa2.Y = (int)(p2.Y + Math.Abs(p1.Y - p2.Y) / 2);
                }
            }
            else
            {
                double iLen1 = Math.Pow(Math.Abs(Math.Pow(p2.X - p1.X, 2) - Math.Pow(p2.Y - p1.Y, 2)), 0.5) / 2;//焦距的长度
                iLen2 = Math.Abs(p2.Y - p1.Y);//距离两个焦点的固定距离和
                if (p2.X > p1.X && p2.Y > p1.Y)//左上到右下
                {
                    pa1.X = (int)(p1.X + Math.Abs(p2.X - p1.X) / 2);
                    pa1.Y = (int)(p1.Y + Math.Abs(p2.Y - p1.Y) / 2 - iLen1);
                    pa2.X = (int)(p1.X + Math.Abs(p2.X - p1.X) / 2);
                    pa2.Y = (int)(p1.Y + Math.Abs(p2.Y - p1.Y) / 2 + iLen1);
                }
                if (p2.X < p1.X && p2.Y < p1.Y)//右下到左上
                {
                    pa1.X = (int)(p2.X + Math.Abs(p2.X - p1.X) / 2);
                    pa1.Y = (int)(p2.Y + Math.Abs(p2.Y - p1.Y) / 2 - iLen1);
                    pa2.X = (int)(p2.X + Math.Abs(p2.X - p1.X) / 2);
                    pa2.Y = (int)(p2.Y + Math.Abs(p2.Y - p1.Y) / 2 + iLen1);
                }
                if (p2.X > p1.X && p2.Y < p1.Y)//左下到右上
                {
                    pa1.X = (int)(p1.X + Math.Abs(p2.X - p1.X) / 2);
                    pa1.Y = (int)(p2.Y + Math.Abs(p2.Y - p1.Y) / 2 - iLen1);
                    pa2.X = (int)(p1.X + Math.Abs(p2.X - p1.X) / 2);
                    pa2.Y = (int)(p2.Y + Math.Abs(p2.Y - p1.Y) / 2 + iLen1);
                }
                if (p2.X < p1.X && p2.Y > p1.Y)//右上到左下
                {
                    pa1.X = (int)(p2.X + Math.Abs(p2.X - p1.X) / 2);
                    pa1.Y = (int)(p1.Y + Math.Abs(p2.Y - p1.Y) / 2 - iLen1);
                    pa2.X = (int)(p2.X + Math.Abs(p2.X - p1.X) / 2);
                    pa2.Y = (int)(p1.Y + Math.Abs(p2.Y - p1.Y) / 2 + iLen1);
                }
            }
            double iLen3 = Math.Pow(Math.Pow(p3.X - pa1.X, 2) + Math.Pow(p3.Y - pa1.Y, 2), 0.5) + Math.Pow(Math.Pow(p3.X - pa2.X, 2) + Math.Pow(p3.Y - pa2.Y, 2), 0.5);
            //p3点距离两个焦点的距离和
            if (Math.Abs(iLen3-iLen2) < 5)
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
            return IsInEllipse(this.getP1(), this.getP2(), testPoint);
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
            EllipseShape copyEllipseShape = new EllipseShape();
            copyEllipseShape.setP1(this.getP1());//复制起点
            copyEllipseShape.setP2(this.getP2());//复制终点
            copyEllipseShape.penColor = this.penColor;
            copyEllipseShape.penwidth = this.penwidth;
            return copyEllipseShape;
        }
    }
}
