using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash
{
    internal sealed unsafe class UnmanagedBuffer<T> : CriticalFinalizerObject
        where T : struct
    {
        private readonly int initialCapacity;
        private readonly int elementSize;

        private int capacity;
        private int count;
        private void* ptr;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public UnmanagedBuffer(int initialCapacity)
        {
            this.initialCapacity = initialCapacity;
            elementSize = Marshal.SizeOf(typeof(T));

            capacity = initialCapacity;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        ~UnmanagedBuffer()
        {
            Clear();
        }

        public int Count
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get { return count; }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            set
            {
                if (value < 0 || value > capacity)
                    throw new ArgumentOutOfRangeException("value");

                count = value;
            }
        }

        public int Capacity
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get { return capacity; }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Clear()
        {
            FreeMemory(ptr);
            ptr = null;

            count = 0;
            capacity = initialCapacity;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public void* GetPointer()
        {
            if (ptr == null)
                ptr = AllocateMemory(capacity);

            return ptr;
        }

        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public void GrowBy(int extraElementCount)
        {
            if (extraElementCount < 0)
                throw new ArgumentOutOfRangeException("extraElementCount");

            int newCount = count + extraElementCount;
            EnsureCapacity(newCount);
            count = newCount;
        }

        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public void SetCapacity(int newCapacity)
        {
            if (newCapacity < 0)
                throw new ArgumentOutOfRangeException("newCapacity");

            if (newCapacity < count)
                count = newCapacity;

            InternalSetCapacity(newCapacity);
        }

        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public void EnsureCapacity(int minimumDesiredCapacity)
        {
            if (minimumDesiredCapacity < 0)
                throw new ArgumentOutOfRangeException("minimumDesiredCapacity");

            if (capacity >= minimumDesiredCapacity)
                return;

            int newCapacity = capacity;
            do
            {
                newCapacity *= 2;
            }
            while (newCapacity < minimumDesiredCapacity);

            InternalSetCapacity(newCapacity);
        }

        private void InternalSetCapacity(int newCapacity)
        {
            if (ptr != null)
            {
                if (count != 0)
                {
                    ptr = ReAllocateMemory(ptr, newCapacity);
                    capacity = newCapacity;
                    return;
                }

                FreeMemory(ptr);
                ptr = null;
            }

            ptr = AllocateMemory(newCapacity);
            capacity = newCapacity;
        }

        private void FreeMemory(void* ptr)
        {
            if (ptr != null)
            {
                Marshal.FreeCoTaskMem(new IntPtr(ptr));
                ptr = null;
            }
        }

        private void* AllocateMemory(int newCapacity)
        {
            return Marshal.AllocCoTaskMem(newCapacity * elementSize).ToPointer();
        }

        private void* ReAllocateMemory(void* oldPtr, int newCapacity)
        {
            return Marshal.ReAllocCoTaskMem(new IntPtr(oldPtr), newCapacity * elementSize).ToPointer();
        }
    }
}
