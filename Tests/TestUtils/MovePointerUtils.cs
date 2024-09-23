using System.Collections;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using InSys = UnityEngine.InputSystem;

namespace ReupVirtualTwinTests.utils
{
    public static class MovePointerUtils
    {
        public static IEnumerator MoveMouse(
            InputTestFixture input,
            Mouse mouse,
            Vector2 startMousePoint,
            Vector2 endMousePoint,
            int steps
        ) {
            return MovePointer(
                startMousePoint,
                endMousePoint,
                steps,
                (Vector2 startPosition) => input.Move(mouse.position, startPosition),
                (Vector2 currentPosition, Vector2 delta) => {
                    input.Move(mouse.position, currentPosition, delta);
                    input.Set(mouse.delta, delta);
                },
                (Vector2 endPosition) => { }
            );
        }
        public static IEnumerator DragMouseLeftButton(
            InputTestFixture input,
            Mouse mouse,
            Vector2 startMousePoint,
            Vector2 endMousePoint,
            int steps
        ) {
            return MovePointer(
                startMousePoint,
                endMousePoint,
                steps,
                (Vector2 startPosition) => { input.Move(mouse.position, startPosition); input.Press(mouse.leftButton); },
                (Vector2 currentPosition, Vector2 delta) => {
                    input.Move(mouse.position, currentPosition, delta);
                    input.Set(mouse.delta, delta);
                },
                (Vector2 endPosition) => input.Release(mouse.leftButton)
            );
        }

        public static IEnumerator MoveFinger(
            InputTestFixture input,
            Touchscreen touch,
            int touchId,
            Vector2 startScreenPosition,
            Vector2 endScreenPosition,
            int steps
        ) {
            return MovePointer(
                startScreenPosition,
                endScreenPosition,
                steps,
                (Vector2 startPosition) => input.SetTouch(touchId, InSys.TouchPhase.Began, startPosition, Vector2.zero, true, touch),
                (Vector2 currentPosition, Vector2 delta) => input.SetTouch(touchId, InSys.TouchPhase.Moved, currentPosition, delta, true, touch),
                (Vector2 endPosition) => input.EndTouch(touchId, endPosition, Vector2.zero, true, touch)
            );
        }

        public static IEnumerator MovePointer(
            Vector2 startScreenPosition,
            Vector2 endScreenPosition,
            int steps,
            Action<Vector2> startAction,
            Action<Vector2, Vector2> stepAction,
            Action<Vector2> endAction
        ) {
            Vector2 startPosition = Camera.main.ViewportToScreenPoint(startScreenPosition);
            Vector2 endPosition = Camera.main.ViewportToScreenPoint(endScreenPosition);
            startAction(startPosition);
            for (int i = 1; i < steps + 1; i++)
            {
                float prevT = (float)(i-1) / steps;
                float t = (float)i / steps;
                Vector2 prevPostion = Vector2.Lerp(startPosition, endPosition, prevT);
                Vector2 currentPosition = Vector2.Lerp(startPosition, endPosition, t);
                Vector2 delta = currentPosition - prevPostion;
                stepAction(currentPosition, delta);
                yield return null;
            }
            endAction(endPosition);
            yield return null;
        }

    }
}
