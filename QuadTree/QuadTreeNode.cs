using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameDemo
{
    class QuadTreeNode<T> where T : IQuadStorable
    {
        private const int NODE_CAPACITY = 2; // Max number of objects in a QuadTree before it divides itself
        private List<QuadTreeObject<T>> objects = null;
        private Rectangle rect;
        private QuadTreeNode<T> parent = null;

        private QuadTreeNode<T> childTL = null; // Top Left Child
        private QuadTreeNode<T> childTR = null; // Top Right Child
        private QuadTreeNode<T> childBL = null; // Bottom Left Child
        private QuadTreeNode<T> childBR = null; // Bottom Right Child

        // The area this Node represents
        public Rectangle QuadRect
        {
            get { return rect; }
        }

        // The top left child for this Node
        public QuadTreeNode<T> TopLeftChild
        {
            get { return childTL; }
        }

        // The top right child for this Node
        public QuadTreeNode<T> TopRightChild
        {
            get { return childTR; }
        }

        // The bottom left child for this Node
        public QuadTreeNode<T> BottomLeftChild
        {
            get { return childBL; }
        }

        // The bottom right child for this Node
        public QuadTreeNode<T> BottomRightChild
        {
            get { return childBR; }
        }

        // This Node's parent
        public QuadTreeNode<T> Parent
        {
            get { return parent; }
        }

        // Objects contained in this Node
        internal List<QuadTreeObject<T>> Objects
        {
            get { return objects; }
        }

        public int Count
        {
            get { return ObjectCount(); }
        }

        public bool IsEmptyNode
        {
            get { return Count == 0 && childTL == null; }
        }

        public QuadTreeNode(Rectangle rect)
        {
            this.rect = rect;
        }

        public QuadTreeNode(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
        }

        private QuadTreeNode(QuadTreeNode<T> parent, Rectangle rect) : this(rect)
        {
            this.parent = parent;
        }

        // Add an item to the object list
        private void Add(QuadTreeObject<T> item)
        {
            if (objects == null)
            {
                objects = new List<QuadTreeObject<T>>();
            }

            item.owner = this;
            objects.Add(item);
        }

        // Remove an item from the object list
        private void Remove(QuadTreeObject<T> item)
        {
            if (objects != null)
            {
                int removeIndex = objects.IndexOf(item);
                if (removeIndex >= 0)
                {
                    int lastObjectIndex = objects.Count - 1;
                    objects[removeIndex] = objects[lastObjectIndex];
                    objects.RemoveAt(lastObjectIndex);
                }
            }
        }

        // Get the total for all objects in this Node, including children
        private int ObjectCount()
        {
            int count = 0;

            if (objects != null)
            {
                count += objects.Count;
            }

            if (childTL != null)
            {
                count += childTL.ObjectCount();
                count += childTR.ObjectCount();
                count += childBL.ObjectCount();
                count += childBR.ObjectCount();
            }

            return count;
        }

        // Subdivide this Node and move it's children into the appropriate Nodes where applicable
        private void Subdivide()
        {
            Point size = new Point(rect.Width / 2, rect.Height / 2);
            Point mid = new Point(rect.X + size.X, rect.Y + size.Y);

            childTL = new QuadTreeNode<T>(this, new Rectangle(rect.Left, rect.Top, size.X, size.Y));
            childTR = new QuadTreeNode<T>(this, new Rectangle(mid.X, rect.Top, size.X, size.Y));
            childBL = new QuadTreeNode<T>(this, new Rectangle(rect.Left, mid.Y, size.X, size.Y));
            childBR = new QuadTreeNode<T>(this, new Rectangle(mid.X, mid.Y, size.X, size.Y));

            // If completely contained by the node, bump objects down
            for (int i = 0; i < objects.Count; i++)
            {
                QuadTreeNode<T> destNode = GetDestinationNode(objects[i]);

                if (destNode != this)
                {
                    // Insert into the appropriate node and remove from this node
                    destNode.Insert(objects[i]);
                    Remove(objects[i]);
                    i--;
                }
            }
        }

        // Get the child Node that will contain an object
        private QuadTreeNode<T> GetDestinationNode(QuadTreeObject<T> item)
        {
            // If a child cannot contain an object, it will live in this Node
            QuadTreeNode<T> destNode = this;

            if (childTL.QuadRect.Contains(item.data.boundry))
            {
                destNode = childTL;
            }
            else if (childTR.QuadRect.Contains(item.data.boundry))
            {
                destNode = childTR;
            }
            else if (childBL.QuadRect.Contains(item.data.boundry))
            {
                destNode = childBL;
            }
            else if (childBR.QuadRect.Contains(item.data.boundry))
            {
                destNode = childBR;
            }

            return destNode;
        }

        // Relocate an Object
        private void Relocate(QuadTreeObject<T> item)
        {
            // Check we are inside the parent
            if (QuadRect.Contains(item.data.boundry))
            {
                // Check if we are inside the children
                if (childTL != null)
                {
                    QuadTreeNode<T> dest = GetDestinationNode(item);
                    if (item.owner != dest)
                    {
                        // Delete the item from this Node and add it to our child
                        QuadTreeNode<T> formerOwner = item.owner;
                        Delete(item, false);
                        dest.Insert(item);

                        // Clean up
                        formerOwner.CleanUpwards();
                    }
                }
            }
            else
            {
                // We don't fit here anymore, move up if we can
                if (parent != null)
                {
                    parent.Relocate(item);
                }
            }
        }

        private void CleanUpwards()
        {
            if (childTL != null)
            {
                // Delete all children if all empty
                if (childTL.IsEmptyNode &&
                    childTR.IsEmptyNode &&
                    childBL.IsEmptyNode &&
                    childBR.IsEmptyNode)
                {
                    childTL = null;
                    childTR = null;
                    childBL = null;
                    childBR = null;

                    if (parent != null && Count == 0)
                    {
                        parent.CleanUpwards();
                    }
                }
            }
            else
            {
                // Get parent to check and potentially clean up too
                if (parent != null && Count == 0)
                {
                    parent.CleanUpwards();
                }
            }
        }

        // Clears the Node of all objects, including any objects living in its children
        internal void Clear()
        {
            // Clear out the children, if we have any
            if (childTL != null)
            {
                childTL.Clear();
                childTR.Clear();
                childBL.Clear();
                childBR.Clear();
            }

            // Clear any objects at this level
            if (objects != null)
            {
                objects.Clear();
                objects = null;
            }

            // Set the children to null
            childTL = null;
            childTR = null;
            childBL = null;
            childBR = null;
        }

        // Deletes an item from this Node. 
        // If the removed object causes this Node to have no objects in its children, clear them too
        internal void Delete(QuadTreeObject<T> item, bool clean)
        {
            if (item.owner != null)
            {
                if (item.owner == this)
                {
                    Remove(item);
                    if (clean)
                    {
                        CleanUpwards();
                    }
                }
                else
                {
                    item.owner.Delete(item, clean);
                }
            }
        }

        // Insert an item into this Node
        internal void Insert(QuadTreeObject<T> item)
        {
            // If this Node doesn't contain the items rectangle, do nothing, unless we are the root
            if (!rect.Contains(item.data.boundry))
            {
                if (parent == null)
                {
                    // This object is outside of the Node bounds, we should add it at the root level
                    Add(item);
                }
                else
                {
                    return;
                }
            }

            if (objects == null ||
                (childTL == null && objects.Count + 1 <= NODE_CAPACITY))
            {
                // If there's room to add the object, just add it
                Add(item);
            }
            else
            {
                // No space, create children and bump objects down where appropriate
                if (childTL == null)
                {
                    Subdivide();
                }

                // Find out which Node this object should go in and add it there
                QuadTreeNode<T> destNode = GetDestinationNode(item);
                if (destNode == this)
                {
                    Add(item);
                }
                else
                {
                    destNode.Insert(item);
                }
            }
        }

        // Get the objects in this Node that intersect with the specified rectangle
        internal List<T> GetObjects(Rectangle searchRect)
        {
            List<T> results = new List<T>();
            GetObjects(searchRect, ref results);
            return results;
        }

        // Get the objects in this Node that intersect with the specified rectangle
        internal void GetObjects(Rectangle searchRect, ref List<T> results)
        {
            if (results != null)
            {
                if (searchRect.Contains(this.rect))
                {
                    // If the search area completely contains this Node, get every object this quad and all it's children have
                    GetAllObjects(ref results);
                }
                else if (searchRect.Intersects(this.rect))
                {
                    // Otherwise, if the Node isn't fully contained, only add objects that intersect with the search rectangle
                    if (objects != null)
                    {
                        for (int i = 0; i < objects.Count; i++)
                        {
                            if (searchRect.Intersects(objects[i].data.boundry))
                            {
                                results.Add(objects[i].data);
                            }
                        }
                    }

                    // Get the objects for the search rectangle from the children
                    if (childTL != null)
                    {
                        childTL.GetObjects(searchRect, ref results);
                        childTR.GetObjects(searchRect, ref results);
                        childBL.GetObjects(searchRect, ref results);
                        childBR.GetObjects(searchRect, ref results);
                    }
                }
            }
        }

        // Get all objects in this Node, and it's children
        internal void GetAllObjects(ref List<T> results)
        {
            // If this Node has objects, add them
            if (objects != null)
            {
                foreach (QuadTreeObject<T> obj in objects)
                {
                    results.Add(obj.data);
                }
            }

            // If we have children, get their objects too
            if (childTL != null)
            {
                childTL.GetAllObjects(ref results);
                childTR.GetAllObjects(ref results);
                childBL.GetAllObjects(ref results);
                childBR.GetAllObjects(ref results);
            }
        }

        // Moves the QuadTree object in the tree
        internal void Move(QuadTreeObject<T> item)
        {
            if (item.owner != null)
            {
                item.owner.Relocate(item);
            }
            else
            {
                Relocate(item);
            }
        }
    }
}
