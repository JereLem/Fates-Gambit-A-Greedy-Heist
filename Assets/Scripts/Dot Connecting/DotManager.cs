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
    public List<Dot> dotsOrdered = new List<Dot>();
    public LineRenderer lineRenderer;

    public int rows = 6;
    public int columns = 6;

    private int[,] adjacentIndices;
    public bool isFirstClick = true;

    void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
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
            //Debug.Log("Success mission!!");
            GameManager.Instance.ExecuteDotGameSuccessEffects();
            DestroyMiniGame();
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

    public bool IsAdjacent(int prevIndex, int currIndex)
    {
        int upIndex = adjacentIndices[currIndex, 0];
        int downIndex = adjacentIndices[currIndex, 1];
        int leftIndex = adjacentIndices[currIndex, 2];
        int rightIndex = adjacentIndices[currIndex, 3];

        return prevIndex == upIndex || prevIndex == downIndex || prevIndex == leftIndex || prevIndex == rightIndex;
    }

    public bool SetPoint(Dot dot)
    {
        dotsOrdered.Add(dot);

        if (dotsOrdered.Count == 1)
        {
            return true;
        }
        else
        {
            int curIndex = CalculateIndex(dot);
            int prevIndex = CalculateIndex(dotsOrdered[dotsOrdered.Count - 2]);
            
            bool isAdjeacnet = IsAdjacent(prevIndex, curIndex);
            if (isAdjeacnet)
            {
                return true;
            }
            else
            {
                dotsOrdered.RemoveAt(dotsOrdered.Count - 1);
                return false;
            }
        }
    }

    public bool RemovePoint(Dot dot)
    {
        int curIndex = CalculateIndex(dot);
        int prevIndex = CalculateIndex(dotsOrdered[dotsOrdered.Count - 1]);

        if (curIndex == prevIndex)
        {
            dotsOrdered.RemoveAt(dotsOrdered.Count - 1);
            //Debug.Log("Remove successful!");
            return true;
        }
        else
            return false;
    }

    public int CalculateIndex(Dot target)
    {
        for (int i = 0; i < dots.Count; i++)
        {
            if (dots[i] == target)
            {
                return i;
            }
        }
        return -1;
    }

    void DestroyMiniGame()
    {
        // Inform GameManager that the mini-game is no longer active
        GameManager.SetMiniGameActive(false);

        // Destroy the mini-game object
        Destroy(transform.parent.parent.gameObject);
    }
}

