#include "../pch.h"
#include "PriorityQueueDLL.h"
#include <stdio.h>
#include <limits>
#include <memory>

// 명시적 인스턴스화 정의
template class PriorityQueueDLL<int>;
template class PriorityQueueDLL<double>;
template class PriorityQueueDLL<float>;
template class PriorityQueueDLL<IntInt>;
template class PriorityQueueDLL<FloatInt>;
template class PriorityQueueDLL<IntFloatChar32>;

extern "C" {
    EXPORT PriorityQueueDLL<int>* MakeIntInstance()
    {
        printf("Int Instance Create\n");
        return new PriorityQueueDLL<int>();
    }

    EXPORT void DeleteIntInstance(PriorityQueueDLL<int>* priorityQueue)
    {
        if (priorityQueue == nullptr)
            return;

        printf("Int Instance Delete\n");
        delete priorityQueue;
    }

    EXPORT PriorityQueueDLL<double>* MakeDoubleInstance()
    {
        printf("Double Instance Create\n");
        return new PriorityQueueDLL<double>();
    }

    EXPORT void DeleteDoubleInstance(PriorityQueueDLL<double>* priorityQueue)
    {
        if (priorityQueue == nullptr)
            return;

        printf("Double Instance Delete\n");
        delete priorityQueue;
    }

    EXPORT PriorityQueueDLL<float>* MakeFloatInstance()
    {
        printf("Float Instance Create\n");
        return new PriorityQueueDLL<float>();
    }

    EXPORT void DeleteFloatInstance(PriorityQueueDLL<float>* priorityQueue)
    {
        if (priorityQueue == nullptr)
            return;

        printf("Float Instance Delete\n");
        delete priorityQueue;
    }

    EXPORT PriorityQueueDLL<IntInt>* MakeIntIntInstance()
    {
        printf("Int Instance Create\n");
        return new PriorityQueueDLL<IntInt>();
    }

    EXPORT void DeleteIntIntInstance(PriorityQueueDLL<IntInt>* priorityQueue)
    {
        if (priorityQueue == nullptr)
            return;

        printf("Int Instance Delete\n");
        delete priorityQueue;
    }
    EXPORT PriorityQueueDLL<FloatInt>* MakeFloatIntInstance()
    {
        printf("Int Instance Create\n");
        return new PriorityQueueDLL<FloatInt>();
    }

    EXPORT void DeleteFloatIntInstance(PriorityQueueDLL<FloatInt>* priorityQueue)
    {
        if (priorityQueue == nullptr)
            return;

        printf("Int Instance Delete\n");
        delete priorityQueue;
    }

    EXPORT PriorityQueueDLL<IntFloatChar32>* MakeIntFloatChar32Instance()
    {
        printf("Int Instance Create\n");
        return new PriorityQueueDLL<IntFloatChar32>();
    }

    EXPORT void DeleteIntFloatChar32Instance(PriorityQueueDLL<IntFloatChar32>* priorityQueue)
    {
        if (priorityQueue == nullptr)
            return;

        printf("Int Instance Delete\n");
        delete priorityQueue;
    }

    EXPORT PriorityQueueDLL<IntFloatChar32&>* MakeIntFloatChar32PointerInstance()
    {
        printf("Int Instance Create\n");
        return new PriorityQueueDLL<IntFloatChar32&>();
    }

    EXPORT void DeleteIntFloatChar32PointerInstance(PriorityQueueDLL<IntFloatChar32&>* priorityQueue)
    {
        if (priorityQueue == nullptr)
            return;

        printf("Int Instance Delete\n");
        delete priorityQueue;
    }
}

template<typename T>
PriorityQueueDLL<T>::PriorityQueueDLL()
{
    priorityQueue = new std::priority_queue<T>();
}

template<typename T>
PriorityQueueDLL<T>::~PriorityQueueDLL()
{
    if (priorityQueue == nullptr)
        return;

    delete priorityQueue;
}

template<typename T>
bool PriorityQueueDLL<T>::empty() const
{
    return priorityQueue->empty();
}

template<typename T>
unsigned int PriorityQueueDLL<T>::size() const
{
    return priorityQueue->size();
}

template<typename T>
const T PriorityQueueDLL<T>::top() const
{
    if (priorityQueue->empty())
        return T{};

    return priorityQueue->top();
}

template<typename T>
void PriorityQueueDLL<T>::push(T addData)
{
    return priorityQueue->push(addData);
}

template<typename T>
void PriorityQueueDLL<T>::pop() const
{
    if (priorityQueue->empty())
        return;

    priorityQueue->pop();
}