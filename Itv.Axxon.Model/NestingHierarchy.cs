using System;
using System.Collections.Generic;
using System.Linq;
using Itv.Axxon.Model.BoundaryCompare;
using Itv.Axxon.Model.Figures;

namespace Itv.Axxon.Model
{
    public class NestingHierarchy
    {
        private readonly List<Node> _rootLevel = new List<Node>();

        public void Add(Triangle figure)
        {
            var newNode = new Node(figure);
            var smallestContainerNode = FindSmallestContainerNode(_rootLevel, newNode);

            if (smallestContainerNode == null)
            {
                _rootLevel.Add(newNode);
                return;
            }

            foreach (var childNode in smallestContainerNode.ChildNodes)
            {
                if (Contains(newNode, childNode))
                {
                    newNode.ChildNodes.Add(childNode);
                }
            }

            newNode.ChildNodes.ForEach(x =>
                smallestContainerNode.ChildNodes.Remove(x));

            smallestContainerNode.ChildNodes.Add(newNode);
            newNode.Parent = smallestContainerNode;
        }

        public IEnumerable<Tuple<int, Triangle>> GetFiguresWithLevels()
        {
            return GetFiguresWithLevels(0, _rootLevel);
        }

        private IEnumerable<Tuple<int, Triangle>> GetFiguresWithLevels(int level, IEnumerable<Node> startNodes)
        {
            var nodes = startNodes.ToArray();
            foreach (var node in nodes)
            {
                yield return new Tuple<int, Triangle>(level, node.Figure);
            }

            foreach (var tuple in GetFiguresWithLevels(level + 1, nodes.SelectMany(x => x.ChildNodes)))
            {
                yield return tuple;
            }
        }

        private Node FindSmallestContainerNode(IEnumerable<Node> nodes, Node comparingNode)
        {
            foreach (var node in nodes)
            {
                if (Contains(node, comparingNode))
                {
                    return FindSmallestContainerNode(node.ChildNodes, comparingNode) ?? node;
                }
            }

            return null;
        }

        private bool Contains(Node node, Node comparingNode)
        {
            var compareResult = node.Figure.CompareTo(comparingNode.Figure);

            if (compareResult == BoundaryContainingResult.Inside)
                return true;

            if (compareResult == BoundaryContainingResult.Intersects)
                throw new FiguresIntersectingException($"Треугольники {node.Figure} и {comparingNode.Figure} пересекаются.");

            return false;
        }

        class Node
        {
            public Node(Triangle figure)
            {
                Figure = figure;
            }

            public Triangle Figure { get; }

            public Node Parent { get; set; }

            public List<Node> ChildNodes { get; } = new List<Node>();
        }
    }

    public class FigureContainingComparer<T1, T2>
        where T1 : IBoundaryContainingComparable<T2>
    {
        public BoundaryContainingResult Contains(T1 x, T2 y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            return x.CompareTo(y);
        }
    }
}