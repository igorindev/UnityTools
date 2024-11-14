using UnityEngine;

namespace Tween
{
    public static class TweenUtils
    {
        /// <summary>
        /// The easing type
        /// </summary>
        public enum TweenType
        {
            immediate,
            linear,
            spring,
            easeInQuad,
            easeOutQuad,
            easeInOutQuad,
            easeInCubic,
            easeOutCubic,
            easeInOutCubic,
            easeInQuart,
            easeOutQuart,
            easeInOutQuart,
            easeInQuint,
            easeOutQuint,
            easeInOutQuint,
            easeInSine,
            easeOutSine,
            easeInOutSine,
            easeInExpo,
            easeOutExpo,
            easeInOutExpo,
            easeInCirc,
            easeOutCirc,
            easeInOutCirc,
            easeInBounce,
            easeOutBounce,
            easeInOutBounce,
            easeInBack,
            easeOutBack,
            easeInOutBack,
            easeInElastic,
            easeOutElastic,
            easeInOutElastic
        }

        /// <summary>
        /// Move a float value between two points given an easing function.
        /// </summary>
        /// <param name="tweenType">The type of easing to use</param>
        /// <param name="duration">The amount of time to interpolate</param>
        /// <param name="currentTime">The current time</param>
        /// <param name="start">The starting scroll position</param>
        /// <param name="end">The ending scroll position</param>
        /// <returns></returns>
        public static float TweenPosition(TweenType tweenType, float duration, float currentTime, float start, float end)
        {
            return tweenType switch
            {
                TweenType.linear => Linear(start, end, (currentTime / duration)),
                TweenType.spring => Spring(start, end, (currentTime / duration)),
                TweenType.easeInQuad => EaseInQuad(start, end, (currentTime / duration)),
                TweenType.easeOutQuad => EaseOutQuad(start, end, (currentTime / duration)),
                TweenType.easeInOutQuad => EaseInOutQuad(start, end, (currentTime / duration)),
                TweenType.easeInCubic => EaseInCubic(start, end, (currentTime / duration)),
                TweenType.easeOutCubic => EaseOutCubic(start, end, (currentTime / duration)),
                TweenType.easeInOutCubic => EaseInOutCubic(start, end, (currentTime / duration)),
                TweenType.easeInQuart => EaseInQuart(start, end, (currentTime / duration)),
                TweenType.easeOutQuart => EaseOutQuart(start, end, (currentTime / duration)),
                TweenType.easeInOutQuart => EaseInOutQuart(start, end, (currentTime / duration)),
                TweenType.easeInQuint => EaseInQuint(start, end, (currentTime / duration)),
                TweenType.easeOutQuint => EaseOutQuint(start, end, (currentTime / duration)),
                TweenType.easeInOutQuint => EaseInOutQuint(start, end, (currentTime / duration)),
                TweenType.easeInSine => easeInSine(start, end, (currentTime / duration)),
                TweenType.easeOutSine => easeOutSine(start, end, (currentTime / duration)),
                TweenType.easeInOutSine => EaseInOutSine(start, end, (currentTime / duration)),
                TweenType.easeInExpo => easeInExpo(start, end, (currentTime / duration)),
                TweenType.easeOutExpo => EaseOutExpo(start, end, (currentTime / duration)),
                TweenType.easeInOutExpo => EaseInOutExpo(start, end, (currentTime / duration)),
                TweenType.easeInCirc => EaseInCirc(start, end, (currentTime / duration)),
                TweenType.easeOutCirc => EaseOutCirc(start, end, (currentTime / duration)),
                TweenType.easeInOutCirc => EaseInOutCirc(start, end, (currentTime / duration)),
                TweenType.easeInBounce => EaseInBounce(start, end, (currentTime / duration)),
                TweenType.easeOutBounce => EaseOutBounce(start, end, (currentTime / duration)),
                TweenType.easeInOutBounce => EaseInOutBounce(start, end, (currentTime / duration)),
                TweenType.easeInBack => EaseInBack(start, end, (currentTime / duration)),
                TweenType.easeOutBack => EaseOutBack(start, end, (currentTime / duration)),
                TweenType.easeInOutBack => EaseInOutBack(start, end, (currentTime / duration)),
                TweenType.easeInElastic => EaseInElastic(start, end, (currentTime / duration)),
                TweenType.easeOutElastic => EaseOutElastic(start, end, (currentTime / duration)),
                TweenType.easeInOutElastic => EaseInOutElastic(start, end, (currentTime / duration)),
                TweenType.immediate => end,
                _ => 0,
            };
        }

        private static float Linear(float start, float end, float val)
        {
            return Mathf.Lerp(start, end, val);
        }

        private static float Spring(float start, float end, float val)
        {
            val = Mathf.Clamp01(val);
            val = (Mathf.Sin(val * Mathf.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) * (1f + (1.2f * (1f - val)));
            return start + (end - start) * val;
        }

        private static float EaseInQuad(float start, float end, float val)
        {
            end -= start;
            return end * val * val + start;
        }

        private static float EaseOutQuad(float start, float end, float val)
        {
            end -= start;
            return -end * val * (val - 2) + start;
        }

        private static float EaseInOutQuad(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val + start;
            val--;
            return -end / 2 * (val * (val - 2) - 1) + start;
        }

        private static float EaseInCubic(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val + start;
        }

        private static float EaseOutCubic(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val + 1) + start;
        }

        private static float EaseInOutCubic(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val + 2) + start;
        }

        private static float EaseInQuart(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val + start;
        }

        private static float EaseOutQuart(float start, float end, float val)
        {
            val--;
            end -= start;
            return -end * (val * val * val * val - 1) + start;
        }

        private static float EaseInOutQuart(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val + start;
            val -= 2;
            return -end / 2 * (val * val * val * val - 2) + start;
        }

        private static float EaseInQuint(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val * val + start;
        }

        private static float EaseOutQuint(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val * val * val + 1) + start;
        }

        private static float EaseInOutQuint(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val * val * val + 2) + start;
        }

        private static float easeInSine(float start, float end, float val)
        {
            end -= start;
            return -end * Mathf.Cos(val / 1 * (Mathf.PI / 2)) + end + start;
        }

        private static float easeOutSine(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Sin(val / 1 * (Mathf.PI / 2)) + start;
        }

        private static float EaseInOutSine(float start, float end, float val)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1) + start;
        }

        private static float easeInExpo(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (val / 1 - 1)) + start;
        }

        private static float EaseOutExpo(float start, float end, float val)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * val / 1) + 1) + start;
        }

        private static float EaseInOutExpo(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * Mathf.Pow(2, 10 * (val - 1)) + start;
            val--;
            return end / 2 * (-Mathf.Pow(2, -10 * val) + 2) + start;
        }

        private static float EaseInCirc(float start, float end, float val)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - val * val) - 1) + start;
        }

        private static float EaseOutCirc(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * Mathf.Sqrt(1 - val * val) + start;
        }

        private static float EaseInOutCirc(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return -end / 2 * (Mathf.Sqrt(1 - val * val) - 1) + start;
            val -= 2;
            return end / 2 * (Mathf.Sqrt(1 - val * val) + 1) + start;
        }

        private static float EaseInBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - val) + start;
        }

        private static float EaseOutBounce(float start, float end, float val)
        {
            val /= 1f;
            end -= start;
            if (val < (1 / 2.75f))
            {
                return end * (7.5625f * val * val) + start;
            }
            else if (val < (2 / 2.75f))
            {
                val -= (1.5f / 2.75f);
                return end * (7.5625f * (val) * val + .75f) + start;
            }
            else if (val < (2.5 / 2.75))
            {
                val -= (2.25f / 2.75f);
                return end * (7.5625f * (val) * val + .9375f) + start;
            }
            else
            {
                val -= (2.625f / 2.75f);
                return end * (7.5625f * (val) * val + .984375f) + start;
            }
        }

        private static float EaseInOutBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            if (val < d / 2) return EaseInBounce(0, end, val * 2) * 0.5f + start;
            else return EaseOutBounce(0, end, val * 2 - d) * 0.5f + end * 0.5f + start;
        }

        private static float EaseInBack(float start, float end, float val)
        {
            end -= start;
            val /= 1;
            float s = 1.70158f;
            return end * (val) * val * ((s + 1) * val - s) + start;
        }

        private static float EaseOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val = (val / 1) - 1;
            return end * ((val) * val * ((s + 1) * val + s) + 1) + start;
        }

        private static float EaseInOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val /= .5f;
            if ((val) < 1)
            {
                s *= (1.525f);
                return end / 2 * (val * val * (((s) + 1) * val - s)) + start;
            }
            val -= 2;
            s *= (1.525f);
            return end / 2 * ((val) * val * (((s) + 1) * val + s) + 2) + start;
        }

        private static float EaseInElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;
            val = val / d;
            if (val == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }
            val = val - 1;
            return -(a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        private static float EaseOutElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;

            val = val / d;
            if (val == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        private static float EaseInOutElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;

            val = val / (d / 2);
            if (val == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (val < 1)
            {
                val = val - 1;
                return -0.5f * (a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
            }
            val = val - 1;
            return a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
    }
}
