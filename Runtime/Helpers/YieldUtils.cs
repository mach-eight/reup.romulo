using System.Collections;
using System;

namespace ReupVirtualTwin.helpers{
    public static class YieldUtils{

        public static Func<T, IEnumerator> YieldifyAction<T>(Action<T> action)
        {
            return (T input) => Yieldify(action, input);
        }

        public static Func<T, U, IEnumerator> YieldifyAction<T, U>(Action<T, U> action)
        {
            return (T input0, U input1) => Yieldify(action, input0, input1);
        }

        public static IEnumerator Yieldify<T>(Action<T> action, T input)
        {
            action(input);
            yield return null;
        }
        public static IEnumerator Yieldify<T, U>(Action<T, U> action, T input0, U input1)
        {
            action(input0, input1);
            yield return null;
        }
    }

}