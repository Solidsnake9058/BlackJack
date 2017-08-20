using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    public SpriteRenderer cardFace;
    public SpriteRenderer cardBack;
    public Sprite[] cardFaces;
    public Sprite[] cardBacks;
    public int cardIndex = 1;
    public float moveSpeed = 10f;

    private bool faceOn = true;
    public bool setToPos = false;
    public Vector3 settingPos = new Vector3();

    public Card() { }

    public bool GetIsFaceOn
    {
        get { return faceOn; }
    }

    public List<int> point
    {
        get {
            int point = cardIndex % 13;
            List<int> points = new List<int>();
            switch (point)
            {
                case 1:
                    points.Add(1);
                    points.Add(11);
                    break;
                case 10:
                case 11:
                case 12:
                case 0:
                    points.Add(10);
                    break;
                default:
                    points.Add(point);
                    break;
            }
            return points;
        }
    }

    public int hlPoint
    {
        get
        {
            int point = cardIndex % 13;
            int hlPoint = 0;

            switch (point)
            {
                case 1:
                case 10:
                case 11:
                case 12:
                case 0:
                    hlPoint = -1;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    hlPoint = 1;
                    break;
                default:
                    break;
            }
            return hlPoint;
        }
    }

    public void SetCardFace()
    {
        cardFace.sprite = cardFaces[cardIndex];
    }

    public void TurnCard()
    {
        transform.Rotate(0, 180, 0);
        faceOn = !faceOn;
    }

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (setToPos && Vector3.Distance(transform.position, settingPos) > 0.1f)
        {
            transform.position += (settingPos - transform.position).normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            if (setToPos)
            {
                transform.position = settingPos;
                setToPos = false;
            }
        }

        //SetCardFace();
    }
}
