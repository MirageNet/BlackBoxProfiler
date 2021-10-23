using UnityEngine;
using UnityEngine.UIElements;

namespace Mirage.Profiler
{
    public class ColumnManipulator : MouseManipulator
    {
        private Vector2 _start;
        private bool _active;
        private readonly VisualElement _targetElement;

        public ColumnManipulator(VisualElement target)
        {
            _targetElement = target;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            _active = false;
        }

        #region Overrides of Manipulator

        /// <summary>
        ///   <para>Called to register event callbacks on the target element.</para>
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            _targetElement.RegisterCallback<MouseDownEvent>(OnMouseDown);
            _targetElement.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            _targetElement.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        /// <summary>
        ///   <para>Called to unregister event callbacks from the target element.</para>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            _targetElement.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            _targetElement.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            _targetElement.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        #endregion

        private void OnMouseDown(MouseDownEvent e)
        {
            if (_active)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (CanStartManipulation(e))
            {
                _start = e.localMousePosition;

                _active = true;

                _targetElement.CaptureMouse();

                e.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!_active || !_targetElement.HasMouseCapture())
                return;

            Vector2 diff = e.localMousePosition - _start;

            //_targetElement.parent.style.height = _targetElement.parent.layout.height + diff.x;
            _targetElement.style.width = _targetElement.layout.width + (diff.x > 0 ? 0.5f : -0.5f);

            e.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (!_active || !_targetElement.HasMouseCapture() || !CanStopManipulation(e))
                return;

            _active = false;

            _targetElement.ReleaseMouse();

            e.StopPropagation();
        }
    }
}
