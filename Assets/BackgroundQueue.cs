/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class BackgroundQueue : MonoBehaviour
    {
        public const int DefaultMaxCostPerFrame = 100;
        public const int DefaultTaskCost = DefaultMaxCostPerFrame / 20;
        const int StatsAmount = 30;

        int[] m_MeanCost;
        public float MeanCost
        {
            get
            {
                float amount = 0f;
                for (int i = 0; i < m_MeanCost.Length; ++i)
                    amount += m_MeanCost[i];
                return amount / StatsAmount;
            }
        }

        public struct Task
        {
            public Action FN;
            public int Cost;
        }

        ConcurrentQueue<Task> m_PendingTasks;
        ConcurrentQueue<Action> m_OnIdleTasks;

        public static BackgroundQueue Mgr;

        int m_MaxCostPerFrame;

        public int GetPendingTaskCount() => m_PendingTasks.Count;
        public int GetScheduledTaskCount() => m_PendingTasks.Count + m_OnIdleTasks.Count;
        public int GetOnIdleTaskCount() => m_OnIdleTasks.Count;

        private void Awake()
        {
            m_PendingTasks = new ConcurrentQueue<Task>();
            m_OnIdleTasks = new ConcurrentQueue<Action>();
            m_MeanCost = new int[StatsAmount];
            m_MaxCostPerFrame = DefaultMaxCostPerFrame;
            Mgr = this;
        }
        void Update()
        {
            int curCost = 0;
            while (m_PendingTasks.TryDequeue(out Task task))
            {
                task.FN();
                curCost += task.Cost;
                if (curCost >= m_MaxCostPerFrame)
                {
                    break;
                }
            }
            m_MeanCost[Time.frameCount % StatsAmount] = curCost;
            if(curCost == 0)
            {
                if(m_OnIdleTasks.TryDequeue(out Action task))
                {
                    task();
                    m_MeanCost[Time.frameCount % StatsAmount] = DefaultTaskCost;
                }
            }
        }
        public void ScheduleOnIdleTask(Action task)
        {
            m_OnIdleTasks.Enqueue(task);
        }
        public void ScheduleTask(Task task)
        {
            m_PendingTasks.Enqueue(task);
        }
        public void ScheduleTask(Action task)
        {
            ScheduleTask(new Task()
            {
                FN = task,
                Cost = DefaultTaskCost
            });
        }
        public void ScheduleTask(List<Task> tasks)
        {
            if (tasks == null)
                return;
            for (int i = 0; i < tasks.Count; ++i) ScheduleTask(tasks[i]);
        }
        public void ScheduleTask(List<Action> tasks)
        {
            if (tasks == null)
                return;
            for (int i = 0; i < tasks.Count; ++i) ScheduleTask(tasks[i]);
        }
        public void FlushTasksNextFrame()
        {
            m_MaxCostPerFrame = int.MaxValue;
            ScheduleTask(new Task()
            {
                FN = () => { m_MaxCostPerFrame = DefaultMaxCostPerFrame; },
                Cost = 0
            });
        }
        public void FlushTasks()
        {
            while (m_PendingTasks.Count > 0)
                Update();
        }
    }
}