using UnityEngine;

namespace AnimatorLODSystem
{
    public class AnimatorLOD : MonoBehaviour
    {
        private Transform _transform;
        private Animator _animator;
        private SkinnedMeshRenderer[] _skinnedMeshRenderers;

        private SkinQuality _currentSkinnedMeshQuality;
        private int _currentCascadeLevel;
        private float _accumulatedDeltaTime;

        private int CurrentCascadeLevel => _currentCascadeLevel;
        public Vector3 WorldPosition => _transform.position;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            _transform = transform;
        }

        private void OnEnable()
        {
            Register();
        }

        private void OnDisable()
        {
            Deregister();
        }

        //Late Update is Called After Animator Update
        private void LateUpdate()
        {
            LaterAnimatorUpdate();
        }

        private void Register()
        {
            AnimatorLODSubsystem.Register(this);
        }

        private void Deregister()
        {
            AnimatorLODSubsystem.Deregister(this);
        }

        private void UpdateMeshQuality(SkinQuality currentSkinnedMeshQuality)
        {
            foreach (SkinnedMeshRenderer item in _skinnedMeshRenderers)
            {
                item.quality = currentSkinnedMeshQuality;
            }
        }

        public void UpdateDeltaTime(float deltaTime, float frameCount, AnimatorLODData animationCascadeData, int cascadeLevel)
        {
            _accumulatedDeltaTime += deltaTime;
            _currentCascadeLevel = cascadeLevel;

            if (_currentCascadeLevel == -1)
            {
                return;
            }

            if (animationCascadeData.updateRate == 1)
            {
                UpdateTick(animationCascadeData);
                return;
            }

            if (frameCount % animationCascadeData.updateRate == 0)
            {
                UpdateTick(animationCascadeData);
            }
        }

        private void UpdateTick(AnimatorLODData cascade)
        {
            Tick(_accumulatedDeltaTime, cascade);
            _accumulatedDeltaTime = 0;
        }

        private void Tick(float accumulatedDeltaTime, AnimatorLODData cascade)
        {
            _animator.speed = accumulatedDeltaTime / Time.unscaledDeltaTime;
            _animator.enabled = true;

            if (_currentSkinnedMeshQuality != cascade.skinnedMeshSkinQuality)
            {
                _currentSkinnedMeshQuality = cascade.skinnedMeshSkinQuality;
                UpdateMeshQuality(_currentSkinnedMeshQuality);
            }
        }

        private void LaterAnimatorUpdate()
        {
            if (CurrentCascadeLevel != 0)
            {
                _animator.enabled = false;
            }
        }
    }
}
