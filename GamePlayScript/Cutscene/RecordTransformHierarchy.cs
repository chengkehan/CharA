using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace GameScript.Cutscene
{
    // This is a editor script used for record Physics simulate.
    // We will not use it at runtime.
    public class RecordTransformHierarchy : MonoBehaviour
    {
        public AnimationClip clip;

        public GameObject target = null;

#if UNITY_EDITOR
        private GameObjectRecorder m_Recorder;

        void Start()
        {
            // Create recorder and record the script GameObject.
            m_Recorder = new GameObjectRecorder(target);

            // Bind all the Transforms on the GameObject and all its children.
            m_Recorder.BindComponentsOfType<Transform>(target, true);
        }

        void LateUpdate()
        {
            if (clip == null)
                return;

            // Take a snapshot and record all the bindings values for this frame.
            m_Recorder.TakeSnapshot(Time.deltaTime);
        }

        void OnDisable()
        {
            if (clip == null)
                return;

            if (m_Recorder != null && m_Recorder.isRecording)
            {
                // Save the recorded session to the clip.
                m_Recorder.SaveToClip(clip);
            }
        }
#endif
    }
}