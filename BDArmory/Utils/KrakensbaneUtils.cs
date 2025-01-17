using System.Runtime.CompilerServices;
using UnityEngine;

namespace BDArmory.Utils
{
    /// <summary>
    /// Optimised access to Krakensbane and FloatingOrigin adjustments.
    /// Krakensbane corrections happen some time between the Late and BetterLateThanNever timing phases (which both occur after the FlightIntegrator phase) (I'm reasonably sure that they occur during the Late phase).
    /// If you need access to these adjustments during the BetterLateThanNever phase, then use Krakensbane or FloatingOrigin directly to ensure order of operations.
    ///
    /// With agressive inling, this reduces access time by a factor of ~4 for frequent access per frame.
    /// Note: the access time is already quite small, but these are used frequently every frame (e.g., for bullets, rockets, explosions and countermeasures).
    /// </summary>
    public static class BDKrakensbane
    {
        public static Vector3 FrameVelocityV3f => BDKrakensbaneSingleton.Instance.GetFrameVelocityV3f;
        public static Vector3d FloatingOriginOffset => BDKrakensbaneSingleton.Instance.FloatingOriginOffset;
        public static Vector3d FloatingOriginOffsetNonKrakensbane => BDKrakensbaneSingleton.Instance.FloatingOriginOffsetNonKrakensbane;
        public static bool IsActive => BDKrakensbaneSingleton.Instance.IsActive;
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class BDKrakensbaneSingleton : MonoBehaviour
    {
        public static BDKrakensbaneSingleton Instance;
        void Awake()
        {
            if (Instance != null) Destroy(this);
            Instance = this;
        }
        void OnDestroy()
        { TimingManager.FixedUpdateRemove(TimingManager.TimingStage.BetterLateThanNever, Reset); }

        public Vector3 GetFrameVelocityV3f
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_frameVelocityCheckedThisFrame) return _frameVelocityV3f;
                _frameVelocityV3f = Krakensbane.GetFrameVelocityV3f();
                _frameVelocityCheckedThisFrame = true;
                return _frameVelocityV3f;
            }
        }
        Vector3 _frameVelocityV3f = default;
        bool _frameVelocityCheckedThisFrame = false;

        public Vector3d FloatingOriginOffset
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_floatingOriginOffsetCheckedThisFrame) return _floatingOriginOffset;
                _floatingOriginOffset = FloatingOrigin.Offset;
                _floatingOriginOffsetCheckedThisFrame = true;
                return _floatingOriginOffset;
            }
        }
        Vector3d _floatingOriginOffset = default;
        bool _floatingOriginOffsetCheckedThisFrame = false;

        public Vector3d FloatingOriginOffsetNonKrakensbane
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_floatingOriginOffsetNonKrakensbaneCheckedThisFrame) return _floatingOriginOffsetNonKrakensbane;
                _floatingOriginOffsetNonKrakensbane = FloatingOrigin.OffsetNonKrakensbane;
                _floatingOriginOffsetNonKrakensbaneCheckedThisFrame = true;
                return _floatingOriginOffsetNonKrakensbane;
            }
        }
        Vector3d _floatingOriginOffsetNonKrakensbane = default;
        bool _floatingOriginOffsetNonKrakensbaneCheckedThisFrame = false;

        public bool IsActive
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_isActiveCheckedThisFrame) return isActive;
                isActive = !FloatingOriginOffset.IsZero() || !GetFrameVelocityV3f.IsZero();
                _isActiveCheckedThisFrame = true;
                return isActive;
            }
        }
        bool isActive = false;
        bool _isActiveCheckedThisFrame = false;

        void Start()
        {
            TimingManager.FixedUpdateAdd(TimingManager.TimingStage.BetterLateThanNever, Reset); // Reset the flags before anything else runs.
        }
        void Reset()
        {
            _frameVelocityCheckedThisFrame = false;
            _floatingOriginOffsetCheckedThisFrame = false;
            _floatingOriginOffsetNonKrakensbaneCheckedThisFrame = false;
            _isActiveCheckedThisFrame = false;
        }
    }
}