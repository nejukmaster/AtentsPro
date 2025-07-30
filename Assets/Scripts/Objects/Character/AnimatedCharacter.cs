using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class AnimatedCharacter : MonoBehaviour
    {
        public Action skillStartCallback;

        Queue<Action> skillActionQueue = new Queue<Action>();

        public void ReserveSkillAction(Action action)
        {
            skillActionQueue.Enqueue(action);
        }

        public void DequeueSkillAction()
        {
            if (skillActionQueue.Count > 0)
            {
                skillActionQueue.Dequeue().Invoke();
            }
        }

        public void SkillStartCallback()
        {
            skillStartCallback?.Invoke();
            skillStartCallback = null;
        }

        public void AnimationEndCallback()
        {
            GetComponentInParent<Character>().CommandBufferEndCallback();
        }

        public Animator GetAnimator()
        {
            return GetComponent<Animator>();
        }
    }
}
