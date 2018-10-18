#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QDebug>

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);

    person=new Actor();
    person->Eat();
    person->Run();
    InitConnection();
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::InitConnection()
{
   QObject::connect(ui->Btn_ClickTest,&QPushButton::clicked, this, &MainWindow::S_MyCLick);
   QObject::connect(ui->Btn_SendText,&QPushButton::clicked,this,&MainWindow::S_MyCLick);
}

void MainWindow::MySlot(QString msg)
{
    qDebug()<<msg;
}
void MainWindow::S_MyCLick()
{

    qDebug()<<"Click";
}
