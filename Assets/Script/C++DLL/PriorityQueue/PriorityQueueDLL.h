#pragma once

#ifdef _WINDLL
#define EXPORT __declspec(dllexport)
#else
#define EXPORT __declspec(dllimport)
#endif

#include "../ReturnType.h"

#include <queue>

template <typename T>
class PriorityQueueDLL
{
public:
    PriorityQueueDLL();
    ~PriorityQueueDLL();

    bool empty() const;
    unsigned int size() const;
    const T top() const;
    void push(T addData);
    void pop() const;

    std::priority_queue<T>* GetQueue() { return priorityQueue; }

private:
    std::priority_queue<T>* priorityQueue;
};

// 명시적 인스턴스화 선언
extern template class EXPORT PriorityQueueDLL<int>;
extern template class EXPORT PriorityQueueDLL<double>;
extern template class EXPORT PriorityQueueDLL<float>;
extern template class EXPORT PriorityQueueDLL<IntInt>;
extern template class EXPORT PriorityQueueDLL<FloatInt>;
extern template class EXPORT PriorityQueueDLL<IntFloatChar32>;

extern "C"
{
    EXPORT PriorityQueueDLL<int>* MakeIntInstance();
    EXPORT void DeleteIntInstance(PriorityQueueDLL<int>* priorityQueue);

    EXPORT PriorityQueueDLL<double>* MakeDoubleInstance();
    EXPORT void DeleteDoubleInstance(PriorityQueueDLL<double>* priorityQueue);

    EXPORT PriorityQueueDLL<float>* MakeFloatInstance();
    EXPORT void DeleteFloatInstance(PriorityQueueDLL<float>* priorityQueue);

    EXPORT PriorityQueueDLL<IntInt>* MakeIntIntInstance();
    EXPORT void DeleteIntIntInstance(PriorityQueueDLL<IntInt>* priorityQueue);

    EXPORT PriorityQueueDLL<FloatInt>* MakeFloatIntInstance();
    EXPORT void DeleteFloatIntInstance(PriorityQueueDLL<FloatInt>* priorityQueue);

    EXPORT PriorityQueueDLL<IntFloatChar32>* MakeIntFloatChar32Instance();
    EXPORT void DeleteIntFloatChar32Instance(PriorityQueueDLL<IntFloatChar32>* priorityQueue);

    EXPORT PriorityQueueDLL<IntFloatChar32&>* MakeIntFloatChar32PointerInstance();
    EXPORT void DeleteIntFloatChar32PointerInstance(PriorityQueueDLL<IntFloatChar32&>* priorityQueue);

    EXPORT bool EmptyInt(PriorityQueueDLL<int>* instance) { return instance->empty(); }
    EXPORT bool EmptyFloat(PriorityQueueDLL<float>* instance) { return instance->empty(); }
    EXPORT bool EmptyDouble(PriorityQueueDLL<double>* instance) { return instance->empty(); }
    EXPORT bool EmptyIntInt(PriorityQueueDLL<IntInt>* instance) { return instance->empty(); }
    EXPORT bool EmptyFloatInt(PriorityQueueDLL<FloatInt>* instance) { return instance->empty(); }
    EXPORT bool EmptyIntFloatChar32(PriorityQueueDLL<IntFloatChar32>* instance) { return instance->empty(); }

    EXPORT unsigned int SizeInt(PriorityQueueDLL<int>* instance) { return instance->size(); }
    EXPORT unsigned int SizeFloat(PriorityQueueDLL<float>* instance) { return instance->size(); }
    EXPORT unsigned int SizeDouble(PriorityQueueDLL<double>* instance) { return instance->size(); }
    EXPORT unsigned int SizeIntInt(PriorityQueueDLL<IntInt>* instance) { return instance->size(); }
    EXPORT unsigned int SizeFloatInt(PriorityQueueDLL<FloatInt>* instance) { return instance->size(); }
    EXPORT unsigned int SizeIntFloatChar32(PriorityQueueDLL<IntFloatChar32>* instance) { return instance->size(); }

    EXPORT int TopInt(PriorityQueueDLL<int>* instance) { return instance->top(); }
    EXPORT float TopFloat(PriorityQueueDLL<float>* instance) { return instance->top(); }
    EXPORT double TopDouble(PriorityQueueDLL<double>* instance) { return instance->top(); }

    EXPORT int64_t TopIntInt(PriorityQueueDLL<IntInt>* instance)
    { 
        IntInt result = instance->top();

        // 두 개의 int 값을 long long으로 묶어서 반환
        int64_t combined_value = ((long long)result.a << 32) | (result.b & 0xFFFFFFFFLL);
        return combined_value;
    }

    EXPORT int64_t TopFloatInt(PriorityQueueDLL<FloatInt>* instance)
    {
        FloatInt result = instance->top();

        // float, int 값을 long long으로 묶어서 반환
        int64_t combined_value = ((long long)(*(int*)&result.a) << 32) | (result.b & 0xFFFFFFFFLL);
        return combined_value;
    }

    EXPORT int64_t TopIntFloatChar32(PriorityQueueDLL<IntFloatChar32>* instance, char* out_c)
    {
        IntFloatChar32 result = instance->top();

        // float a와 int b를 long long으로 묶어서 반환
        int64_t combined_value = ((int64_t)(result.a) << 32) | (*(int64_t*)&result.b & 0xFFFFFFFFLL);

        // char 배열 c를 out_c에 복사
        strcpy(out_c, result.c);

        return combined_value;
    }

    EXPORT void PushInt(PriorityQueueDLL<int>* instance, int data) { instance->push(data); }
    EXPORT void PushFloat(PriorityQueueDLL<float>* instance, float data) { instance->push(data); }
    EXPORT void PushDouble(PriorityQueueDLL<double>* instance, double data) { instance->push(data); }
    EXPORT void PushIntInt(PriorityQueueDLL<IntInt>* instance, IntInt data) { instance->push(data); }
    EXPORT void PushFloatInt(PriorityQueueDLL<FloatInt>* instance, FloatInt data) { instance->push(data); }
    EXPORT void PushIntFloatChar32(PriorityQueueDLL<IntFloatChar32>* instance, IntFloatChar32 data) { instance->push(data); }

    EXPORT void PopInt(PriorityQueueDLL<int>* instance) { instance->pop(); }
    EXPORT void PopFloat(PriorityQueueDLL<float>* instance) { instance->pop(); }
    EXPORT void PopDouble(PriorityQueueDLL<double>* instance) { instance->pop(); }
    EXPORT void PopIntInt(PriorityQueueDLL<IntInt>* instance) { instance->pop(); }
    EXPORT void PopFloatInt(PriorityQueueDLL<FloatInt>* instance) { instance->pop(); }
    EXPORT void PopIntFloatChar32(PriorityQueueDLL<IntFloatChar32>* instance) { instance->pop(); }
}