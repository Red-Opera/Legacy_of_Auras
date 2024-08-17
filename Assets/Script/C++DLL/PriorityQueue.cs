using System;
using System.Runtime.InteropServices;
using UnityEngine;

// �������̽� ����
public abstract class PriorityQueue<T>
{
    public abstract void MakeInstance();
    public abstract void DeleteInstance(IntPtr priorityQueue);
    public abstract bool empty();
    public abstract uint size();
    public abstract T top();
    public abstract void push(T addData);
    public abstract void pop();

    public static PriorityQueue<T> Create()
    {
        if (typeof(T) == typeof(int))
            return (PriorityQueue<T>)(object)new PriorityQueueINT();

        else if (typeof(T) == typeof(float))
            return (PriorityQueue<T>)(object)new PriorityQueueFloat();

        else if (typeof(T) == typeof(double))
            return (PriorityQueue<T>)(object)new PriorityQueueDouble();

        else if (typeof(T) == typeof(IntInt))
            return (PriorityQueue<T>)(object)new PriorityQueueIntInt();

        else if (typeof(T) == typeof(FloatInt))
            return (PriorityQueue<T>)(object)new PriorityQueueFloatInt();

        else if (typeof(T) == typeof(IntFloatChar32))
            return (PriorityQueue<T>)(object)new PriorityQueueIntFloatChar32();

        throw new NotImplementedException("This type is not supported.");
    }
}

public class PriorityQueueINT : PriorityQueue<int>
{
    // DLL �Լ� ����
    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MakeIntInstance();

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void DeleteIntInstance(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool EmptyInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint SizeInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern int TopInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PushInt(IntPtr priorityQueue, int addData);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PopInt(IntPtr priorityQueue);

    // ��� ����
    private IntPtr instancePtr;

    // ������
    public PriorityQueueINT() => instancePtr = MakeIntInstance();

    // �Ҹ���
    ~PriorityQueueINT() 
    { 
        DeleteIntInstance(instancePtr); 
        instancePtr = IntPtr.Zero; 
    }

    public override void MakeInstance() { instancePtr = MakeIntInstance(); }
    public override void DeleteInstance(IntPtr priorityQueue) { DeleteIntInstance(instancePtr); }

    public override bool empty() { return EmptyInt(instancePtr); }
    public override uint size() { return SizeInt(instancePtr); }
    public override int top() { return TopInt(instancePtr); }
    public override void push(int addData) { PushInt(instancePtr, addData); }
    public override void pop() { PopInt(instancePtr); }
}

public class PriorityQueueFloat : PriorityQueue<float>
{
    // DLL �Լ� ����
    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MakeFloatInstance();

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void DeleteFloatInstance(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool EmptyFloat(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint SizeFloat(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern float TopFloat(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PushFloat(IntPtr priorityQueue, float addData);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PopFloat(IntPtr priorityQueue);

    // ��� ����
    private IntPtr instancePtr;

    // ������
    public PriorityQueueFloat() => instancePtr = MakeFloatInstance();

    // �Ҹ���
    ~PriorityQueueFloat()
    {
        DeleteFloatInstance(instancePtr);
        instancePtr = IntPtr.Zero;
    }

    public override void MakeInstance() { instancePtr = MakeFloatInstance(); }
    public override void DeleteInstance(IntPtr priorityQueue) { DeleteFloatInstance(instancePtr); }

    public override bool empty() { return EmptyFloat(instancePtr); }
    public override uint size() { return SizeFloat(instancePtr); }
    public override float top() { return TopFloat(instancePtr); }
    public override void push(float addData) { PushFloat(instancePtr, addData); }
    public override void pop() { PopFloat(instancePtr); }
}

public class PriorityQueueDouble : PriorityQueue<double>
{
    // DLL �Լ� ����
    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MakeDoubleInstance();

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void DeleteDoubleInstance(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool EmptyDouble(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint SizeDouble(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern double TopDouble(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PushDouble(IntPtr priorityQueue, double addData);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PopDouble(IntPtr priorityQueue);

    // ��� ����
    private IntPtr instancePtr;

    // ������
    public PriorityQueueDouble() => instancePtr = MakeDoubleInstance();

    // �Ҹ���
    ~PriorityQueueDouble()
    {
        DeleteDoubleInstance(instancePtr);
        instancePtr = IntPtr.Zero;
    }

    public override void MakeInstance() { instancePtr = MakeDoubleInstance(); }
    public override void DeleteInstance(IntPtr priorityQueue) { DeleteDoubleInstance(instancePtr); }

    public override bool empty() { return EmptyDouble(instancePtr); }
    public override uint size() { return SizeDouble(instancePtr); }
    public override double top() { return TopDouble(instancePtr); }
    public override void push(double addData) { PushDouble(instancePtr, addData); }
    public override void pop() { PopDouble(instancePtr); }
}

public class PriorityQueueIntInt : PriorityQueue<IntInt>
{
    // DLL �Լ� ����
    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MakeIntIntInstance();

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void DeleteIntIntInstance(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool EmptyIntInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint SizeIntInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern long TopIntInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PushIntInt(IntPtr priorityQueue, IntInt addData);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PopIntInt(IntPtr priorityQueue);

    // ��� ����
    private IntPtr instancePtr;

    // ������
    public PriorityQueueIntInt() => instancePtr = MakeIntIntInstance();

    // �Ҹ���
    ~PriorityQueueIntInt()
    {
        DeleteIntIntInstance(instancePtr);
        instancePtr = IntPtr.Zero;
    }

    public override void MakeInstance() { instancePtr = MakeIntIntInstance(); }
    public override void DeleteInstance(IntPtr priorityQueue) { DeleteIntIntInstance(instancePtr); }

    public override bool empty() { return EmptyIntInt(instancePtr); }
    public override uint size() { return SizeIntInt(instancePtr); }
    public override IntInt top()
    {
        // Call the C++ DLL function to get the combined int64_t value
        long result = TopIntInt(instancePtr);

        // ������ ��ȯ�ؼ� �ٽ� ����ü ������ ����
        int restored_a = (int)(result >> 32);           // ���� 32��Ʈ�� �����Ͽ� int�� ��ȯ
        int restored_b = (int)(result & 0xFFFFFFFF);  // ���� 32��Ʈ�� �����Ͽ� int�� ��ȯ

        // Create and return the IntInt struct
        return new IntInt(restored_a, restored_b);
    }
    public override void push(IntInt addData) { PushIntInt(instancePtr, addData); }
    public override void pop() { PopIntInt(instancePtr); }
}

public class PriorityQueueFloatInt : PriorityQueue<FloatInt>
{
    // DLL �Լ� ����
    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MakeFloatIntInstance();

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void DeleteFloatIntInstance(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool EmptyFloatInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint SizeFloatInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern long TopFloatInt(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PushFloatInt(IntPtr priorityQueue, FloatInt addData);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PopFloatInt(IntPtr priorityQueue);

    // ��� ����
    private IntPtr instancePtr;

    // ������
    public PriorityQueueFloatInt() => instancePtr = MakeFloatIntInstance();

    // �Ҹ���
    ~PriorityQueueFloatInt()
    {
        DeleteFloatIntInstance(instancePtr);
        instancePtr = IntPtr.Zero;
    }

    public override void MakeInstance() { instancePtr = MakeFloatIntInstance(); }
    public override void DeleteInstance(IntPtr priorityQueue) { DeleteFloatIntInstance(instancePtr); }

    public override bool empty() { return EmptyFloatInt(instancePtr); }
    public override uint size() { return SizeFloatInt(instancePtr); }
    public override FloatInt top()
    {
        // Call the C++ DLL function to get the combined int64_t value
        long result = TopFloatInt(instancePtr);

        // long�� byte �迭�� ��ȯ
        byte[] longBytes = BitConverter.GetBytes(result);

        
        float floatValue = BitConverter.ToSingle(longBytes, 4);     // ���� 4����Ʈ�� float�� ��ȯ
        int intValue = BitConverter.ToInt32(longBytes, 0);          // ���� 4����Ʈ�� int�� ��ȯ

        // Create and return the IntInt struct
        return new FloatInt(floatValue, intValue);
    }
    public override void push(FloatInt addData) { PushFloatInt(instancePtr, addData); }
    public override void pop() { PopFloatInt(instancePtr); }
}

public class PriorityQueueIntFloatChar32 : PriorityQueue<IntFloatChar32>
{
    // DLL �Լ� ����
    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MakeIntFloatChar32Instance();

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void DeleteIntFloatChar32Instance(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool EmptyIntFloatChar32(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint SizeIntFloatChar32(IntPtr priorityQueue);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern long TopIntFloatChar32(IntPtr priorityQueue, IntPtr char32);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PushIntFloatChar32(IntPtr priorityQueue, IntFloatChar32 addData);

    [DllImport("C++DLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PopIntFloatChar32(IntPtr priorityQueue);

    // ��� ����
    private IntPtr instancePtr;

    // ������
    public PriorityQueueIntFloatChar32() => instancePtr = MakeIntFloatChar32Instance();

    // �Ҹ���
    ~PriorityQueueIntFloatChar32()
    {
        DeleteIntFloatChar32Instance(instancePtr);
        instancePtr = IntPtr.Zero;
    }

    public override void MakeInstance() { instancePtr = MakeIntFloatChar32Instance(); }
    public override void DeleteInstance(IntPtr priorityQueue) { DeleteIntFloatChar32Instance(instancePtr); }

    public override bool empty() { return EmptyIntFloatChar32(instancePtr); }
    public override uint size() { return SizeIntFloatChar32(instancePtr); }
    public override IntFloatChar32 top()
    {
        IntPtr char32 = Marshal.AllocHGlobal(32);
        long result = TopIntFloatChar32(instancePtr, char32);

        // long�� byte �迭�� ��ȯ
        byte[] longBytes = BitConverter.GetBytes(result);

        int intValue = BitConverter.ToInt32(longBytes, 4);          // ���� 4����Ʈ�� int�� ��ȯ
        float floatValue = BitConverter.ToSingle(longBytes, 0);     // ���� 4����Ʈ�� float�� ��ȯ

        string stringValue = Marshal.PtrToStringAnsi(char32);

        // Create and return the IntInt struct
        return new IntFloatChar32(intValue, floatValue, stringValue);
    }
    public override void push(IntFloatChar32 addData) { PushIntFloatChar32(instancePtr, addData); }
    public override void pop() { PopIntFloatChar32(instancePtr); }
}

[StructLayout(LayoutKind.Sequential)]
public struct IntInt
{
    public int a, b;
    
    public IntInt(int a, int b) { this.a = a; this.b = b; }
}

[StructLayout(LayoutKind.Sequential)]
public struct FloatInt
{
    public float a;
    public int b;

    public FloatInt(float a, int b) { this.a = a; this.b = b; }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct IntFloatChar32
{
    public int a;
    public float b;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string c;

    public IntFloatChar32(int a, float b, string c) { this.a = a; this.b = b; this.c = c; }
}

public class PriorityQueue : MonoBehaviour
{
    //private PriorityQueue<IntFloatChar32> intQueue;

    private void Start()
    {
        // int Ÿ���� �켱���� ť ��� ����
        //intQueue = PriorityQueue<IntFloatChar32>.Create();
        //
        //Stopwatch sw = new Stopwatch();
        //
        //// ������ ����
        //sw.Start();
        //
        //for (int i = 0; i < 1000000; i++)
        //    intQueue.push(new IntFloatChar32(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-500.0f, 500.0f), i.ToString()));
        //
        //sw.Stop();
        //UnityEngine.Debug.Log($"Data insertion time: {sw.ElapsedMilliseconds} ms");
        //
        //sw.Reset();
        //sw.Start();
        //while (!intQueue.empty())
        //{
        //    //IntFloatChar32 result = intQueue.top();
        //    //UnityEngine.Debug.Log($"TopIntInt result: {result.a}, {result.b}, {result.c}");
        //
        //    intQueue.pop();
        //}
        //
        //sw.Stop();
        //UnityEngine.Debug.Log($"Data retrieval time: {sw.ElapsedMilliseconds} ms");;
    }
}