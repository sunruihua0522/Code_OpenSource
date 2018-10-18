#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "teacher.h"
#include "actor.h"

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

private:
    Ui::MainWindow *ui;
    IPerson* person=nullptr;
    void InitConnection();
public  slots:
    void MySlot(QString strMsg);
    void S_MyCLick();

};

#endif // MAINWINDOW_H
