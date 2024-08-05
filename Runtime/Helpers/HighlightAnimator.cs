using ReupVirtualTwin.helperInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class HighlightAnimator : MonoBehaviour, IHighlightAnimator
    {
        HaloApplier haloApplier;
        public float TIME_TO_HIGHLIGHT_IN_SECS = 0.4f;
        public float TIME_TO_UNHIGHLIGHT_IN_SECS = 1.0f;
        public float INITIAL_INTENSITY = 0.0f;
        public float FINAL_INTENSITY = 0.015f;

        public HighlightAnimator()
        {
            haloApplier = new HaloApplier();
        }

        public void HighlighObjectsEaseInEaseOut(List<GameObject> objs)
        {
            StartCoroutine("DoAnimation", objs);
        }

        private IEnumerator DoAnimation(List<GameObject> objs)
        {
            yield return StartCoroutine("HighlightObjectsCoroutine", objs);
            yield return StartCoroutine("UnhighlightObjectsCoroutine", objs);
            UnhighlightObject(objs);
        }

        private IEnumerator HighlightObjectsCoroutine(List<GameObject> objs)
        {
            haloApplier.emissionIntensity = INITIAL_INTENSITY;
            var endTime = Time.time + TIME_TO_HIGHLIGHT_IN_SECS;
            float intensityDistance = FINAL_INTENSITY - haloApplier.emissionIntensity;
            while (Time.time < endTime)
            {
                float deltaIntensity = intensityDistance * Time.deltaTime / (TIME_TO_HIGHLIGHT_IN_SECS);
                haloApplier.emissionIntensity += deltaIntensity;
                HighlightObject(objs);
                yield return null;
            }
        }
        private IEnumerator UnhighlightObjectsCoroutine(List<GameObject> objs)
        {
            var endTime = Time.time + TIME_TO_UNHIGHLIGHT_IN_SECS;
            float intensityDistance = haloApplier.emissionIntensity - INITIAL_INTENSITY;
            while (Time.time < endTime)
            {
                float deltaIntensity = intensityDistance * Time.deltaTime / (TIME_TO_HIGHLIGHT_IN_SECS);
                haloApplier.emissionIntensity -= deltaIntensity;
                HighlightObject(objs);
                yield return null;
            }
        }
        private void HighlightObject(List<GameObject> objs)
        {
            foreach (var obj in objs)
            {
                haloApplier.HighlightObject(obj);
            }
        }
        private void UnhighlightObject(List<GameObject> objs)
        {
            foreach (var obj in objs)
            {
                haloApplier.UnhighlightObject(obj);
            }
        }
    }
}
