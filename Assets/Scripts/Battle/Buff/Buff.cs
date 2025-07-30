using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public abstract class Buff
    {
        public float duration;

        public Guid id;

        public Buff(Guid guid, float duration)
        {
            this.id = guid;
            this.duration = duration;
        }
        //���� ó�� ������� ��
        public abstract void OnEffect(Character holder);
        //������ ��ϵ��ִ� ���� ƽ���� ȣ��
        public abstract void Effect(Character holder);
        //������ ���� �Ǿ��� ��.
        public abstract void OnUneffect(Character holder);
    }
}
