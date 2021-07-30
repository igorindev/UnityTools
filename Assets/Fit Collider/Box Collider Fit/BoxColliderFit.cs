using UnityEngine;

namespace FBC
{
    public enum FitState
    {
        Not,
        Need,
        Done
    }

    [RequireComponent(typeof(BoxCollider))]
    public class BoxColliderFit : MonoBehaviour
	{
        public FitState state = FitState.Not;

        public bool isBestFit = false;

        public bool isDynamic = true;
        public bool isDynamicPosition = false;

        public bool isRootMotion = false;
        public Transform rootBone;

        public int samplingSize = 20;

        public Vector3 size = Vector3.one;
        public Vector3 scale = Vector3.one;

        private Bounds bounds;

        void Start()
		{
        }

        public bool HasSkinnedMeshRenderer()
        {
            var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            return renderer != null;
        }

        public bool HasTransform(Transform target)
        {
            return HasTransformRecursive(transform, target);
        }

        private bool HasTransformRecursive(Transform parent, Transform target)
        {
            if (parent == target)
                return true;

            for (int i = 0; i < parent.childCount; ++i)
            {
                if (HasTransformRecursive(parent.GetChild(i), target))
                    return true;
            }

            return false;
        }

        public bool HasBoxCollider()
        {
            return GetComponent<BoxCollider>() != null;
        }

        public void ApplyUpdateWhenOffscreen()
        {
            var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinnedMeshRenderers)
                renderer.updateWhenOffscreen = isBestFit;
        }

        public void FitColliderFromScratch()
        {
            bounds = new Bounds(Vector3.zero, Vector3.zero);
            ContinueToFitCollider();
        }

        public void ContinueToFitCollider()
        {
            var bc = GetComponent<BoxCollider>();
            if (bc == null)
                return;

            Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);

            var renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                var renderer = renderers[i];
                if (i == 0)
                    newBounds = renderer.bounds;
                else
                    newBounds.Encapsulate(renderer.bounds);
            }

            bounds.center = newBounds.center;
            bounds.Encapsulate(newBounds);

            if (bounds.size.sqrMagnitude > 0)
            {
                bc.center = bounds.center - transform.position;
                bc.size = bounds.size;
            }
            else
            {
                bc.size = bc.center = Vector3.zero;
                bc.size = Vector3.zero;
            }

            size = bc.size;
        }

        public void ApplyScale()
        {
            var bc = GetComponent<BoxCollider>();
            bc.size = new Vector3(size.x * scale.x, size.y * scale.y, size.z * scale.z);
        }

        public void Relocate()
        {
            var bc = GetComponent<BoxCollider>();
            if (bc == null)
                return;

            Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);

            var renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                var renderer = renderers[i];
                if (i == 0)
                    newBounds = renderer.bounds;
                else
                    newBounds.Encapsulate(renderer.bounds);
            }

            if (newBounds.size.sqrMagnitude > 0)
            {
                bc.center = newBounds.center - transform.position;
            }
        }

        void LateUpdate()
        {
            if (isDynamic)
            {
                ContinueToFitCollider();
                ApplyScale();
            }
            else
            {
                if (isDynamicPosition)
                {
                    Relocate();
                }
            }
        }
	}
}
