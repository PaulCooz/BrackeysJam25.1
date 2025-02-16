using System;
using UnityEngine;

namespace JamSpace
{
    /// <summary>
    /// A convenience struct to easily manage a time countdown, based on <see cref="P:Sandbox.Time.Now" />.<br />
    /// <br />
    /// Typical usage would see you assigning to a variable of this type a necessary amount of seconds.
    /// Then the struct would return the time countdown, or can be used as a bool i.e.:
    /// <code>
    /// TimeUntil nextAttack = 10;
    /// if ( nextAttack ) { /*Do something*/ }
    /// </code>
    /// </summary>
    public struct TimeUntil : IEquatable<TimeUntil>
    {
        private double _finishTime;
        private double _startTime;

        public static implicit operator bool(TimeUntil ts)  => Time.timeAsDouble >= ts._finishTime;
        public static implicit operator float(TimeUntil ts) => (float)(ts._finishTime - Time.timeAsDouble);

        public static bool operator <(in TimeUntil ts, float f) => ts.Relative < (double)f;

        public static bool operator >(in TimeUntil ts, float f) => ts.Relative > (double)f;

        public static bool operator <=(in TimeUntil ts, float f) => ts.Relative <= (double)f;

        public static bool operator >=(in TimeUntil ts, float f) => ts.Relative >= (double)f;

        public static bool operator <(in TimeUntil ts, int f) => ts.Relative < (double)f;

        public static bool operator >(in TimeUntil ts, int f) => ts.Relative > (double)f;

        public static bool operator <=(in TimeUntil ts, int f) => ts.Relative <= (double)f;

        public static bool operator >=(in TimeUntil ts, int f) => ts.Relative >= (double)f;

        public static implicit operator TimeUntil(float ts)
        {
            return new TimeUntil
            {
                _finishTime = Time.timeAsDouble + ts,
                _startTime  = Time.timeAsDouble
            };
        }

        /// <summary>
        /// Time to which we are counting down to, based on <see cref="P:Sandbox.Time.Now" />.
        /// </summary>
        public float Absolute => (float)_finishTime;

        /// <summary>The actual countdown, in seconds.</summary>
        public float Relative => this;

        /// <summary>Amount of seconds passed since the countdown started.</summary>
        public float Passed => (float)(Time.timeAsDouble - _startTime);

        /// <summary>
        /// The countdown, but as a fraction, i.e. a value from 0 (start of countdown) to 1 (end of countdown)
        /// </summary>
        public float Fraction =>
            Math.Clamp((float)((Time.timeAsDouble - _startTime) / (_finishTime - _startTime)), 0.0f, 1f);

        public override string ToString() => $"timer from {_startTime} to {_finishTime}";

        public static bool operator ==(TimeUntil left, TimeUntil right) => left.Equals(right);

        public static bool operator !=(TimeUntil left, TimeUntil right) => !(left == right);

        public readonly override bool Equals(object obj) => obj is TimeUntil o && Equals(o);

        public readonly bool Equals(TimeUntil o) => _finishTime == o._finishTime;

        public readonly override int GetHashCode() => HashCode.Combine(_finishTime);
    }
}