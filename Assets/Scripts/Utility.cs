﻿using System.Collections;

public class Utility {

     public static T[] ShuffleArray<T>(T[] array, int seed) {

        System.Random rnd = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randIndex = rnd.Next(i, array.Length);

            T tempItem = array[randIndex];
            array[randIndex] = array[i];
            array[i] = tempItem;

        }

        return array;
    }

}
