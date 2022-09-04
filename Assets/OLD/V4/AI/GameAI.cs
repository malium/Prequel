/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets.AI
//{
//    public enum AIState
//    {
//        IDLE,       // Static in place
//        ROAMING,    // Moving around (friendly)
//        AWARE,      // Has heard an enemy, roaming (unfriendly)
//        ALERT,      // Has seen an enemy, going after it
//        AGRESSIVE,  // Attacking the enemy
//        FLEE,       // Escaping from the fight
//        DEAD,       // Died in combat

//        COUNT
//    }

//    public interface IAIFunction
//    {
//        AIState GetState();

//        void Execute();

//        AIState ComputeNextState();
//    }

//    public class AIFunction : IAIFunction
//    {
//        [SerializeField]
//        AIState m_State;
//        Action m_ExecuteState;
//        Func<AIState> m_ComputeNextState;

//        public AIFunction(AIState state, Action executeFN, Func<AIState> nextStateFN)
//        {
//            m_State = state;
//            m_ExecuteState = executeFN;
//            m_ComputeNextState = nextStateFN;
//        }

//        public void Execute()
//        {
//            m_ExecuteState();
//        }

//        public AIState GetState()
//        {
//            return m_State;
//        }

//        public AIState ComputeNextState()
//        {
//            return m_ComputeNextState();
//        }

//        public static AIFunction CreateEmpty(AIState state)
//        {
//            return new AIFunction(state, () => { }, () => { return state; });
//        }
//    }

//    //public class AIFunctionT<T> : IAIFunction
//    //{
//    //    AIState m_State;
//    //    Func<T, AIState> m_Function;
//    //    T m_Data;

//    //    public AIFunctionT(AIState state, T data, Func<T, AIState> function)
//    //    {
//    //        m_State = state;
//    //        m_Data = data;
//    //        m_Function = function;
//    //    }

//    //    public AIState Execute()
//    //    {
//    //        return m_Function.Invoke(m_Data);
//    //    }

//    //    public AIState GetState()
//    //    {
//    //        return m_State;
//    //    }

//    //    public static AIFunctionT<T> CreateEmpty(AIState state, T data)
//    //    {
//    //        return new AIFunctionT<T>(state, data, (T) => { return state; });
//    //    }
//    //}

//    [Serializable]
//    public class GameAI
//    {
//        IAIFunction[] m_Functions;
//        float m_ReactionTime;
//        float m_NextUpdate;
//        //AIState m_NextState;
//        [SerializeField]
//        AIState m_CurrentFunction;

//        public GameAI()
//        {
//            m_Functions = new IAIFunction[(int)AIState.COUNT];
//            for (int i = 0; i < m_Functions.Length; ++i)
//            {
//                m_Functions[i] = AIFunction.CreateEmpty((AIState)i);
//            }
//            m_ReactionTime = 1.0f;
//            //m_NextState = AIState.IDLE;
//            m_CurrentFunction = AIState.IDLE;
//        }

//        public void Update()
//        {
//            m_Functions[(int)m_CurrentFunction].Execute();
//            if (m_NextUpdate < Time.time/* || m_CurrentFunction != (int)m_NextState*/)
//            {
//                var nextState = m_Functions[(int)m_CurrentFunction].ComputeNextState();
//                m_CurrentFunction = nextState;
//                m_NextUpdate = Time.time + m_ReactionTime;
//            }
//        }

//        public void SetReactionTime(float react)
//        {
//            m_ReactionTime = react;
//        }

//        public IAIFunction GetFunction(AIState state)
//        {
//            return m_Functions[(int)state];
//        }

//        public void SetFunction(IAIFunction function)
//        {
//            m_Functions[(int)function.GetState()] = function;
//        }

//        public AIState GetCurrentState()
//        {
//            return m_CurrentFunction;
//        }

//        public void ChangeStateTo(AIState state)
//        {
//            m_CurrentFunction = state;
//            m_NextUpdate = Time.time;
//        }
//    }
//}
