using System;
using NUnit.Framework;
using GoodLuckValley.Core.DI.Core;
using GoodLuckValley.Core.DI.Exceptions;
using GoodLuckValley.Core.DI.Injection;
using GoodLuckValley.Core.DI.Interfaces;

namespace GoodLuckValley.Tests.EditMode.DI
{
    [TestFixture]
    public class ContainerTests
    {
        [SetUp]
        public void SetUp()
        {
            InjectionCache.Clear();
            DisposeTracker.Reset();
        }
        
        // --- Singleton / Transient / Instance ---

        [Test]
        public void Resolve_Singleton_ReturnsSameInstance()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterSingleton<IServiceC, ServiceC>();
            IContainer container = builder.Build();

            IServiceC first = container.Resolve<IServiceC>();
            IServiceC second = container.Resolve<IServiceC>();
            
            Assert.AreSame(first, second);
        }

        [Test]
        public void Resolve_Transient_ReturnsDifferentInstances()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterTransient<IServiceC, ServiceC>();
            IContainer container = builder.Build();
            
            IServiceC first = container.Resolve<IServiceC>();
            IServiceC second = container.Resolve<IServiceC>();
            
            Assert.AreNotSame(first, second);
        }

        [Test]
        public void Resolve_Instance_ReturnsProvidedInstance()
        {
            ServiceC instance = new ServiceC();
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterInstance<IServiceC>(instance);
            IContainer container = builder.Build();
            
            IServiceC resolved = container.Resolve<IServiceC>();
            
            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void RegisterSingleton_ConcreteType_ResolvesWithoutInterface()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterSingleton<ServiceC>();
            IContainer container = builder.Build();
            
            ServiceC resolved = container.Resolve<ServiceC>();
            
            Assert.IsNotNull(resolved);
        }

        [Test]
        public void Resolve_ConstructorInjection_InjectsParameter()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterSingleton<IServiceB, ServiceB>();
            builder.RegisterSingleton<IServiceC, ServiceC>();
            IContainer container = builder.Build();
            
            IServiceB resolved = container.Resolve<IServiceB>();
            ServiceB serviceB = (ServiceB)resolved;

            Assert.IsNotNull(serviceB);
            Assert.IsNotNull(serviceB.ServiceC);
        }

        [Test]
        public void Resolve_DeepChain_ResolvesAll()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterSingleton<IServiceA, ServiceA>();
            builder.RegisterSingleton<IServiceB, ServiceB>();
            builder.RegisterSingleton<IServiceC, ServiceC>();
            IContainer container = builder.Build();
            
            IServiceA resolved = container.Resolve<IServiceA>();
            ServiceA serviceA = (ServiceA)resolved;
            ServiceB serviceB = (ServiceB)serviceA.ServiceB;
            
            Assert.IsNotNull(serviceA);
            Assert.IsNotNull(serviceA.ServiceB);
            Assert.IsNotNull(serviceB.ServiceC);
        }
        
        // --- Error Cases ---
        [Test]
        public void Resolve_MissingRegistration_ThrowsMissingRegistrationException()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            IContainer container = builder.Build();
            
            Assert.Throws<MissingRegistrationException>(() => container.Resolve<IServiceA>());
        }

        [Test]
        public void Resolve_CircularDependency_ThrowsCircularDependencyException()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterSingleton<ICircularA, CircularA>();
            builder.RegisterSingleton<ICircularB, CircularB>();
            IContainer container = builder.Build();
            
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ICircularA>());
        }

        [Test]
        public void Resolve_CircularDependency_MessageShowsChain()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterSingleton<ICircularA, CircularA>();
            builder.RegisterSingleton<ICircularB, CircularB>();
            IContainer container = builder.Build();
            
            CircularDependencyException exception = Assert.Throws<CircularDependencyException>(() => container.Resolve<ICircularA>());
            
            StringAssert.Contains("CircularA", exception.Message);
            StringAssert.Contains("CircularB", exception.Message);
        }
        
        // --- Scoping ---
        
        [Test]
        public void Scope_ResolvesLocalRegistrations()
        {
            ContainerBuilder builder = new ContainerBuilder("Parent");
            builder.RegisterSingleton<IServiceC, ServiceC>();
            IContainer parent = builder.Build();
            
            IScopeBuilder scopeBuilder = parent.CreateScope("Child");
            scopeBuilder.Import<IServiceC>();
            scopeBuilder.RegisterSingleton<IServiceB, ServiceB>();
            IContainer child = scopeBuilder.Build();
            
            IServiceB resolved = child.Resolve<IServiceB>();
            
            Assert.IsNotNull(resolved);
        }

        [Test]
        public void Scope_ResolvesImportedTypes()
        {
            ContainerBuilder builder = new ContainerBuilder("Parent");
            builder.RegisterSingleton<IServiceC, ServiceC>();
            IContainer parent = builder.Build();
            
            IScopeBuilder scopeBuilder = parent.CreateScope("Child");
            scopeBuilder.Import<IServiceC>();
            IContainer child = scopeBuilder.Build();

            IServiceC fromParent = parent.Resolve<IServiceC>();
            IServiceC fromChild = child.Resolve<IServiceC>();
            
            Assert.AreSame(fromParent, fromChild);
        }

        [Test]
        public void Scope_RejectsNonImportedParentTypes()
        {
            ContainerBuilder builder = new ContainerBuilder("Parent");
            builder.RegisterSingleton<IServiceC, ServiceC>();
            IContainer parent = builder.Build();

            IScopeBuilder scopeBuilder = parent.CreateScope("Child");
            IContainer child = scopeBuilder.Build();
            
            Assert.Throws<MissingRegistrationException>(() => child.Resolve<IServiceC>());
        }

        [Test]
        public void Scope_LocalOverridesImport()
        {
            ContainerBuilder builder = new ContainerBuilder("Parent");
            builder.RegisterSingleton<IServiceC, ServiceC>();
            IContainer parent = builder.Build();
            
            IScopeBuilder scopeBuilder = parent.CreateScope("Child");
            scopeBuilder.Import<IServiceC>();
            scopeBuilder.RegisterSingleton<IServiceC, AlternateServiceC>();
            IContainer child = scopeBuilder.Build();
            
            IServiceC fromChild = child.Resolve<IServiceC>();
            
            Assert.IsInstanceOf<AlternateServiceC>(fromChild);
        }

        [Test]
        public void Import_ThrowsIfParentLacksType()
        {
            ContainerBuilder builder = new ContainerBuilder("Parent");
            IContainer parent = builder.Build();
            
            IScopeBuilder scopeBuilder = parent.CreateScope("Child");
            
            Assert.Throws<InjectionException>(() => scopeBuilder.Import<IServiceC>());
        }

        // --- Disposal ---
        
        [Test]
        public void Dispose_CascadesToChildren()
        {
            ContainerBuilder builder = new ContainerBuilder("Parent");
            builder.RegisterSingleton<IServiceC, ServiceC>();
            IContainer parent = builder.Build();
            
            IScopeBuilder scopeBuilder = parent.CreateScope("Child");
            scopeBuilder.Import<IServiceC>();
            IContainer child = scopeBuilder.Build();

            parent.Dispose();
            
            Assert.Throws<ObjectDisposedException>(() => child.Resolve<IServiceC>());
        }

        [Test]
        public void Dispose_CallsIDisposableOnSingletons()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterSingleton<IDisposableServiceA, DisposableServiceA>();
            builder.RegisterSingleton<IDisposableServiceB, DisposableServiceB>();
            IContainer container = builder.Build();

            // Resolve A first, the nB - creation order determines disposal order
            DisposableServiceA serviceA = (DisposableServiceA)container.Resolve<IDisposableServiceA>();
            DisposableServiceB serviceB = (DisposableServiceB)container.Resolve<IDisposableServiceB>();
            
            container.Dispose();
            
            // B was created second, so it should be disposed first (reverse order)
            Assert.AreEqual(2, DisposeTracker.Log.Count);
            Assert.AreSame(serviceB, DisposeTracker.Log[0]);
            Assert.AreSame(serviceA, DisposeTracker.Log[1]);
        }

        [Test]
        public void Dispose_DoesNotDisposeTransients()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterTransient<IDisposableServiceA, DisposableServiceA>();
            IContainer container = builder.Build();
            
            DisposableServiceA service = (DisposableServiceA)container.Resolve<IDisposableServiceA>();

            container.Dispose();
            
            Assert.IsFalse(service.IsDisposed);
        }
        
        // --- InjectionCache ---

        [Test]
        public void InjectionCache_CachesReflectionData()
        {
            TypeInjectionInfo first = InjectionCache.GetOrCreate(typeof(InjectableTarget));
            TypeInjectionInfo second = InjectionCache.GetOrCreate(typeof(InjectableTarget));
            
            Assert.AreSame(first, second);
        }

        [Test]
        public void InjectionCache_FindsInjectFields()
        {
            TypeInjectionInfo info = InjectionCache.GetOrCreate(typeof(InjectableTarget));
            
            Assert.AreEqual(1, info.InjectableFields.Length);
            Assert.AreEqual("_serviceA", info.InjectableFields[0].Name);
        }

        [Test]
        public void InjectionCache_FindsInjectProperties()
        {
            TypeInjectionInfo info = InjectionCache.GetOrCreate(typeof(InjectableTarget));
            
            Assert.AreEqual(1, info.InjectableProperties.Length);
            Assert.AreEqual("ServiceB", info.InjectableProperties[0].Name);
        }

        [Test]
        public void InjectionCache_IgnoresNonInjectFields()
        {
            TypeInjectionInfo info = InjectionCache.GetOrCreate(typeof(InjectableTarget));
            
            // InjectableTarget has two private fields (_serviceA and _serviceC),
            // but only _serviceA has [Inject] - so exactly 1 injectable field
            Assert.AreEqual(1, info.InjectableFields.Length);
        }
        
        // --- Factories ---
        [Test]
        public void Factory_CreatesNewInstances()
        {
            ContainerBuilder builder = new ContainerBuilder("Test");
            builder.RegisterTransient<ServiceC>();
            builder.RegisterFactory<ServiceC>();
            IContainer container = builder.Build();
            
            IFactory<ServiceC> factory = container.Resolve<IFactory<ServiceC>>();
            ServiceC first = factory.Create();
            ServiceC second = factory.Create();

            Assert.IsNotNull(first);
            Assert.IsNotNull(second);
            Assert.AreNotSame(first, second);
        }
    }
}