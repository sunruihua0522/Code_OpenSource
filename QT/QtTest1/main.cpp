#include "mainwindow.h"
#include <QApplication>
#include <QLabel>
#include <QVBoxLayout>
int main(int argc, char *argv[])
{
    QApplication a(argc, argv);


    //窗口显示
    MainWindow w;
    QVBoxLayout* layout=new QVBoxLayout(&w);

    QPushButton *pButton=new QPushButton("one",&w);

    layout->addWidget(pButton);
    //layout->addWidget(new QPushButton("Two",&w));
    //layout->addWidget(new QPushButton("Three",&w));
    //layout->addWidget(new QPushButton("Four",&w));
    pButton->setText("sfsfsfsfsfsfsfsfsfsf");
    pButton->connect(pButton,&QPushButton::clicked,&w,&MainWindow::close);
    w.setLayout(layout);
    w.show();

    return a.exec();
}
