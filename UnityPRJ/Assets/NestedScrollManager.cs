using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;
    public Transform contentTr;
    public Slider tabSlider;
    public RectTransform[] BtnRect, BtnImageRect;

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
        if (Time.time < 0.1f) return;
        
        tabSlider.value = scrollbar.value;

        if (!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
            for(int i = 0; i < SIZE; i++)
            {
                BtnRect[i].sizeDelta = new Vector2(i == targetIndex ? 360 : 180, BtnRect[i].sizeDelta.y);
            }
        }

        for(int i = 0; i < SIZE; i++)
        {
            Vector3 BtnTargetPos = BtnRect[i].anchoredPosition3D;
            Vector3 BtnTargetScale = Vector3.one;
            bool textActive = false;

            if(i== targetIndex)
            {
                BtnTargetPos.y = -23f;
                BtnTargetScale = new Vector3(1.2f, 1.2f, 1);
                textActive = true;
            }

            BtnImageRect[i].anchoredPosition3D = Vector3.Lerp(BtnImageRect[i].anchoredPosition3D, BtnTargetPos, 0.25f);
            BtnImageRect[i].localScale = Vector3.Lerp(BtnImageRect[i].localScale, BtnTargetScale, 0.25f);
            BtnImageRect[i].transform.GetChild(0).gameObject.SetActive(textActive);
        }
    }

    public void TabClick(int n)
    {
        targetIndex = n;
        targetPos = pos[n];
    }
}
