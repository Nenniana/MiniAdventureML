using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class RandomUtilities
{
    static System.Random random = new System.Random();

    public static T GetRandomWeightedOption<T>(Dictionary<T, int> table)
    {
        int total = 0;
        int randomNumber;
        int currentFloor = 0;

        foreach (KeyValuePair<T, int> entry in table)
        {
            total += entry.Value;
        }

        randomNumber = Random.Range(0, total);

        foreach (KeyValuePair<T, int> entry in table)
        {
            if (randomNumber >= currentFloor && randomNumber <= (entry.Value + currentFloor))
            {
                return entry.Key;
            }
            else
            {
                currentFloor += entry.Value;
            }
        }

        return default(T);
    }

    public static T GetRandomWeightedOptionEqualize<T>(ref Dictionary<T, int> table, int differenceAmount)
    {
        int total = 0;
        int randomNumber;
        int currentFloor = 0;

        foreach (KeyValuePair<T, int> entry in table)
        {
            table[entry.Key] += differenceAmount;
            total += entry.Value;
        }

        randomNumber = Random.Range(0, total);

        foreach (KeyValuePair<T, int> entry in table)
        {
            if (randomNumber >= currentFloor && randomNumber <= (entry.Value + currentFloor))
            {
                table[entry.Key] -= differenceAmount;
                return entry.Key;
            }
            else
            {
                currentFloor += entry.Value;
            }
        }

        return default(T);
    }

    public static T GetRandomWeightedType<T>(Dictionary<T, int> table, Type classToCheck)
    {
        Dictionary<T, int> tempDictionary = new Dictionary<T, int>();

        foreach (var item in table)
        {
            if (item.Key.GetType() == classToCheck)
            {
                tempDictionary.Add(item.Key, item.Value);
            }
        }

        if (tempDictionary.Count > 0)
            return GetRandomWeightedOption(tempDictionary);

        return default(T);
    }

    public static T RandomEnumValue<T>()
    {
        var v = Enum.GetValues(typeof(T));
        return (T)v.GetValue(random.Next(v.Length));
    }

    public static T RandomEnumValueExcluding<T>(T type)
    {
        List<T> v = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        if (v.Contains(type))
            v.Remove(type);

        var e = v.ToArray();

        return (T)e.GetValue(random.Next(e.Length));
    }

    public static float GetWeightedChance<T>(Dictionary<T, int> table, int chance)
    {
        float total = 0;

        foreach (KeyValuePair<T, int> entry in table)
        {
            total += entry.Value;
        }

        return (chance / total) * 100f;
    }

    public static List<T> GetRandomElementsExcept<T>(this IEnumerable<T> list, int elementsCount, T elementToAvoid)
    {
        List<T> tempList = list.ToList();

        if (tempList.Contains(elementToAvoid))
            tempList.Remove(elementToAvoid);

        return tempList.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }

    public static List<T> GetRandomElementsExcept<T>(this IEnumerable<T> list, int elementsCount, IEnumerable<T> elementsToAvoid)
    {
        List<T> tempList = list.ToList();

        foreach (T element in elementsToAvoid)
        {
            if (tempList.Contains(element))
                tempList.Remove(element);
        }

        return tempList.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }

    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
    {
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }

    public static bool RandomBoolByChance(float chance)
    {
        if (Random.value > chance)
        {
            return false;
        }

        return true;
    }
}
