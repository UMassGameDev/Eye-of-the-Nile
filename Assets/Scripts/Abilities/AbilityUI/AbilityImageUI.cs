using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityImageUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public BaseAbilityInfo CurAbilityInfo { get; set; }
    public Image CurImage { get; set; }

    [HideInInspector] public Transform previousParent;
    [HideInInspector] public Transform nextParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CurImage.raycastTarget = false;
        transform.Find("AbilitySubIconUI").GetComponent<Image>().raycastTarget = false;
        previousParent = transform.parent;
        nextParent = previousParent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CurImage.raycastTarget = true;
        transform.Find("AbilitySubIconUI").GetComponent<Image>().raycastTarget = true;
        transform.SetParent(nextParent);
    }

    void Awake()
    {
        CurImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
