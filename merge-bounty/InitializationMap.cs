using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Main.ConstValues;
using Main.Map;

namespace Main.Checking
{
    public class InitializationMap : MonoBehaviour
    {
        //private static float Radius = 24;
        internal static List<CellEditing> LineMerge = new List<CellEditing>();
        static public int n = 10, k = 10;
        public CellEditing[,] mass = new CellEditing[n, k];


        private void Start()
        {
            LineMerge.Clear();
            CheckForThreeple();
        }

        public void CheckForThreeple()
        {
            LineMerge.Clear();
            CellEditing[] q = FindObjectsOfType<CellEditing>();
            
            foreach (CellEditing item in q)
            {
                if (item.GetComponent<CellEditing>())
                {
                    LineMerge.Add(item);
                }
            }

            Debug.Log($"Объектов: {LineMerge.Count}");


            var lineX = LineMerge.OrderBy(x => x.transform.position.x).Select(x=>x.transform.position.x).Distinct().ToArray();
            var lineY = LineMerge.OrderBy(x => x.transform.position.y).Select(x => x.transform.position.y).Distinct().ToArray();
            for (int i=0;i<n ;i++)
            {
                for (int j = 0; j < k; j++)
                {
                    mass[i, j] = LineMerge.Where(x => x.transform.position.x == lineX[i] && x.transform.position.y == lineY[j]).FirstOrDefault();
                }
            }
            

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var curr = LineMerge.Where(x => x.transform.position.x.Equals(collision.transform.position)).FirstOrDefault();
            if (curr == null)
                return;
            List<CellEditing> friends = new List<CellEditing>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    if (mass[i, j].transform.position.Equals(curr.transform.position))
                    {
                        friends.AddRange(new CellEditing[]{ GetFriend(i - 1, j), GetFriend(i, j - 1), GetFriend(i + 1, j),
                                         GetFriend(i, j + 1), GetFriend(i, j - 1), GetFriend(i + 1, j + 1),
                                         GetFriend(i - 1, j - 1) });
                    }
                }
            }

        }
        private CellEditing GetFriend(int i, int j)
        {
            if (i < 0 || j < 0 || 
                i > n || j > k)
            {
                return null;
            }
            return mass[i, j];
        }
    }
}