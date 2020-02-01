using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface.Converter
{
    public static class Op
    {

        public static T Add<T>(T left, T right) =>
            LambdaCache2<T>.GetLambda(Expression.Add)(left, right);

        public static T Sub<T>(T left, T right) =>
            LambdaCache2<T>.GetLambda(Expression.Subtract)(left, right);

        public static T Mul<T>(T left, T right) =>
            LambdaCache2<T>.GetLambda(Expression.Multiply)(left, right);

        public static T Div<T>(T left, T right) =>
            LambdaCache2<T>.GetLambda(Expression.Divide)(left, right);


        private static class LambdaCache2<T>
        {

            public delegate BinaryExpression OpExp(Expression left, Expression right);

            public delegate T OpLambda(T left, T right);

            public static OpLambda GetLambda(OpExp op)
            {
                if (Cache.TryGetValue(op, out var opLambda))
                    return opLambda;
                var left = Expression.Parameter(typeof(T));
                var right = Expression.Parameter(typeof(T));
                return Cache[op] = Expression.Lambda<OpLambda>(op(left, right), left, right).Compile();
            }

            public static Dictionary<OpExp, OpLambda>
                Cache = new Dictionary<OpExp, OpLambda>();
        }


    }
}
