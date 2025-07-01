using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game
{
    public interface  IViewController
    {
        public List<Renderer> Renderers { get; }
        public MaterialPropertyBlock MPBAlpha { get; }

        public float Alpha { get; set; }
        public string SortingLayer { get; }
        public int SortingOrder { get; }

        public void ResetRendererList() { }
    }
}