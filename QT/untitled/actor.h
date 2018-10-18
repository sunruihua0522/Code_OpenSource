#ifndef ACTOR_H
#define ACTOR_H
#include "iperson.h"

class Actor : public IPerson
{
protected:
    ~Actor();
public:
    Actor();
    void Eat();
    void Run();
};

#endif // ACTOR_H
