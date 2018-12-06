﻿using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Views.Pooling
{
    public interface IViewPool : IPool<object>
    {
        void PreAllocate(int allocationCount);
        void DeAllocate(int dellocationCount);
        void EmptyPool();
    }
}