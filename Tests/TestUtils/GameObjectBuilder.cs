using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectBuilder
{
    public static GameObject CreateGameObjectHierarchy(string input)
    {
        if (!IsBalanced(input))
        {
            throw new ArgumentException("Parentheses are not well structured.");
        }

        return BuildHierarchy(input);
    }

    private static GameObject BuildHierarchy(string input)
    {
        int objectsCount = 0;
        if (input == "")
        {
            return new GameObject($"{objectsCount++}");
        }

        GameObject root = new GameObject($"{objectsCount++}");

        GameObject currentParent = root;
        Stack<GameObject> parentStack = new Stack<GameObject>();

        foreach (char c in input)
        {
            if (c == '(')
            {
                GameObject child = new GameObject($"{objectsCount++}");
                child.transform.parent = currentParent.transform;
                parentStack.Push(currentParent);
                currentParent = child;
            }
            else if (c == ')')
            {
                currentParent = parentStack.Pop();
            }
        }
        return root;
    }

    private static bool IsBalanced(string input)
    {
        int balance = 0;
        foreach (char c in input)
        {
            if (c == '(')
            {
                balance++;
            }
            else if (c == ')')
            {
                balance--;
            }
            if (balance < 0)
            {
                return false;
            }
        }
        return balance == 0;
    }
}