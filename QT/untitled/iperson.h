#pragma once
class IPerson
{
protected:
    ~IPerson();
public:
    virtual void Eat()=0;
    virtual void Run()=0;
};
