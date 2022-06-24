using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;
    public Transform contentTr;

    const int SIZE = 4;
    
    float[] pos = new float[SIZE];
    float distance, curPos, targetPos;
    int targetIndex;

    bool isDrag;

    // Start is called before the first frame update
    void Start()
    {
        distance = 1f / (SIZE - 1);
        for(int i = 0; i < SIZE; i++)
        {
            pos[i] = distance * i;
        }

        print(pos[0]);
        print(pos[1]);
        print(pos[2]);
        print(pos[3]);
    }

    float SetPos()
    {
        for (int i = 0; i < SIZE; i++)
        {
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        }
        return 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        curPos = SetPos();
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        targetPos = SetPos();
        // ȭ�� �� �̻� �ȳѱ⵵ �ӵ��� ���� ȭ������ �ѱ��.
        if (curPos == targetPos)
        {
            // <- �������� �ѱ� ��
            if(eventData.delta.x > 18 && targetIndex > 0)
            {
                targetPos = pos[--targetIndex];
            }
            // -> �������� �ѱ� ��
            else if (eventData.delta.x < -18 && targetIndex < SIZE - 1)
            {
                targetPos = pos[++targetIndex];
            }
        }

        for(int i = 0; i < SIZE; i++)
        {
            if(contentTr.GetChild(i).GetComponent<ScrollScript>() && curPos != pos[i] && targetPos == pos[i])
            {
                contentTr.GetChild(i).GetChild(1).GetComponent<Scrollbar>().value = 1;
            }
        }

        isDrag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
        }
    }
}
