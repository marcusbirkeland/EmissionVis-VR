using System;
using UnityEngine;

namespace Tutorial
{
    /// <summary>
    /// A serializable class representing a single tutorial slide.
    /// </summary>
    [Serializable]
    public class TutorialSlide
    {
        public string title = "Title";
        public string description = "Description";
        public Sprite tutorialImage;
    }
}