using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        private List<PathNode> openList;
        private List<PathNode> closedList;

        List<List<PathNode>> chessCells;
        private Vector2Int gridField;

        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            chessCells = new List<List<PathNode>>();

            openList = new List<PathNode>();
            closedList = new List<PathNode>();

            List<PathNode> unitNeighboursList = new List<PathNode>();

            gridField = grid.Size;

            for (int i = 0; i < grid.Size.y; i++)
            {
                chessCells.Add(new List<PathNode>());
                for (int j = 0; j < grid.Size.x; j++)
                {
                    chessCells[i].Add(new PathNode(j, i));
                    chessCells[i][j].Cost = int.MaxValue;

                    chessCells[i][j].cameFromNode = null;

                    if (grid.Get(i, j) != null)
                    {
                        // Делаем клетку не доступной, на ней стоит фигура = препятствие 
                        chessCells[i][j].SetNodeAccessible(false);
                    }


                }
            }

            PathNode startNode = chessCells[from.y][from.x];
            PathNode endNode = chessCells[to.y][to.x];


            openList.Add(startNode);

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestCostNode(openList);

                if (currentNode == endNode)
                    return CalculatePath(startNode, endNode);

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                switch (unit)
                {
                    case ChessUnitType.Pon:
                        unitNeighboursList = GetPonNeighbours(currentNode);
                        break;
                    case ChessUnitType.King:
                        unitNeighboursList = GetKingNeighbours(currentNode);
                        break;
                    case ChessUnitType.Queen:
                        unitNeighboursList = GetQueenNeighbours(currentNode);
                        break;
                    case ChessUnitType.Rook:
                        unitNeighboursList = GetRookNeighbours(currentNode);
                        break;
                    case ChessUnitType.Knight:
                        unitNeighboursList = GetKnightNeighbours(currentNode);
                        break;
                    case ChessUnitType.Bishop:
                        unitNeighboursList = GetBishopNeighbours(currentNode);
                        break;
                    default:
                        break;
                }

                foreach (PathNode neighbourNode in unitNeighboursList)
                {
                    if (closedList.Contains(neighbourNode)) continue;

                    int tentativeGCost = currentNode.Cost + 1;
                    if (tentativeGCost < neighbourNode.Cost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.Cost++;


                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }

                }
            }
            return null;

        }

        private PathNode GetLowestCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].Cost < lowestFCostNode.Cost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            return lowestFCostNode;
        }
        private List<Vector2Int> CalculatePath(PathNode startNode, PathNode endNode)
        {
            List<Vector2Int> pathList = new List<Vector2Int>();
            PathNode currentNode = endNode;
            pathList.Add(endNode.Position);

            while (currentNode.cameFromNode != startNode) // Стартовую точку не включаем ( по усл. )
            {
                pathList.Add(currentNode.cameFromNode.Position);
                currentNode = currentNode.cameFromNode;
            }
            pathList.Reverse();

            return pathList;
        }


        private List<PathNode> GetKnightNeighbours(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            int x = currentNode.Position.x;
            int y = currentNode.Position.y;

            // Правая половина
            if (x + 1 < gridField.x && y + 2 < gridField.y && chessCells[y + 2][x + 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y + 2][x + 1]);
            }
            if (x + 2 < gridField.x && y + 1 < gridField.y && chessCells[y + 1][x + 2].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y + 1][x + 2]);
            }
            if (x + 2 < gridField.x && y - 1 >= 0 && chessCells[y - 1][x + 2].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y - 1][x + 2]);
            }
            if (x + 1 < gridField.x && y - 2 >= 0 && chessCells[y - 2][x + 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y - 2][x + 1]);
            }
            //Левая половина
            if (x - 1 >= 0 && y - 2 >= 0 && chessCells[y - 2][x - 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y - 2][x - 1]);
            }
            if (x - 2 >= 0 && y - 1 >= 0 && chessCells[y - 1][x - 2].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y - 1][x - 2]);
            }
            if (x - 2 >= 0 && y + 1 < gridField.y && chessCells[y + 1][x - 2].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y + 1][x - 2]);
            }
            if (x - 1 >= 0 && y + 2 < gridField.y && chessCells[y + 2][x - 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y + 2][x - 1]);
            }

            return neighbourList;
        }
        private List<PathNode> GetPonNeighbours(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            int x = currentNode.Position.x;
            int y = currentNode.Position.y;


            if (y + 1 < gridField.y && chessCells[y + 1][x].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y + 1][x]);
            }

            if (y - 1 >= 0 && chessCells[y - 1][x].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y - 1][x]);
            }



            return neighbourList;
        }
        private List<PathNode> GetQueenNeighbours(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();
            neighbourList = GetBishopNeighbours(currentNode);
            neighbourList.AddRange(GetRookNeighbours(currentNode));

            return neighbourList;
        }
        private List<PathNode> GetBishopNeighbours(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            int x = currentNode.Position.x;
            int y = currentNode.Position.y;


            //Вправо вверх
            for (int i = x + 1, j = y + 1; i < gridField.x && j < gridField.y; i++, j++)
            {
                if (chessCells[j][i].GetNodeAccessible())
                {
                    neighbourList.Add(chessCells[j][i]);
                }
                else
                {
                    break;
                }
            }
            //Вправо вниз
            for (int i = x + 1, j = y - 1; i < gridField.x && j >= 0; i++, j--)
            {
                if (chessCells[j][i].GetNodeAccessible())
                {
                    neighbourList.Add(chessCells[j][i]);
                }
                else
                {
                    break;
                }
            }
            //Влево вниз
            for (int i = x - 1, j = y - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (chessCells[j][i].GetNodeAccessible())
                {
                    neighbourList.Add(chessCells[j][i]);
                }
                else
                {
                    break;
                }
            }
            //Влево верх
            for (int i = x - 1, j = y + 1; i >= 0 && j < gridField.x; i--, j++)
            {
                if (chessCells[j][i].GetNodeAccessible())
                {
                    neighbourList.Add(chessCells[j][i]);
                }
                else
                {
                    break;
                }
            }


            return neighbourList;

        }
        private List<PathNode> GetRookNeighbours(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            int x = currentNode.Position.x;
            int y = currentNode.Position.y;

            for (int i = x + 1; i < gridField.x; i++)
            {
                if (chessCells[y][i].GetNodeAccessible())
                {
                    neighbourList.Add(chessCells[y][i]);
                }
                else
                {
                    break;
                }
            }
            for (int i = x - 1; i >= 0; i--)
            {
                if (chessCells[y][i].GetNodeAccessible())
                {
                    neighbourList.Add(chessCells[y][i]);
                }
                else
                {
                    break;
                }
            }

            for (int i = y + 1; i < gridField.y; i++)
            {
                if (chessCells[i][x].GetNodeAccessible())
                {
                    neighbourList.Add(chessCells[i][x]);
                }
                else
                {
                    break;
                }
            }
            for (int i = y - 1; i >= 0; i--)
            {
                if (chessCells[i][x].GetNodeAccessible())
                {
                    neighbourList.Add(chessCells[i][x]);
                }
                else
                {
                    break;
                }
            }

            return neighbourList;
        }
        private List<PathNode> GetKingNeighbours(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            int x = currentNode.Position.x;
            int y = currentNode.Position.y;

            // Влево
            if (x - 1 >= 0 && chessCells[y][x - 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y][x - 1]);
            }

            // Вправо
            if (x + 1 < gridField.x && chessCells[y][x + 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y][x + 1]);
            }

            // Вниз
            if (y - 1 >= 0 && chessCells[y - 1][x].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y - 1][x]);
            }

            // Вверх
            if (y + 1 < gridField.y && chessCells[y + 1][x].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y + 1][x]);
            }

            // Влево вверх
            if (x - 1 >= 0 && y + 1 < gridField.y && chessCells[y + 1][x - 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y + 1][x - 1]);
            }

            // Вправо вверх
            if (y + 1 < gridField.y && x + 1 < gridField.x && chessCells[y + 1][x + 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y + 1][x + 1]);
            }

            // Вправо вниз
            if (y - 1 >= 0 && x + 1 < gridField.x && chessCells[y - 1][x + 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y - 1][x + 1]);
            }

            // Вниз влево
            if (x - 1 >= 0 && y - 1 >= 0 && chessCells[y - 1][x - 1].GetNodeAccessible())
            {
                neighbourList.Add(chessCells[y - 1][x - 1]);
            }


            return neighbourList;
        }
    }
    public class PathNode
    {
        public PathNode(int positionX, int positionY)
        {
            Position = new Vector2Int(positionX, positionY);
        }

        public Vector2Int Position { get; set; }
        public PathNode cameFromNode;

        public int Cost;

        private bool isAccessible = true;

        public bool GetNodeAccessible()
        {
            return isAccessible;
        }
        public void SetNodeAccessible(bool isAccessible)
        {
            this.isAccessible = isAccessible;
        }

    }
}