using System.Collections.Generic;
using UnityEngine;
using Main.ConstValues;
using Main.Checking;
using Main.Map;

namespace Main
{
    public class Merge : MonoBehaviour
    {
        internal static List<GameObject> LineMerge = new List<GameObject>();
        InitializationMap im;

        public GameObject MergedObject; // у каждого объекта задаетс€ объект после мерджа
        Transform Block1;
        Transform Block2;
        public float Distance;   //  дл€ плавности мерджа,
        public float MergeSpeed; // элементы аккуратно прит€гиваютс€ друг к другу. работало пока не прикрутила центрирование к гриду

        bool CanMerge;
        bool MouseReleased;

        private void FixedUpdate() // всегда пытаемс€ померджить
        {
            im = FindObjectOfType<InitializationMap>();
            //MoveTowards();
        }

        public void MoveTowards() // возможно нет необходимости в дополнительной функции и ее можно перенести в OnCollisionStay2D?
        {
            if (CanMerge)
            {
                transform.position = Vector2.MoveTowards(Block1.position, Block2.position, MergeSpeed);
                GameObject O = Instantiate(MergedObject, transform.position, Quaternion.identity);// спавним новый объект
                O.GetComponent<ObjectData>().currentPosition = Block1.gameObject.GetComponent<ObjectData>().currentPosition;
                gameObject.GetComponent<ObjectData>().currentPosition.GetComponent<GridData>().isEmpty = true;
                Block2.gameObject.GetComponent<ObjectData>().currentPosition.GetComponent<GridData>().isEmpty = true;
                Destroy(Block2.gameObject);// уничтожаем те, что
                Destroy(gameObject);       //    смерджились
            }
        }

        private void OnCollisionStay2D(Collision2D collision) // пока один объект касаетс€ другого
        {
            if (MouseReleased) // если мышь отпустили
            {
                if (collision.gameObject.CompareTag(Tags.MergeBlock)) // если объект касаетс€ объекта, который может смерджитьс€
                {
                    if (collision.gameObject.GetComponent<SpriteRenderer>().sprite == GetComponent<SpriteRenderer>().sprite)
                    {
                        Block1 = transform;
                        Block2 = collision.transform;
                        CanMerge = true;
                    }
                }
                MouseReleased = false; // как только проверили, можно ли померджить, мышь снова в позиции "не отпустили"
            }
        }

        void OnMouseUp()
        {
            MouseReleased = true;
            CellEditing.ChangeObj(gameObject);
            im.FindFriendsCell(Input.mousePosition);
        }
    }
}