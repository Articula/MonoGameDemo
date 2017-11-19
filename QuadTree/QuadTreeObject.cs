using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameDemo
{
    internal class QuadTreeObject<T> where T : IQuadStorable
    {
        public T data {get; private set;}
        internal QuadTreeNode<T> owner { get; set; }

        public QuadTreeObject(T data)
        {
            this.data = data;
        }
    }
}
