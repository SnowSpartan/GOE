using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    public Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closeList;

    public Pathfinding(int width, int height)
    {
        Instance = this;
        grid = new Grid<PathNode>(width, height, 5f, new Vector3(-75, 0, -75), (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    /*public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if(path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one + grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }*/
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        openList = new List<PathNode> { startNode };
        closeList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if(currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach(PathNode neighborNode in GetNeighborList(currentNode))//This checks neighbors of an area, so could maybe be used to check if you can be placed in a neighbor of endpoint or if player/enemy is next to them
            {//could maybe use getneighborlist code + contains to do enemy attack
                if (closeList.Contains(neighborNode))
                {
                    continue;
                }
                if (!neighborNode.isWalkable)
                {
                    closeList.Add(neighborNode);
                    continue;
                }

                int tentativeCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);
                if(tentativeCost < neighborNode.gCost)
                {
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();
                }

                if (!openList.Contains(neighborNode))
                {
                    openList.Add(neighborNode);
                }
            }
        }
        //ran out of nodes
        return null;
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        if(currentNode.x - 1 >= 0)
        {
            neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));

            if (currentNode.y - 1 >= 0)
            {
                neighborList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            }
            if (currentNode.y + 1 < grid.GetHeight())
            {
                neighborList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
        }
        if (currentNode.x + 1 >= 0)
        {
            neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));

            if (currentNode.y - 1 >= 0)
            {
                neighborList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            }
            if (currentNode.y + 1 < grid.GetHeight())
            {
                neighborList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
        }
        if (currentNode.y - 1 >= 0)
        {
            neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));
        }
        if (currentNode.y + 1 >= 0)
        {
            neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));
        }

        return neighborList;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetValue(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();

        while(path.Count > 2)//if target is more than 2 spaces away, move only the first three of those spaces. A temporary solution for the moment.
        {
            path.RemoveAt(path.Count - 1);
        }
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
