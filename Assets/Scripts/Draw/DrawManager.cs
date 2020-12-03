using System;
using System.Collections.Generic;
using System.Linq;
using Networks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Draw
{
    public class DrawManager : MonoBehaviour
    {
        public static DrawManager Instance;
        public int circleRadius = 5;
        public Color32[] colorLayers;

        public event Action Clear;
        public event Action<Vector2> DrawCircle;
        public event Action<Vector2,Vector2> DrawDrag;
        public event Action InputDownStatus;
        
        
        
        private Vector2 _textureSize;
        private RawImage _rawImage;
        private Texture2D _texture2D;
        
       

        private Texture2DDrawingHelper _texture2DDrawingHelper;
        private InputEvent _inputEvent;
        private bool[] _updatedPosIndex;
        private readonly Dictionary<Player, bool[]> _updatedPosIndexNetwork = new Dictionary<Player, bool[]>();
        /// <summary>
        /// pxIndex=>colorIndex
        /// </summary>
        private int[] _indexData;
        private Dictionary<Color32, int> _colorLayersRel;
        private RectTransform _rectTransform;

        private Vector2 _from, _to;
        private bool _draw;

        private void Awake()
        {
            SetComponents();
            InitInputEvent();
            EnableDrawing(false);
            NetworkManager.Instance.JoinRoom += JoinRoom;
        }

        private void JoinRoom()
        {
            EnableDrawing(true);
        }


        // Start is called before the first frame update
        void Start()
        {
            SetTextureProps();
            CreateTexture2DHelper();
            SetNewUpdatedPosIndex();
            ClearTexture();
        }

        private void OnEnable()
        {
            Instance = this;
        }
        
        private void Update()
        {
            if(!_draw) return;
            _draw = false;
            InputDrag(_from,_to);
        }
        
        
        
      
        

        private void CreateTexture2DHelper()
        {
            _texture2DDrawingHelper = new Texture2DDrawingHelper(_texture2D,circleRadius);
            _indexData =  new int[_texture2DDrawingHelper.GetDataLength()];
        }

        private void SetComponents()
        {
            _rawImage = gameObject.GetComponent<RawImage>();
            _rectTransform = _rawImage.rectTransform;
        }
        private void InitInputEvent()
        {
            _inputEvent = gameObject.GetComponent<InputEvent>();
            _inputEvent.InputDown += InputDown;
            _inputEvent.InputDrag += InputDrag;
        }


        private void SetTextureProps()
        {
            var rectTransformRect = _rectTransform.rect;
            _textureSize = new Vector2(rectTransformRect.width,rectTransformRect.height);
            _texture2D = new Texture2D((int)_textureSize.x, (int)_textureSize.y);
            _rawImage.texture = _texture2D;
        }
        
        public void ClearTexture(bool isNetwork = false)
        {
            int index = 0;
            
            _texture2DDrawingHelper.FillAll(colorLayers[index]);
            for (int i = 0; i < _indexData.Length; i++)
            {
                _indexData[i] = index;
            }
            SetNewUpdatedPosIndex();

            if (!isNetwork)
            {
                Clear?.Invoke();
            }
        }

        private void InputDown(bool downed)
        {
            if (downed)
            {
                GetRectPosition(Input.mousePosition,out var pos);
                InputDown(pos);
                return;
            }
            InputDownStatus?.Invoke();
            SetNewUpdatedPosIndex();
        }

        public void SetInputDownStatus(Player player)
        {
            SetNewUpdatedPosIndexNetwork(player);
        }

        public void InputDown(Vector2 pos,Player player = null)
        {
            if (player != null)
            {
                Color32? SetColor(int index)
                {
                    return TrySetColor(index, GetSetNewUpdatedPosIndexNetwork(player));
                }
                _texture2DDrawingHelper.DrawCircleFillInternal((int)pos.x, (int)pos.y, SetColor);
                return;
            }
            
            _texture2DDrawingHelper.DrawCircleFillInternal((int)pos.x, (int)pos.y, TrySetColorMaster);
            DrawCircle?.Invoke(pos);
        }

        private void InputDrag(PointerEventData eventData)
        {
           
            if (!GetRectPosition(eventData.position - eventData.delta, out _from,eventData.pressEventCamera))
            {
                return;
            }
            if (!GetRectPosition(eventData.position, out _to,eventData.pressEventCamera))
            {
                return;
            }
            _draw = true;
        }


        public void InputDrag(Vector2 from, Vector2 to,Player player = null)
        {
            if (player != null)
            {
                Color32? SetColor(int index)
                {
                    return TrySetColor(index, GetSetNewUpdatedPosIndexNetwork(player));
                }
                _texture2DDrawingHelper.Fill(from,to, SetColor);
                return;
            }
            
            _texture2DDrawingHelper.Fill(from,to, TrySetColorMaster);
            DrawDrag?.Invoke(from,to);
        }

        private bool GetRectPosition(Vector3 pos, out Vector2 localPoint, Camera cam = null)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, pos, cam, out localPoint))
            {
                return false;
            }
            
            localPoint.x += _rectTransform.pivot.x * _rectTransform.rect.width;
            localPoint.y += _rectTransform.pivot.y * _rectTransform.rect.height;
            
            return true;
        }

             


        private Color32? TrySetColorMaster(int index)
        {
            return TrySetColor(index,_updatedPosIndex);
        }
        
       
        
        private Color32? TrySetColor(int index, bool[] updatedPosIndex)
        {
            if (updatedPosIndex[index])
            {
                return null;
            }
            
            int i =  _indexData[index];
            
            if (++i == colorLayers.Length)
            {
                return null;
            }

            _indexData[index] = i;
            updatedPosIndex[index] = true;
            return colorLayers[i];
        }


       

        private void EnableDrawing(bool enable)
        {
            gameObject.GetComponent<InputEvent>().enabled = enable;
        }

        private void SetNewUpdatedPosIndex()
        {
            _updatedPosIndex = new bool[_texture2DDrawingHelper.GetDataLength()];
        }
        
        private void SetNewUpdatedPosIndexNetwork(Player player)
        {
            _updatedPosIndexNetwork[player] = new bool[_texture2DDrawingHelper.GetDataLength()];
        }

        private bool[] GetSetNewUpdatedPosIndexNetwork(Player player)
        {
            if (!_updatedPosIndexNetwork.ContainsKey(player))
            {
                SetNewUpdatedPosIndexNetwork(player);
            }
            return _updatedPosIndexNetwork[player];
        }
        

      

       
    }
}
