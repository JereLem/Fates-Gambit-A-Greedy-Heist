using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class DotManager : MonoBehaviour
{
    public List<RectTransform> childRectTransforms = new List<RectTransform>();

    public List<Dot> dots = new List<Dot>();

    public int rows = 6;
    public int columns = 6;

    private int[,] adjacentIndices;

    void OnEnable()
    {
        CollectChildRectTransforms();

        Dot[] childDots = GetComponentsInChildren<Dot>();
        foreach (Dot dot in childDots)
        {
            dot.dotType = DotType.NonActive;
            dot.isConnected = false;
            dots.Add(dot);
        }

        int start = 1;
        int end = 1;

        List<Dot> shuffledDots = new List<Dot>(dots);
        shuffledDots.Shuffle(); 

        for (int i = 0; i < start; i++)
        {
            shuffledDots[i].dotType = DotType.Start;
        }

        for (int i = start; i < start + end; i++)
        {
            shuffledDots[i].dotType = DotType.End;
        }

        int activeDotCount = 28; 
        for (int i = start + end; i < start + end + activeDotCount; i++)
        {
            shuffledDots[i].dotType = DotType.Active;
        }
    }

    void CollectChildRectTransforms()
    {
        childRectTransforms.Clear(); 

        foreach (Transform child in transform)
        {
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                childRectTransforms.Add(rectTransform);
            }
        }
        // Remove the line information
        childRectTransforms.RemoveAt(childRectTransforms.Count - 1);
    }

    void Start()
    {
        ComputeAdjacentIndices();
        //PrintAdjacentPoints();
    }

    private void Update()
    {
        CheckDotStatus();
    }
    private void CheckDotStatus()
    {
        int connectedTargetCount = 0; 

        foreach (var dot in dots)
        {
            if (dot.dotType == DotType.Start && dot.isConnected || dot.dotType == DotType.End && dot.isConnected)
            {
                connectedTargetCount++;
            }
        }

        if (connectedTargetCount == 2)
        {
            Debug.Log("Success mission!!");
        }
    }

    void ComputeAdjacentIndices()
    {
        adjacentIndices = new int[rows * columns, 4]; 

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;

                adjacentIndices[index, 0] = (i > 0) ? ((i - 1) * columns + j) : -1; // Up
                adjacentIndices[index, 1] = (i < rows - 1) ? ((i + 1) * columns + j) : -1; // Down 
                adjacentIndices[index, 2] = (j > 0) ? (i * columns + j - 1) : -1; // Left
                adjacentIndices[index, 3] = (j < columns - 1) ? (i * columns + j + 1) : -1; // Right
            }
        }
    }


    void PrintAdjacentPoints()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;
                Debug.Log("점 [" + i + ", " + j + "]의 인접한 점들:");

                for (int k = 0; k < 4; k++)
                {
                    int adjacentIndex = adjacentIndices[index, k];
                    if (adjacentIndex != -1)
                    {
                        int adjacentRow = adjacentIndex / columns;
                        int adjacentCol = adjacentIndex % columns;
                        Debug.Log("  - [" + adjacentRow + ", " + adjacentCol + "]");
                    }
                }
            }
        }
    }

    public List<int> GetAdjacentIndices(int rowIndex, int colIndex)
    {
        List<int> adjacentIndexList = new List<int>();

        int index = rowIndex * columns + colIndex;

        for (int i = 0; i < 4; i++)
        {
            int adjacentIndex = adjacentIndices[index, i];
            if (adjacentIndex != -1)
            {
                adjacentIndexList.Add(adjacentIndex);
            }
        }

        return adjacentIndexList;
    }
}

