using System;
using System.Collections.Generic;

namespace LaborServices.ViewModel
{
    public class MenuViewModel
    {
        public long id { get; set; }
        public MenuViewModel[] children { get; set; }

        public IEnumerable<MenuViewModel> Traverse(MenuViewModel root)
        {
            var stack = new Stack<MenuViewModel>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                if(current.children == null) continue;
                foreach (var child in current.children)
                    stack.Push(child);
            }
        }
    }
}
