using System.Collections;
using System.Collections.Generic;


namespace behaviorTree
{

    public enum NodeState
    {
        RUNNING,
        SUCCES,
        FAILURE
      
    }
    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children;

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();
        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node _child in children)
            {
                _Attach(_child);
            }
        }

        public void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public bool ClearData(string key)
        {

            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;

                node = node.parent;
            }
            return false;
        }

        public void SetDate(string key, object value)
        {
            _dataContext[key] = value;
        }

        public object getData(string key)
        {
            object value = null;
            if(_dataContext.TryGetValue(key, out value))
            {
                return value;
            }

            Node node = parent;
            while(node != null)
            {
                value = node.getData(key);
                if (value != null)
                    return value;

                node = node.parent;
            }
            return null;
        }
   
    }
}

