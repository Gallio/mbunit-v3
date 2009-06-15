using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    internal abstract class ArgumentValidationContractOptionsBase<TTarget> : AbstractContract, IContract
    {
        private readonly List<Case> cases = new List<Case>();
        private Func<TTarget> factory;

        protected class Case
        {
            public Type ExceptionType;
            public Action Action;
        }

        protected void AddCase(Case newCase)
        {
            if (newCase == null)
                throw new ArgumentNullException("newCase");

            cases.Add(newCase);
        }

        protected void SetFactory(Func<TTarget> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            this.factory = factory;
        }

        protected TTarget GetSafeInstance()
        {
            TTarget target = default(TTarget);

            AssertionHelper.Explain(() =>
                Assert.DoesNotThrow(() => target = factory == null ? Activator.CreateInstance<TTarget>() : factory()),
                innerFailures => new AssertionFailureBuilder(String.Format(
                    "Cannot instantiate a default instance of the tested type '{0}'.", typeof(TTarget)))
                    .AddInnerFailures(innerFailures)
                    .SetStackTrace(Context.GetStackTraceData())
                    .ToAssertionFailure());

            return target;
        }

        protected override IEnumerable<Test> GetContractVerificationTests()
        {
            int i = 0;

            foreach (var @case in cases)
            {
                yield return CreateTest(@case, ++i);
            }
        }

        private Test CreateTest(Case @case, int count)
        {
            return new TestCase(String.Format("Test{0}", count), () =>
            {
                AssertionHelper.Explain(() => 
                    Assert.Throws(@case.ExceptionType, @case.Action),
                    innerFailures => new AssertionFailureBuilder("Oops!")
                        .AddInnerFailures(innerFailures)
                        .SetStackTrace(Context.GetStackTraceData())
                        .ToAssertionFailure());
            });
        }
    }

    internal class ArgumentValidationContractOptions<T1, TTarget> : ArgumentValidationContractOptionsBase<TTarget>, IArgumentValidationContractOptions<T1, TTarget>
    {
        private Func<T1, TTarget> constructor;
        private Action<TTarget, T1> method;

        internal IArgumentValidationContractOptions<T1, TTarget> For(Func<T1, TTarget> constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");

            this.constructor = constructor;
            return this;
        }

        internal IArgumentValidationContractOptions<T1, TTarget> For(Action<TTarget, T1> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.method = method;
            return this;
        }

        public IArgumentValidationContractOptions<T1, TTarget> With(Func<TTarget> factory)
        {
            SetFactory(factory);
            return this;
        }

        public IArgumentValidationContractOptions<T1, TTarget> ShouldThrow<TException>(T1 arg1)
            where TException : Exception
        {
            AddCase(new Case
            {
                ExceptionType = typeof(TException),
                Action = constructor == null
                    ? new Action(() => method(GetSafeInstance(), arg1))
                    : new Action(() => constructor(arg1))
            });

            return this;
        }
    }

    internal class ArgumentValidationContractOptions<T1, T2, TTarget> : ArgumentValidationContractOptionsBase<TTarget>, IArgumentValidationContractOptions<T1, T2, TTarget>
    {
        private Func<T1, T2, TTarget> constructor;
        private Action<TTarget, T1, T2> method;

        internal IArgumentValidationContractOptions<T1, T2, TTarget> For(Func<T1, T2, TTarget> constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");

            this.constructor = constructor;
            return this;
        }

        internal IArgumentValidationContractOptions<T1, T2, TTarget> For(Action<TTarget, T1, T2> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.method = method;
            return this;
        }

        public IArgumentValidationContractOptions<T1, T2, TTarget> With(Func<TTarget> factory)
        {
            SetFactory(factory);
            return this;
        }

        public IArgumentValidationContractOptions<T1, T2, TTarget> ShouldThrow<TException>(T1 arg1, T2 arg2)
            where TException : Exception
        {
            AddCase(new Case
            {
                ExceptionType = typeof(TException),
                Action = constructor == null
                    ? new Action(() => method(GetSafeInstance(), arg1, arg2))
                    : new Action(() => constructor(arg1, arg2))
            });

            return this;
        }
    }

    internal class ArgumentValidationContractOptions<T1, T2, T3, TTarget> : ArgumentValidationContractOptionsBase<TTarget>, IArgumentValidationContractOptions<T1, T2, T3, TTarget>
    {
        private Func<T1, T2, T3, TTarget> constructor;
        private Action<TTarget, T1, T2, T3> method;

        internal IArgumentValidationContractOptions<T1, T2, T3, TTarget> For(Func<T1, T2, T3, TTarget> constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");

            this.constructor = constructor;
            return this;
        }

        internal IArgumentValidationContractOptions<T1, T2, T3, TTarget> For(Action<TTarget, T1, T2, T3> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.method = method;
            return this;
        }

        public IArgumentValidationContractOptions<T1, T2, T3, TTarget> With(Func<TTarget> factory)
        {
            SetFactory(factory);
            return this;
        }

        public IArgumentValidationContractOptions<T1, T2, T3, TTarget> ShouldThrow<TException>(T1 arg1, T2 arg2, T3 arg3)
            where TException : Exception
        {
            AddCase(new Case
            {
                ExceptionType = typeof(TException),
                Action = constructor == null
                    ? new Action(() => method(GetSafeInstance(), arg1, arg2, arg3))
                    : new Action(() => constructor(arg1, arg2, arg3))
            });

            return this;
        }
    }

    internal class ArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> : ArgumentValidationContractOptionsBase<TTarget>, IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget>
    {
        private Func<T1, T2, T3, T4, TTarget> constructor;
        private Action<TTarget, T1, T2, T3, T4> method;

        internal IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> For(Func<T1, T2, T3, T4, TTarget> constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");

            this.constructor = constructor;
            return this;
        }

        internal IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> For(Action<TTarget, T1, T2, T3, T4> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.method = method;
            return this;
        }

        public IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> With(Func<TTarget> factory)
        {
            SetFactory(factory);
            return this;
        }

        public IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> ShouldThrow<TException>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where TException : Exception
        {
            AddCase(new Case
            {
                ExceptionType = typeof(TException),
                Action = constructor == null
                    ? new Action(() => method(GetSafeInstance(), arg1, arg2, arg3, arg4))
                    : new Action(() => constructor(arg1, arg2, arg3, arg4))
            });

            return this;
        }
    }
}
