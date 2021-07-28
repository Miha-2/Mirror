using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;
using Random = UnityEngine.Random;

[RequireComponent(typeof(DecalProjector))]
public class BulletHole : SelfDestroy
{
   [SerializeField] private DecalProjector decalProjector;
   [SerializeField] private float fadeLenght = 1f;
   private float _totalTime;
   private float _timer;
   private bool _canTime;
   [SerializeField] private float _defaultFade;

   private void OnValidate()
   {
      fadeLenght = Mathf.Clamp(fadeLenght, 0f, destructionTime);
      if (!decalProjector)
         decalProjector = GetComponent<DecalProjector>();
      _defaultFade = decalProjector.fadeFactor;
   }

   protected override void Start() { }

   private UnityEvent _currentEvent;
   
   public void Activate(IHittable hittable)
   {
      gameObject.SetActive(true);
      NetworkServer.Spawn(gameObject);

      if (_canTime)
         RpcUpdateLocation(transform.position, transform.rotation);
      
      _currentEvent = hittable.OnDestroy;
      _currentEvent.AddListener(OnObjectDestroyed);

      _canTime = true;
      _timer = 0f;
      _totalTime = destructionTime + Random.Range(0f, additionalRandomDelay);
      StartFade(_totalTime);
   }

   [ClientRpc]
   private void RpcUpdateLocation(Vector3 position, Quaternion rotation)
   {
      transform.position = position;
      transform.rotation = rotation;
   }
   
   
   [ClientRpc]
   private void StartFade(float lenght)
   {
      _canTime = true;
      _timer = 0f;
      _totalTime = lenght;
   }

   private void OnObjectDestroyed()
   {
      Debug.Log("OnDestroy invoked");
      _currentEvent.RemoveListener(OnObjectDestroyed);
      DestroyHole();
   }
   
   private void Update()
   {
      //Running timer (server and client)
      if(_canTime)
         _timer += Time.deltaTime;
      
      //Fading hole (on a client)
      if (isClient && _canTime)
      {
         float fadeDelta = Mathf.Clamp01((_totalTime - _timer) / fadeLenght);
         decalProjector.fadeFactor = Mathf.Lerp(0f, _defaultFade, fadeDelta);
      }

      //Unspawning hole from a server
      //Hiding it on a server
      if (isServer && _timer >= _totalTime)
         DestroyHole();
   }

   private void DestroyHole()
   {
      decalProjector.fadeFactor = 0f;
      _timer = 0f;
      _canTime = false;
      NetworkServer.UnSpawn(gameObject);
      gameObject.SetActive(false);
   }
}
