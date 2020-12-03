using System;
using Unity.Collections;
using UnityEngine;

namespace Draw
{
    public class Texture2DDrawingHelper
    {
        private NativeArray<Color32> _textureData;
        private readonly Texture2D _texture2D;
        private int _maskTextureWidth, _maskTextureHeight;
        private readonly Color32[] _circlePixels;
        private readonly int _radius, _width, _height;

        public delegate Color32? GetColor(int index);
        
        


        public Texture2DDrawingHelper(Texture2D texture2D, int radius)
        {
            _radius = radius;
            _texture2D = texture2D;
            _textureData = _texture2D.GetRawTextureData<Color32>();
            _circlePixels = GetCirclePixels(radius);
            _width = _texture2D.width;
            _height = _texture2D.height;
        }

        public void FillAll(Color32 color32)
        {
            for (int i = 0; i < _textureData.Length; i++)
            {
                _textureData[i] = color32;
            }

            _texture2D.Apply();
        }


      


        public void Fill(Vector2 from, Vector2 to, GetColor getColor)
        {
            
            //cache the pixels forming the circle in an array, so we don't have to compute it every time

            DrawCircleFillInternal((int)from.x , (int)from.y , _radius , _circlePixels, getColor);
            if(from == to)
            {
                _texture2D.Apply();
                return;
            }
            DrawCircleFillInternal((int)to.x , (int)to.y , _radius , _circlePixels, getColor);

            float distance = Vector2.Distance(from, to);
            
            // if distance > _radius*3 create new step
            int steps = (int) distance / (_radius*3)+1;
            float lerp = (distance / steps)/distance;

            for (int i = 0; i < steps; i++)
            {
                var newFrom = Vector2.Lerp(@from,to,lerp*(i));
                var newTo = Vector2.Lerp(@from,to,lerp*(i+1));
                DrawPolygonInternal(newFrom,newTo,getColor);
            }
            
            _texture2D.Apply();
        }



        private void DrawPolygonInternal(Vector2 from, Vector2 to,GetColor getColor)
        {
            Vector2 dir = from - to;
            if (to.y < from.y)
            {
                dir = to - from;
            }
            else
            {
                dir = from - to;
            }
            float angle = Vector2.Angle(dir, Vector2.left) * Mathf.Deg2Rad + Mathf.PI / 2;


            float cos = _radius*Mathf.Cos(angle);
            float sin = _radius*Mathf.Sin(angle);
         
            Vector2[] shape =
            {
                new Vector2 (from.x+cos,from.y+sin),
                new Vector2 (from.x-cos,from.y-sin),
                new Vector2 (to.x-cos,to.y-sin),
                new Vector2 (to.x+cos,to.y+sin),
            };
          
            
            DrawPolygonInternal(shape, getColor);
        }
        
        
        private Color32[] GetCirclePixels(int diameter)
        {
            int size = (diameter * 2 + 1) * (diameter * 2 + 1);
            Color32[] circlePixels = new Color32[size];
            for (int i = 0; i < size; i++)
            {
                circlePixels[i] = new Color32(0, 0, 0, 0);
            }
            for (int i = -diameter; i <= diameter; i++)
            {
                int d = Mathf.CeilToInt(Mathf.Sqrt(diameter * diameter - i * i));
                for (int j = -d; j <= d; j++)
                {
                    circlePixels[(j + diameter) * (2 * diameter + 1) + i + diameter] = new Color32(0, 0, 0, 1);
                }
            }
            return circlePixels;
        }
        
        private void DrawCircleFillInternal(int x, int y, int radius, Color32[] circlePixels, GetColor getColor)
        {
            for (int i = -radius; i <= radius; i++)
            {
                if (x + i < 0 || x + i >= _width)
                    continue;
                for (int j = -radius; j <= radius; j++)
                {
                    if (y + j < 0 || y + j >= _height)
                        continue;
                    int circleIndex = (j + radius) * (2 * radius + 1) + i + radius;
                    if(circlePixels[circleIndex].a ==0) continue;
                    SetPixel(i + x, j + y,getColor);
                }
            }
        }


        public void DrawCircleFillInternal(int x, int y,GetColor getColor)
        {
            DrawCircleFillInternal(x, y, _radius, _circlePixels, getColor);
            _texture2D.Apply();
        }
       
        
        private void DrawPolygonInternal(Vector2[] points, GetColor getColor)
        {
            float minx = points[0].x;
            float miny = points[0].y;
            float maxx = points[0].x;
            float maxy = points[0].y;

            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].x < minx)
                {
                    minx = points[i].x;
                }
                if (points[i].y < miny)
                {
                    miny = points[i].y;
                }
                if (points[i].x > maxx)
                {
                    maxx = points[i].x;
                }
                if (points[i].y > maxy)
                {
                    maxy = points[i].y;
                }
            }

            var floorToInt = Mathf.FloorToInt(minx);
            for (int x = floorToInt; x <= maxx; x++)
            {
                var toInt = Mathf.FloorToInt(miny);
                for (int y = toInt; y <= maxy; y++)
                {
                    bool draw = false;
                    for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
                    {
                        if ((points[i].y > y) != (points[j].y > y) &&
                            x < (points[j].x - points[i].x) * (y - points[i].y) / (points[j].y - points[i].y) + points[i].x)
                        {
                            draw = !draw;
                        }
                    }
                    if (draw)
                    {
                        SetPixel(x, y,getColor);
                    }
                }
            }
        }

        private void SetPixel(int x, int y, GetColor getColor)
        {
            if (x < 0 || x >= _width)
                return;
            if (y < 0 || y >= _height)
                return;
            
            int index = GetIndex(x, y);
            Color32? color32 = getColor(index);
            if (color32 != null)
            {
                _textureData[index] = (Color32)color32;
            }
        }
        
        private int GetIndex(int x, int y)
        {
            return y * _width + x;
        }

        public int GetDataLength()
        {
            return _textureData.Length;
        }
    }
}