using System.Collections.Generic;
using MapUI;
using UnityEngine;
using UnityEngine.UI;
using Visualization;

namespace OldMapUI
{
    public class OldTimelineSlider : MonoBehaviour
    {
        public GameObject cloudManager;
        public Slider slider;

        public Text text;
        public float playbackRate = 0.5f;

        public bool isPlaying = false;
        private float time =  0;
        private float prevTime = 0;
        private List<Renderer> _cloudRenderers = new();
        private CloudManager _cloudManager;

        private static int NumSteps(float prev, float current){
            return Mathf.FloorToInt(current) - Mathf.FloorToInt(prev);
        }

        // Start is called before the first frame update
        void Start()
        {
            _cloudManager = cloudManager.GetComponent<CloudManager>();

            LOD[] lods = gameObject.GetComponent<LODGroup>().GetLODs();
            foreach (LOD lod in lods)
            {
                foreach (Renderer ren in lod.renderers)
                {
                    _cloudRenderers.Add(ren);
                    ren.material.SetFloat("_ColorMapAlpha", time % 1);
                }
            }

            slider.minValue = 0;
            slider.maxValue = _cloudManager.MapCount - 1.01f;
            slider.value = 0;
            slider.onValueChanged.AddListener(delegate {OnSliderChange ();});
            text.text = slider.value.ToString("F2");

            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat("_ColorMapAlpha", time % 1);
            }
            //ChangeTime(slider.value);
        }

        // Update is called once per frame
        void Update()
        {
            //slider.value = floatSlider;

            if(slider.maxValue < 1){
                slider.maxValue = _cloudManager.MapCount - 1.01f;
                //ChangeTime(slider.value);
            }

            if(isPlaying){
                slider.value += playbackRate * Time.deltaTime;
                if(slider.value >= slider.maxValue){
                    Pause();
                }
            }
        }

        public void Play(){
            // Reset slider when replaying
            if(slider.value >= slider.maxValue - 0.2f){
                slider.value = 0.0f;
            }
            isPlaying = true;
        }

        public void Pause() {
            isPlaying = false;
        }

        public void OnSliderChange(){
            //Debug.Log("Slider value: " + slider.value);
            ChangeTime(slider.value);
            text.text = slider.value.ToString("F2");
        }

        public bool IsPlaying(){
            return isPlaying;
        }

        private void ChangeTime(float value)
        {
            time = value; 
            if (prevTime != time)
            {
                int nSteps = NumSteps(prevTime, time);
                //Debug.Log("NSteps: " + nSteps);
                if (nSteps != 0)
                {
                    _cloudManager.UpdateTime(nSteps);
                }
                prevTime = time;
            }

            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat("_ColorMapAlpha", time % 1);
            }
        }
    }
}