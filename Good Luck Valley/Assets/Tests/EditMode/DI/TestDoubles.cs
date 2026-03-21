using System;
using System.Collections.Generic;
using GoodLuckValley.Core.DI.Attributes;

namespace GoodLuckValley.Tests.EditMode.DI
{
    // --- Service chain for constructor injection tests: A -> B -> C ---
    public interface IServiceA { }
    public interface IServiceB { }
    public interface IServiceC { }
    
    public class ServiceA : IServiceA
    {
        public IServiceB ServiceB { get; }
        public ServiceA(IServiceB serviceB) => ServiceB = serviceB;
    }
    
    public class ServiceB : IServiceB
    {
        public IServiceC ServiceC { get; }
        public ServiceB(IServiceC serviceC) => ServiceC = serviceC;
    }
    
    public class ServiceC : IServiceC { }
    
    // --- Alternate implementation for scope override tests ---
    public class AlternateServiceC : IServiceC { }
    
    // --- Circular dependency chain: A -> B -> A ---
    public interface ICircularA { }
    public interface ICircularB { }

    public class CircularA : ICircularA
    {
        public CircularA(ICircularB b) { }
    }
    
    public class CircularB : ICircularB
    {
        public CircularB(ICircularA a) { }
    }
    
    // --- Disposable services for disposal tests ---
    public interface IDisposableServiceA { }
    public interface IDisposableServiceB { }

    public class DisposableServiceA : IDisposableServiceA, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
            DisposeTracker.Record(this);
        }
    }

    public class DisposableServiceB : IDisposableServiceB, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
            DisposeTracker.Record(this);
        }
    }

    public static class DisposeTracker
    {
        private static readonly List<object> _log = new List<object>();
        
        public static IReadOnlyList<object> Log => _log;

        public static void Record(object disposed) => _log.Add(disposed);
        public static void Reset() => _log.Clear();
    }
    
    // --- Injectable target for InjectionCache tests ---
    public class InjectableTarget
    {
        [Inject] private IServiceA _serviceA;
        
        [Inject] public IServiceB ServiceB { get; private set; }

        private IServiceC _serviceC;

        public IServiceA GetServiceA() => _serviceA;
        
        public IServiceC GetServiceC() => _serviceC;
    }
}