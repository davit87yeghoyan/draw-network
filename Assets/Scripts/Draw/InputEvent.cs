using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Draw
{
    public sealed class InputEvent:MonoBehaviour,IDragHandler
    {
        public event Action<bool> InputDown;
        public event Action<PointerEventData> InputDrag;

      
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnInputDown(true);

            }else if (Input.GetMouseButtonUp(0))
            {
                OnInputDown(false);
            }
            
        }


        private void OnInputDown(bool obj)
        {
            InputDown?.Invoke(obj);
        }

        private void OnInputDrag(PointerEventData eventData)
        {
            InputDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnInputDrag(eventData);
        }
    }
}