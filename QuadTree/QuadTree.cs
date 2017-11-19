using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace MonoGameDemo
{
    class QuadTree<T> : ICollection<T> where T : IQuadStorable
    {
        private readonly Dictionary<T, QuadTreeObject<T>> wrappedDictionary = new Dictionary<T, QuadTreeObject<T>>();
        private readonly QuadTreeNode<T> quadTreeRoot;

        public QuadTree(Rectangle rect)
        {
            quadTreeRoot = new QuadTreeNode<T>(rect);
        }

        public QuadTree(int x, int y, int width, int height)
        {
            quadTreeRoot = new QuadTreeNode<T>(new Rectangle(x, y, width, height));
        }

        // Gets the rectangle that bounds this QuadTree
        public Rectangle QuadRect
        {
            get { return quadTreeRoot.QuadRect; }
        }

        // Get the objects in this tree that intersect with the specified rectangle
        public List<T> GetObjects(Rectangle rect)
        {
            return quadTreeRoot.GetObjects(rect);
        }

        // Get the objects in this tree that intersect with the specified rectangle
        // results: A reference to a list that will be populated with the results
        public void GetObjects(Rectangle rect, ref List<T> results)
        {
            quadTreeRoot.GetObjects(rect, ref results);
        }

        // Get all objects in this Quad, and it's children
        public List<T> GetAllObjects()
        {
            return new List<T>(wrappedDictionary.Keys);
        }

        // Move an object in the tree
        public bool Move(T item)
        {
            if (Contains(item))
            {
                quadTreeRoot.Move(wrappedDictionary[item]);
                return true;
            } else
            {
                return false;
            }
        }

        // Add an object to the tree
        public void Add(T item)
        {
            QuadTreeObject<T> wrappedObject = new QuadTreeObject<T>(item);
            wrappedDictionary.Add(item, wrappedObject);
            quadTreeRoot.Insert(wrappedObject);
        }
        
        // Remove all items from the tree
        public void Clear()
        {
            wrappedDictionary.Clear();
            quadTreeRoot.Clear();
        }

        public bool Contains(T item)
        {
            return wrappedDictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            wrappedDictionary.Keys.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return wrappedDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        // Removes the first occurance of a specific object from the tree
        public bool Remove(T item)
        {
            if (Contains(item))
            {
                quadTreeRoot.Delete(wrappedDictionary[item], true);
                wrappedDictionary.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Returns an enumerator that iterates through the collection
        public IEnumerator<T> GetEnumerator()
        {
            return wrappedDictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // The top left child for the QuadTree
        public QuadTreeNode<T> RootQuad
        {
            get { return quadTreeRoot; }
        }
    }
}
