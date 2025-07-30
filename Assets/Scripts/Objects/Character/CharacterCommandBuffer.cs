using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AtentsPro {
    public struct CharacterCommand
    {
        public Action action;
    }
    public class CharacterCommandBuffer
    {
        public int Count
        {
            get
            {
                return commandQueue.Count;
            }
        }

        Queue<CharacterCommand> commandQueue = new Queue<CharacterCommand>();
        bool bCmdReady = true;

        public void Enqueue(CharacterCommand command)
        {
            commandQueue.Enqueue(command);
        }
        public CharacterCommand Dequeue()
        {
            return commandQueue.Dequeue();
        }
        public void Execute()
        {
            if(commandQueue.Count > 0 && bCmdReady)
            {
                CharacterCommand buffer = Dequeue();
                bCmdReady = false;
                buffer.action.Invoke();
            }
        }
        public void Clear()
        {
            commandQueue.Clear();
        }
        public void Ready()
        {
            bCmdReady = true;
        }
        public bool IsReady()
        {
            return bCmdReady;
        }
    }
}
