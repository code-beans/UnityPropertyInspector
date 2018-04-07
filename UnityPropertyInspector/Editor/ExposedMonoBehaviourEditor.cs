using UnityEditor;
using UnityEngine;
using UnityPropertyInspector.Scripts;

namespace UnityPropertyInspector.Editor {
    /// <summary>
    /// Editor Script for ExposedMonoBehaviourEditor
    /// </summary>
    [CustomEditor(typeof(ExposedMonoBehaviour),true), DisallowMultipleComponent]
    public class ExposedMonoBehaviourEditor : UnityEditor.Editor {
        private ExposedMonoBehaviour _instance;
        private PropertyField[] _instanceFields;

        #region UnityEvents

        void OnEnable()
        {
            _instance = target as ExposedMonoBehaviour;
            _instanceFields = ExposeProperties.GetProperties(_instance);
        }

        public override void OnInspectorGUI()
        {
            if (_instance == null)
                return;
            DrawDefaultInspector();
            ExposeProperties.Expose(_instanceFields);
        }
        #endregion

    }
}
