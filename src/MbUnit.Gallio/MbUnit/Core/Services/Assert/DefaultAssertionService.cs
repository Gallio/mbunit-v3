using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using MbUnit.Core.Exceptions;
using MbUnit.Core.Services.Context;
using MbUnit.Framework;

namespace MbUnit.Core.Services.Assert
{
    /// <summary>
    /// Default implementation of the assertion service.
    /// </summary>
    public sealed class DefaultAssertionService : IAssertionService
    {
        private const string ContextDataKey = "$$DefaultAssertionService.ContextData$$";

        /// <summary>
        /// The thread-local assertion scope stack.
        /// </summary>
        [ThreadStatic]
        private static Stack<AssertionScope> scopeStack;

        public int GetAssertionCount(IContext context)
        {
            lock (context.SyncRoot)
            {
                ContextData contextData = GetInitializedContextData(context);
                return contextData.AssertionCount;
            }
        }

        public AssertionResult[] Run(IContext context, Block block, bool allowMultipleFailures)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (block == null)
                throw new ArgumentNullException("block");

            Stack<AssertionScope> currentScopeStack = GetInitializedScopeStack();
            AssertionScope currentScope = new AssertionScope(allowMultipleFailures, true);
            AssertionResult result = null;

            currentScopeStack.Push(currentScope);
            try
            {
                block();
            }
            catch (AssertionException ex)
            {
                result = ex.AssertionResult;

                if (result == null)
                {
                    // We received an old-style AssertionException without an attached
                    // AssertionResult.  Construct a suitable AssertionResult and add
                    // it to the list of verified assertion result for the scope.
                    Assertion assertion = new Assertion("Unknown.AssertionException",
                                                        "An AssertionException was intercepted without an associated AssertionResult.", "");
                    result = AssertionResult.CreateFailureResult(
                        ex.Message, ex, null);
                    result.Assertion = assertion;
                }
                else if (!currentScope.Results.Contains(result))
                {
                    // We received a new-style AssertionException specifying an AssertionResult
                    // that we did not verify within this scope.  This is somewhat unusual behavior
                    // but we tolerate it by appending the result to the list for this scope.
                    result = null;
                }
                // Otherwise we can ignore the exception since we must already have verified it.
            }
            catch (Exception ex)
            {
                // Create an assertion for the implied condition that the block should
                // have executed without error.
                Assertion assertion = new Assertion("Unknown.Error",
                                                    "Verifies that a block of code will run without throwing exceptions.", "");
                result = new AssertionResult(AssertionResultType.Error,
                                             String.Format(CultureInfo.CurrentCulture, "An exception occurred of type '{0}'.", ex.GetType().FullName),
                                             ex, null);
                result.Assertion = assertion;
            }
            finally
            {
                currentScopeStack.Pop();
            }

            // If the block exited abnormally, verify the result and add it to the scope.
            if (result != null)
            {
                VerifyWithoutExaminingScope(context, result);
                currentScope.AddResult(result);
            }

            return currentScope.Results.ToArray();
        }

        public AssertionResult Evaluate(IContext context, Assertion assertion, AssertionCondition condition)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (assertion == null)
                throw new ArgumentNullException("assertion");
            if (condition == null)
                throw new ArgumentNullException("condition");

            OnEvaluateBegin(context, assertion);

            Stack<AssertionScope> currentScopeStack = GetInitializedScopeStack();
            AssertionScope currentScope = new AssertionScope(false, false);
            AssertionResult result;

            currentScopeStack.Push(currentScope);
            try
            {
                result = condition(assertion);
            }
            catch (Exception ex)
            {
                result = new AssertionResult(AssertionResultType.Error,
                                             String.Format(CultureInfo.CurrentCulture, "An exception occurred of type '{0}'.", ex.GetType().FullName),
                                             ex, null);
            }
            finally
            {
                currentScopeStack.Pop();
            }

            result.Assertion = assertion;

            OnEvaluateEnd(context, result);
            return result;
        }

        public void Verify(IContext context, AssertionResult result)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (result == null)
                throw new ArgumentNullException("result");

            VerifyWithoutExaminingScope(context, result);

            Stack<AssertionScope> currentScopeStack = scopeStack;
            if (currentScopeStack != null && currentScopeStack.Count != 0)
            {
                // Add the assertion to the current scope.
                AssertionScope currentScope = currentScopeStack.Peek();
                currentScope.AddResult(result);

                // If we allow multiple failures then don't bother checking the status.
                if (currentScope.AllowMultipleFailures)
                    return;
            }

            if (!result.IsOk)
                throw new AssertionException(result);
        }

        private void VerifyWithoutExaminingScope(IContext context, AssertionResult result)
        {
            ContextData contextData = GetInitializedContextData(context);
            contextData.IncrementAssertionCount();

            AssertionUtils.EnsureAssertionResultIsWellFormed(result);

            OnVerify(context, result);
        }

        /// <summary>
        /// Called when an assertion evaluation begins.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="assertion">The assertion about to be evaluated</param>
        private void OnEvaluateBegin(IContext context, Assertion assertion)
        {
        }

        /// <summary>
        /// Called when an assertion evaluation completed.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="result">The assertion result that was produced</param>
        private void OnEvaluateEnd(IContext context, AssertionResult result)
        {
        }

        /// <summary>
        /// Called when an assertion evaluation begins.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="result">The assertion result to verify</param>
        private void OnVerify(IContext context, AssertionResult result)
        {
        }

        /// <summary>
        /// Gets the assertion service data for the specified context.
        /// Initializes it if absent.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The assertion state</returns>
        private static ContextData GetInitializedContextData(IContext context)
        {
            lock (context.SyncRoot)
            {
                ContextData state;
                if (context.TryGetData(ContextDataKey, out state))
                {
                    state = new ContextData();
                    context.SetData(ContextDataKey, state);
                }

                return state;
            }
        }

        /// <summary>
        /// Gets the thread-local assertion scope stack. 
        /// Initializes it if absent.
        /// </summary>
        private static Stack<AssertionScope> GetInitializedScopeStack()
        {
            if (scopeStack == null)
                scopeStack = new Stack<AssertionScope>();

            return scopeStack;
        }

        /// <summary>
        /// Maintains assertion state information associated with the context.
        /// </summary>
        private class ContextData
        {
            /// <summary>
            /// Gets the assertion count for the context.
            /// </summary>
            public int AssertionCount
            {
                get { return assertionCount; }
            }
            private int assertionCount;

            /// <summary>
            /// Increments the assertion count for the context.
            /// Only verified assertions should be counted.
            /// </summary>
            public void IncrementAssertionCount()
            {
                Interlocked.Increment(ref assertionCount);
            }
        }

        /// <summary>
        /// Maintains information associated with the current thread to handle
        /// dynamic scoping for controlling and monitoring the behavior of the
        /// Verify method within Run and Evaluate delegates.
        /// </summary>
        private class AssertionScope
        {
            /// <summary>
            /// Creates an assertion scope.
            /// </summary>
            /// <param name="allowMultipleFailures">True if Verify should allow multiple failures</param>
            /// <param name="collectResults">If true, collects results in a list, otherwise ignores them</param>
            public AssertionScope(bool allowMultipleFailures, bool collectResults)
            {
                this.allowMultipleFailures = allowMultipleFailures;

                if (collectResults)
                    results = new List<AssertionResult>();
            }

            /// <summary>
            /// Gets a flag that indicates whether Verify should allow multiple failures
            /// or else throw AssertionExceptions each time.
            /// </summary>
            public bool AllowMultipleFailures
            {
                get { return allowMultipleFailures; }
            }
            private bool allowMultipleFailures;

            /// <summary>
            /// Gets the list of assertion results or null if results are not being collected.
            /// </summary>
            public List<AssertionResult> Results
            {
                get { return results; }
            }
            private List<AssertionResult> results;

            /// <summary>
            /// Adds a result to the list of results if they are to be collected.
            /// </summary>
            /// <param name="result">The result to add</param>
            public void AddResult(AssertionResult result)
            {
                if (results != null)
                    results.Add(result);
            }
        }
    }
}
