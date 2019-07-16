using System;
using System.Collections.Generic;
using System.Linq;
using Itv.Axxon.Model.BoundaryCompare;
using Itv.Axxon.Model.Figures;

namespace Itv.Axxon.Model
{
    public class NestingHierarchy
    {
        private readonly Node _rootNode = new Node(null);

        public NestingHierarchy()
        {
        }

        public NestingHierarchy(IEnumerable<Triangle> triangles)
        {
            foreach (var figure in triangles)
            {
                Add(figure);
            }   
        }

        public int GetHeight()
        {
            return GetFiguresWithLevels(0, _rootNode.ChildNodes).GroupBy(x => x.Item1).Count();
        }

        public IEnumerable<Triangle> GetSorted()
        {
            return GetFiguresWithLevels(0, _rootNode.ChildNodes).Select(x => x.Item2);
        }

        public void Add(Triangle figure)
        {
            var newNode = new Node(figure);

            // находим узел самой вложенной фигуры, заключающей добавляемую.
            var smallestContainerNode = FindSmallestContainerNode(_rootNode.ChildNodes, newNode);

            // если такой не найдено, берем корневой узел
            if (smallestContainerNode == null)
            {
                smallestContainerNode = _rootNode;
            }

            // находим среди дочерних узлов те, фигуры которых вложены в добавляемую, 
            // переносим их уровнем ниже (в дочерние узлы нового узла)
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
        }

        private IEnumerable<Tuple<int, Triangle>> GetFiguresWithLevels(int level, List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                yield return new Tuple<int, Triangle>(level, node.Figure);
            }

            var childLevel = nodes.SelectMany(x => x.ChildNodes).ToList();
            if (childLevel.Any())
            foreach (var tuple in GetFiguresWithLevels(level + 1, childLevel))
            {
                yield return tuple;
            }
        }

        private static Node FindSmallestContainerNode(IEnumerable<Node> nodes, Node comparingNode)
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

        private static bool Contains(Node node, Node comparingNode)
        {
            var compareResult = node.Figure.CompareTo(comparingNode.Figure);

            if (compareResult == BoundaryContainingResult.Contains)
                return true;

            if (compareResult == BoundaryContainingResult.Intersects)
                throw new FiguresIntersectingException($"Треугольники {node.Figure} и {comparingNode.Figure} пересекаются.");

            return false;
        }

        private class Node
        {
            public Node(Triangle figure)
            {
                Figure = figure;
            }

            public Triangle Figure { get; }

            public List<Node> ChildNodes { get; } = new List<Node>();
        }
    }
}