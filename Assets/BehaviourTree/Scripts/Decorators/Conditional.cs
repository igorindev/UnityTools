using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public class Conditional : DecoratorNode
    {
        public bool canMove;
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (canMove)
            {
                return child.Update();
            }
            else
            {
                child.Abort();
                return State.Failure;
            }
        }
    }
}