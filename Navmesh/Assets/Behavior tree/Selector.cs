using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace behaviorTree 
{
    public class Selector : Node
    {
        public Selector() : base() {}
        public Selector(List<Node> children) : base(children) { }


        public override NodeState Evaluate()
        {
            foreach (Node _node in children)
            {
                switch (_node.Evaluate())
                {
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    case NodeState.SUCCES:
                        state = NodeState.SUCCES;
                        return state;
                    case NodeState.FAILURE:
                        continue;                    
                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}


