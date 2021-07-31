using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
// ReSharper disable Unity.InefficientPropertyAccess

public class Minimap : MonoBehaviour
{
    [SerializeField] private Camera mapCamera;
    [SerializeField] private RectTransform bigMap;
    [SerializeField] private RectTransform mapGraphic;
    [SerializeField] private Image ownerPointer;
    [SerializeField] private Transform pointerRoot;
    [SerializeField] private Pointer pointerPrefab;
    [SerializeField] private BigPointer bigPointerPrefab;
    private RectTransform _rectTransform;

    private float _zoom = 2.5f;
    public float Zoom
    {
        get => _zoom;
        set
        {
            mapGraphic.transform.localScale = Vector3.one * value; 
            _zoom = value;
        }
    }

    private void Start()
    {
        _rectTransform = transform as RectTransform;
        
        mapCamera.Render();

        GameSystem.InputManager.PlayerInput.UiActions.Map.performed += MapOnPerformed;

        GameSystem.InputManager.PlayerInput.UiActions.Map.canceled += MapOnPerformed;
    }

    private void MapOnPerformed(InputAction.CallbackContext obj) => bigMap.gameObject.SetActive(obj.performed);

    private void OnChange()
    {
        
    }

    private Transform _ownerTransform;
    private readonly List<PointerPair> pointerPairs = new List<PointerPair>();

    [Serializable]
    private struct PointerPair
    {
        public Transform TargetTransform;
        public Pointer Pointer;
        public BigPointer BigPointer;
    }
    
    public void AddPointer(Transform targetTransform, Color pointerColor, bool isOwner)
    {
        Pointer pointer = Instantiate(pointerPrefab, pointerRoot);
        pointer.IsOwner = isOwner;
        pointer.Color = pointerColor;

        BigPointer bigPointer = Instantiate(bigPointerPrefab, bigMap);
        bigPointer.Image.color = pointerColor;

        if (isOwner)
        {
            _ownerTransform = targetTransform;
            ownerPointer.color = pointerColor;
        }

        pointerPairs.Add(new PointerPair { Pointer = pointer, BigPointer = bigPointer, TargetTransform = targetTransform});
    }

    private void LateUpdate()
    {
        if (_ownerTransform)
        {
            mapGraphic.parent.rotation = Quaternion.Euler(0f, 0f, _ownerTransform.rotation.eulerAngles.y);
            pointerRoot.rotation = Quaternion.Euler(0f, 0f, _ownerTransform.rotation.eulerAngles.y);

            Vector2 normalizedPosition = new Vector2(_ownerTransform.position.x - mapCamera.transform.position.x,
                _ownerTransform.position.z - mapCamera.transform.position.z) / mapCamera.orthographicSize * Zoom;
            
            mapGraphic.anchoredPosition = -normalizedPosition * mapGraphic.rect.height / 2;
        }
        else
        {
            mapGraphic.parent.rotation = Quaternion.identity;
            pointerRoot.rotation = Quaternion.identity;
            mapGraphic.anchoredPosition = Vector2.zero;
        }
        
        
        foreach (PointerPair pointerPair in pointerPairs)
        {
            if (pointerPair.TargetTransform == null)
            {
                Destroy(pointerPair.Pointer);
                Destroy(pointerPair.BigPointer);
                
                pointerPairs.Remove(pointerPair);
                
                continue;
            }


            Vector2 playerNormalized =
                new Vector2(pointerPair.TargetTransform.position.x - (_ownerTransform ? _ownerTransform.position.x : 0f),
                    pointerPair.TargetTransform.position.z - (_ownerTransform ? _ownerTransform.position.z : 0f)) / mapCamera.orthographicSize;
            
            pointerPair.Pointer.SetPosition(playerNormalized, pointerPair.TargetTransform.rotation.eulerAngles.y, Zoom, _rectTransform.rect.height);


            Vector2 normalized = new Vector2(pointerPair.TargetTransform.position.x - mapCamera.transform.position.x,
                pointerPair.TargetTransform.position.z - mapCamera.transform.position.z) / mapCamera.orthographicSize;
            pointerPair.BigPointer.SetPosition(normalized, pointerPair.TargetTransform.rotation.eulerAngles.y, bigMap.rect.height);
        }
    }

    private void OnDestroy()
    {
        GameSystem.InputManager.PlayerInput.UiActions.Map.performed -= MapOnPerformed;

        GameSystem.InputManager.PlayerInput.UiActions.Map.canceled -= MapOnPerformed;
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(Minimap))]
public class MinimapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        serializedObject.Update();
        
        Minimap minimap = target as Minimap;

        minimap.Zoom = EditorGUILayout.Slider("Zoom", minimap.Zoom, 1f, 5f);
        
        GUILayout.Space(12f);
        if(GUILayout.Button("Render Minimap"))
        {
            Camera cam = serializedObject.FindProperty("mapCamera").objectReferenceValue as Camera;
            cam.Render();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif