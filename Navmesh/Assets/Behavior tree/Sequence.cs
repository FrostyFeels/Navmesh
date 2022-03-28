using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace behaviorTree 
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;                        
                    case NodeState.SUCCES:
                        continue;                       
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    default:
                        state = NodeState.SUCCES;
                        return state;
  
                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCES;
            return state;
        }

    }
}


